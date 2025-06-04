import asyncio

from semantic_kernel import Kernel
from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai import PromptExecutionSettings
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase

async def simple_chat(kernel, chat_completion_service, chat_history):
    while True:
        user_input = input("User Input > ")
        if not user_input.strip():
            print("User Input> ")
            continue
        if user_input.lower() == "exit":
            break
        chat_history.add_user_message(user_input)

        settings : PromptExecutionSettings = PromptExecutionSettings()

        response = await chat_completion_service.get_chat_message_content(
            chat_history=chat_history,settings=settings)
        
        if response and hasattr(response, 'content'):
            print(f"AI Response> {response.content}")
            chat_history.add_assistant_message(response.content)

async def streaming_chat(kernel, chat_completion_service, chat_history):
    while True:
        user_input = input("User Input > ")
        if not user_input.strip():
            print("User Input> ")
            continue
        if user_input.lower() == "exit":
            break
        chat_history.add_user_message(user_input)

        settings : PromptExecutionSettings = PromptExecutionSettings()

        stream = chat_completion_service.get_streaming_chat_message_content(
            chat_history=chat_history,settings=settings)
        
        full_response = ""

        async for chunk in stream:
            print(chunk.content, end="", flush=True)
            full_response += chunk.content
            if chunk.metadata is not None and "usage" in chunk.metadata:
                print("\n")
                print("Prompt Tokens: ", chunk.metadata["usage"].prompt_tokens)
                print("Completion Tokens: ", chunk.metadata["usage"].completion_tokens)

        if full_response:
            chat_history.add_assistant_message(full_response)

async def main():
    use_openai = True

    # Initialize the kernel
    kernel = Kernel()

    if use_openai:
        # Add OpenAI chat completion service. Credentials are stored in the '.env' file.

        # For OpenAI, file should be structure as below:
        # GLOBAL_LLM_SERVICE="AzureOpenAI"
        # AZURE_OPENAI_API_KEY="..."
        # AZURE_OPENAI_ENDPOINT="https://..."
        # AZURE_OPENAI_CHAT_DEPLOYMENT_NAME="..."
        # AZURE_OPENAI_TEXT_DEPLOYMENT_NAME="..."
        # AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME="..."
        # AZURE_OPENAI_API_VERSION="..."

        oai_chat_service = OpenAIChatCompletion()

        kernel.add_service(oai_chat_service)
    else:
        # Add Ollama chat completion service.
        llama_chat_service = OllamaChatCompletion(
            host="http://localhost:11434",
            ai_model_id="llama3.2:latest",
        )
        kernel.add_service(llama_chat_service)

    # Retrieve the chat completion service by type.
    chat_service = kernel.get_service(type=ChatCompletionClientBase)

    chat_history = ChatHistory()

    assistant_role = """
        You are a helpful assistant specialized in movies and TV shows. 
        If user try to ask something else, simple remind the user that you are specialized in movies and TV shows and 
        that you are not able to help with other topics.
    """
    chat_history.add_system_message(assistant_role)

    print("Select demo to run:")
    print("1 - Simple chat (not streaming)")
    print("2 - Streaming chat")
    demo_choice = input("Enter 1 or 2: ").strip()
    if demo_choice == "1":
        await simple_chat(kernel, chat_service, chat_history)
    elif demo_choice == "2":
        await streaming_chat(kernel, chat_service, chat_history)
    else:
        print("Invalid choice. Exiting.")

if __name__ == "__main__":
    asyncio.run(main())
