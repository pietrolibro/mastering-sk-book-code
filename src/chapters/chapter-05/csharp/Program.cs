using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

using AdvancedAIShoppingAssistant.Demos;
using AdvancedAIShoppingAssistant.Services;
using AdvancedAIShoppingAssistant.NativePlugins;

namespace AdvancedAIShoppingAssistant;

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

#pragma warning disable SKEXP0001, SKEXP0010, SKEXP0020, SKEXP0040, SKEXP0050, SKEXP0070

        var builder = Kernel.CreateBuilder();

        if (useOpenAI)
        {
            string? openAIApiKey = configuration["OpenAI:ApiKey"];

            if (string.IsNullOrEmpty(openAIApiKey))
            {
                Console.WriteLine("OpenAI API key is not set. Please set it in your user secrets.");
                return;
            }

            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4.1-mini",
                apiKey: openAIApiKey);

            builder.AddOpenAIEmbeddingGenerator(
                modelId: "text-embedding-ada-002",
                apiKey: openAIApiKey);
        }
        else
        {
            HttpClient httpClient = new() { BaseAddress = new Uri("http://localhost:11434") };

            builder.AddOllamaChatCompletion(
                modelId: "llama3.2:latest",
                httpClient: httpClient);

            builder.AddOllamaEmbeddingGenerator(
                modelId: "nomic-embed-text:latest",
                httpClient: httpClient);
        }

        // Adding needed dependencies for the Plugins.
        builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Error));

        // Adding the services needed by the plugins and injected like dependencies.
        builder.Services.AddSingleton<ProductCatalogService>();
        builder.Services.AddSingleton<UserProfileService>();

        // Adding the Plugin to the Kernel.
        builder.Plugins.AddFromType<CartPlugin>("CartPlugin");
        builder.Plugins.AddFromType<ProductsPlugin>("ProductsPlugin");
        builder.Plugins.AddFromType<UserProfilePlugin>("UserProfilePlugin");

        builder.Services.AddInMemoryVectorStore();

        // Building the Kernel.
        var kernel = builder.Build();

        while (true)
        {
            Console.WriteLine("\n=== AI Advanced Shopping Assistant Demos ===");
            Console.WriteLine("1. Authenticated/Guest Handlebars System Prompt Template");
            Console.WriteLine("2. User Profile as Plugin");
            Console.WriteLine("3. Duplicate-Question Detection (FAQ)");
            Console.WriteLine("4. LLM-as-Judge Evaluation");
            Console.WriteLine("5. Exit");
            Console.Write("Select a demo to run (1-5): ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    string systemPrompt = await Scenarios.RunRenderSystemPromptDemo(kernel);
                    break;
                case "2":
                    systemPrompt = await Scenarios.RunRenderSystemPromptDemo(kernel);
                    await Scenarios.RunUserProfileAsPluginDemoAsync(kernel, systemPrompt);
                    break;
                case "3":
                    systemPrompt = await Scenarios.RunRenderSystemPromptDemo(kernel);
                    await Scenarios.RunDuplicateQuestionDemoAsync(kernel, systemPrompt);
                    break;
                case "4":
                    systemPrompt = await Scenarios.RunRenderSystemPromptDemo(kernel);
                    await Scenarios.RunLLMEvaluationDemoAsync(kernel, systemPrompt);
                    break;
                case "5":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    break;
            }
        }
    }
}