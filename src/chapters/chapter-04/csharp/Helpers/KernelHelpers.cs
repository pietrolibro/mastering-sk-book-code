using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
// using Microsoft.SemanticKernel.PromptTemplates;
// using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AIShoppingAssistant.Models;
using AIShoppingAssistant.Services;

namespace AIShoppingAssistant.Helpers
{
    public static class KernelHelpers
    {
        // public static async Task<string> ChatAsync(IChatCompletionService chatCompletionService, ChatHistory history, string userPrompt, Kernel kernel)
        // {
        //     history.AddUserMessage(userPrompt);
        //     Console.WriteLine($"User >>> {userPrompt}");

        //     var result = await chatCompletionService.GetChatMessageContentAsync(history,
        //         executionSettings: new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
        //         kernel: kernel);

        //     Console.WriteLine($"Assistant >>> {result}");

        //     history.AddAssistantMessage(result.Content);

        //     return result.Content;
        // }

        public static async Task<string> ChatAsync(Kernel kernel, IChatCompletionService chatCompletionService,
            ChatHistory history, string userPrompt)
        {
            history.AddUserMessage(userPrompt);

            Console.WriteLine($"User >>> {userPrompt}");


            var result = await chatCompletionService.GetChatMessageContentAsync(history,
                executionSettings: new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
                kernel: kernel);

            string content = result?.Content ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(content))// Change Here.
            {
                Console.WriteLine($"Assistant >>> {content}");
                history.AddAssistantMessage(content);
                return content;
            }

            return string.Empty;
        }
    }
}