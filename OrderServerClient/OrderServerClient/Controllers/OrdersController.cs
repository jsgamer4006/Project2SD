using Microsoft.AspNetCore.Mvc;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using System.Data;
using OrderServerClient.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Grpc.Core;
using Grpc.Net.Client;
using OrderServerClient.Protos;
using Microsoft.AspNetCore.Http.Features;
using OrderServerClient.Repositories;
using System.Runtime.CompilerServices;
using System.Diagnostics;
namespace OrderServerClient.Controllers
{
    
    [ApiController]
    [Route("api/Orders")]
    public class OrdersController: ControllerBase
    {
        
      
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger=logger;
        }
        

        [HttpGet]
        public ActionResult getAllOrders()
        {

            return StatusCode(200, OrdersRepository.getAllOrders());
           
        }
        [HttpGet("{id:int}")]
        public ActionResult getOrderById(int id)
        {

            if (id < 0 ) return StatusCode(400,"Invalid Id");
           
           var order=OrdersRepository.getOrderById(id);
            if (order == null) return StatusCode(404, "Order not found");
            return StatusCode(200, order);
           

        }
        [HttpPost]
        public async Task<ActionResult> createOrder([FromBody] Order order)
        {
            if (order == null || order.id < 0 || String.IsNullOrWhiteSpace(order.name) || order.orderProducts.Count==0)
                return StatusCode(400, "Invalid data");
           
            try
            {
                using var channel = GrpcChannel.ForAddress("http://localhost:10000");
                var client = new OrderService.OrderServiceClient(channel);
                var request = new OrderRequest();
                foreach(var item in order.orderProducts)
                {
                    var product=ProductsRepository.getProductByid(item.IdProduct);
                    if(product == null) return StatusCode(404,"Product doesn't exists");
                    if (item.quantity <= 0) return StatusCode(400, "The quantity must be positive ");
                    request.Items.Add(new ItemRequest
                    {
                        Quantity = item.quantity,
                        Price = product.price
                    });
                    item.name = product.name;
                    item.price = product.price;
                   
                }
                var response = await client.CalculateTotalAsync(request);
                order.total = response.Total;
                order.subtotal=response.SubTotal;
                order.discount = response.Discount;
                order.date=DateTime.Now;
                order.orderProducts= OrdersRepository.getOrdersClean(order);
                order.totalOfProducts = order.orderProducts.Sum(p => p.quantity);
                int status= OrdersRepository.addOrder(order);
                switch (status)
                {
                    case 201:
                        return StatusCode(201, order);
                    case 409:
                        return StatusCode(409, "This order already exists ");
                    default:
                        return StatusCode(500, "Unknown error");
                }

            }
            catch (RpcException error) {
                _logger.LogError(error, "Failed to communicate with the Python gRPC server. Status: {Status}", error.StatusCode);
                return StatusCode(503, "The calculation service is temporarily unavailable. Please check if the Python server is running.");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Unexpected internal error while creating the order.");
                return StatusCode(500, "An internal server error occurred while processing your request.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> updateOrders(int id, Order newOrder)
        {
            if (newOrder == null || newOrder.id < 0 || String.IsNullOrWhiteSpace(newOrder.name) || newOrder.orderProducts.Count==0) return StatusCode(400, "Invalid data");
            
            try
            {
                using var channel = GrpcChannel.ForAddress("http://localhost:10000");
                var client = new OrderService.OrderServiceClient(channel);
                var request = new OrderRequest();
                foreach (var item in newOrder.orderProducts)
                {
                    if (item.quantity <= 0)
                        return StatusCode(400, "Quantity must be positive");

                    var product = ProductsRepository.getProductByid(item.IdProduct);
                    if (product == null) return StatusCode(404, "Product doesn't exist");
                    request.Items.Add(new ItemRequest
                    {
                        Quantity = item.quantity,
                        Price = product.price
                    });
                    item.name = product.name;
                    item.price = product.price;

                }

                var response = await client.CalculateTotalAsync(request);
                newOrder.total = response.Total;
                newOrder.discount = response.Discount;
                newOrder.subtotal = response.SubTotal;
                int status =OrdersRepository.updateOrder(id, newOrder);
                switch (status) {
                    case 204:
                        return StatusCode(204);
                    case 400:
                        return StatusCode(400, "Invalid arguments");
                    case 404:
                        return StatusCode(404, "Order doesn't exist");
                    case 409:
                        return StatusCode(409, "Conflict with Orders");
                    default:
                        return StatusCode(500, "Unknown error");
                    
                }

            }
            catch (RpcException error)
            {
                _logger.LogError(error, "Failed to communicate with the Python gRPC server. Status: {Status}", error.StatusCode);
                return StatusCode(503, "The calculation service is temporarily unavailable. Please check if the Python server is running.");
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Unexpected internal error while creating the order.");
                return StatusCode(500, "An internal server error occurred while processing your request.");
            }
        }
        
        [HttpDelete("{id:int}")]
        public ActionResult deleteOrderById(int id)
        {
            int status=OrdersRepository.deleteOrderById(id);
            switch (status)
            {
                case 204:
                    return StatusCode(204);
                case 400:
                    return StatusCode(400, "Invalid Arguments");
                case 404:
                    return StatusCode(404, "Order Not found");
                default:
                    return StatusCode(500, "Unknown error");

            }
        }
        [HttpDelete]
        public ActionResult deleteAllOrders()
        {
            OrdersRepository.deleteAllOrders();
            return StatusCode(204);
        }

    }

    
}
