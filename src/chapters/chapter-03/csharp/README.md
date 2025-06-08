# AI Shopping Assistant (C#)

## Overview
This project implements an AI-powered shopping assistant using C#. It demonstrates how to use Semantic Kernel to build an interactive assistant for shopping scenarios.

## Prerequisites
- .NET 8 SDK or later

## Setup
```zsh
dotnet restore
```

## Add Required Packages
Add the necessary NuGet packages as referenced in the project:
```zsh
dotnet add package Microsoft.Extensions.Configuration --version 9.0.5
dotnet add package Microsoft.Extensions.Configuration.UserSecrets --version 9.0.5
dotnet add package Microsoft.SemanticKernel --version 1.54.0
dotnet add package Microsoft.SemanticKernel.Connectors.Ollama --version 1.54.0-alpha
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI --version 1.54.0
dotnet add package Microsoft.SemanticKernel.PromptTemplates.Handlebars --version 1.54.0
dotnet add package Microsoft.SemanticKernel.PromptTemplates.Liquid --version 1.54.0-preview
dotnet add package Microsoft.SemanticKernel.Yaml --version 1.54.0
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
- Configure any required API keys or user secrets for AI services.
- For production, use a secure method to store secrets (such as Azure Key Vault).
