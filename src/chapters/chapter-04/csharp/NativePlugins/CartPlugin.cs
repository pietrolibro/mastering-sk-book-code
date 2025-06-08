using Microsoft.SemanticKernel;

using System.ComponentModel;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using AIShoppingAssistant.Models;
using AIShoppingAssistant.Services;

namespace AIShoppingAssistant.NativePlugins;

public class CartPlugin
{
    private readonly List<Product> _cart = new();
    private readonly ProductCatalogService _catalog;
    private readonly ILogger<ProductsPlugin> _logger;

    /// <summary>
    /// Initializes a new instance of the CartPlugin class with a product catalog service.
    /// </summary>
    /// <param name="catalog">The product catalog service.</param>
    public CartPlugin(ProductCatalogService catalog)
    {
        _catalog = catalog;
    }

    /// <summary>
    /// Adds a product to the cart using its unique product code.
    /// </summary>
    /// <param name="productCode">The unique code of the product to add.</param>
    /// <returns>Confirmation message.</returns>
    [KernelFunction("add_to_cart")]
    [Description("Adds a product to the cart using its unique product code.")]
    public string AddToCart(
        [Description("The unique product code of the product to add.")] string productCode)
    {
        var product = _catalog
            .GetAvailableProducts()
            .FirstOrDefault(p => p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));

        if (product != null)
        {
            _cart.Add(product);
            return $"{product.Name} has been added to your cart.";
        }

        return $"Product with code '{productCode}' not found.";
    }

    /// <summary>
    /// Removes a product from the cart using its product code.
    /// </summary>
    /// <param name="productCode">The unique code of the product to remove.</param>
    /// <returns>Confirmation or error message.</returns>
    [KernelFunction("remove_from_cart")]
    [Description("Removes a product from the cart using its unique product code.")]
    public string RemoveFromCart(
        [Description("The unique product code of the product to remove.")] string productCode)
    {
        var product = _cart.FirstOrDefault(p => p.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
        if (product != null)
        {
            _cart.Remove(product);
            return $"{product.Name} has been removed from your cart.";
        }
        return $"Product with code '{productCode}' is not in your cart.";
    }

    /// <summary>
    /// Shows the list of products currently in the cart.
    /// </summary>
    /// <returns>Formatted list of cart items.</returns>
    [KernelFunction("view_cart")]
    [Description("Shows the list of products currently in the cart.")]
    public string ViewCart()
    {
        if (_cart.Count == 0)
            return "Your cart is empty.";

        var summary = "Your cart contains:\n";
        foreach (var product in _cart)
        {
            summary += $"- [{product.ProductCode}] {product.Name}: {product.Price:C}\n";
        }
        return summary;
    }

    /// <summary>
    /// Calculates the total price of items in the cart.
    /// </summary>
    /// <returns>Formatted total price string.</returns>
    [KernelFunction("get_total")]
    [Description("Calculates and returns the total price of all items in the cart.")]
    public string GetTotal()
    {
        var total = _cart.Sum(p => p.Price);
        return $"Total amount: {total:C}";
    }
}
