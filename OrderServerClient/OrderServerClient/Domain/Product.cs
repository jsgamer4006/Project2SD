using Microsoft.IdentityModel.Tokens;

namespace OrderServerClient.Domain
{
    public class Product
    {
        public int id { get; set; }
        public string name {  get; set; }
        public double price { get; set; }

        
    }
}
