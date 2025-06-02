using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel;
using System.Text.Json.Serialization;

using AdvancedAIShoppingAssistant.Models;

namespace AdvancedAIShoppingAssistant.Services;

public class UserProfileService
{
    private readonly Dictionary<string, UserProfile> _userProfiles = new();

    public UserProfileService()
    {
        // Initialize with 3 fictitious users.
        _userProfiles.Add("user1", new UserProfile
        {
            UserId = "user1",
            Name = "Alice",
            Email = "alice@example.com",
            BrandAffinity = "BrandA",
            Budget = 500,
            CategoryInterests = new List<string> { "Electronics", "Books" },
            LatestVisitedProducts = new List<string> { "Smartphone X", "Cloud Book" }
        });

        _userProfiles.Add("user2",new UserProfile
        {
            UserId = "user2",
            Name = "Bob",
            Email = "bob@example.com",
            BrandAffinity = "BrandB",
            Budget = 1000,
            CategoryInterests = new List<string> { "Fitness", "Electronics" },
            LatestVisitedProducts = new List<string> { "Treadmill", "Smartwatch" }
        });

        _userProfiles.Add("user3", new UserProfile
        {
            UserId = "user3",
            Name = "Charlie",
            Email = "charlie@example.com",
            BrandAffinity = "BrandC",
            Budget = 200,
            CategoryInterests = new List<string> { "Books", "Fitness" },
            LatestVisitedProducts = new List<string> { "Yoga Mat", "Resistance Bands" }
        });
    }

    public Dictionary<string, UserProfile> UserProfiles
    {
        get => _userProfiles;
    }
}
