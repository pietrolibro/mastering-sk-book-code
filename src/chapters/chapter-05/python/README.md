# Advanced AI Shopping Assistant (Python)

## Overview
This project is an advanced AI-powered shopping assistant using Python and Semantic Kernel. It includes demos, plugins, and helper modules for complex shopping assistant scenarios.

## Prerequisites
- Python 3.10 or later

## Setup

### 1. Create a Virtual Environment
```zsh
python3 -m venv ai-shopping-assistant-env
```

### 2. Activate the Virtual Environment
- **macOS/Linux (zsh/bash):**
  ```zsh
  source ai-shopping-assistant-env/bin/activate
  ```
- **Windows (Command Prompt):**
  ```cmd
  ai-shopping-assistant-env\Scripts\activate.bat
  ```
- **Windows (PowerShell):**
  ```powershell
  ai-shopping-assistant-env\Scripts\Activate.ps1
  ```

### 3. Install Dependencies
#### Option 1: Using requirements.txt (if available)
```zsh
pip install -r requirements.txt
```
#### Option 2: Install Semantic Kernel directly
```zsh
pip install semantic-kernel
pip install 'semantic-kernel[ollama]'
```
For more installation options and extras, see the [Semantic Kernel PyPI page](https://pypi.org/project/semantic-kernel/).

## Running the Project
```zsh
python main.py
```

## Notes
- Set any required API keys or environment variables as needed for AI services.

## Main Menu Use Cases

When you run `main.py`, you can select from the following demos:

1. **Basic Semantic Function**: Simple prompt-based AI response.
2. **Semantic Function with SK Prompt Template**: Uses Semantic Kernel's prompt template.
3. **Semantic Function with Handlebars Template**: Uses Handlebars templating for prompts.
4. **Semantic Function with Jinja2 Template**: Uses Jinja2 templating for prompts.
5. **Semantic Function from YAML**: Loads prompt template from a YAML file.

Select the corresponding number to run each demo.

---

For more details, see the code and comments in `main.py`.
