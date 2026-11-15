using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc;
using OrderServerClient.Domain;
using OrderServerClient.Repositories;
namespace OrderServerClient.Controllers
{
    [ApiController]
    [Route("api/Products")]
    public class ProductController: ControllerBase
    {
       
       
        [HttpGet]
        public ActionResult GetAllProducts()
        {
            var products=ProductsRepository.GetProducts();
            if (products.Count==0)
                return StatusCode(204);
            return StatusCode(200, products);
        }

        [HttpGet("{id:int}")]
        public ActionResult GetProductById(int id)
        {
            if (id < 0)
                return StatusCode(400, "invalid ID");
            var product=ProductsRepository.getProductByid(id);
            if (product == null)
                return StatusCode(404, "product not found");
            return StatusCode(200, product);
        }

       
        
    }
}
