# HelloWorldSKChat (C#)

## Overview
This project demonstrates a simple Semantic Kernel chat application using C#. It showcases how to interact with AI models for basic chat functionality.

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
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.DependencyInjection
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
- Ensure you have any required API keys or user secrets set if the project uses external AI services.
- For production, use a secure method to store secrets (such as Azure Key Vault).
