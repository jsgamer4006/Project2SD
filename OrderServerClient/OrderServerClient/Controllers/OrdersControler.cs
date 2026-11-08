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
    public class OrdersControler: ControllerBase
    {
        
      
        private readonly ILogger<OrdersControler> _logger;

        public OrdersControler(ILogger<OrdersControler> logger)
        {
            _logger=logger;
        }
        

        [HttpGet]
        public ActionResult getAllOrderes()
        {

            return StatusCode(200, OrdersRepository.getAllOrders());
           
        }
        [HttpGet("{id:int}")]
        public ActionResult getOrderById(int id)
        {

            if (id < 0) return StatusCode(400,"Invalid Id");
           
           var order=OrdersRepository.getOrderById(id);
            if (order == null) return StatusCode(404, "Order not found");
            return StatusCode(200, order);
           

        }
        [HttpPost]
        public async Task<ActionResult> createOrder([FromBody] Orders order)
        {
            if (order == null || order.Id < 0 || String.IsNullOrWhiteSpace(order.Name) || order.orderProducts.Count==0)
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

                }
                var response = await client.CalculateTotalAsync(request);
                order.total = response.Total;
                order.subtotal=response.SubTotal;
                order.discount = response.Discount;
                order.date=DateTime.Now;
                int status= OrdersRepository.addOrder(order);
                switch (status)
                {
                    case 201:
                        return StatusCode(201, order);
                    case 409:
                        return StatusCode(409, "This order already exists ");
                    default:
                        return StatusCode(500, "Unknown erro");
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
        public async Task<ActionResult> updateOrders(int id, Orders newOrder)
        {
            if (newOrder == null || newOrder.Id < 0 || String.IsNullOrWhiteSpace(newOrder.Name) || newOrder.orderProducts.Count==0) return StatusCode(400, "Invalid data");
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
                    if (product == null) return StatusCode(404, "Product doesn't exists");
                    request.Items.Add(new ItemRequest
                    {
                        Quantity = item.quantity,
                        Price = product.price
                    });

                }

                var response = await client.CalculateTotalAsync(request);
                int status =OrdersRepository.updateOrder(id, newOrder,response.Total,response.Discount,response.SubTotal);
                switch (status) {
                    case 200:
                        return StatusCode(200, "Uppdated successfully");
                    case 400:
                        return StatusCode(400, "Invalid arguments");
                    case 404:
                        return StatusCode(404, "Order doesn't exists");
                    case 409:
                        return StatusCode(409, "Conflit with Orders");
                    default:
                        return StatusCode(500, "unknown erro");
                    
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
                case 200:
                    return StatusCode(200, "Deleted sucessfully");
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
            return StatusCode(200, "delete sucessfully");
        }

    }
}
