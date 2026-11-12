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
                return orders.ToList();
            }
            finally
            {
                mutex.Release();
            }
        }

        public static Orders getOrderById(int id)
        {
            mutex.Wait();
            try
            {
                foreach (Orders order in orders)
                {
                    if (order.id == id) return order;
                }
                return null;
            }finally { mutex.Release(); }

            
                
        }

        public static int addOrder(Orders order)
        {
            try
            {
                mutex.Wait();
                if (orders.Any(p => p.id == order.id)) return 409;
                orders.Add(order);
                return 201  ;
            }finally { mutex.Release(); }
           
        }

        public static int deleteOrderById(int id ) {
            if (id < 0) return 400;
            mutex.Wait();
            try
            {
                var order = orders.FirstOrDefault(p => p.id == id);
                if (order == null)
                    return 404;
                orders.Remove(order);
                return 204;
            }
            finally
            {
                mutex.Release();
            }
          
        }

        public static int updateOrder(int oldId, Orders newOrder,double total, double discount, double subtotal,double totalOfProducts)
        {
            if (String.IsNullOrWhiteSpace(newOrder.name) || newOrder.id < 0) return 400;

            mutex.Wait();
            try
            {
                var order = orders.FirstOrDefault(p=>p.id == oldId);
                if (order == null) return 404;

                if (orders.Any(p => p.id == newOrder.id) && newOrder.id!=oldId) return 409;

                order.id= newOrder.id;
                order.name= newOrder.name;
                order.orderProducts= newOrder.orderProducts;
                order.date = DateTime.Now;
                order.discount= discount;
                order.subtotal= subtotal;
                order.total= total;

                return 204;
            }
            finally
            {
                mutex.Release();
            }
        }
        public static void deleteAllOrders()
        {
            mutex.Wait();
            try
            {
                orders.Clear();
            }
            finally
            {
                mutex.Release();
            }
            
            
        }

       
    }
}
