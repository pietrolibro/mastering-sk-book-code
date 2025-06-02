using System.ComponentModel;

namespace AdvancedAIShoppingAssistant.Models;

public class Product
{

    [Description("name")]
    public string? Name { get; set; } = "";

    [Description("Product code.")]
    public string? ProductCode { get; set; } = "";

    [Description("Product description.")]
    public string? Description { get; set; } = "";

    [Description("Product price.")]
    public decimal? Price { get; set; } = 0;

    [Description("Product category.")]
    public string? Category { get; set; } = "";

    [Description("Product brand.")]
    public string? Brand { get; set; } = "";
}