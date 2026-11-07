using Microsoft.AspNetCore.Mvc;
using OrderServerClient.Domain;
using OrderServerClient.Repositories;
namespace OrderServerClient.Controllers
{
    public class ProductController: ControllerBase
    {
       
        SemaphoreSlim mutex = new SemaphoreSlim(1, 1);
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
            if (product.id < 0)
                return StatusCode(400, "Id Invalid");
            var myProduct = ProductsRepository.getProductByid(product.id);
            if (myProduct != null)
                return StatusCode(409, "This product already exists");
            ProductsRepository.addProduct(product);
            return StatusCode(200,"created sucessefully");
        }

        [HttpPut("{id:int}")] // REVER A PARTE DO SWITCH CASE 
        public ActionResult updateProduct(int id, [FromBody] Product product)
        {
            int statusCode = ProductsRepository.updateProdutc(id, product);
            switch (statusCode)
            {
                case 200:
                    return StatusCode(200, "updated sucessefully");
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
                case 200:
                    return StatusCode(200, "delete sucessefully");
                case 404:
                    return StatusCode(404, "Product not found ");
                default:
                    return StatusCode(500, "unkown error");
            }
            

        }
        [HttpDelete]
        public ActionResult deleteAllProducts()
        {
            mutex.WaitAsync();
            ProductsRepository.deleteAllProducts();
            mutex.Release();
            return StatusCode(200, "Delete sucessefully");
        }
    }
}
