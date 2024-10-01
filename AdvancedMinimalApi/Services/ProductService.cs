using AdvancedMinimalApi.Contacts;
using AdvancedMinimalApi.Models;

namespace AdvancedMinimalApi.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new List<Product>
         {
           new Product { Id = 1, Name = "Product 1", Price = 10 },
           new Product { Id = 2, Name = "Product 2", Price = 20 }
         };

        public Task<IEnumerable<Product>> GetProductsAsync() => Task.FromResult(_products.AsEnumerable());

        public Task<Product?> GetProductByIdAsync(int id) => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<Product> CreateProductAsync(CreateProductRequest request)
        {
            var newProduct = new Product
            {
                Id = _products.Max(p => p.Id) + 1,
                Name = request.Name,
                Price = request.Price
            };
            _products.Add(newProduct);
            return Task.FromResult(newProduct);
        }

        public Task<Product?> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct is not null)
            {
                existingProduct.Name = request.Name;
                existingProduct.Price = request.Price;
            }
            return Task.FromResult(existingProduct);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is not null)
            {
                _products.Remove(product);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
