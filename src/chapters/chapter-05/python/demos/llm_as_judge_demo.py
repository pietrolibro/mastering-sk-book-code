from semantic_kernel import Kernel
from semantic_kernel.functions import KernelFunction, KernelArguments
from semantic_kernel.connectors.memory.in_memory import InMemoryVectorStore


from typing import TypeVar

from semantic_kernel.connectors.ai.embedding_generator_base import EmbeddingGeneratorBase
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase


from semantic_kernel.contents.chat_history import ChatHistory

from helpers.kernel_helper import KernelHelper
from models.user_profile import UserProfile

from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase

async def run_llm_evaluation_demo(
        kernel: Kernel, 
        chat_service: ChatCompletionClientBase, 
        user_profile : UserProfile ) -> None:

    print("=== DEMO: LLM Evaluation of Assistant Response ===")
    user_id = user_profile.user_id
    user_name = user_profile.name

    user_prompt:str = "I need a recommendation for a product."

    system_prompt = (
        f"You are an AI assistant. The user is {user_name} (ID: {user_id}). "
        f"Provide a product recommendation tailored to their profile."
    )

    chat_history = ChatHistory()
    chat_history.add_system_message(system_prompt)

    helper = KernelHelper()

    response = await helper.chat_async(kernel, chat_service, chat_history, user_prompt)

    evaluation_prompt = (
        "You are an AI evaluation assistant. Your task is to assess how relevant and context-aware "
        "the following assistant response is, based on the user's profile and query.\n\n"
        "Rate the assistant's reply on a scale from 1 to 6:\n"
        "1 = Completely irrelevant\n"
        "2 = Mostly irrelevant or off-topic\n"
        "3 = Somewhat relevant, but lacks personalization or context awareness\n"
        "4 = Mostly relevant, uses some context or memory appropriately\n"
        "5 = Very relevant and mostly personalized\n"
        "6 = Fully relevant, highly personalized, and contextually accurate\n\n"
        f"User ID: {user_id}\nUser Name: {user_name}\n"
        f"User Query: {user_prompt}\nAssistant Response: {response}\n\n"
        "Please respond with a single numeric score (1 to 6) with no additional text.\n"
        "Score:"
    )

    score_function = helper.create_semantic_function(
        kernel=kernel,
        system_prompt=evaluation_prompt,
        function_name="llm_evaluation",
        use_local_model=False
    )

    score_response = await kernel.invoke(score_function,
                                         arguments=KernelArguments(user_id=user_id, user_name=user_name))

    print(f"Assistant (evaluation) >>> Score: {score_response}\n")

