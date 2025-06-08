import asyncio
from semantic_kernel import Kernel
from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.connectors.ai import PromptExecutionSettings
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.openapi_plugin import OpenAPIFunctionExecutionParameters 
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase
from semantic_kernel.functions import KernelArguments

from dataclasses import dataclass
from typing import List

from models.product import Product
from services.product_catalog import ProductCatalogService

from native_plugins.products_plugin import ProductsPlugin
from native_plugins.cart_plugin import CartPlugin

import logging

def products_as_string(products:List[Product]) -> str:
    product_strings = [
        f"- Product Code: {product.product_code},  Name: {product.name}, Description: {product.description}, PRICE: ${product.price}, Category: {product.category}"
        for product in products
    ]
    return "\n".join(product_strings)

async def chat(chat_completion_service, history, user_prompt, kernel):

    history.add_user_message(user_prompt)
    print(f"\nUser >>> {user_prompt}")

    executionSettings: PromptExecutionSettings = PromptExecutionSettings()
    executionSettings.function_choice_behavior = FunctionChoiceBehavior.Auto()

    stream = chat_completion_service.get_streaming_chat_message_content(
        chat_history=history, settings=executionSettings, kernel=kernel 
    )

    print("Assistant >>> ", end="", flush=True) 
    full_response = ""  # Initialize full_response
    async for chunk in stream:
        print(chunk.content, end="", flush=True)
        full_response += chunk.content

    if full_response:
        history.add_assistant_message(full_response)

async def run_semantic_functions_demo(kernel: Kernel, products : List[Product])-> None:
    print("\n--- Semantic Functions Demo ---")

    plugins_directory = "../../plugins/"

    ai_shopping_plugin = kernel.add_plugin(parent_directory=plugins_directory, plugin_name="ai_shopping")

    recommend_product = ai_shopping_plugin["recommend_product"]

    result = await kernel.invoke(recommend_product, KernelArguments(
            products=products_as_string(products),
            input="I'm looking for a book to learn about cloud."
        ))
    
    print(f"Recommend Products: {result}")

    # Compare two products.
    productA = "- Name: Smartwatch A, Description: Fitness smartwatch with heart rate monitoring, PRICE: $149.99, Category: Fitness"
    productB = "- Name: Tablet P, Description: Lightweight tablet with a stunning display, PRICE: $499.99, Category: Electronics"

    result = await kernel.invoke(ai_shopping_plugin["compare_products"], KernelArguments(
            productA=productA,
            productB=productB,
        ))
    
    print(f"Product Comparison: {result}")

    # Find products by category.
    result = await kernel.invoke(ai_shopping_plugin["find_by_category"], KernelArguments(
            products=products,
            category="electronics",
        ))
        
    print(f"Find by Category: {result}")

    print("\n--- End of Semantic Functions Demo ---")


async def run_plugin_direct_invocation_demo(kernel: Kernel, products_Catalog_plugin: ProductsPlugin, products: List[Product]) -> None:
    print("\n--- Semantic Functions Direct Invocation Demo ---")

    available_products_fn = products_Catalog_plugin["get_available_products"]
    available_products =  await kernel.invoke(available_products_fn, KernelArguments(products=products))

    for product in available_products.value:
        print(f"Product: {product.name}, Description: {product.description}, Price: ${product.price}, Category: {product.category}")

    # products_by_Category_fn = products_Catalog_plugin["get_available_products_by_category"]
    # products_by_Category  =  await kernel.invoke(products_by_Category_fn, KernelArguments(
    #     products=products, category="Books"))

    # for product in products_by_Category:
    #     print(f"Product: {product.name}, Description: {product.description}, Price: ${product.price}, Category: {product.category}")
    print("\n--- End of Semantic Functions Direct Invocation Demo ---")

async def run_cart_plugin_with_chat_capabilities_demo(kernel: Kernel, chat_completion_service: ChatCompletionClientBase):
    print("\n--- Cart Plugin with Chat Capabilities ---")
    
    history_cart = ChatHistory()

    # Define the system prompt
    system_prompt = """
        You are an AI Shopping Assistant for an online store.

        You can recommend products, compare them, and manage a shopping cart.

        If the user asks to buy or add something, add the product to the cart.

        If they ask to remove something, remove it from the cart.

        You can always show the cart contents and the total amount upon request.
        Be concise and friendly, and always confirm actions performed on the cart.
    """

    # Add the system prompt to the chat history.
    history_cart.add_system_message(system_prompt)

    # Simulate a chat session with the user
    queres = [
        "What are the available products?", 
        "Add Smartphone X to my cart.",
        "Add Cloud Book to my cart.",
        "What's in my cart?",
        "Remove Cloud Book",
        "How much is the total?"
    ]
    for query in queres:
        await chat(chat_completion_service, history_cart, query, kernel)

    print("\n--- End of Cart Plugin with Chat Capabilities ---")

async def run_openapi_plugin_with_chat_capabilities_demo(kernel: Kernel, chat_completion_service: ChatCompletionClientBase):
    print("\n--- OpenAPI Plugin with Chat Capabilities ---")

    # old_products_catalog_plugin = kernel.get_plugin("products_plugin")
    del kernel.plugins["products_plugin"]

    # Add the ProductsPlugin as OpenAPI.
    # The OpenAPI plugin is imported from the Swagger/OpenAPI specification.
    kernel.add_plugin_from_openapi(
        plugin_name="ProductsCatalogPlugin",
        openapi_document_path="http://localhost:5054/swagger/v1/swagger.json",
        execution_settings=OpenAPIFunctionExecutionParameters(
        # Determines whether payload parameter names are augmented with namespaces.
        # Namespaces prevent naming conflicts by adding the parent parameter name
        # as a prefix, separated by dots
        enable_payload_namespacing=True
        ),
    )

    # Define the system prompt
    system_prompt = """
        You are an AI Shopping Assistant for an online store.

        You can recommend products, compare them, and manage a shopping cart.

        If the user asks to buy or add something, add the product to the cart.

        If they ask to remove something, remove it from the cart.

        You can always show the cart contents and the total amount upon request.
        Be concise and friendly, and always confirm actions performed on the cart.
    """

    # Add the system prompt to the chat history.
    history_cart = ChatHistory()
    history_cart.add_system_message(system_prompt)

    # Simulate a chat session with the user
    queres = [
        "What are the available products?", 
        "Add Smartphone X to my cart.",
        "Add Cloud Book to my cart.",
        "What's in my cart?",
        "Remove Cloud Book",
        "How much is the total?"
    ]
    for query in queres:
        await chat(chat_completion_service, history_cart, query, kernel)

    print("\n--- End of OpenAPI Plugin with Chat Capabilities ---")

async def main():

    use_openai = True

    kernel = Kernel()
    if use_openai:
        oai_chat_service = OpenAIChatCompletion()
        kernel.add_service(oai_chat_service)
    else:
        llama_chat_service = OllamaChatCompletion(
            host="http://localhost:11434",
            ai_model_id="llama3.2:latest",
        )
        kernel.add_service(llama_chat_service)

    products_plugin_logger = logging.getLogger("ProductsPlugin")
    cart_plugin_logger = logging.getLogger("CartPlugin")
    logging.basicConfig(level=logging.ERROR)

    catalog_service = ProductCatalogService()
    cart_plugin_instance = CartPlugin(catalog_service, cart_plugin_logger)
    products_plugin_instance = ProductsPlugin(catalog_service, products_plugin_logger)

    registered_products_plugin = kernel.add_plugin(products_plugin_instance, plugin_name="products_plugin")
    registered_cart_plugin = kernel.add_plugin(cart_plugin_instance, plugin_name="cart_plugin")

    products = catalog_service.get_available_products()
    
    chat_completion_service = kernel.get_service(type=ChatCompletionClientBase)

    while True:
        print("Select demo to run:")
        print("1 - Semantic Functions Demo")
        print("2 - Semantic Functions Direct Invocation Demo")
        print("3 - Cart Plugin with Chat Capabilities")
        print("4 - OpenAPI Plugin with Chat Capabilities")
        print("5 - Exit")

        demo_choice = input("Enter 1, 2, 3, 4 or 5: ").strip()
        if demo_choice == "1":
            await run_semantic_functions_demo(kernel, products)
        elif demo_choice == "2":
            await run_plugin_direct_invocation_demo(kernel,registered_products_plugin, products)
        elif demo_choice == "3":
            await run_cart_plugin_with_chat_capabilities_demo(kernel, chat_completion_service)
        elif demo_choice == "4":
            await run_openapi_plugin_with_chat_capabilities_demo(kernel, chat_completion_service)
        elif demo_choice == "5":
            print("Exiting.")
            break
        else:
            print("Invalid choice. Please select a valid option.")

if __name__ == "__main__":
    asyncio.run(main())
