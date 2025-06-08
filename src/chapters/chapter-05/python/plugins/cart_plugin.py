from semantic_kernel.functions import kernel_function
from typing import List
from models.product import Product
from services.product_catalog_service import ProductCatalogService

class CartPlugin:
    def __init__(self, catalog_service, logger):
        self._logger = logger
        self._catalog = catalog_service
        self._cart: List[Product] = []

    @kernel_function(name="add_to_cart", description="Adds a product to the cart using its unique product code.")
    def add_to_cart(self, product_code: str) -> str:
        product = next((p for p in self._catalog.get_available_products() if p.product_code == product_code), None)
        if product:
            self._cart.append(product)
            return f"{product.name} added to cart."
        return f"Product with code {product_code} not found."

    @kernel_function(name="remove_from_cart", description="Removes a product from the cart using its unique product code.")
    def remove_from_cart(self, product_code: str) -> str:
        product = next((p for p in self._cart if p.product_code == product_code), None)
        if product:
            self._cart.remove(product)
            return f"{product.name} removed from cart."
        return f"Product with code {product_code} not found in cart."

    @kernel_function(name="view_cart", description="Shows the list of products currently in the cart.")
    def view_cart(self) -> str:
        if not self._cart:
            return "Your cart is currently empty."
        return "\n".join(f"{p.name} - ${p.price}" for p in self._cart)

    @kernel_function(name="get_total", description="Calculates and returns the total price of all items in the cart.")
    def get_total(self) -> str:
        total = sum(p.price for p in self._cart)
        return f"Total: ${total:.2f}"
