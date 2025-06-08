from dataclasses import dataclass, field

@dataclass
class Product:
    name: str = field(metadata={"description": "name"})
    product_code: str = field(metadata={"description": "Product code."})
    description: str = field(metadata={"description": "Product description."})
    price: float = field(metadata={"description": "Product price."})
    category: str = field(metadata={"description": "Product category."})
    brand: str = field(metadata={"description": "Product brand."})