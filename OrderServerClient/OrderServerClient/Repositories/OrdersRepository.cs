
using OrderServerClient.Domain;
namespace OrderServerClient.Repositories
{
    public class OrdersRepository
    {
        private readonly static List<Order> orders = new List<Order>();

        private readonly static SemaphoreSlim mutex= new SemaphoreSlim(1,1);

        public static List<Order> getAllOrders()
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

        public static Order getOrderById(int id)
        {
            mutex.Wait();
            try
            {
                return orders.FirstOrDefault(o => o.id == id);
                
            }finally { mutex.Release(); }

            
                
        }

      public static List<OrderProduct> getOrdersClean(Order order)
        {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (OrderProduct orderProduct in order.orderProducts)
            {
                if (!map.ContainsKey(orderProduct.IdProduct))
                {
                    map.Add(orderProduct.IdProduct, orderProduct.quantity);
                }
                else
                {
                    map[orderProduct.IdProduct] += orderProduct.quantity;
                }
            }
            List<OrderProduct> newOrderProducts = new List<OrderProduct>();
            foreach (KeyValuePair<int, int> item in map)
            {
                OrderProduct productForList = new OrderProduct();
                OrderProduct product = order.orderProducts.FirstOrDefault(p => p.IdProduct == item.Key);
                if (product == null)
                    continue;
                productForList.IdProduct = item.Key;
                productForList.quantity = item.Value;
                productForList.name = product.name;
                productForList.price = product.price;
                newOrderProducts.Add(productForList);
            }
            return newOrderProducts;
     

        }

        public static int addOrder(Order order)
        {
            mutex.Wait();
            try
            {
               
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

        public static int updateOrder(int oldId, Order newOrder)
        {
            if (String.IsNullOrWhiteSpace(newOrder.name) || newOrder.id < 0) return 400;

            mutex.Wait();
            try
            {
                var order = orders.FirstOrDefault(p=>p.id == oldId);
                if (order == null) return 404;

                if (orders.Any(p => p.id == newOrder.id) && newOrder.id!=oldId) return 409;

                newOrder.orderProducts = getOrdersClean(newOrder);
                order.id= newOrder.id;
                order.name= newOrder.name;
                order.orderProducts= newOrder.orderProducts;
                order.discount= newOrder.discount;
                order.subtotal= newOrder.subtotal;
                order.total= newOrder.total;
                order.totalOfProducts = order.totalOfProducts = order.orderProducts.Sum(p => p.quantity); 
                
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
