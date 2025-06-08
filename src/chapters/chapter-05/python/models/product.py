from dataclasses import dataclass
from semantic_kernel.functions import kernel_function
from typing import List, Optional
from dataclasses import dataclass, field

@dataclass
class Product:
    name: str = field(metadata={"description": "Short name of the product."})
    category: str = field(metadata={"description": "Product category."})
    description: str = field(metadata={"description": "Description of the product."})
    price: float = field(metadata={"description": "Price of the product."})
    product_code: str = field(metadata={"description": "Unique product code."})
    brand : str = field(metadata={"description": "Brand of the product."})