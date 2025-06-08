from dataclasses import dataclass, field
from typing import Annotated


# import asyncio
# from collections.abc import Sequence
from dataclasses import dataclass, field
from typing import Annotated
# from uuid import uuid4
# from typing import TypeVar

from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import (
    OpenAIEmbeddingPromptExecutionSettings,
    OpenAITextEmbedding,
)
# from semantic_kernel.connectors.memory.in_memory import InMemoryVectorCollection
# from semantic_kernel.data import VectorSearchResult
from semantic_kernel.data import (
    # VectorSearchFilter,
    # VectorSearchOptions,
    VectorStoreRecordDataField,
    VectorStoreRecordKeyField,
    VectorStoreRecordVectorField,
    vectorstoremodel,
)

from semantic_kernel.data.const import DISTANCE_FUNCTION_DIRECTION_HELPER, DistanceFunction, IndexKind
# from semantic_kernel.data.vector_search import add_vector_to_records

DISTANCE_FUNCTION = DistanceFunction.COSINE_SIMILARITY
# The in memory collection does not actually use a index, so this variable is not relevant, here for completeness
INDEX_KIND = IndexKind.IVF_FLAT

@vectorstoremodel
@dataclass
class FaqRecord:
    id: Annotated[int, VectorStoreRecordKeyField()] = -1,
    question: Annotated[
        str,
        VectorStoreRecordDataField(
            has_embedding=True,
            embedding_property_name="question_embedding",
            property_type="str"
            # is_full_text_searchable=True,
        ),
    ] = "",
    # answer: Annotated[str, VectorStoreRecordDataField(property_type="str", is_filterable=True)] = ""
    answer: Annotated[str, VectorStoreRecordDataField(property_type="str")] = ""
    question_embedding: Annotated[
        list[float] | None,
        VectorStoreRecordVectorField(
            # embedding_settings={"embedding": OpenAIEmbeddingPromptExecutionSettings()},
            # index_kind=INDEX_KIND,
            dimensions=1536,
            # distance_function=DISTANCE_FUNCTION,
            property_type="float",
        ),
    ] = None