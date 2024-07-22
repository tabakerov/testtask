namespace Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAllProducts()
    {
        _logger.LogInformation("Request to list all products");
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        _logger.LogInformation($"Request to get product with ID: {id}");
        var product = _service.GetById(id);
        if (product == null) {
            _logger.LogWarning($"Product with ID {id} not found");
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        _logger.LogInformation($"Request to create new product");
        if (!await _service.CategoryExists(product.CategoryId)) return BadRequest("Invalid category ID");

        _service.Add(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id) {
            _logger.LogWarning("Mismatched ID in update request");
            return BadRequest();
        }

        _logger.LogInformation($"Request to update product with ID: {id}");
        var existingProduct = _service.GetById(id);
        if (existingProduct == null) return NotFound();
        if (!await _service.CategoryExists(product.CategoryId)) return BadRequest("Invalid category ID");

        _service.Update(product);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        _logger.LogInformation($"Request to delete product with ID: {id}");
        _service.Delete(id);
        return NoContent();
    }
}