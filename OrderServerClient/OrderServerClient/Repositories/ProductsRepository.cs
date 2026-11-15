namespace OrderServerClient.Repositories
{
    using OrderServerClient.Domain;
    public class ProductsRepository
    {
        private readonly static List<Product> products = new List<Product> {

        new Product { id = 1, name = "Rato Attack Shark X8SE", price = 45.99 },
        new Product { id = 2, name = "Teclado Mecânico", price = 75.50 },
        new Product { id = 3, name = "Monitor 165Hz", price = 199.99 },
        new Product { id = 4, name = "SSD Kioxia 1TB", price = 60.00 },
        new Product { id = 5, name = "Tapete XXL", price = 15.00 },
        new Product{id=6,name="Rato gamer xpto",price=100.00}

        };
        
        private readonly static SemaphoreSlim mutex= new SemaphoreSlim(1,1);

        

        public static List<Product> GetProducts()
        {
            mutex.Wait();
            try
            {
                return products.ToList();
            }finally {
                mutex.Release(); 
            }
            
        }

        public static Product getProductByid(int id )
        {
            mutex.Wait();
            try
            {
                foreach (Product p in products)
                {
                    if (p.id == id)
                        return p;
                }
                
                return null;
            }finally { mutex.Release(); }

           
        }
    }
}
