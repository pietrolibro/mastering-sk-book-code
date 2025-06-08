# Chapter 4 - Python AI Shopping Assistant

This folder contains the Python implementation of the AI Shopping Assistant demo for Chapter 4 of the book.

## Structure
- `main.py`: Main entry point for the Python AI Shopping Assistant.
- `models/`: Contains the `Product` dataclass and related models.
- `services/`: Business logic and product catalog service.
- `native_plugins/`: Native plugins for cart and product management.
- `ai-shopping-assistant-env/`: Python virtual environment (not included in version control).

## Features
- Product catalog management
- Cart management with chat capabilities
- Integration with OpenAI and Ollama
- OpenAPI plugin support

## Prerequisite: Run the AI Shopping API
For the use cases in this project, you must first build and run the console web API app `ai-shopping-api-cs` located in the `../ai-shopping-api-cs` folder.

To build and run the API (from the chapter-04 folder):
```powershell
cd ../ai-shopping-api-cs
# Build the API project
dotnet build
# Run the API
dotnet run
```
Leave this API running in a separate terminal window while you use the Python AI Shopping Assistant app.

## How to Run
1. Ensure you have Python 3.10+ installed.
2. Create and activate a virtual environment:
   ```sh
   python -m venv ai-shopping-assistant-env
   ai-shopping-assistant-env\Scripts\activate  # On Windows
   source ai-shopping-assistant-env/bin/activate  # On macOS/Linux
   ```
3. Install dependencies:
   ```sh
   pip install -r requirements.txt
   ```
4. Run the app:
   ```sh
   python main.py
   ```

## Notes
- Update your OpenAI API key in the environment or configuration if using OpenAI.
- The project demonstrates both native and OpenAPI plugin integration with Semantic Kernel.

---
For more details, see the book or the main project README.
