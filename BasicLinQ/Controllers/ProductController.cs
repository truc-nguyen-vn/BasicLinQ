using BasicLinQ.Context;
using BasicLinQ.Entities;
using BasicLinQ.Models.Product;
using BasicLinQ.Operators;
using Microsoft.AspNetCore.Mvc;

namespace BasicLinQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        [HttpGet]
        public IEnumerable<Product> Get([FromQuery] ProductRequestModel model)
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Get list Product by filters:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new ApplicationDbContext();

            var productsQuery = from prod in context.Products
                                select prod;
            Helper.LogListData(productsQuery);

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                productsQuery = productsQuery.WhereWithTerm(x => x.Name.Contains(model.SearchTerm)).AsQueryable();
                Helper.LogListData(productsQuery);
            }

            #region Log last execution
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Last execution:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion
            var result = productsQuery.Skip(model.Skip).Take(model.Take).ToList().AsEnumerable();

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

            return result;
        }

        [HttpGet("{id}")]
        public IActionResult GetProductBySupplierId([FromRoute] int id)
        {
            #region Log start get
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Get Product by Supplier id:");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            using ApplicationDbContext context = new ApplicationDbContext();

            Product detail = new();
            var productQuery = context.Products.AsQueryable();
            productQuery = productQuery.WhereWithTerm(x => x.SupplierId == id).AsQueryable();

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

            return Ok(detail);
        }
    }
}
