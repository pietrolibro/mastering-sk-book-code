from dataclasses import dataclass, field
from typing import List

from models.product import Product

class ProductCatalogService:
    def get_available_products(self) -> List[Product]:
        return [
            Product(product_code="P001", name="Smartphone X", description="AI-powered phone", price=999.99, category="Electronics", brand="BrandA"),
            Product(product_code="P002", name="Cloud Book", description="Learn cloud computing", price=49.99, category="Books", brand="BrandB"),
            Product(product_code="P003", name="Wireless Headphones", description="Noise-canceling", price=199.99, category="Electronics", brand="BrandC"),
            Product(product_code="P004", name="Laptop Z", description="High-performance laptop", price=1299.99, category="Electronics", brand="BrandA"),
            Product(product_code="P005", name="Smartwatch A", description="Fitness smartwatch", price=149.99, category="Fitness", brand="BrandB"),
            Product(product_code="P006", name="Treadmill B", description="High-quality treadmill", price=799.99, category="Fitness", brand="BrandC"),
            Product(product_code="P007", name="Yoga Mat D", description="Premium yoga mat", price=24.99, category="Fitness", brand="BrandA"),
            Product(product_code="P008", name="Resistance Bands C", description="Set of resistance bands", price=29.99, category="Fitness", brand="BrandB"),
            Product(product_code="P009", name="Tablet P", description="Lightweight tablet", price=499.99, category="Electronics", brand="BrandC"),
            Product(product_code="P010", name="Cybersecurity Basics", description="Guide on online security", price=34.99, category="Books", brand="BrandA"),
            Product(product_code="P011", name="Gaming Console", description="Next-gen gaming console", price=399.99, category="Electronics", brand="BrandB"),
            Product(product_code="P012", name="Bluetooth Speaker", description="Portable speaker", price=99.99, category="Electronics", brand="BrandC"),
            Product(product_code="P013", name="Smart Light Bulb", description="Wi-Fi enabled light bulb", price=19.99, category="Electronics", brand="BrandA"),
            Product(product_code="P014", name="Electric Kettle", description="Fast boiling kettle", price=49.99, category="Home Appliances", brand="BrandB"),
            Product(product_code="P015", name="Air Purifier", description="HEPA filter purifier", price=199.99, category="Home Appliances", brand="BrandC"),
            Product(product_code="P016", name="Smart Thermostat", description="Energy-saving thermostat", price=249.99, category="Home Appliances", brand="BrandA"),
            Product(product_code="P017", name="Fitness Tracker", description="Track your workouts", price=129.99, category="Fitness", brand="BrandB"),
            Product(product_code="P018", name="E-Reader", description="Read books on the go", price=89.99, category="Books", brand="BrandC"),
            Product(product_code="P019", name="Noise-Canceling Earbuds", description="Compact and powerful", price=149.99, category="Electronics", brand="BrandA"),
            Product(product_code="P020", name="Smart Doorbell", description="Video-enabled doorbell", price=199.99, category="Home Appliances", brand="BrandB"),
        ]