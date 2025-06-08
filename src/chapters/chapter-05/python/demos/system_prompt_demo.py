from helpers.kernel_helper import KernelHelper
from semantic_kernel.functions import KernelArguments

async def run_render_system_prompt_demo(kernel) -> str:
    print("\n=== DEMO: Authenticated/Guest Handlebars System Prompt Template ===\n")

    system_prompt_template = """
        <message role="system">
            You are an AI Shopping Assistant for a web store.

            You can:
            - Recommend products based on user preferences or product views
            - Suggest max {{num_of_products_to_suggest}} products at time.
            - Compare items
            - Manage a shopping cart (add, remove, total)
            - Answer questions about the catalog

        {{#if user_is_authenticated}}

            The user is logged in and has a profile. Use the SearchPlugin to find relevant user profile information:
            
            User id: {{user_id}}
            User name: {{user_name}}
            
            You must:
            - Greet and address the user by their name to personalize the conversation.
            - Use the user ID to retrieve their profile and preferences.
            - Recommend products based on the user's stored preferences, including favorite brands, category interests, and budget.
            - Always place products from the user's favorite brands at the top of the recommendations list.
            - Do not repeatedly ask the user for preferences, brand choices, or budget unless they explicitly request to update or provide them.
            - Sort the recommended products by Brand Affinity (favorite brands first), followed by Category Interests, and then by Budget suitability.

        {{else}}

            The user is a Guest. Recommend popular or generally useful products. IF is not required do not recommend books.

        {{/if}}

        </message>
    """

    helper = KernelHelper()

    arguments = KernelArguments( user_is_authenticated=True, 
                                user_name="Bob", 
                                user_id="user2", 
                                num_of_products_to_suggest=3 )
    
    rendered_system_prompt = await helper.render_prompt_template_async(
            kernel,system_prompt_template,arguments,"handlebars"
    )

    print(f"Rendered System Prompt:\n{rendered_system_prompt}\n")

    return rendered_system_prompt

