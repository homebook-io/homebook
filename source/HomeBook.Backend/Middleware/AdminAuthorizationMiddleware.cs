using HomeBook.Backend.Abstractions.Contracts;
using HomeBook.Backend.Data.Contracts;

namespace HomeBook.Backend.Middleware;

/// <summary>
/// Attribute to mark endpoints that require admin authorization
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RequireAdminAttribute : Attribute
{
}

/// <summary>
/// Middleware to ensure only admin users can access protected endpoints
/// </summary>
public class AdminAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AdminAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService, IUserRepository userRepository)
    {
        // Check if the endpoint requires admin authorization
        var endpoint = context.GetEndpoint();
        bool requiresAdmin = endpoint?.Metadata.GetMetadata<RequireAdminAttribute>() != null;

        if (requiresAdmin)
        {
            string? authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing or invalid authorization header");
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            if (!jwtService.ValidateToken(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token");
                return;
            }

            Guid? userId = jwtService.GetUserIdFromToken(token);
            if (userId == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token - no user ID");
                return;
            }

            // Get user from database to check admin status
            var user = await userRepository.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("User not found");
                return;
            }

            if (!user.IsAdmin)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Admin access required");
                return;
            }

            // Add user information to context for use in handlers
            context.Items["UserId"] = userId.Value;
            context.Items["User"] = user;
        }

        await _next(context);
    }
}
