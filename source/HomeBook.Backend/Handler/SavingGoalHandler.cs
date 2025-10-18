using System.Security.Claims;
using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.Mappings;
using HomeBook.Backend.Responses;
using HomeBook.Backend.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public class SavingGoalHandler
{
    /// <summary>
    /// gets the user finance saving goals
    /// </summary>
    /// <param name="user"></param>
    /// <param name="logger"></param>
    /// <param name="savingGoalsProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleGetSavingGoals(ClaimsPrincipal user,
        [FromServices] ILogger<SavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            SavingGoalDto[] savingGoalDtos = await savingGoalsProvider.GetAllSavingGoalsAsync(userId,
                cancellationToken);
            FinanceSavingGoalResponse[] savingGoals = savingGoalDtos.Select(sg => sg.ToResponse()).ToArray();

            // wait 10 seconds
            await Task.Delay(10_000, cancellationToken);

            return TypedResults.Ok(new GetFinanceSavingGoalsResponse(savingGoals));
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while getting saving goals for user {UserId}",
                user.GetUserId());
            return TypedResults.InternalServerError(err.Message);
        }
    }
}
