using Microsoft.Extensions.AI;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;

using AdvancedAIShoppingAssistant.Models;
using AdvancedAIShoppingAssistant.Helpers;


namespace AdvancedAIShoppingAssistant.Demos;
public partial class Scenarios
{
    public static async Task RunDuplicateQuestionDemoAsync(Kernel kernel, string systemPrompt)
    {
        Console.WriteLine("\n=== DEMO: Duplicate-Question Detection (FAQ) ===\n");

        var vectorStore = kernel.GetRequiredService<InMemoryVectorStore>();
        var chatClient = kernel.GetRequiredService<IChatClient>();
        var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

        var faqCollection = vectorStore.GetCollection<int, FaqRecord>("faq_products");
        await faqCollection.EnsureCollectionExistsAsync();

        // Seed a mini FAQ ------------------------------------------------------------
        var faqPairs = new (int id, string q, string a)[]
        {
                (1, "What is a good smartphone under $1000?", "Smartphone X ($999.99) packs AI‑powered features and top‑tier performance."),
                (2, "I'm looking for affordable fitness equipment.", "Resistance Bands C ($29.99) and Yoga Mat D ($24.99) are budget‑friendly options."),
                (3, "Which laptop is best for high-performance gaming?", "Laptop Z ($1299.99) delivers excellent gaming power courtesy of BrandA's latest GPU."),
                (4, "Do you have noise-canceling headphones?", "Wireless Headphones ($199.99) provide superb ANC with 30‑hour battery life."),
        };

        foreach (var (id, q, a) in faqPairs)
        {
            await faqCollection.UpsertAsync(new FaqRecord
            {
                Id = id,
                Question = q,
                Answer = a,
                QuestionEmbedding = await embeddingGenerator.GenerateAsync(q)
            });
        }

        // ChatHistory chatHistory = new();
        // chatHistory.AddSystemMessage(systemPrompt); // Change Here.

        // ChatHistory chatHistory = new ChatHistory();
        List<ChatMessage> chatHistory =[new ChatMessage(ChatRole.System, systemPrompt)]; // Change Here.


        var queries = new[]
        {
            "What is a good smartphone under $1000?",
            "Looking for cheaper fitness equipment.",
            "I'm looking for Hairdry"
        };

        foreach (var query in queries)
        {
            await KernelHelper.ChatAsync(kernel,chatClient, embeddingGenerator, chatHistory, query, faqCollection);
        }

    }
}