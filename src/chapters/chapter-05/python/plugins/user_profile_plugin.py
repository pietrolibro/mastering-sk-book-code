import logging

from services.user_profile_service import UserProfileService

from semantic_kernel.functions import kernel_function
from typing import List, Optional
from dataclasses import dataclass, field

class UserProfilePlugin:
    def __init__(self, user_profile_service: UserProfileService, logger: logging.Logger):
        self._logger = logger
        self._user_profile_service = user_profile_service

    # #A Method to retrieve the budget limit for a user
    # Retrieves the budget limit for a specific user.
    # Arguments:
    # - user_id: The ID of the user whose budget limit is to be retrieved.
    @kernel_function(name="get_budget_limit", description="Retrieves the budget limit for a specific user.")
    def get_budget_limit(self, user_id: str) -> str:
        if user_id not in self._user_profile_service.user_profiles:
            return f"User with username '{user_id}' does not exist."

        profile = self._user_profile_service.user_profiles[user_id]
        return f"Budget for {user_id}: $ {profile.budget}"

    # #B Method to retrieve the brand affinity for a user
    # Retrieves the brand affinity for a specific user.
    # Arguments:
    # - user_id: The ID of the user whose brand affinity is to be retrieved.
    @kernel_function(name="get_brand_affinity", description="Retrieves the brand affinity for a specific user.")
    def get_brand_affinity(self, user_id: str) -> str:
        if user_id not in self._user_profile_service.user_profiles:
            return f"User with username '{user_id}' does not exist."

        profile = self._user_profile_service.user_profiles[user_id]
        return f"Brand Affinity for {user_id}: {profile.brand_affinity}"

    # #C Method to retrieve the category interests for a user
    # Retrieves the category interests for a specific user.
    # Arguments:
    # - user_id: The ID of the user whose category interests are to be retrieved.
    @kernel_function(name="get_category_interests", description="Retrieves the categories of interests for a specific user.")
    def get_category_interests(self, user_id: str) -> str:
        if user_id not in self._user_profile_service.user_profiles:
            return f"User with username '{user_id}' does not exist."

        profile = self._user_profile_service.user_profiles[user_id]
        return f"Category Interests for {user_id}: {', '.join(profile.category_interests)}"

    # #D Method to retrieve the email address for a user
    # Retrieves the email address for a specific user.
    # Arguments:
    # - user_id: The ID of the user whose email address is to be retrieved.
    @kernel_function(name="get_email_address", description="Retrieves the email address for a specific user.")
    def get_email_address(self, user_id: str) -> str:
        if user_id not in self._user_profile_service.user_profiles:
            return f"User with username '{user_id}' does not exist."

        profile = self._user_profile_service.user_profiles[user_id]
        return f"Email Address for {user_id}: {profile.email}"

    # #E Method to retrieve the latest visited products for a user
    # Retrieves the latest 5 visited products for a specific user.
    # Arguments:
    # - user_id: The ID of the user whose latest visited products are to be retrieved.
    @kernel_function(name="get_latest_visited_products", description="Retrieves the latest visited products for a specific user.")
    def get_latest_visited_products(self, user_id: str) -> str:
        if user_id not in self._user_profile_service.user_profiles:
            return f"User with username '{user_id}' does not exist."

        profile = self._user_profile_service.user_profiles[user_id]
        return f"Latest Visited Products for {user_id}: {', '.join(profile.latest_visited_products)}"

