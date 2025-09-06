using HomeBook.Backend.Handler;
using HomeBook.Backend.Requests;
using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        RouteGroupBuilder group = routeBuilder
            .MapGroup("/account")
            .WithTags("account")
            .WithDescription("Endpoints for Account Management");

        group.MapPost("/login", AccountHandler.HandleLogin)
            .WithName("Login")
            .WithDescription("Authenticates a user and returns access tokens")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "User Login",
                Description = "Authenticates a user with email and password, returns JWT tokens"
            })
            .Accepts<LoginRequest>("application/json")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", AccountHandler.HandleLogout)
            .WithName("Logout")
            .WithDescription("Logs out the current user and invalidates their token")
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
                Summary = "User Logout",
                Description = "Invalidates the current user's access token"
            })
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest);

        return routeBuilder;
    }
}
