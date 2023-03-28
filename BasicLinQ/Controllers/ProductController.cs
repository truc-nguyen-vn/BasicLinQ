using BasicLinQ.Context;
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
        public IActionResult GetProducts()
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Get Products by Supplier id:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new();
            var productsQuery = context.Products.AsQueryable();

            var listProduct = productsQuery.ToList();

            var productsAny = productsQuery.Any(x => x.SupplierId == 1);

            //All
            var listProductCategory = listProduct.Where(x => x.CategoryId == 1);
            var isContainAInProduct = listProductCategory.All(s => s.Name.Contains("a"));
            Console.WriteLine("All: " + isContainAInProduct);

            //Distinct
            var listCategoryDistinct = listProduct.Select(s => s.CategoryId).Distinct();
            Console.WriteLine("Distinct");
            Helper.LogListData(listCategoryDistinct);

            //Except
            var listProductCate01 = listProduct.Where(x => x.Name == "a");
            var listProductCate02 = listProduct.Where(x => x.Name == "e");
            var listProductExcept = listProductCate01.Except(listProductCate02);
            Console.WriteLine("Except");
            Helper.LogListData(listProductExcept);

            // Union
            var listProductUnion = listProductCate02.Union(listProductCate01);
            Console.WriteLine("Union");
            Helper.LogListData(listProductUnion);

            //SkipWhile 
            var listProductSkipWhile = listProduct.SkipWhile(x => x.Name.Length > 10);
            Console.WriteLine("SkipWhile");
            Helper.LogListData(listProductSkipWhile);

            //TakeWhile 
            var listProductTakeWhile = listProduct.TakeWhile(x => x.Name.Length > 10);
            Console.WriteLine("TakeWhile");
            Helper.LogListData(listProductTakeWhile);

            return Ok(productsQuery);
        }

    }
}
