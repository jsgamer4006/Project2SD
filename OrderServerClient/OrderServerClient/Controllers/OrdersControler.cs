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
namespace OrderServerClient.Controllers
{
    
    [ApiController]
    [Route("api/Orders")]
    public class OrdersControler: ControllerBase
    {
        
        public static SemaphoreSlim mutex = new SemaphoreSlim(1,1);

        

        [HttpGet]
        public ActionResult getAllOrderes()
        {
            try
            {
                mutex.Wait();
                return Ok(orders);
            }finally { 
                mutex.Release(); 
            }
           
        }
        [HttpGet("/{id:int}")]
        public ActionResult getOrderById(int id)
        {

            if (id < 0) return BadRequest("Invalid Id");
            mutex.Wait();
            try
            {
                foreach (Orders order in orders)
                {
                    if (order.Id == id) return Ok(order);
                }
                return NotFound("The order doens't exists");
            }finally{ 
                mutex.Release(); 
            }
           

        }
        [HttpPost]
        public async Task<ActionResult> createOrder([FromBody] Orders order)
        {
            if (order.Id < 0)
                return BadRequest("Invalid ID");

            await mutex.WaitAsync();
            try
            {
                using var channel = GrpcChannel.ForAddress("http://localhost:10000");
                var client = new OrderService.OrderServiceClient(channel);
                var itemsToServer = new OrderRequest();
                foreach(var myOrder in orders)
                {
                    if()
                }

            }
            catch (RpcException error)
            {
                if (error.StatusCode == Grpc.Core.StatusCode.InvalidArgument)
                    return BadRequest(error.Status.Detail);

                // Se o Python estiver desligado
                return StatusCode(503, "The python server is not responding");
            }
            try
            {
                foreach (Orders myOrder in orders)
                {
                    if (myOrder.Id == order.Id)
                    {
                        return BadRequest("This id already exists");
                    }
                }
                orders.Add(order);
                return Ok("Create sucessefully");
            }finally { 
                mutex.Release(); 
            }
        }

        [HttpPut]
        public async Task<ActionResult> updateOrders()
        {

        }

        
    }
}
