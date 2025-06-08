namespace ai_shopping_api_cs.Services;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public class Product
{
    /// <summary>
    /// The name of the product.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The unique product code.
    /// </summary>
    public string ProductCode { get; set; }

    /// <summary>
    /// A description of the product.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The category the product belongs to.
    /// </summary>
    public string Category { get; set; }
}

/// <summary>
/// Service for managing the product catalog.
/// </summary>
public class ProductCatalogService
{
    /// <summary>
    /// Retrieves a list of all available products.
    /// </summary>
    /// <returns>A list of available products.</returns>
    public List<Product> GetAvailableProducts()
    {
        return new List<Product>
        {
            new Product { ProductCode = "P001", Name = "Smartphone X", Description = "AI-powered phone", Price = 999.99M, Category = "Electronics" },
            new Product { ProductCode = "P002", Name = "Cloud Book", Description = "Learn cloud computing", Price = 49.99M, Category = "Books" },
            new Product { ProductCode = "P003", Name = "Wireless Headphones", Description = "Noise-canceling", Price = 199.99M, Category = "Electronics" },
            new Product { ProductCode = "P004", Name = "Laptop Z", Description = "High-performance laptop", Price = 1299.99M, Category = "Electronics" }
        };
    }
}