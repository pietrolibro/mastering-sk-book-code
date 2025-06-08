using Microsoft.AspNetCore.Mvc;
using ai_shopping_api_cs.Services;

namespace ai_shopping_api_cs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCatalogController : ControllerBase
{
    private readonly ProductCatalogService _catalogService;

    public ProductCatalogController(ProductCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    /// <summary>
    /// Retrieves a list of all available products in the catalog.
    /// </summary>
    /// <returns>A list of available products.</returns>
    [HttpGet("products", Name = "get_products")]
    public IActionResult GetAvailableProducts()
    {
        var products = _catalogService.GetAvailableProducts();
        return Ok(products);
    }

    /// <summary>
    /// Retrieves products filtered by a specific category.
    /// </summary>
    /// <param name="category">The category to filter products by.</param>
    /// <returns>A list of products in the specified category.</returns>
    [HttpGet("products/cat/{category}", Name = "get_products_by_category")]
    public IActionResult GetProductsByCategory(string category)
    {
        var products = _catalogService.GetAvailableProducts()
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Ok(products);
    }

    /// <summary>
    /// Searches for a product by its name.
    /// </summary>
    /// <param name="name">The name or part of the name of the product to search for.</param>
    /// <returns>The first matching product, or a 404 status if no product is found.</returns>
    [HttpGet("products/search", Name = "find_product_by_name")]
    public IActionResult FindProductByName([FromQuery] string name)
    {
        var product = _catalogService.GetAvailableProducts()
            .FirstOrDefault(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        return product != null ? Ok(product) : NotFound();
    }
}
