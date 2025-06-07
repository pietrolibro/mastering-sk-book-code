using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using AdvancedAIShoppingAssistant.Helpers;
using OllamaSharp;

namespace AdvancedAIShoppingAssistant.Demos;

public partial class Scenarios
{
    public static async Task RunUserProfileAsPluginDemoAsync(Kernel kernel, string systemPrompt)
    {
        Console.WriteLine("\n=== DEMO: User Profile as Plugin ===\n");

        var chatClient = kernel.GetRequiredService<IChatClient>();

        // ChatHistory chatHistory = new ChatHistory();
        List<ChatMessage> chatHistory =[new ChatMessage(ChatRole.System, systemPrompt)]; // Change Here.

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
            await KernelHelper.ChatAsync(kernel, chatClient, chatHistory, query);
        }
    }
}