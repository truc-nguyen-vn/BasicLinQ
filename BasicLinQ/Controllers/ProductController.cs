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
    }
}
