using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Requests;
using HomeBook.Backend.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HomeBook.Backend.Abstractions.Models;

namespace HomeBook.Backend.Handler;

public static class AccountHandler
{
    public static async Task<Results<Ok<LoginResponse>, BadRequest<ValidationProblemDetails>, UnauthorizedHttpResult>>
        HandleLogin([FromBody] LoginRequest request,
            [FromServices] IAccountProvider accountProvider,
            [FromServices] ILogger<object> logger,
            CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Login attempt for user: {Email}", request.Username);

            JwtTokenResult? loginResult = await accountProvider.LoginAsync(
                request.Username,
                request.Password,
                cancellationToken);

            if (loginResult is null)
            {
                logger.LogWarning("Login failed for user: {Email}", request.Username);
                return TypedResults.Unauthorized();
            }

            logger.LogInformation("Login successful for user: {Email}", request.Username);

            LoginResponse response = new()
            {
                Token = loginResult.Token,
                RefreshToken = loginResult.RefreshToken,
                ExpiresAt = loginResult.ExpiresAt,
                UserId = loginResult.UserId,
                Username = loginResult.Username
            };

            return TypedResults.Ok(response);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation error during login for user: {Email}", request.Username);
            var problemDetails = new ValidationProblemDetails
            {
                Title = "Validation Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            };
            return TypedResults.BadRequest(problemDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for user: {Email}", request.Username);
            return TypedResults.Unauthorized();
        }
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> HandleLogout(
        [FromServices] IAccountProvider accountProvider,
        [FromServices] ILogger<object> logger,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        try
        {
            HttpContext? httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                logger.LogWarning("HttpContext is null during logout");
                return TypedResults.BadRequest("Invalid request context");
            }

            // Get the current user's token from the authorization header
            string? authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                logger.LogWarning("No valid authorization header found during logout");
                return TypedResults.BadRequest("No valid token provided");
            }

            string token = authHeader["Bearer ".Length..].Trim();

            logger.LogInformation("Logout attempt for token");

            bool success = await accountProvider.LogoutAsync(token, cancellationToken);

            if (!success)
            {
                logger.LogWarning("Logout failed - invalid or expired token");
                return TypedResults.BadRequest("Logout failed");
            }

            logger.LogInformation("Logout successful");
            return TypedResults.Ok("Logout successful");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during logout");
            return TypedResults.BadRequest("Logout failed");
        }
    }
}
