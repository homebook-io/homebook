Feature: User Authentication API
As a user of the HomeBook API
I want to authenticate with username and password
So that I can access protected resources

    Background:
        Given the API is available at the configured base URL
        And I have valid test user credentials

    Scenario: Successful user login and logout
        Given I have valid user credentials
        When I attempt to login
        Then the login should succeed
        And an access token should be provided
        And user information should be returned
        When I attempt to logout
        Then the logout should succeed
