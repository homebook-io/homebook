Feature: Get Backend Version
As a user of the HomeBook API
I want to retrieve the backend version
So that I can verify the API version

    Background:
        Given the API is available at the configured base URL

    Scenario: Get backend version
        When I request the backend version
        Then the response status code should be 200
        And the response should contain a valid version string
