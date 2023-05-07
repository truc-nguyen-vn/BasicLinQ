using BasicLinQ.Operators;
using Microsoft.AspNetCore.Mvc;

namespace BasicLinQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutionController : Controller
    {
        [HttpGet("deferred")]
        public IActionResult ExecutionDeferred()
        {
            var categories = InitData.Categories;
            var b = categories.WhereExecution(x => x.Id <= 2);

            Console.WriteLine("Deferred Streaming");
            foreach (var category in b) {
                Console.WriteLine("Category Id: " + category.Id);
            }

            Console.WriteLine("\nDeferred Non-Streaming 1");
            foreach (var category in b.ToList())
            {
                Console.WriteLine("Category Id: " + category.Id);
            }

            Console.WriteLine("\nDeferred Non-Streaming 2");
            foreach (var category in b.ToList())
            {
                Console.WriteLine("Category Id: " + category.Id);
            }
            Console.WriteLine();

            return Ok();
        }

        [HttpGet("immediate")]
        public IActionResult ExecutionImmediate()
        {
            Console.WriteLine("Immediate");
            var categories = InitData.Categories;
            var b = categories.WhereExecution(x => x.Id <= 2).ToList();

            Console.WriteLine("Immediate 1");
            foreach (var category in b)
            {
                Console.WriteLine("Category Id: " + category.Id);
            }

            Console.WriteLine("Immediate 2");
            foreach (var category in b.ToList())
            {
                Console.WriteLine("Category Id: " + category.Id);
            }
            Console.WriteLine();

            return Ok();
        }
    }
}
