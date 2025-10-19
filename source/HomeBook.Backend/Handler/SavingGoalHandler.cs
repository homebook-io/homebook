using System.Security.Claims;
using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.Mappings;
using HomeBook.Backend.Requests;
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="savingGoalsProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleCreateSavingGoal(ClaimsPrincipal user,
        [FromBody] SavingGoalRequest request,
        [FromServices] ILogger<SavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            Guid savingGoalId = await savingGoalsProvider.CreateSavingGoalAsync(userId,
                request.Name,
                request.Color,
                request.TargetAmount,
                request.CurrentAmount,
                request.TargetDate,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while getting saving goals for user {UserId}",
                user.GetUserId());
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="savingGoalId"></param>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="savingGoalsProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleUpdateSavingGoal(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromBody] SavingGoalRequest request,
        [FromServices] ILogger<SavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await savingGoalsProvider.UpdateSavingGoalAsync(userId,
                savingGoalId,
                request.Name,
                request.Color,
                request.TargetAmount,
                request.CurrentAmount,
                request.TargetDate,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while getting saving goals for user {UserId}",
                user.GetUserId());
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="savingGoalId"></param>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="savingGoalsProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleDeleteSavingGoal(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromBody] SavingGoalRequest request,
        [FromServices] ILogger<SavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await savingGoalsProvider.DeleteSavingGoalAsync(userId,
                savingGoalId,
                cancellationToken);

            return TypedResults.Ok();
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
