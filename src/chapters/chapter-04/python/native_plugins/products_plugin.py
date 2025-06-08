from semantic_kernel.functions import kernel_function
from typing import List, Optional

from models.product import Product
from services.product_catalog import ProductCatalogService

class ProductsPlugin:
    def __init__(self, catalog : ProductCatalogService, logger):
        self._catalog = catalog
        self._logger = logger

    # #A Method implementing product listing from the catalog
    @kernel_function(name="get_available_products", description="Gets a list of available products.")
    def get_available_products(self) -> List[Product]:
        return self._catalog.get_available_products()

    # #B Method implementing category-based product filtering
    @kernel_function(name="get_available_products_by_category", description="Gets a list of available products in a specific category.")
    def get_products_by_category(self, products: List[Product], category: str) -> List[Product]:
        return [p for p in products if p.category.lower() == category.lower()]

    # #C Method implementing product search by name (partial or full match)
    @kernel_function(name="find_product_by_name", description="Finds a product by name.")
    def find_product_by_name(self, products: List[Product], name: str) -> Optional[Product]:
        return next((p for p in products if name.lower() in p.name.lower()), None)

    # #D Method implementing product comparison based on name, price, and description
    @kernel_function(name="compare_products", description="Compares two products and returns a summary.")
    def compare_products(self, p1: Product, p2: Product) -> str:
        return (
            f"Comparison:\n\n"
            f"{p1.name} (${p1.price}) - {p1.description}\n"
            f"VS\n"
            f"{p2.name} (${p2.price}) - {p2.description}\n"
        )
