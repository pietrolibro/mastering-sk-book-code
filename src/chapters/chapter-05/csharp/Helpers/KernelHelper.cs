using Microsoft.SemanticKernel;
// using Microsoft.SemanticKernel.Data;
// using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.PromptTemplates;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

using AdvancedAIShoppingAssistant.Models;

namespace AdvancedAIShoppingAssistant.Helpers;

#pragma warning disable SKEXP0001, SKEXP0010, SKEXP0020, SKEXP0040, SKEXP0050, SKEXP0070

public static class KernelHelper
{
    public static async Task<string> ChatAsync(Kernel kernel,
        IChatCompletionService chatCompletionService,
        ChatHistory history,
        string userPrompt)
    {
        history.AddUserMessage(userPrompt);
        Console.WriteLine($"User >>> {userPrompt}");

        var result = await chatCompletionService.GetChatMessageContentAsync(history,
            executionSettings: new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
            kernel: kernel);

        string content = result?.Content ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(content))
        {
            Console.WriteLine($"Assistant >>> {content}");
            history.AddAssistantMessage(content);
            return content;
        }

        return string.Empty;
    }

    public static async Task<string> ChatAsync(IChatCompletionService chatCompletionService,
        Microsoft.Extensions.AI.IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        ChatHistory history, string userPrompt,
        Kernel kernel,
        VectorStoreCollection<int, FaqRecord> faqCollection)
    {
        history.AddUserMessage(userPrompt);
        Console.WriteLine($"User >>> {userPrompt}");

        Microsoft.Extensions.AI.Embedding<float> incomingVec = await embeddingGenerator.GenerateAsync(userPrompt);
        var bestMatch = await faqCollection.SearchAsync(incomingVec, top: 1).FirstOrDefaultAsync();

        string? answer = bestMatch != null && bestMatch.Score >= 0.90
            ? bestMatch.Record.Answer
            : (await chatCompletionService.GetChatMessageContentAsync(history,
                executionSettings: new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
                kernel: kernel)).Content;

        Console.WriteLine(bestMatch != null && bestMatch.Score >= 0.90
            ? $"Duplicate detected (score {bestMatch.Score:F2}) returning cached answer:\n{answer}\n"
            : "No close duplicate asking the model …\n");

        Console.WriteLine($"Assistant >>> {answer}");
        history.AddAssistantMessage(answer);

        return answer;
    }

    public static KernelFunction CreateSemanticFunction(Kernel kernel,
        string promptTemplate, string functionName, bool useOpenAI, string templateFormat = "semantic-kernel",
        IPromptTemplateFactory? promptTemplateFactory = default, float temperature = 0.6f, int maxTokens = 100)
    {
        PromptExecutionSettings executionSettings = useOpenAI
            ?
             new OpenAIPromptExecutionSettings { Temperature = temperature, MaxTokens = maxTokens }
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
    
    /// <summary>
    /// This function takes the following inputs as arguments: Kernel, promptTemplate, KernelArguments, templateFormat, and an optional promptTemplateFactory.
    /// It returns a string displaying the rendered template in the terminal. This allows us to preview how the final prompt will be structured before sending it to the AI model.
    /// </summary>
    /// <param name="kernel">The Kernel instance used for rendering the template.</param>
    /// <param name="promptTemplate">The template string to be rendered.</param>
    /// <param name="arguments">The KernelArguments containing the data to be injected into the template.</param>
    /// <param name="templateFormat">The format of the template (default is "semantic-kernel").</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the rendered template string.</returns>
    public static async Task<string> RenderPromptTemplateAsync(Kernel kernel, string promptTemplate, KernelArguments arguments,string templateFormat = "semantic-kernel")
    {
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = promptTemplate,
            TemplateFormat = templateFormat,
        };

        IPromptTemplateFactory promptTemplateFactory = templateFormat?.Equals("handlebars", StringComparison.OrdinalIgnoreCase) == true
            ? new HandlebarsPromptTemplateFactory()
            : new KernelPromptTemplateFactory();

        var template = promptTemplateFactory.Create(promptTemplateConfig);

        var renderedPrompt = await template.RenderAsync(kernel, arguments);

        return renderedPrompt;
    }
}