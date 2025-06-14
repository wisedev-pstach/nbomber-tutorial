using ProductApi.Models;

namespace ProductApi.Repositories;

public class ProductRepository
{
    private static readonly List<Product> _products = new();
    private static int _nextId = 1;

    static ProductRepository()
    {
        // Seed with some initial products
        for (int i = 0; i < 1000; i++)
        {
            _products.Add(new Product
            {
                Id = _nextId++,
                Name = $"Product {i}",
                Price = 10.99m + i,
                Description = $"This is product {i}",
                StockQuantity = 100
            });
        }
    }

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Product> Search(string term)
    {
        // Deliberately inefficient search to demonstrate performance issues
        Thread.Sleep(10); // Simulate some processing time
        return _products.Where(p => 
            p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) || 
            p.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    public Product Add(Product product)
    {
        product.Id = _nextId++;
        _products.Add(product);
        return product;
    }
}