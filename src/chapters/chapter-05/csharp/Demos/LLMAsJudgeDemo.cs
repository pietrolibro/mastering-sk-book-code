using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;

using OpenAI.Chat;

using Microsoft.Extensions.AI;

using AdvancedAIShoppingAssistant.Models;
using AdvancedAIShoppingAssistant.Helpers;
using AdvancedAIShoppingAssistant.Services;
using AdvancedAIShoppingAssistant.NativePlugins;

namespace AdvancedAIShoppingAssistant.Demos;
public partial class Scenarios
{
    public static async Task RunLLMEvaluationDemoAsync(Kernel kernel, string systemPrompt)
    {
        Console.WriteLine("\n=== DEMO: LLM-as-Judge Evaluation ===\n");

        var vectorStore = kernel.GetRequiredService<InMemoryVectorStore>();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var textEmbeddingGenerationService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

        var faqCollection = vectorStore.GetCollection<int, FaqRecord>("faq_products");
        await faqCollection.EnsureCollectionExistsAsync();

        // Seed a mini FAQ ------------------------------------------------------------
        var faqPairs = new (int id, string q, string a)[]
        {
                (1, "What is a good smartphone under $1000?", "Smartphone X ($999.99) packs AI-Powered features and top tier performance."),
                (2, "I'm looking for affordable fitness equipment.", "Resistance Bands C ($29.99) and Yoga Mat D ($24.99) are budget friendly options."),
                (3, "Which laptop is best for high-performance gaming?", "Laptop Z ($1299.99) delivers excellent gaming power courtesy of BrandA's latest GPU."),
                (4, "Do you have noise-canceling headphones?", "Wireless Headphones ($199.99) provide superb ANC with 30 hour battery life."),
        };

        foreach (var (id, q, a) in faqPairs)
        {
            await faqCollection.UpsertAsync(new FaqRecord
            {
                Id = id,
                Question = q,
                Answer = a,
                QuestionEmbedding = await textEmbeddingGenerationService.GenerateAsync(q)
            });
        }

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(systemPrompt);

        var queries = new[] {
        "What is a good smartphone under $1000?",
        "Looking for cheaper fitness equipment."
    };

        foreach (var query in queries)
        {
            // Get assistant response
            string assistantResponse = await KernelHelper.ChatAsync(chatCompletionService, textEmbeddingGenerationService, chatHistory, query, kernel, faqCollection);

            // Evaluate the response using LLM-as-Judge
            string evaluationPrompt = """
                You are acting as an AI evaluation assistant. Your task is to assess how relevant and context-aware the following product recommendation or response is, based on the user input.

                Rate the assistan's reply on a scale from 1 to 6:
                1 = Completely irrelevant  
                2 = Mostly irrelevant or off-topic  
                3 = Somewhat relevant, but lacks personalization or context awareness  
                4 = Mostly relevant, uses some context or memory appropriately  
                5 = Very relevant and mostly personalized  
                6 = Fully relevant, highly personalized, and contextually accurate

                Use the SearchPlugin to find relevant user profile information for the response evaulation.
                User id: 
                {{$user_id}}
                User name: 
                {{$user_name}}

                User Input:
                {{$query}}

                Assistant Response:
                {{$response}}

                Please respond with a single numeric score (1 to 6) and no additional explanation.

                Score:
            """;

            var fn = KernelHelper.CreateSemanticFunction(kernel, evaluationPrompt, "fn_evaluate_response", true);

            var evaluationResult = await fn.InvokeAsync(kernel, new KernelArguments()
            {
                { "query", query },
                { "user_id", "user2" },
                { "user_name", "Bob" },
                { "response", assistantResponse }
            });

            Console.WriteLine($"Evaluation Score: {evaluationResult}");
        }
    }
}