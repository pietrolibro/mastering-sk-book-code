using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;

using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace HelloWorldAIChatApp;

class Program
{
    static async Task Main(string[] args)
    {
        // Configuration and kernel setup.
        var useOpenAI = false;
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        var builder = Kernel.CreateBuilder();

        if (useOpenAI)
        {
            string openAIApiKey = configuration["OpenAI:ApiKey"];
            builder.AddOpenAIChatCompletion(modelId: "gpt-4.1-mini", apiKey: openAIApiKey);
        }
        else
        {
            #pragma warning disable SKEXP0001, SKEXP0020, SKEXP0050, SKEXP0070
            builder.AddOllamaChatCompletion(
                modelId: "llama3.2:latest",
                httpClient: new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:11434")
                }
            );
        }

        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var chatHistory = new ChatHistory();

        // System prompt for the assistant.
        const string assistantRole = @"You are a helpful assistant specialized in movies and TV shows.
                If user try to ask something else, simply remind the user that you are specialized in movies and TV shows and that you are not able to help with other topics.";
        chatHistory.AddMessage(AuthorRole.System, assistantRole);

        Console.WriteLine("Select demo to run:");
        Console.WriteLine("1 - Simple chat (not streaming)");
        Console.WriteLine("2 - Streaming chat");
        Console.Write("Enter 1 or 2: ");

        var demoChoice = Console.ReadLine();
        if (demoChoice == "1")
        {
            await RunSimpleChatAsync(kernel, chatCompletionService, chatHistory);
        }
        else if (demoChoice == "2")
        {
            await RunStreamingChatAsync(kernel, chatCompletionService, chatHistory);
        }
        else
        {
            Console.WriteLine("Invalid choice. Exiting.");
        }
    }

    static async Task RunSimpleChatAsync(Kernel kernel, IChatCompletionService chatCompletionService, ChatHistory chatHistory)
    {
        while (true)
        {
            Console.Write("User Input > ");
            var userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput))
                continue;
            if (userInput.Trim().ToLower() == "exit")
                break;

            chatHistory.AddUserMessage(userInput);
            ChatMessageContent response = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory: chatHistory, kernel: kernel);
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                Console.WriteLine($"AI Response> {response.Content}");
                chatHistory.AddMessage(AuthorRole.Assistant, response.Content);
            }
        }
        Console.WriteLine("Exiting the application. Goodbye!");
    }

    static async Task RunStreamingChatAsync(Kernel kernel, IChatCompletionService chatCompletionService, ChatHistory chatHistory)
    {
        while (true)
        {
            Console.Write("User Input > ");
            var userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput))
                continue;
            if (userInput.Trim().ToLower() == "exit")
                break;

            chatHistory.AddUserMessage(userInput);

            IAsyncEnumerable<StreamingChatMessageContent> streamingResponse =
                chatCompletionService.GetStreamingChatMessageContentsAsync(
                    chatHistory: chatHistory, kernel: kernel);

            string? fullResponse = string.Empty;
            bool printedRole = false;
            await foreach (StreamingChatMessageContent chunk in streamingResponse)
            {
                if (!printedRole && chunk.Role.HasValue && !string.IsNullOrEmpty(chunk.Content))
                {
                    Console.Write($"{chunk.Role.Value}: ");
                    printedRole = true;
                }
                if (!string.IsNullOrEmpty(chunk.Content))
                {
                    fullResponse += chunk.Content;
                    Console.Write(chunk.Content);
                }
            }

            if (!string.IsNullOrWhiteSpace(fullResponse))
            {
                Console.WriteLine();
                chatHistory.AddMessage(AuthorRole.Assistant, fullResponse);
            }
        }
        Console.WriteLine("Exiting the application. Goodbye!");
    }
}
