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
                return StatusCode(204,"No Content");
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

        [HttpPost]
        public  ActionResult createProduct([FromBody] Product product)
        {
            if (product.id < 0 || String.IsNullOrWhiteSpace(product.name) || product.price<0)
                return StatusCode(400, "Id Invalid");
           
            int status=ProductsRepository.addProduct(product);
            switch (status)
            {
                case 201:
                    return StatusCode(201, "Product created successfully");
                case 409:
                    return StatusCode(409, "Product already exists");
                default:
                    return StatusCode(500, "Server Error");

            }
            
        }

        [HttpPut("{id:int}")] // REVER A PARTE DO SWITCH CASE 
        public ActionResult updateProduct(int id, [FromBody] Product product)
        {
            if (product==null || product.id < 0 || String.IsNullOrWhiteSpace(product.name) || product.price < 0)
                return StatusCode(400, "Id Invalid");
            int statusCode = ProductsRepository.updateProdutc(id, product);
            switch (statusCode)
            {
                case 204:
                    return StatusCode(204, "updated sucessefully");
                case 400:
                    return StatusCode(400, "Invalid Values");
                case 404:
                    return StatusCode(404, "product not found");
                case 409:
                    return StatusCode(409, "This product already exists");
                default:
                    return StatusCode(500, "Unknown Erro");
            }
        }
        // REVER A PARTE DO SWITCH CASE 
        [HttpDelete("{id:int}")]
        public ActionResult deleteProductById(int id)
        {
            int statusCode = ProductsRepository.deleteProduct(id);
            switch (statusCode)
            {
                case 204:
                    return StatusCode(204, "delete sucessefully");
                case 404:
                    return StatusCode(404, "Product not found ");
                default:
                    return StatusCode(500, "unkown error");
            }
        }
        [HttpDelete]
        public ActionResult deleteAllProducts()
        {
            ProductsRepository.deleteAllProducts();
            return StatusCode(204, "Delete sucessefully");
        }
    }
}
