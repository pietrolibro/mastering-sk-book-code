from typing import Dict
from models.user_profile import UserProfile

class UserProfileService:
    def __init__(self):
        self._user_profiles: Dict[str, UserProfile] = {}
        self._user_profiles["user1"] = UserProfile(
            user_id="user1",
            name="Alice",
            email="alice@example.com",
            brand_affinity="BrandA",
            budget=500.0,
            category_interests=["Electronics", "Books"],
            latest_visited_products=["Smartphone X", "Cloud Book"],
        )
        self._user_profiles["user2"] = UserProfile(
            user_id="user2",
            name="Bob",
            email="bob@example.com",
            brand_affinity="BrandB",
            budget=1000.0,
            category_interests=["Fitness", "Electronics"],
            latest_visited_products=["Treadmill B", "Smartwatch A"],
        )
        self._user_profiles["user3"] = UserProfile(
            user_id="user3",
            name="Charlie",
            email="charlie@example.com",
            brand_affinity="BrandC",
            budget=200.0,
            category_interests=["Books", "Fitness"],
            latest_visited_products=["Yoga Mat D", "Resistance Bands C"],
        )

    def get_user_profile(self, user_id: str):
        return self._user_profiles.get(user_id)

    @property
    def user_profiles(self):
        return self._user_profiles

