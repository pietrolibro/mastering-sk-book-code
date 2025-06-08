# Advanced AI Shopping Assistant (C#)

## Overview
This project is an advanced AI-powered shopping assistant built with C#. It demonstrates complex scenarios, including plugins, user profiles, and advanced chat features using Semantic Kernel.

## Prerequisites
- .NET 8 SDK or later

## Setup
```zsh
dotnet restore
```

## Add Required Packages
Add the necessary NuGet packages as referenced in the project:
```zsh
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.Extensions.Http
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
```

## User Secrets (Recommended for API Keys and Sensitive Data)
This project supports [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for managing sensitive configuration during development.

To set a secret (e.g., an API key):
```zsh
dotnet user-secrets init
# Replace KEY and VALUE with your actual secret name and value
# Example: dotnet user-secrets set "OpenAI:ApiKey" "sk-...your_api_key_here..."
dotnet user-secrets set "<KEY>" "<VALUE>"
```

Secrets set this way are not committed to source control and are available to your app via configuration.

## Running the Project
```zsh
dotnet run
```

## Notes
- Ensure all required API keys or user secrets are configured for AI services.
- For production, use a secure method to store secrets (such as Azure Key Vault).
