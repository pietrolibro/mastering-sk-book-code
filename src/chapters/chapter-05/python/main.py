import asyncio
import logging

from services.product_catalog_service import ProductCatalogService
from services.user_profile_service import UserProfileService

from plugins.cart_plugin import CartPlugin
from plugins.products_plugin import ProductsPlugin
from plugins.user_profile_plugin import UserProfilePlugin

from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.embedding_generator_base import EmbeddingGeneratorBase
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion,OllamaTextEmbedding
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion, OpenAITextEmbedding
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase
from semantic_kernel.functions import KernelArguments

from demos import (
    system_prompt_demo,
    user_profile_plugin_demo,
    duplicate_questions_demo,
    llm_as_judge_demo,
)

async def main():
    use_openai = True

    kernel = Kernel()

    # Initialize services.
    user_profile_service = UserProfileService()
    product_catalog_service = ProductCatalogService()

    cart_plugin_logger = logging.getLogger("CartPlugin")
    products_plugin_logger = logging.getLogger("ProductsPlugin")
    user_profile_plugin_logger = logging.getLogger("CartPlugin")

    logging.basicConfig(level=logging.ERROR) # Set logging level to ERROR for plugins.

    # Initialize plugins.
    cart_plugin = CartPlugin(product_catalog_service,cart_plugin_logger)
    products_plugin = ProductsPlugin(product_catalog_service, products_plugin_logger)
    user_profile_plugin = UserProfilePlugin(user_profile_service,user_profile_plugin_logger)

    # Register plugins with the kernel.
    registered_cart_plugin = kernel.add_plugin(cart_plugin, plugin_name="cart_plugin")
    registered_products_plugin = kernel.add_plugin(products_plugin, plugin_name="products_plugin")
    registered_user_profile_plugin = kernel.add_plugin(user_profile_plugin, plugin_name="user_profile_plugin")

    if use_openai:
        # Add OpenAI services.
        oai_chat_service = OpenAIChatCompletion()
        kernel.add_service(oai_chat_service)

        oai_embedding_gen = OpenAITextEmbedding(service_id="embedding")
        kernel.add_service(oai_embedding_gen)
    else:
        # Add Ollama services.
        ollama_chat_service = OllamaChatCompletion(
            host="http://localhost:11434",
            ai_model_id="llama3.2:latest",
        )
        kernel.add_service(ollama_chat_service)

        ollama_embedding_service = OllamaTextEmbedding(
            service_id="embedding", 
            ai_model_id="nomic-embed-text",
            host="http://localhost:11434")

        kernel.add_service(ollama_embedding_service)

    # Retrieve the chat completion service and the embedding generator service by type.
    chat_service = kernel.get_service(type=ChatCompletionClientBase)
    embed_service = kernel.get_service(type=EmbeddingGeneratorBase)

    while True:
        # Prompt user to select a demo scenario
        print("Select a demo scenario to run:")
        print("1. System Prompt Demo")
        print("2. User Profile Plugin Demo")
        print("3. Duplicate Questions Demo")
        print("4. LLM Evaluation Demo")
        print("Q. Quit")
        choice = input("Enter the scenario number (1-4) or Q to quit: ").strip()

        if choice == "1":
            await system_prompt_demo.run_render_system_prompt_demo(kernel)

        elif choice == "2":
            rendered_prompt = await system_prompt_demo.run_render_system_prompt_demo(kernel)
            await user_profile_plugin_demo.run_user_profile_as_plugin_demo(kernel, chat_service, rendered_prompt)

        elif choice == "3":
            rendered_prompt = await system_prompt_demo.run_render_system_prompt_demo(kernel)
            await duplicate_questions_demo.run_duplicate_questions_demo(kernel, chat_service, embed_service, rendered_prompt)

        elif choice == "4":
            rendered_prompt = await system_prompt_demo.run_render_system_prompt_demo(kernel)
            user_profile = user_profile_service.get_user_profile("user1")
            await llm_as_judge_demo.run_llm_evaluation_demo(kernel, chat_service, user_profile)

        elif choice.lower() == "q":
            print("Exiting.")
            break
        else:
            print("Invalid choice. Please try again.")

if __name__ == "__main__":
    asyncio.run(main())