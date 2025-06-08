# AI Shopping Assistant (Python)

## Overview
This project implements an AI-powered shopping assistant using Python and Semantic Kernel. It provides an example of building an interactive assistant for shopping tasks.

## Prerequisites
- Python 3.10 or later

## Setup

### 1. Create a Virtual Environment
```zsh
python -m venv ai-shopping-assistant-env
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
