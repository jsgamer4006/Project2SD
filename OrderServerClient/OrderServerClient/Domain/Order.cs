
namespace OrderServerClient.Domain
{
    public class Order
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }

        public List<OrderProduct> orderProducts { get; set; }

        public double total { get; set; }
        public double discount { get; set; }
        public double subtotal { get; set; }

        public int totalOfProducts { get; set; } = 0;


    }
}
