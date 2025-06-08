# Chapter 4 - C# AI Shopping Assistant

This folder contains the C# implementation of the AI Shopping Assistant demo for Chapter 4 of the book.

## Structure
- `Program.cs`: Main entry point for the C# AI Shopping Assistant.
- `Models/`: Contains the `Product` model and related data classes.
- `Services/`: Business logic and product catalog service.
- `NativePlugins/`: Native plugins for cart and product management.
- `Helpers/`: Helper classes for chat and kernel operations.

## Features
- Product catalog management
- Cart management with chat capabilities
- Integration with OpenAI and Ollama
- OpenAPI plugin support

## Setup
```zsh
dotnet restore
```

## Add Required Packages
Add the necessary NuGet packages as referenced in the project:
```zsh
dotnet add package Microsoft.Extensions.Configuration --version 9.0.5
dotnet add package Microsoft.Extensions.Configuration.UserSecrets --version 9.0.5
dotnet add package Microsoft.Extensions.Logging --version 9.0.5
dotnet add package Microsoft.Extensions.Logging.Console --version 9.0.5
dotnet add package Microsoft.SemanticKernel --version 1.54.00
dotnet add package Microsoft.SemanticKernel.Connectors.Ollama --version 1.54.0-alpha
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI --version 1.54.0
dotnet add package Microsoft.SemanticKernel.Plugins.OpenApi --version 1.54.0
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

## Prerequisite: Run the AI Shopping API
For the use cases in this project, you must first build and run the console web API app `ai-shopping-api-cs` located in the `ai-shopping-api-cs` folder.

To build and run the API:
```powershell
cd ../ai-shopping-api-cs
# Build the API project
 dotnet build
# Run the API
 dotnet run
```
Leave this API running in a separate terminal window while you use the AI Shopping Assistant app.

## Running the Project
```zsh
dotnet run
```

## Notes
- Configure any required API keys or user secrets for AI services.
- For production, use a secure method to store secrets (such as Azure Key Vault).

---
For more details, see the book or the main project README.
