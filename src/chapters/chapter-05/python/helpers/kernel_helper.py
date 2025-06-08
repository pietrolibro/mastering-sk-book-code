import numpy as np
from typing import Optional, List, overload
from models.faq_record import FaqRecord

import asyncio
import yaml

from semantic_kernel.functions import KernelArguments

from semantic_kernel import Kernel
from semantic_kernel.functions import KernelFunction
from semantic_kernel.contents.chat_history import ChatHistory
from semantic_kernel.connectors.ai import PromptExecutionSettings
from semantic_kernel.connectors.memory.in_memory import InMemoryVectorCollection
from semantic_kernel.connectors.ai.function_choice_behavior import FunctionChoiceBehavior
from semantic_kernel.prompt_template import KernelPromptTemplate, HandlebarsPromptTemplate, Jinja2PromptTemplate
from semantic_kernel.prompt_template import InputVariable, PromptTemplateConfig
from semantic_kernel.connectors.ai.embedding_generator_base import EmbeddingGeneratorBase
from semantic_kernel.connectors.ai.ollama import OllamaChatCompletion,  OllamaChatPromptExecutionSettings
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion, OpenAIChatPromptExecutionSettings
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase
from semantic_kernel.functions import KernelArguments
from semantic_kernel.data import (
    VectorizableTextSearchMixin,
    VectorizedSearchMixin,
    VectorSearchFilter,
    VectorSearchOptions,
    VectorStoreRecordCollection,
    VectorStoreRecordDataField,
    VectorStoreRecordKeyField,
    VectorStoreRecordVectorField,
    VectorTextSearchMixin,
    vectorstoremodel,
)

class KernelHelper:
    """Helper for chat interactions and embedding-based FAQ detection."""
    
    async def stream_and_collect_response(self,
        kernel: Kernel, 
        executionSettings: PromptExecutionSettings,
        chat_service: ChatCompletionClientBase,
        history: ChatHistory) -> str:
        """Stream assistant response, print it, and add to history."""

        stream = chat_service.get_streaming_chat_message_content(
            chat_history=history, settings=executionSettings, kernel=kernel 
        )

        full_response = ""
        async for chunk in stream:
            print(chunk.content, end="", flush=True)
            full_response += chunk.content

        if full_response:
            history.add_assistant_message(full_response)

        return full_response

    def create_semantic_function(
            self,
        kernel: Kernel, 
        system_prompt: str, 
        function_name: str, 
        use_local_model: bool, 
        template_format="semantic-kernel",
        temperature:float = 0.6, 
        max_tokens: int = 100 ) -> KernelFunction:

        if use_local_model:
            execution_settings = OllamaChatPromptExecutionSettings(temperature=temperature, max_tokens=max_tokens)
        else:
            execution_settings = OpenAIChatPromptExecutionSettings(temperature=temperature, num_predict=max_tokens)

        prompt_template_config = PromptTemplateConfig(
            template=system_prompt,
            name=function_name + "_config",
            template_format=template_format,
            input_variables=[
                InputVariable(name="products", description="Available products", is_required=True),
                InputVariable(name="input", description="User input", is_required=True),
            ],
            execution_settings=execution_settings,
        )

        sk_function = kernel.add_function(
            function_name=function_name,
            plugin_name="AIShoppingPlugin",
            prompt_template_config=prompt_template_config,
        )

        return sk_function

    async def render_prompt_template_async(self,
            kernel: Kernel, 
            prompt_template:str, 
            arguments: KernelArguments, 
            template_format="semantic-kernel") -> str:

        prompt_template_config = PromptTemplateConfig(
            template=prompt_template,
            name="render",
            template_format=template_format,
            input_variables=[
                InputVariable(name="products", description="The products available in the shop", is_required=True),
                InputVariable(name="input", description="The user input", is_required=True),
            ]
        )

        template_classes = {
            "semantic-kernel": KernelPromptTemplate,
            "handlebars": HandlebarsPromptTemplate,
            "jinja2": Jinja2PromptTemplate
        }

        try:
            prompt_template_class = template_classes[prompt_template_config.template_format]
        except KeyError:
            raise ValueError(f"Invalid template format: {template_format}")

        prompt_template = prompt_template_class(prompt_template_config=prompt_template_config, allow_dangerously_set_content=False)

        rendered_prompt = await prompt_template.render(kernel, arguments)

        return rendered_prompt
    
    async def chat_async(self,
            kernel: Kernel, 
            chat_service: ChatCompletionClientBase,
            history: ChatHistory,
            user_prompt: str,
            ) -> None:

            history.add_user_message(user_prompt)
            print(f"User >>> {user_prompt}")

            executionSettings: PromptExecutionSettings = PromptExecutionSettings()
            executionSettings.function_choice_behavior = FunctionChoiceBehavior.Auto()

            stream = chat_service.get_streaming_chat_message_content(
                chat_history=history, settings=executionSettings, kernel=kernel 
            )

            generated_response = await self.stream_and_collect_response(
                kernel=kernel,
                executionSettings=executionSettings,
                chat_service=chat_service,
                history=history
            )

            print(f"\nAssistant >>> {generated_response}\n")

    async def chat_with_faq_async(self,
        kernel: Kernel, 
        chat_service: ChatCompletionClientBase, 
        embed_service: EmbeddingGeneratorBase, 
        history: ChatHistory, 
        user_prompt: str,
        faq_collection:InMemoryVectorCollection[int, FaqRecord]) -> str:

        history.add_user_message(user_prompt)
        print(f"User >>> {user_prompt}")

        execution_settings: PromptExecutionSettings = PromptExecutionSettings()
        execution_settings.function_choice_behavior = FunctionChoiceBehavior.Auto()

        incoming_vec = (await embed_service.generate_raw_embeddings([user_prompt]))[0]

        options = VectorSearchOptions(
            vector_field_name="question_embedding",
            include_vectors=True,
            top = 1
        )

        search_results = await faq_collection.vectorized_search(vector=incoming_vec, options=options)

        if search_results.total_count != None and search_results.total_count >0 and search_results.results[0].score >= 0.75:
            print(f"Duplicate detected (score {search_results.results[0].score:.2f}) returning cached answer:\n{search_results.results[0].record.answer}\n")
            answer = search_results.results[0].record.answer
        else:
            print("No close duplicate asking the model …\n")
            
            generated_response = await self.stream_and_collect_response(
                kernel=kernel,
                executionSettings=execution_settings,
                chat_service=chat_service,
                history=history
            )

        print(f"Assistant >>> {generated_response}")    

        return generated_response
