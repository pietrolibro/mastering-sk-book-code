using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.PromptTemplates;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using Microsoft.Extensions.Configuration;

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

        var kernel = builder.Build();

        while (true)
        {
            Console.WriteLine("Select demo to run:");
            Console.WriteLine("1 - Basic Semantic Function");
            Console.WriteLine("2 - Semantic Function with SK Prompt Template");
            Console.WriteLine("3 - Semantic Function with Handlebars Template");
            Console.WriteLine("4 - Semantic Function with Liquid Template");
            Console.WriteLine("5 - Semantic Function from YAML");
            Console.WriteLine("0 - Exit");
            Console.Write("Enter 1, 2, 3, 4, 5 or 0: ");
            var demoChoice = Console.ReadLine();

            switch (demoChoice)
            {
                case "1":
                    await RunBasicSemanticFunctionAsync(kernel);
                    break;
                case "2":
                    await RunSKPromptTemplateAsync(kernel);
                    break;
                case "3":
                    await RunHandlebarsTemplateAsync(kernel, useOpenAI);
                    break;
                case "4":
                    await RunLiquidTemplateAsync(kernel, useOpenAI);
                    break;
                case "5":
                    await RunYamlTemplateAsync(kernel);
                    break;
                case "0":
                    Console.WriteLine("Exiting.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
            Console.WriteLine();
        }
    }

    static async Task RunBasicSemanticFunctionAsync(Kernel kernel)
    {
        var prompt = "I'm looking for a Book to learn about cloud. Do you have something?";
        var function = kernel.CreateFunctionFromPrompt(prompt);
        var result = await kernel.InvokeAsync(function);
        Console.WriteLine(result);
    }

    static async Task RunSKPromptTemplateAsync(Kernel kernel)
    {
        string systemPrompt = """
            You are a helpful assistant specialized in assisting customers with shopping. 

            Following are the products available in the shop:

            {{$products}}
            {{$input}}
        """;

        var userPrompt = "I'm looking for a product in the Electronics category Can you recommend 2 of them?";

        var arguments = new KernelArguments {
            { "products", GetAvailableProductsAsObjects().ToProductListString() },
            { "input", userPrompt }
        };

        var renderedPrompt = await kernel.RenderPromptTemplateAsync(systemPrompt, arguments);

        Console.WriteLine($"Rendered Prompt:\n{renderedPrompt}\n");
    }

    static async Task RunHandlebarsTemplateAsync(Kernel kernel, bool useOpenAI)
    {
        string handlebarsPromptTemplate = """
            <message role="system">
                You are a helpful assistant specialized in assisting customers with shopping.

                Following are the products available in the shop:

                {{#each products}}
                - Name: {{this.name}} 
                - Description: {{this.description}} 
                - Price: ${{this.price}} 
                - Category: {{this.category}}

                {{/each}}

            </message>
            <message role="user">
            {{input}}
            </message>
        """;

        var handlebarsTemplateFactory = new HandlebarsPromptTemplateFactory();

        var arguments = new KernelArguments {
            { "products", GetAvailableProductsAsObjects() },
            { "input", "I'm looking for a product in the Electronics category Can you recommend me something?" } };

        var renderedPrompt = await kernel.RenderPromptTemplateAsync(
                handlebarsPromptTemplate,
                arguments, "handlebars",
                handlebarsTemplateFactory);

        Console.WriteLine($"Rendered Prompt:\n{renderedPrompt}\n");

        var skFunction = kernel.CreateSemanticFunction(handlebarsPromptTemplate, "function01",
                useOpenAI, "handlebars", handlebarsTemplateFactory);

        var skResult = await kernel.InvokeAsync(skFunction, arguments);

        Console.WriteLine(skResult);
    }

    static async Task RunLiquidTemplateAsync(Kernel kernel, bool useOpenAI)
    {
        string liquidPromptTemplate = """
            <message role="system">
                You are a helpful assistant specialized in assisting customers with shopping.

                Following are the products available in the shop:

                {% for product in products %}
                - Name: {{product.name}} 
                - Description: {{product.description}} 
                - Price: ${{product.price}} 
                - Category: {{product.category}}

                {% endfor %}

            </message>
            <message role="user">
            {{input}}
            </message>
        """;

        var liquidTemplateFactory = new LiquidPromptTemplateFactory();

        var arguments = new KernelArguments {
            { "products", GetAvailableProductsAsObjects() },
            { "input", "I'm looking for a product in the Electronics category Can you recommend me something?" }
        };

        var renderedPrompt = await kernel.RenderPromptTemplateAsync(liquidPromptTemplate,
            arguments, "liquid", liquidTemplateFactory);

        Console.WriteLine($"Rendered Prompt:\n{renderedPrompt}\n");

        var skFunction = kernel.CreateSemanticFunction(liquidPromptTemplate,
            "function04", useOpenAI, "liquid", liquidTemplateFactory);

        var skResult = await kernel.InvokeAsync(skFunction, arguments);

        Console.WriteLine(skResult);
    }

    static async Task RunYamlTemplateAsync(Kernel kernel)
    {
        var yamlPromptTemplate = System.IO.File.ReadAllText("ai_shopping_assistant.yaml");

        var handlebarsTemplateFactory = new HandlebarsPromptTemplateFactory();

        var skFunctionYaml = KernelFunctionYaml.FromPromptYaml(yamlPromptTemplate, handlebarsTemplateFactory);

        var arguments = new KernelArguments {
                { "products", GetAvailableProductsAsObjects() },
                { "input", "I'm looking for a product in the Electronics category Can you recommend me something?" } };

        var skResultYaml = await kernel.InvokeAsync(skFunctionYaml, arguments);

        Console.WriteLine(skResultYaml);
    }

    public static List<Product> GetAvailableProductsAsObjects()
    {
        return new List<Product>
        {
            new() { Name = "Smartphone X", Description = "A powerful smartphone with AI-driven features", Price = 999.99m, Category = "Electronics" },
            new() { Name = "Wireless Headphones Y", Description = "Noise-canceling wireless headphones with immersive sound", Price = 199.99m, Category = "Electronics" },
            new() { Name = "Laptop Z", Description = "High-performance laptop for gaming and productivity", Price = 1299.99m, Category = "Electronics" },
            new() { Name = "AI for Beginners", Description = "A beginner-friendly book on Artificial Intelligence", Price = 29.99m, Category = "Books" },
            new() { Name = "The Semantic Kernel Guide", Description = "Comprehensive guide on Microsoft Semantic Kernel", Price = 39.99m, Category = "Books" },
            new() { Name = "Cloud Computing Essentials", Description = "An introduction to cloud computing concepts", Price = 49.99m, Category = "Books" },
            new() { Name = "Smartwatch A", Description = "Fitness smartwatch with heart rate monitoring", Price = 149.99m, Category = "Fitness" },
            new() { Name = "Treadmill B", Description = "High-quality treadmill for home workouts", Price = 799.99m, Category = "Fitness" },
            new() { Name = "Resistance Bands C", Description = "Set of resistance bands for strength training", Price = 29.99m, Category = "Fitness" },
            new() { Name = "Tablet P", Description = "Lightweight tablet with a stunning display", Price = 499.99m, Category = "Electronics" },
            new() { Name = "Cybersecurity Basics", Description = "Essential guide on online security and best practices", Price = 34.99m, Category = "Books" },
            new() { Name = "Yoga Mat D", Description = "Premium yoga mat for comfortable workouts", Price = 24.99m, Category = "Fitness" }
        };
    }
}

public class Product
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}

public static class ProductExtensions
{
    public static string ToProductListString(this IEnumerable<Product> products)
    {
        return string.Join("\n", products.Select(p => $"- Name: {p.Name}, Description: {p.Description}, PRICE: ${p.Price}, Category: {p.Category}"));
    }
}

public static class KernelExtensions
{
    public static async Task<string> RenderPromptTemplateAsync(this Kernel kernel, string promptTemplate, KernelArguments arguments,
        string templateFormat = "semantic-kernel", IPromptTemplateFactory? promptTemplateFactory = default)
    {
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = promptTemplate,
            TemplateFormat = templateFormat,
        };
        if (promptTemplateFactory == null) { promptTemplateFactory = new KernelPromptTemplateFactory(); }
        var template = promptTemplateFactory.Create(promptTemplateConfig);
        var renderedPrompt = await template.RenderAsync(kernel, arguments);
        return renderedPrompt;
    }

    public static KernelFunction CreateSemanticFunction(this Kernel kernel,
        string promptTemplate, string functionName, bool useOpenAI, string templateFormat = "semantic-kernel",
        IPromptTemplateFactory? promptTemplateFactory = default, float temperature = 0.6f, int maxTokens = 100)
    {
        PromptExecutionSettings executionSettings = useOpenAI
            ? new OpenAIPromptExecutionSettings { Temperature = temperature, MaxTokens = maxTokens }
            : new OllamaPromptExecutionSettings { Temperature = temperature, NumPredict = maxTokens };
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = promptTemplate,
            TemplateFormat = templateFormat,
            Name = functionName,
            ExecutionSettings = new Dictionary<string, PromptExecutionSettings>()
            {
                { PromptExecutionSettings.DefaultServiceId, executionSettings }
            }
        };
        var skFunction = kernel.CreateFunctionFromPrompt(promptTemplateConfig, promptTemplateFactory);
        return skFunction;
    }
}