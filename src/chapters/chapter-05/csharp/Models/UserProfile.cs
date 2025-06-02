using System.ComponentModel;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

using Microsoft.SemanticKernel.Data;

namespace AdvancedAIShoppingAssistant.Models;

public class UserProfile
{
    [Description("User id of the user.")]
    public string? UserId { get; set; }

    [Description("Username of the user.")]
    public string? Name { get; set; }

    [Description("Email address of the user.")]
    public string? Email { get; set; }

    [Description("Brand affinity of the user.")]
    public string? BrandAffinity { get; set; }

    [Description("Budget limit of the user.")]
    public decimal? Budget { get; set; }

    [Description("Category interests of the user.")]
    public List<string> CategoryInterests { get; set; } = new();

    [Description("Latest visited products by the user.")]
    public List<string> LatestVisitedProducts { get; set; } = new();
}

public class UserProfileVectorStore
{
    [VectorStoreKey]

    public ulong UserPofileId { get; set; }

    [VectorStoreData]
    [TextSearchResultLink]

    public string? UserId { get; set; }

    [VectorStoreData]
    [TextSearchResultName]
    public string? UserName { get; set; }

    [VectorStoreData]
    [TextSearchResultValue]
    public string? UserProfileAsText { get; set; }

    [VectorStoreVector(Dimensions: 4, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public Embedding<float>? UserProfileAsEmbedding { get; set; }
}