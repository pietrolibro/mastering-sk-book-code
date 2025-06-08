from semantic_kernel.functions import kernel_function
from typing import List, Optional
from dataclasses import dataclass, field

@dataclass
class UserProfile:

    user_id: str = field(metadata={"description":"User ID of the user."})
    name: str = field(metadata={"description":"Username of the user."})
    email: str = field(metadata={"description":"Email address of the user."})
    brand_affinity: str = field(metadata={"description": "Brand affinity of the user."})
    budget: float = field(metadata={"description": "Budget limit of the user."})
    category_interests: List[str] = field(default_factory=list,metadata={"description":"User's interested categories"})
    latest_visited_products: List[str] = field(default_factory=list,metadata={"description":"Recently visited products"})