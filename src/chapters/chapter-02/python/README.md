# HelloWorldSKChat (Python)

## Overview
This project demonstrates a simple Semantic Kernel chat application using Python. It provides a basic example of interacting with AI models for chat.

## Prerequisites
- Python 3.10 or later

## Setup

### 1. Create a Virtual Environment

```zsh
python3 -m venv hello-world-sk-chat-env
```

### 2. Activate the Virtual Environment

- **macOS/Linux (zsh/bash):**
  ```zsh
  source hello-world-sk-chat-env/bin/activate
  ```
- **Windows (Command Prompt):**
  ```cmd
  hello-world-sk-chat-env\Scripts\activate.bat
  ```
- **Windows (PowerShell):**
  ```powershell
  hello-world-sk-chat-env\Scripts\Activate.ps1
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
