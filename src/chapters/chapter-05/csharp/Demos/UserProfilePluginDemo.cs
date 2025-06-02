using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using AdvancedAIShoppingAssistant.Helpers;

namespace AdvancedAIShoppingAssistant.Demos;

public partial class Scenarios
{
    public static async Task RunUserProfileAsPluginDemoAsync(Kernel kernel, string systemPrompt)
    {
        Console.WriteLine("\n=== DEMO: User Profile as Plugin ===\n");

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        ChatHistory chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(systemPrompt);

        var queries = new[]
        {
            "What is my user id?",
            "What is my email address?",
            "What is budget limit?",
            "Can You suggest some products?",
            "What is my category of interest?"
        };

        foreach (var query in queries)
        {
            await KernelHelper.ChatAsync(kernel, chatCompletionService, chatHistory, query);
        }
    }
}