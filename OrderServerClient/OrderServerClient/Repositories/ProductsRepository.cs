namespace OrderServerClient.Repositories
{
    using OrderServerClient.Domain;
    public class ProductsRepository
    {
        private readonly static List<Product> products = new List<Product>();

        private readonly static List<Orders> orderes = new List<Orders>();

        private readonly static SemaphoreSlim mutex= new SemaphoreSlim(1,1);

        public static List<Product> GetProducts()
        {
            mutex.Wait();
            try
            {
                return products;
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

        public static int deleteProduct(int id)
        {
            mutex.Wait();
            try { 
                var product= products.FirstOrDefault(p=> p.id == id);
                if (product == null) return 404;
                products.Remove(product);
                return 200;

            }
            finally
            {
                mutex.Release();
            }
        }


 
        public static void addProduct(Product p)
        {
            mutex.Wait();
            products.Add(p);
            mutex.Release();
        }

        public static void deleteAllProducts()
        {
            mutex.Wait();
            products.Clear();
            mutex.Release();
        }

        public static int updateProdutc(int oldId, Product newProduct)
        {
            mutex.Wait(); 
            try
            {
                
                if (newProduct == null || newProduct.price < 0 || String.IsNullOrWhiteSpace(newProduct.name) || newProduct.id<0) return 400;

                
                var oldProduct = products.FirstOrDefault(p => p.id == oldId);
                if (oldProduct == null) return 404;

               
                if (newProduct.id != oldId && products.Any(p => p.id == newProduct.id))
                    return 409;

                
                oldProduct.id = newProduct.id;
                oldProduct.name = newProduct.name;
                oldProduct.price = newProduct.price;

                return 200;
            }
            finally
            {
                mutex.Release(); 
            }


        }

    }
}
