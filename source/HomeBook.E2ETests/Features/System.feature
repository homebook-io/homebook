@NeedsTestUser
Feature: System Endpoints
As a user of the HomeBook API
I want to manage the system settings

    Background:
        Given the API is available at the configured base URL
        And I have valid user credentials
        And I have valid test user

    Scenario: Get system informations without jwt token
        When I request HTTP GET system
        Then the response status code should be 401

    Scenario: Get system informations with non admin
        Given I have valid non admin user credentials
        When I attempt to login
        Then the login should succeed
        And an access token should be provided

        When I request HTTP GET system
        Then the response status code should be 401

    Scenario: Get system informations with admin
        Given I have valid admin user credentials
        When I attempt to login
        Then the login should succeed
        And an access token should be provided

        When I request HTTP GET system
        Then the response status code should be 200
        And the system info should be valid

    Scenario: Get all users without jwt token
        When I request HTTP GET system users
        Then the response status code should be 401
#
#    Scenario: Get all users with with non admin
#
#    Scenario: Get all users with with admin
#

    Scenario: Disable own user without jwt token
        When I request HTTP PUT system users disable
        Then the response status code should be 401
#
#    Scenario: Disable own user with with non admin
#
#    Scenario: Disable own user with with admin
