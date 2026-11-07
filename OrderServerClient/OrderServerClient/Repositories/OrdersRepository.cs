using Microsoft.AspNetCore.Mvc.Infrastructure;
using OrderServerClient.Domain;
namespace OrderServerClient.Repositories
{
    public class OrdersRepository
    {
        private readonly static List<Orders> orders = new List<Orders>();

        private readonly static SemaphoreSlim mutex= new SemaphoreSlim(1,1);

        public static List<Orders> getAllOrders()
        {
            mutex.Wait();
            try
            {
                return orders;
            }
            finally
            {
                mutex.Release();
            }
        }

        public static Orders getOrderById(int id)
        {
            foreach(Orders order in orders)
            {
                if(order.Id == id) return order;
            }
            return null;
                
        }

        public static void addOrder(Orders order)
        {
            mutex.Wait();
            orders.Add(order);
            mutex.Release();
        }

        public static int deleteOrderById(int id ) {
            try
            {
                var order = orders.FirstOrDefault(p => p.Id == id);
                if (order == null)
                    return 404;
                orders.Remove(order);
                return 200;
            }
            finally
            {
                mutex.Release();
            }
          
        }

        public static int updateOrder(int oldId, Orders newOrder)
        {
            if (String.IsNullOrWhiteSpace(newOrder.Name) || newOrder.Id < 0) return 400;

            mutex.Wait();
            try
            {
                var order = orders.FirstOrDefault(p=>p.Id == oldId);
                if (order == null) return 404;

                if (orders.Any(p => p.Id == newOrder.Id) && newOrder.Id!=oldId) return 409;

                order.Id= newOrder.Id;
                order.Name= newOrder.Name;
                order.orderProducts= newOrder.orderProducts;
                return 200;
            }
            finally
            {
                mutex.Release();
            }
        }

       
    }
}
