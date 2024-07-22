namespace Api;

using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

public interface IService
{
    IEnumerable<Product> GetAll();
    Product GetById(int id);
    Product Add(Product product);
    void Update(Product product);
    void Delete(int id);
    Task<bool> CategoryExists(int categoryId);
}

public class ServiceConfiguration(string host, int port)
{
    public readonly string CategoryServiceHost = host;
    public readonly int CategoryServicePort = port;
}

public class Service : IService
{
    private readonly ConcurrentDictionary<int, Product> _products = new();

    private readonly ILogger<Service> _logger;
    private readonly HttpClient _httpClient;
    private readonly ServiceConfiguration _configuration;
    
    public async Task<bool> CategoryExists(int categoryId)
    {
        var response = await _httpClient.GetAsync($"http://{_configuration.CategoryServiceHost}:{_configuration.CategoryServicePort}/categories/{categoryId}");
        return response.IsSuccessStatusCode;
    }
    
    public Service(ILogger<Service> logger, HttpClient client, ServiceConfiguration configuration)
    {
        _logger = logger;
        _httpClient = client;
        _configuration = configuration;
    }
    

    public IEnumerable<Product> GetAll()
    {
        _logger.LogInformation("Getting all products");
        return _products.Values;
    }

    public Product GetById(int id)
    {
        _logger.LogInformation($"Getting product by ID: {id}");
        return _products.GetValueOrDefault(id);
    }

    public Product Add(Product product)
    {
        product.Id = _products.Count == 0 ? 1 : _products.Keys.Max() + 1;
        _products.TryAdd(product.Id, product);
        _logger.LogInformation($"Product added: {product.Id}");
        return product;
    }

    public void Update(Product product)
    {
        _products.TryUpdate(product.Id, product, _products.GetValueOrDefault(product.Id));
        _logger.LogInformation($"Product updated: {product.Id}");
    }

    public void Delete(int id)
    {
        _products.TryRemove(id, out var deletedProduct);
        if (deletedProduct != null) {
            _logger.LogInformation($"Product deleted: {id}");
        } else {
            _logger.LogWarning($"Failed to delete product: {id}");
        }
    }
}