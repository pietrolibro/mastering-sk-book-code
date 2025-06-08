from semantic_kernel.connectors.memory.in_memory import InMemoryVectorStore


from typing import TypeVar

from semantic_kernel.connectors.ai.embedding_generator_base import EmbeddingGeneratorBase
from semantic_kernel.connectors.ai.chat_completion_client_base import ChatCompletionClientBase

from semantic_kernel.connectors.memory.in_memory import InMemoryVectorCollection
from semantic_kernel.data import (
    VectorSearchOptions,
    VectorSearchResult
)

from semantic_kernel.contents.chat_history import ChatHistory

from helpers.kernel_helper import KernelHelper
from models.faq_record import FaqRecord

# _T = TypeVar("_T")

# def print_record(result: VectorSearchResult[_T] | None = None, record: _T | None = None):
#     if result:
#         record = result.record
#     print(f"  Found id: {record.id}")
#     print(f"    Question: {record.question}")
#     print(f"    Answer: {record.answer}")

#     if result and result.score is not None:
#         print(f"    Score: {result.score}")
#     # print(f"    Content: {record.content}")
#     # print(f"    Tag: {record.tag}")
#     if record.question_embedding is not None:
#         print(f"    Vector (first five): {record.question_embedding[:5]}")
   

async def run_duplicate_questions_demo(kernel, chat_service : ChatCompletionClientBase, 
                                       embed_service : EmbeddingGeneratorBase, 
                                       system_prompt: str) -> None:
    print("=== DEMO: Duplicate-Question Detection (FAQ InMemory) ===")

    vector_store = InMemoryVectorStore()
    faq_collection = vector_store.get_collection("faq_products",FaqRecord)
    await faq_collection.create_collection_if_not_exists()

    # Seed a mini FAQ
    faq_pairs = [
        (1, "What is a good smartphone under $1000?", "Smartphone X ($999.99) packs AI-powered features and top-tier performance."),
        (2, "I'm looking for affordable fitness equipment.", "Resistance Bands C ($29.99) and Yoga Mat D ($24.99) are budget-friendly options."),
        (3, "Which laptop is best for high-performance gaming?", "Laptop Z ($1299.99) delivers excellent gaming power courtesy of BrandA's latest GPU."),
        (4, "Do you have noise-canceling headphones?", "Wireless Headphones ($199.99) provide superb ANC with 30-hour battery life."),
    ]

    # https://github.com/microsoft/semantic-kernel/blob/main/python/samples/concepts/memory/simple_memory.py

    for (id, q, a) in faq_pairs:

        faq_record: FaqRecord = FaqRecord(
            id = id,
            question = q,
            answer = a,
            question_embedding = (await embed_service.generate_raw_embeddings([q]))[0]
        )

        await faq_collection.upsert(faq_record)
    
    options = VectorSearchOptions(
        vector_field_name="question_embedding",
        include_vectors=True,
        top = 1
        )
    


    helper = KernelHelper()

    chat_history = ChatHistory()
    chat_history.add_system_message(system_prompt)

    queries= [
        "What is a good smartphone under $1000?",
        "Looking for cheaper fitness equipment.",
        "I'm looking for Hairdry"
    ]

    for query in queries:
        await helper.chat_with_faq_async(kernel, chat_service, embed_service, chat_history, query, faq_collection)