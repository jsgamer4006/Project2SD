
namespace OrderServerClient.Domain
{
    public class Orders
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime date { get; set; }

        public List<OrderProduct> orderProducts { get; set; }

        public void AddproductToOrder(OrderProduct product)
        {
            this.orderProducts.Add(product);
        }

    }
}
