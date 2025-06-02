using System.ComponentModel;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace AdvancedAIShoppingAssistant.Models;

public sealed class FaqRecord
{
    [VectorStoreKey]
    public int? Id { get; set; } = -1;

    [VectorStoreData]
    public string? Question { get; set; } = "";

    [VectorStoreData]
    public string? Answer { get; set; } = "";

    [VectorStoreVector(1536)]
    public Embedding<float>? QuestionEmbedding { get; set; } = null!;
}