using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;

using Microsoft.SemanticKernel.PromptTemplates;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

using OpenAI.Chat;

using AdvancedAIShoppingAssistant.Models;
using AdvancedAIShoppingAssistant.Helpers;
using AdvancedAIShoppingAssistant.Services;
using AdvancedAIShoppingAssistant.NativePlugins;

namespace AdvancedAIShoppingAssistant.Demos;
public partial class Scenarios
{
   public static async Task<string> RunRenderSystemPromptDemo(Kernel kernel)
    {
        Console.WriteLine("\n=== DEMO: Authenticated/Guest Handlebars System Prompt Template ===\n");

        var historyCart = new ChatHistory();

        string systemPromptTemplate = """
            <message role="system">
            You are an AI Shopping Assistant for a web store.

            You can:
            - Recommend products based on user preferences or product views
            - Suggest max {{num_of_products_to_suggest}} products at time.
            - Compare items
            - Manage a shopping cart (add, remove, total)
            - Answer questions about the catalog

            {{#if user_is_authenticated}}

                The user is logged in and has a profile. Use the user_profile_plugin plugin to find relevant user profile information:
                
                User id: {{user_id}}
                User name: {{user_name}}
                
                You must:
                - Greet and address the user by their name to personalize the conversation.
                - Use the user ID to retrieve their profile and preferences.
                - Recommend products based on the user's stored preferences, including favorite brands, category interests, and budget.
                - Always place products from the user's favorite brands at the top of the recommendations list.
                - Do not repeatedly ask the user for preferences, brand choices, or budget unless they explicitly request to update or provide them.
                - Sort the recommended products by Brand Affinity (favorite brands first), followed by Category Interests, and then by Budget suitability.

            {{else}}

                The user is a Guest. Recommend popular or generally useful products. IF is not required do not recommend books.

            {{/if}}

            </message>
        """;

        // Create the prompt template using handlebars format
        var templateFactory = new HandlebarsPromptTemplateFactory();
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = systemPromptTemplate,
            TemplateFormat = "handlebars",
            Name = "AIShoppingAssistantSystemTemplate",
        };

        var arguments = new KernelArguments()
        {
            { "user_is_authenticated", true },
            { "user_name", "Bob" },
            { "user_id", "user2" },
            { "num_of_products_to_suggest", 3 }
        };

        // Render the System Prompt.
        var renderedSystemPrompt = await KernelHelper.RenderPromptTemplateAsync(kernel,
            systemPromptTemplate, arguments, "handlebars");

        Console.WriteLine($"Rendered Prompt:\n{renderedSystemPrompt}\n");

        return renderedSystemPrompt;
    }
}