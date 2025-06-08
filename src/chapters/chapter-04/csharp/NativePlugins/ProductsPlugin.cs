using Microsoft.SemanticKernel;

using System.ComponentModel;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using System.Text.Json.Serialization;

using AIShoppingAssistant.Models;
using AIShoppingAssistant.Services;

namespace AIShoppingAssistant.NativePlugins;

public class ProductsPlugin
{
    private readonly ProductCatalogService _catalog;
    private readonly ILogger<ProductsPlugin> _logger;

    public ProductsPlugin(ILogger<ProductsPlugin> logger, ProductCatalogService catalog)
    {
        _logger = logger;
        _catalog = catalog;
    }

    /// <summary>
    /// Returns a list of all available products in the catalog.
    /// </summary>
    [KernelFunction("get_available_products")]
    [Description("Gets a list of available products.")]
    public List<Product> GetAvailableProducts()
    {
        return _catalog.GetAvailableProducts();
    }

    /// <summary>
    /// Filters available products by a given category.
    /// </summary>
    /// <param name="products">List of available products.</param>
    /// <param name="category">The category to filter by.</param>
    /// <returns>Filtered list of products belonging to the specified category.</returns>
    [KernelFunction("get_available_products_by_category")]
    [Description("Gets a list of available products in a specific category.")]
    public List<Product> GetProductsByCategory(
        [Description("List of available products")] List<Product> products,
        [Description("Filtering products on specified category")] string category)
    {
        return products
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Finds a product by name using partial or full matching.
    /// </summary>
    /// <param name="products">List of available products.</param>
    /// <param name="name">The name or part of the name of the product to find.</param>
    /// <returns>The first matching product or null if none found.</returns>
    [KernelFunction("find_product_by_name")]
    [Description("Finds a product by name.")]
    public Product? FindProductByName(
         [Description("List of available products")] List<Product> products,
         [Description("Name of the product to find")] string name)
    {
        return products
            .FirstOrDefault(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Compares two products and returns a formatted summary.
    /// </summary>
    /// <param name="p1">First product to compare.</param>
    /// <param name="p2">Second product to compare.</param>
    /// <returns>A string comparing the two products' name, price, and description.</returns>
    [KernelFunction("compare_products")]
    [Description("Compares two products and returns a summary.")]
    public string CompareProducts(
        [Description("First product to compare")] Product p1,
        [Description("Second product to compare")] Product p2)
    {
        return $"Comparison:\n\n" +
               $"{p1.Name} (${p1.Price}) - {p1.Description}\n" +
               $"VS\n" +
               $"{p2.Name} (${p2.Price}) - {p2.Description}\n";
    }
}

