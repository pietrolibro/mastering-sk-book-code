from semantic_kernel.connectors.memory.in_memory import InMemoryVectorStore

from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase


from semantic_kernel.contents.chat_history import ChatHistory

from helpers.kernel_helper import KernelHelper

from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase

async def run_user_profile_as_plugin_demo(kernel, chat_service: ChatCompletionClientBase, system_prompt: str) -> None:
    print("\n=== DEMO: User Profile as Plugin ===\n")

    chat_history = ChatHistory()
    chat_history.add_system_message(system_prompt)

    queries = [
        "Which is my user id?",
        "Which is my email address?",
        "Which is budget limit?",
        "Can You suggest some products?",
        "Which is my category of interest?",
    ]

    helper = KernelHelper()

    for query in queries:
        await helper.chat_async(kernel, chat_service, chat_history, query)


