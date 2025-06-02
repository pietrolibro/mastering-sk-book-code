using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel;
using System.Text.Json.Serialization;

using AdvancedAIShoppingAssistant.Models;

namespace AdvancedAIShoppingAssistant.Services;

public class ProductCatalogService
{
    public List<Product> GetAvailableProducts()
    {
        return new List<Product>
        {
            new Product { ProductCode = "P001", Name = "Smartphone X", Description = "AI-powered phone", Price = 999.99M, Category = "Electronics", Brand = "BrandA" },
            new Product { ProductCode = "P002", Name = "Cloud Book", Description = "Learn cloud computing", Price = 49.99M, Category = "Books", Brand = "BrandB" },
            new Product { ProductCode = "P003", Name = "Wireless Headphones", Description = "Noise-canceling", Price = 199.99M, Category = "Electronics", Brand = "BrandC" },
            new Product { ProductCode = "P004", Name = "Laptop Z", Description = "High-performance laptop", Price = 1299.99M, Category = "Electronics", Brand = "BrandA" },
            new Product { ProductCode = "P005", Name = "Smartwatch A", Description = "Fitness smartwatch", Price = 149.99M, Category = "Fitness", Brand = "BrandB" },
            new Product { ProductCode = "P006", Name = "Treadmill B", Description = "High-quality treadmill", Price = 799.99M, Category = "Fitness", Brand = "BrandC" },
            new Product { ProductCode = "P007", Name = "Yoga Mat D", Description = "Premium yoga mat", Price = 24.99M, Category = "Fitness", Brand = "BrandA" },
            new Product { ProductCode = "P008", Name = "Resistance Bands C", Description = "Set of resistance bands", Price = 29.99M, Category = "Fitness", Brand = "BrandB" },
            new Product { ProductCode = "P009", Name = "Tablet P", Description = "Lightweight tablet", Price = 499.99M, Category = "Electronics", Brand = "BrandC" },
            new Product { ProductCode = "P010", Name = "Cybersecurity Basics", Description = "Guide on online security", Price = 34.99M, Category = "Books", Brand = "BrandA" },
            new Product { ProductCode = "P011", Name = "Gaming Console", Description = "Next-gen gaming console", Price = 399.99M, Category = "Electronics", Brand = "BrandB" },
            new Product { ProductCode = "P012", Name = "Bluetooth Speaker", Description = "Portable speaker", Price = 99.99M, Category = "Electronics", Brand = "BrandC" },
            new Product { ProductCode = "P013", Name = "Smart Light Bulb", Description = "Wi-Fi enabled light bulb", Price = 19.99M, Category = "Electronics", Brand = "BrandA" },
            new Product { ProductCode = "P014", Name = "Electric Kettle", Description = "Fast boiling kettle", Price = 49.99M, Category = "Home Appliances", Brand = "BrandB" },
            new Product { ProductCode = "P015", Name = "Air Purifier", Description = "HEPA filter purifier", Price = 199.99M, Category = "Home Appliances", Brand = "BrandC" },
            new Product { ProductCode = "P016", Name = "Smart Thermostat", Description = "Energy-saving thermostat", Price = 249.99M, Category = "Home Appliances", Brand = "BrandA" },
            new Product { ProductCode = "P017", Name = "Fitness Tracker", Description = "Track your workouts", Price = 129.99M, Category = "Fitness", Brand = "BrandB" },
            new Product { ProductCode = "P018", Name = "E-Reader", Description = "Read books on the go", Price = 89.99M, Category = "Books", Brand = "BrandC" },
            new Product { ProductCode = "P019", Name = "Noise-Canceling Earbuds", Description = "Compact and powerful", Price = 149.99M, Category = "Electronics", Brand = "BrandA" },
            new Product { ProductCode = "P020", Name = "Smart Doorbell", Description = "Video-enabled doorbell", Price = 199.99M, Category = "Home Appliances", Brand = "BrandB" }
        };
    }
}