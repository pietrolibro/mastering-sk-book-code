import asyncio
import os
import yaml
from typing import Literal

from semantic_kernel import Kernel
from semantic_kernel.functions import KernelFunction,KernelArguments
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion,  OllamaChatPromptExecutionSettings
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion, OpenAIChatPromptExecutionSettings
from semantic_kernel.prompt_template import InputVariable, PromptTemplateConfig
from semantic_kernel.prompt_template import KernelPromptTemplate, HandlebarsPromptTemplate, Jinja2PromptTemplate

PRODUCTS = [
    {
        "name": "Smartphone X",
        "description": "A powerful smartphone with AI-driven features",
        "price": 999.99,
        "category": "Electronics"
    },
    {
        "name": "Wireless Headphones Y",
        "description": "Noise-canceling wireless headphones with immersive sound",
        "price": 199.99,
        "category": "Electronics"
    },
    {
        "name": "Laptop Z",
        "description": "High-performance laptop for gaming and productivity",
        "price": 1299.99,
        "category": "Electronics"
    },
    {
        "name": "AI for Beginners",
        "description": "A beginner-friendly book on Artificial Intelligence",
        "price": 29.99,
        "category": "Books"
    },
    {
        "name": "The Semantic Kernel Guide",
        "description": "Comprehensive guide on Microsoft Semantic Kernel",
        "price": 39.99,
        "category": "Books"
    },
    {
        "name": "Cloud Computing Essentials",
        "description": "An introduction to cloud computing concepts",
        "price": 49.99,
        "category": "Books"
    },
    {
        "name": "Smartwatch A",
        "description": "Fitness smartwatch with heart rate monitoring",
        "price": 149.99,
        "category": "Fitness"
    },
    {
        "name": "Treadmill B",
        "description": "High-quality treadmill for home workouts",
        "price": 799.99,
        "category": "Fitness"
    },
    {
        "name": "Resistance Bands C",
        "description": "Set of resistance bands for strength training",
        "price": 29.99,
        "category": "Fitness"
    },
    {
        "name": "Tablet P",
        "description": "Lightweight tablet with a stunning display",
        "price": 499.99,
        "category": "Electronics"
    },
    {
        "name": "Cybersecurity Basics",
        "description": "Essential guide on online security and best practices",
        "price": 34.99,
        "category": "Books"
    },
    {
        "name": "Yoga Mat D",
        "description": "Premium yoga mat for comfortable workouts",
        "price": 24.99,
        "category": "Fitness"
    }
]

def products_as_string(products):
    return "\n".join(
        f"- Name: {p['name']}, Description: {p['description']}, PRICE: ${p['price']}, Category: {p['category']}"
        for p in products
    )


# This function demonstrates how to run a basic semantic function in Semantic Kernel.
async def run_basic_semantic_function(kernel: Kernel):
    

# This function demonstrates how to use Semantic Kernel prompt template with a semantic function.
async def run_sk_prompt_template(kernel: Kernel, use_openai: bool):

    sk_prompt_template = """
        You are a helpful assistant specialized in assisting customers with shopping. 

        Following are the products available in the shop:

        {{$products}}
        {{$input}}
    """
    
    user_prompt = "I'm looking for a product in the Electronics category Can you recommend 2 of them?"
    arguments = KernelArguments(products=products_as_string(PRODUCTS), input=user_prompt)

    rendered_prompt = await render_prompt_template(kernel, sk_prompt_template, arguments)
    print(f"Rendered Prompt:\n{rendered_prompt}\n")

    sk_function = create_semantic_function(kernel, sk_prompt_template , "suggest_products", use_openai, "semantic-kernel") 

    generated_response = await kernel.invoke(sk_function, arguments)
    print(f"Generated Result:\n{generated_response}\n")

# This function demonstrates how to use Handlebars template engine with Semantic Kernel.
async def run_handlebars_template(kernel: Kernel, use_openai: bool):

    handlebars_prompt_template = """
        <message role="system">
            You are a helpful assistant specialized in assisting customers with shopping.

            Following are the products available in the shop:

            {{#each products}}
            - Name: {{this.name}} 
            - Description: {{this.description}} 
            - Price: ${{this.price}} 
            - Category: {{this.category}}

            {{/each}}

        </message>
        <message role="user">
        {{input}}
        </message>
    """

    user_prompt = "I'm looking for a product in the Electronics category Can you recommend me something?"
    arguments = KernelArguments(products=PRODUCTS, input=user_prompt)
    
    rendered_prompt = await render_prompt_template(kernel, handlebars_prompt_template, arguments, "handlebars")
    print(f"Rendered Prompt:\n{rendered_prompt}\n")

    # sk_function = create_semantic_function(kernel, handlebars_prompt_template, "suggest_products", use_openai, "handlebars") 

    # generated_response = await kernel.invoke(sk_function, arguments)
    # print(f"Generated Result:\n{generated_response}\n")

# This function demonstrates how to use jinja2 template engine with Semantic Kernel.
async def run_jinja2_template(kernel: Kernel, use_openai: bool):
    jinja_prompt_template = """
        You are a helpful assistant specialized in assisting customers with shopping.

        Following are the products available in the shop:

        {% for item in products %}
        - Name: {{item.name}} 
        - Description: {{item.description}} 
        - Price: ${{item.price}} 
        - Category: {{item.category}}

        {% endfor %}

        {{input}}
    """

    user_prompt = "I'm looking for a product in the Electronics category Can you recommend me something?"
    arguments = KernelArguments(products=PRODUCTS, input=user_prompt)

    rendered_prompt = await render_prompt_template(kernel, jinja_prompt_template, arguments, "jinja2")
    print(f"Rendered Prompt: {rendered_prompt}")

    sk_function = create_semantic_function(kernel, jinja_prompt_template, "suggest_products", use_openai, "jinja2") 

    generated_response = await kernel.invoke(sk_function, arguments)
    print(f"Generated Result:\n{generated_response}\n")

async def run_yaml_template(kernel: Kernel, use_openai: bool):

    yaml_path = os.path.join(os.path.dirname(__file__), "ai_shopping_assistant.yaml")
    with open(yaml_path, "r") as f:
        yaml_prompt_template = yaml.safe_load(f)  # Use yaml.safe_load to parse YAML

    # Retrieve the prompt template as string.
    prompt_template = yaml_prompt_template.get("prompt", "")  # Now this is a dict, so .get works

    user_prompt = "I'm looking for a Book to learn about cloud. Do you have something?"
    arguments = KernelArguments(products=PRODUCTS, input=user_prompt)

    rendered_prompt = await render_prompt_template(kernel, prompt_template, arguments, "handlebars")
    print(f"Rendered Prompt: {rendered_prompt}")

    sk_function = create_semantic_function(kernel, prompt_template, "suggest_products", use_openai,"handlebars") 

    generated_response = await kernel.invoke(sk_function, arguments)
    print(f"Generated Result:\n{generated_response}\n")

async def render_prompt_template(kernel: Kernel, prompt_template: str, arguments: KernelArguments, 
                                 template_format: Literal['semantic-kernel', 'handlebars', 'jinja2'] = 'semantic-kernel') -> str:
    
    # Always use the template format from PromptTemplateConfig, ignoring the template_format argument if set in config.
    prompt_template_config = PromptTemplateConfig(
        template=prompt_template,
        name="render",
        template_format=template_format,
        input_variables=[
            InputVariable(name="products", description="The products available in the shop", is_required=True),
            InputVariable(name="input", description="The user input", is_required=True),
        ]
    )

    # Use the template format from PromptTemplateConfig, not the function argument.
    template_classes = {
        "semantic-kernel": KernelPromptTemplate,
        "handlebars": HandlebarsPromptTemplate,
        "jinja2": Jinja2PromptTemplate
    }

    try:
        prompt_template_class = template_classes[prompt_template_config.template_format]
    except KeyError:
        raise ValueError(f"Invalid template format: {prompt_template_config.template_format}")

    prompt_template = prompt_template_class(prompt_template_config=prompt_template_config, allow_dangerously_set_content=False)

    rendered_prompt = await prompt_template.render(kernel, arguments)

    return rendered_prompt

def create_semantic_function(kernel: Kernel, prompt_template: str, function_name: str, use_openai: bool, 
                           template_format: Literal['semantic-kernel', 'handlebars', 'jinja2'] = 'semantic-kernel', 
                           temperature:float = 0.6, max_tokens: int = 100 ) -> KernelFunction:
    if use_openai:
        execution_settings = OpenAIChatPromptExecutionSettings(temperature=temperature, num_predict=max_tokens)
    else:
        execution_settings = OllamaChatPromptExecutionSettings(temperature=temperature, max_tokens=max_tokens)

    prompt_template_config = PromptTemplateConfig(
        template=prompt_template,
        name=function_name + "_config",
        template_format=template_format,
        input_variables=[
            InputVariable(name="products", description="The products available in the shop", is_required=True),
            InputVariable(name="input", description="The user in∏put", is_required=True),
        ],
        execution_settings=execution_settings
    )

    sk_function = kernel.add_function(function_name=function_name,
                                      plugin_name="AIShoppingPlugin",
                                      prompt_template_config=prompt_template_config,
                                      template_format=prompt_template_config.template_format)

    return sk_function

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

    while True:
        print("Select demo to run:")
        print("1 - Basic Semantic Function")
        print("2 - Semantic Function with SK Prompt Template")
        print("3 - Semantic Function with Handlebars Template")
        print("4 - Semantic Function with Jinja2 Template")
        print("5 - Semantic Function from YAML")
        print("0 - Exit")
        demo_choice = input("Enter 1, 2, 3, 4, 5 or 0: ").strip()
        if demo_choice == "1":
            await run_basic_semantic_function(kernel)
        elif demo_choice == "2":
            await run_sk_prompt_template(kernel, use_openai)
        elif demo_choice == "3":
            await run_handlebars_template(kernel, use_openai)
        elif demo_choice == "4":
            await run_jinja2_template(kernel, use_openai)
        elif demo_choice == "5":
            await run_yaml_template(kernel,use_openai)
        elif demo_choice == "0":
            print("Exiting.")
            break
        else:
            print("Invalid choice. Try again.")
        print()

if __name__ == "__main__":
    asyncio.run(main())
