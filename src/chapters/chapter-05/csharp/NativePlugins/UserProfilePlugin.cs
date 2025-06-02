
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using System.ComponentModel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using AdvancedAIShoppingAssistant.Models;
using AdvancedAIShoppingAssistant.Services;

namespace AdvancedAIShoppingAssistant.NativePlugins;

public class UserProfilePlugin
{
    private readonly ILogger<UserProfilePlugin> _logger;
    private readonly UserProfileService _userProfileService;

    /// <summary>
    /// Initializes a new instance of the UserProfilePlugin class with logging and user profile service.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userProfileService">The user profile service.</param>
    public UserProfilePlugin(ILogger<UserProfilePlugin> logger, UserProfileService userProfileService)
    {
        _logger = logger;
        _userProfileService = userProfileService;
    }

    /// <summary>
    /// Retrieves the budget limit for a specific user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>The budget preference or an error message.</returns>
    [KernelFunction("get_budget_limit")]
    [Description("Retrieves the budget limit for a specific user.")]
    public string GetBudgetLimit([Description("The user id of the user.")] string userId)
    {
        if (!_userProfileService.UserProfiles.ContainsKey(userId))
        {
            return $"User with username '{userId}' does not exist.";
        }

        var profile = _userProfileService.UserProfiles[userId];

        return $"Budget for {userId}: $ {profile.Budget}";
    }

    /// <summary>
    /// Retrieves the brand affinity for a specific user.
    /// </summary>
    /// <param name="userId">The user id of the user.</param>
    /// <returns>The brand affinity or an error message.</returns>
    [KernelFunction("get_brand_affinity")]
    [Description("Retrieves the brand affinity for a specific user.")]
    public string GetBrandAffinity(
        [Description("The user Id of the user.")] string userId)
    {
        if (!_userProfileService.UserProfiles.ContainsKey(userId))
        {
            return $"User with username '{userId}' does not exist.";
        }

        var profile = _userProfileService.UserProfiles[userId];

        return $"Brand Affinity for {userId}: {profile.BrandAffinity}";
    }

    /// <summary>
    /// Retrieves the category interests for a specific user.
    /// </summary>
    /// <param name="userId">The user id of the user.</param>
    /// <returns>The category interests or an error message.</returns>
    [KernelFunction("get_category_interests")]
    [Description("Retrieves the categories of interests for a specific user.")]
    public string GetCategoryInterests(
        [Description("The user id of the user.")] string userId)
    {
        if (!_userProfileService.UserProfiles.ContainsKey(userId))
        {
            return $"User with username '{userId}' does not exist.";
        }

        var profile = _userProfileService.UserProfiles[userId];
        return $"Category Interests for {userId}: {string.Join(", ", profile.CategoryInterests)}";
    }

    /// <summary>
    /// Retrieves the email address for a specific user.
    /// </summary>
    /// <param name="userId">The user id of the user.</param>
    /// <returns>The email address of the user. </returns>
    [KernelFunction("get_email_address")]
    [Description("Retrieves the email address for a specific user.")]
    public string GetEmailAddress([Description("The user id of the user.")] string userId)
    {
        if (!_userProfileService.UserProfiles.ContainsKey(userId))
        {
            return $"User with username '{userId}' does not exist.";
        }

        var profile = _userProfileService.UserProfiles[userId];
        return $"Email Address for {userId}: {profile.Email}";
    }

    /// <summary>
    /// Retrieves the latest 5 visited products for a specific user.
    /// </summary>
    /// <param name="userId">The user id of the user.</param>
    /// <returns>The latest 5 visited products or an error message.</returns>
    [KernelFunction("get_latest_visited_products")]
    [Description("Retrieves the latest visited products for a specific user.")]
    public string GetLatestVisitedProducts(
        [Description("The user id of the user.")] string userId)
    {
        if (!_userProfileService.UserProfiles.ContainsKey(userId))
        {
            return $"User with username '{userId}' does not exist.";
        }

        var profile = _userProfileService.UserProfiles[userId];

        return $"Latest Visited Products for {userId}: {string.Join(", ", profile.LatestVisitedProducts)}";
    }
}