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
            using ApplicationDbContext context = new ApplicationDbContext();

            var productsQuery = from prod in context.Products
                                select prod;

            if (model.IsSortSimple)
            {
                productsQuery = productsQuery.AscendingSort();
                Helper.LogListData(productsQuery);
                productsQuery = productsQuery.DescendingSort();
                Helper.LogListData(productsQuery);

            }
            if (model.IsSortUseThenBy)
            {
                productsQuery = productsQuery.AscendingThenByDescendingSort();
                Helper.LogListData(productsQuery);
                productsQuery = productsQuery.DescendingThenByAscendingSort();
                Helper.LogListData(productsQuery);
            }


            return context.Products.ToArray();
        }
    }
}
