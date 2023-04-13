﻿using BasicLinQ.Context;
using BasicLinQ.Entities;
using BasicLinQ.Models.Product;
using BasicLinQ.Operators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasicLinQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery] ProductRequestModel model)
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Get list Product by filters:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new();

            var productsQuery = from prod in context.Products
                                select prod;
            Helper.LogListData(productsQuery);

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                productsQuery = productsQuery.WhereCustomize(x => x.Name.Contains(model.SearchTerm));
                Helper.LogListData(productsQuery);
            }

            #region Log last execution
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Last execution:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion
            IEnumerable<Product> result = productsQuery.Skip(model.Skip).Take(model.Take).ToList();

            if (model.IsSortSimple)
            {
                result = result.AscendingSort();
                Helper.LogListData(result);
                result = result.DescendingSort();
                Helper.LogListData(result);

            }
            if (model.IsSortUseThenBy)
            {
                result = result.AscendingThenByDescendingSort();
                Helper.LogListData(result);
                result = result.DescendingThenByAscendingSort();
                Helper.LogListData(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductBySupplierId([FromRoute] int id)
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Get Product by Supplier id:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new();

            Product detail = new();
            var productQuery = context.Products.AsQueryable();
            productQuery = productQuery.WhereCustomize(x => x.SupplierId == id);

            if (productQuery.CheckExisted())
            {
                try
                {
                    detail = productQuery.SingleAndSingleDefault();
                }
                catch (Exception ex)
                {
                    #region Log SingleOrDefault() ex
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("SingleOrDefault() exception: " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    #endregion 

                    detail = productQuery.FirstAndFirstDefault();
                }
            }
            else
            {
                detail = productQuery.FirstAndFirstDefault();
            }
            Helper.LogListData(new Product[] { detail });

            return Ok(detail);
        }

        [HttpPost("product-by-categories-id")]
        public IActionResult GetProductByCategoriesId([FromBody] int[] categoriesId)
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Get list Supplier is offering:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new();
            var productsCategoriesQuery = context.ProductCategories
                .Include(x => x.Products)
                .WhereCustomize(x => categoriesId.Contains(x.Id));

            var productsMethod = productsCategoriesQuery
               .SelectMany(x => x.Products)
               .ToList();
            Helper.LogListData(productsMethod);

            var productsSyntax = from category in productsCategoriesQuery
                                 from product in category.Products
                                 select product;
            Helper.LogListData(productsSyntax);

            var prodsOfCatesMethodQuery = context.ProductCategories
                .Join(context.Products, cateProd => cateProd.Id, prod => prod.CategoryId, (cateProd, prod) => prod)
                .WhereCustomize(x => categoriesId.Contains(x.Id));
            Helper.LogListData(prodsOfCatesMethodQuery);

            var prodsOfCatesSyntaxQuery = from cateProd in context.ProductCategories
                                          join prod in context.Products on cateProd.Id equals prod.CategoryId
                                          into cateProdJoined
                                          from prodJoined in cateProdJoined.DefaultIfEmpty()
                                          where categoriesId.Contains(prodJoined.CategoryId)
                                          select prodJoined;
            Helper.LogListData(prodsOfCatesMethodQuery);

            return Ok();
        }

        [HttpGet("products")]
        public IActionResult GetListProduct()
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;

            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new();
            var productsQuery = context.Products.AsQueryable();

            var listProduct = productsQuery.ToList();

            var a = productsQuery.All(s => s.Name.Contains("a"));

            //All
            var isContainAInProductEx1 = productsQuery.Where(x => x.CategoryId == 1).All(s => s.Name.Contains("a"));
            Console.WriteLine("All: " + isContainAInProductEx1);

            var isContainAInProductEx02 = listProduct.Where(x => x.CategoryId == 1).All(s => s.Name.Contains("a"));
            Console.WriteLine("All: " + isContainAInProductEx02);

            //Distinct

            var listProductDistinct01 = productsQuery.Select(s => s.Name).Distinct();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("List Product Distint 01");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var product in listProductDistinct01)
            {
                Console.WriteLine(product);
            }

            var listCategoryDistinct02 = listProduct.Select(s => s.Name).Distinct();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("List Product Distint 02");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var product in listCategoryDistinct02)
            {
                Console.WriteLine(product);
            }

            //Except
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Except");
            Console.ForegroundColor = ConsoleColor.Gray;
            var listProductExcept01 = productsQuery.Where(x => x.Name.Contains("a"));
            Helper.LogListData(listProductExcept01);
            var listProductExcept02 = productsQuery.Where(x => x.Name.Contains("e"));
            Helper.LogListData(listProductExcept02);
            var listProductExceptEx = listProductExcept01.Except(listProductExcept02);
            Helper.LogListData(listProductExceptEx);

            var listProductExcept03 = listProduct.Where(x => x.Name.Contains("a"));
            Helper.LogListData(listProductExcept03);
            var listProductExcept04 = listProduct.Where(x => x.Name.Contains("e"));
            Helper.LogListData(listProductExcept04);
            var listProductExcept = listProductExcept03.Except(listProductExcept04);
            Helper.LogListData(listProductExcept);

            //Union
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Union");
            Console.ForegroundColor = ConsoleColor.Gray;
            var listProductUnion01 = listProductExcept01.Union(listProductExcept02);
            Helper.LogListData(listProductUnion01);

            var listProductUnion02 = listProductExcept03.Union(listProductExcept04);
            Helper.LogListData(listProductUnion02);

            //Skip 
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Skip");
            Console.ForegroundColor = ConsoleColor.Gray;
            var listProductSkipWhile01 = productsQuery.OrderBy(x => x.Name).Skip(10);
            Helper.LogListData(listProductSkipWhile01);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("SkipWhile");
            Console.ForegroundColor = ConsoleColor.Gray;
            var listProductSkipWhile02 = listProduct.SkipWhile(x => x.Name.Length > 10);
            Helper.LogListData(listProductSkipWhile02);

            //Take

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Take");
            Console.ForegroundColor = ConsoleColor.Gray;
            var listProductTakeWhile01 = productsQuery.OrderBy(x => x.Name).Take(5);
            Helper.LogListData(listProductTakeWhile01);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("TakeWhile");
            Console.ForegroundColor = ConsoleColor.Gray;
            var listProductTakeWhile02 = listProduct.TakeWhile(x => x.Name.Length > 5);
            Helper.LogListData(listProductTakeWhile02);

            return Ok(listProduct);
        }

    }
}
