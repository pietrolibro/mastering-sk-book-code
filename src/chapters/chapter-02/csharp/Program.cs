using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Microsoft.Extensions.Configuration;

using OllamaSharp.Models.Chat;

namespace HelloWorldSKChat;

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
        Console.WriteLine("3 - Streaming chat, with token stats");
        Console.Write("Enter 1,2 or 3: ");

        var demoChoice = Console.ReadLine();
        if (demoChoice == "1")
        {
            await RunSimpleChatAsync(kernel, chatCompletionService, chatHistory);
        }
        else if (demoChoice == "2")
        {
            await RunStreamingChatAsync(kernel, chatCompletionService, chatHistory);
        }
        else if (demoChoice == "3")
        {
            await RunStreamingChatWithStatsAsync(kernel, chatCompletionService, chatHistory);
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
            chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory: chatHistory);

            string fullResponse = await StreamMessageOutputAsync(streamingResponse);

            if (!string.IsNullOrWhiteSpace(fullResponse))
            {
                Console.WriteLine();
                chatHistory.AddMessage(AuthorRole.Assistant, fullResponse);
            }
        }
        Console.WriteLine("Exiting the application. Goodbye!");
    }

    static async Task RunStreamingChatWithStatsAsync(Kernel kernel, IChatCompletionService chatCompletionService, ChatHistory chatHistory)
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
            chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory: chatHistory);

            string fullResponse = await StreamMessageOutputWithTokensStatsAsync(streamingResponse);

            if (!string.IsNullOrWhiteSpace(fullResponse))
            {
                Console.WriteLine();
                chatHistory.AddMessage(AuthorRole.Assistant, fullResponse);
            }
        }
        Console.WriteLine("Exiting the application. Goodbye!");
    }

    static async Task<string> StreamMessageOutputAsync(IAsyncEnumerable<StreamingChatMessageContent> streamingResponse)
    {
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
        return fullResponse;
    }

    /// <summary>
    /// This method is used to stream the message output with the token stats.
    /// Reference:
    /// - https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Ollama_ChatCompletionStreaming.cs
    /// - https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/OpenAI_ChatCompletionStreaming.cs
    /// </summary>
    /// <param name="streamingResponse"></param>
    /// <returns></returns>
    private static async Task<string> StreamMessageOutputWithTokensStatsAsync(IAsyncEnumerable<StreamingChatMessageContent> streamingResponse)
    {
        string? fullResponse = string.Empty;
        await foreach (StreamingChatMessageContent chunk in streamingResponse)
        {
            if (chunk is OpenAIStreamingChatMessageContent openAIChunk)
            {
                fullResponse += openAIChunk.Content;
                Console.Write(openAIChunk.Content);

                // The last message in the chunk has the usage metadata.
                // https://platform.openai.com/docs/api-reference/chat/create#chat-create-stream_options
                if (openAIChunk.Metadata?["Usage"] is not null) OutputInnerContent(openAIChunk);
            }

            if (chunk.InnerContent is OllamaSharp.Models.Chat.ChatResponseStream ollamaChunk)
            {
                // https://github.com/awaescher/OllamaSharp/blob/main/src/Models/Chat/Message.cs
                fullResponse += ollamaChunk?.Message?.Content;
                Console.Write(ollamaChunk?.Message?.Content);
            }

            if (chunk.InnerContent is OllamaSharp.Models.Chat.ChatDoneResponseStream chatDoneResponse)
            {
                OutputInnerContent(chatDoneResponse);
            }
        }
        return fullResponse;
    }

    private static void OutputInnerContent(OpenAIStreamingChatMessageContent? doneStream)
    {
        if (doneStream is null) { return; }
        // https://github.com/openai/openai-dotnet/blob/main/src/Generated/Models/ChatTokenUsage.cs
        OpenAI.Chat.ChatTokenUsage? usage = doneStream.Metadata?["Usage"] as OpenAI.Chat.ChatTokenUsage;

        Console.WriteLine("---------- STATS --------------");
        Console.WriteLine($"Output Tokens: {usage?.InputTokenCount}");
        Console.WriteLine($"Input Tokens: {usage?.OutputTokenCount}");
        Console.WriteLine($"Total Tokens: {usage?.TotalTokenCount}");
        Console.WriteLine("-------------------------------");
    }

    private static void OutputInnerContent(ChatDoneResponseStream doneStream)
    {
        if (doneStream is null) { return; }
        // https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Concepts/ChatCompletion/Ollama_ChatCompletionStreaming.cs#L259
        Console.WriteLine("");
        Console.WriteLine("---------- STATS --------------");
        Console.WriteLine($"The number of tokens in the response: {doneStream.EvalCount}");
        Console.WriteLine($"The number of tokens in the prompt: {doneStream.PromptEvalCount}");
        Console.WriteLine($"Prompt eval duration: {doneStream.PromptEvalDuration}");
        Console.WriteLine("-------------------------------");
    }
}
