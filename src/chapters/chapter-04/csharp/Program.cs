using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Plugins.OpenApi;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Chat;

using AIShoppingAssistant.Models;
using AIShoppingAssistant.Services;
using AIShoppingAssistant.Helpers;
using AIShoppingAssistant.NativePlugins;

namespace AIShoppingAssistant;

class Program
{
    static async Task Main(string[] args)
    {
        // Set to true to use OpenAI, false to use Ollama.
        var useOpenAI = true;

        // Configuration and kernel setup.
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

#pragma warning disable SKEXP0001, SKEXP0020, SKEXP0050, SKEXP0070

        var builder = Kernel.CreateBuilder();

        if (useOpenAI)
        {
            string openAIApiKey = configuration["OpenAI:ApiKey"];
            builder.AddOpenAIChatCompletion(modelId: "gpt-4.1-mini", apiKey: openAIApiKey);
        }
        else
        {
            builder.AddOllamaChatCompletion(
                modelId: "llama3.2:latest",
                httpClient: new HttpClient { BaseAddress = new Uri("http://localhost:11434") }
            );
        }

        // Adding needed dependencies for the Plugins.
        builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Error));
        builder.Services.AddSingleton<ProductCatalogService>();

        // Adding the Plugin to the Kernel.
        builder.Plugins.AddFromType<CartPlugin>("CartPlugin");
        builder.Plugins.AddFromType<ProductsPlugin>("ProductsPlugin");

        var kernel = builder.Build();

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        while (true)
        {
            Console.WriteLine("Select demo to run:");
            Console.WriteLine("1 - Semantic Functions Demo");
            Console.WriteLine("2 - Semantic Functions Direct Invocation Demo");
            Console.WriteLine("3 - Cart Plugin with Chat Capabilities");
            Console.WriteLine("4 - OpenAPI Plugin with Chat Capabilities");
            Console.WriteLine("5 - Exit");
            Console.Write("Enter 1, 2, 3, 4, 5 or 6: ");

            var demoChoice = Console.ReadLine();

            switch (demoChoice)
            {
                case "1":
                    await RunSemanticFunctionsDemoAsync(kernel);
                    break;
                case "2":
                    await RunPluginWithDirectInvocationDemoAsync(kernel);
                    break;
                case "3":
                    await RunCartPluginChatDemoAsync(kernel,chatCompletionService);
                    break;
                case "4":
                    await RunOpenApiPluginChatDemoAsync(kernel,chatCompletionService);
                    break;
                case "5":
                    Console.WriteLine("Exiting.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    break;
            }
        }

        static async Task RunSemanticFunctionsDemoAsync(Kernel kernel)
        {
            Console.WriteLine("--- Semantic Functions Demo ---");

            var AIShoppingPlugin = kernel.ImportPluginFromPromptDirectory("../../plugins/ai_shopping", "AIShoppingPlugin");

            var productCatalogService = kernel.GetRequiredService<ProductCatalogService>();

            var recommend_products = await kernel.InvokeAsync(
                    AIShoppingPlugin["recommend_product"],
                    new() {
                        {"input", "I'm looking for a book to learning about cloud."},
                        { "products",  productCatalogService.GetFormattedAvailableProducts() } }
                    );

            Console.WriteLine("Recommend Products: " + recommend_products);

            // Compare two products.
            var productA = "- Name: Smartwatch A, Description: Fitness smartwatch with heart rate monitoring, PRICE: $149.99, Category: Fitness";
            var productB = "- Name: Tablet P, Description: Lightweight tablet with a stunning display, PRICE: $499.99, Category: Electronics";

            var compare_products = await kernel.InvokeAsync(AIShoppingPlugin["compare_products"],
                new() {
                    { "productA",  productA},
                    { "productB",  productB}
                    });

            Console.WriteLine("Compare Products: " + compare_products);

            // Find products by category.
            var find_by_category = await kernel.InvokeAsync(AIShoppingPlugin["find_by_category"],
                new() {
                    { "products", productCatalogService.GetFormattedAvailableProducts() },
                    { "category",  "electronics"}
                    });
            Console.WriteLine("Find by Category: " + find_by_category);

            Console.WriteLine("--- End of Semantic Functions Demo ---");
        }

        static async Task RunPluginWithDirectInvocationDemoAsync(Kernel kernel)
        {
            Console.WriteLine("--- Semantic Functions Direct Invocation Demo ---");

            List<Product> availableProducts = await kernel.InvokeAsync<List<Product>>("ProductsPlugin", "get_available_products");

            foreach (var product in availableProducts)
            {
                Console.WriteLine($"Product: {product.Name}, Description: {product.Description}, Price: {product.Price}, Category: {product.Category}");
            }

            List<Product> productsByCategory = await kernel.InvokeAsync<List<Product>>("ProductsPlugin", "get_available_products_by_category",
                new()  {
                {"products", availableProducts },
                { "category", "Books" }
                    });

            foreach (var product in productsByCategory)
            {
                Console.WriteLine($"Product: {product.Name}, Description: {product.Description}, Price: {product.Price}, Category: {product.Category}");
            }

            Console.WriteLine("--- End of Semantic Functions Direct Invocation Demo ---");
        }

        static async Task RunCartPluginChatDemoAsync(Kernel kernel, IChatCompletionService chatCompletionService)
        {
            Console.WriteLine("--- Cart Plugin with Chat Capabilities ---");

            var historyCart = new ChatHistory();

            string systemPrompt = """
                You are an AI Shopping Assistant for an online store.

                You can recommend products, compare them, and manage a shopping cart.

                If the user asks to buy or add something, add the product to the cart.

                If they ask to remove something, remove it from the cart.

                You can always show the cart contents and the total amount upon request.
                Be concise and friendly, and always confirm actions performed on the cart.
            """;

            historyCart.AddSystemMessage(systemPrompt);
            var queries = new[]
{
                "What are the available products?",
                "Add Smartphone X to my cart.",
                "Add Cloud Book to my cart.",
                "What's in my cart?",
                "Remove Cloud Book",
                "How much is the total?"
            };

            foreach (var query in queries)
            {
                await KernelHelpers.ChatAsync(kernel, chatCompletionService, historyCart, query);
            }
            
            Console.WriteLine("--- End of Cart Plugin with Chat Capabilities ---");
        }


        static async Task RunOpenApiPluginChatDemoAsync(Kernel kernel, IChatCompletionService chatCompletionService)
        {
            Console.WriteLine("--- OpenAPI Plugin with Chat Capabilities ---");

            // Remove the preoviusly imported plugins.
            var oldProductsCatalogPlugin = kernel.Plugins["ProductsPlugin"];
            kernel.Plugins.Remove(oldProductsCatalogPlugin);

            // Add the ProductsPlugin as OpenAPI.
            // The OpenAPI plugin is imported from the Swagger/OpenAPI specification.
            var plugin = await kernel.ImportPluginFromOpenApiAsync(
                pluginName: "ProductsCatalogPlugin",
                uri: new Uri("http://localhost:5054/swagger/v1/swagger.json"),
                executionParameters: new OpenApiFunctionExecutionParameters()
                {
                    // Determines whether payload parameter names are augmented with namespaces.
                    // Namespaces prevent naming conflicts by adding the parent parameter name
                    // as a prefix, separated by dots
                    EnablePayloadNamespacing = true
                }
            );

            string systemPrompt = """
                You are an AI Shopping Assistant for an online store.

                You can recommend products, compare them, and manage a shopping cart.

                If the user asks to buy or add something, add the product to the cart.

                If they ask to remove something, remove it from the cart.

                You can always show the cart contents and the total amount upon request.
                Be concise and friendly, and always confirm actions performed on the cart.
            """;

            var historyCart = new ChatHistory();
            historyCart.AddSystemMessage(systemPrompt);

            var queries = new[]
            {
                "What are the available products?",
                "Add Smartphone X to my cart.",
                "Add Cloud Book to my cart.",
                "What's in my cart?",
                "Remove Cloud Book",
                "How much is the total?"
            };

            foreach (var query in queries)
            {
                await KernelHelpers.ChatAsync(kernel, chatCompletionService, historyCart, query);
            }

            Console.WriteLine("--- End of OpenAPI Plugin with Chat Capabilities ---");
        }
    }
}

public static class ProductCatalogServiceExtensions
{
    public static string GetFormattedAvailableProducts(this ProductCatalogService productCatalogService)
    {
        var products = productCatalogService.GetAvailableProducts();
        return string.Join("\n", products.Select(product =>
            $"- Name: {product.Name}, ProductCode: {product.ProductCode}, Description: {product.Description}, Price: {product.Price}, Category: {product.Category}"));
    }
}