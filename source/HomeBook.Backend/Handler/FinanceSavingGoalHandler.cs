using System.Security.Claims;
using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.DTOs.Requests.Finances;
using HomeBook.Backend.DTOs.Responses.Finances;
using HomeBook.Backend.Mappings;
using HomeBook.Backend.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public class FinanceSavingGoalHandler
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
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            SavingGoalDto[] savingGoalDtos = await savingGoalsProvider.GetAllSavingGoalsAsync(userId,
                cancellationToken);
            FinanceSavingGoalResponse[] savingGoals = savingGoalDtos.Select(sg => sg.ToResponse()).ToArray();

            return TypedResults.Ok(new FinanceSavingGoalListResponse(savingGoals));
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
        [FromBody] CreateSavingGoalRequest request,
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            Guid savingGoalId = await savingGoalsProvider.CreateSavingGoalAsync(userId,
                request.Name,
                request.Color,
                request.Icon,
                request.TargetAmount,
                request.CurrentAmount,
                request.MonthlyPayment,
                request.InterestRateOption,
                request.InterestRate,
                request.TargetDate,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while creating saving goal");
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
    public static async Task<IResult> HandleUpdateSavingGoalName(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromBody] UpdateSavingGoalNameRequest request,
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await savingGoalsProvider.UpdateSavingGoalNameAsync(userId,
                savingGoalId,
                request.Name,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while updating saving goal name for {SavingGoalId}",
                savingGoalId);
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
    public static async Task<IResult> HandleUpdateSavingGoalAppearance(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromBody] UpdateSavingGoalAppearanceRequest request,
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await savingGoalsProvider.UpdateSavingGoalAppearanceAsync(userId,
                savingGoalId,
                request.Color,
                request.Icon,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while updating saving goal appearance for {SavingGoalId}",
                savingGoalId);
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
    public static async Task<IResult> HandleUpdateSavingGoalAmounts(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromBody] UpdateSavingGoalAmountsRequest request,
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await savingGoalsProvider.UpdateSavingGoalAmountsAsync(userId,
                savingGoalId,
                request.TargetAmount,
                request.CurrentAmount,
                request.MonthlyPayment,
                request.InterestRateOption,
                request.InterestRate,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while updating saving goal amounts for {SavingGoalId}",
                savingGoalId);
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
    public static async Task<IResult> HandleUpdateSavingGoalInfo(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromBody] UpdateSavingGoalInfoRequest request,
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
        [FromServices] ISavingGoalsProvider savingGoalsProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid userId = user.GetUserId();

            await savingGoalsProvider.UpdateSavingGoalInfoAsync(userId,
                savingGoalId,
                request.TargetDate,
                cancellationToken);

            return TypedResults.Ok();
        }
        catch (Exception err)
        {
            logger.LogError(err,
                "Error while updating saving goal infos for {SavingGoalId}",
                savingGoalId);
            return TypedResults.InternalServerError(err.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="savingGoalId"></param>
    /// <param name="user"></param>
    /// <param name="logger"></param>
    /// <param name="savingGoalsProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleDeleteSavingGoal(Guid savingGoalId,
        ClaimsPrincipal user,
        [FromServices] ILogger<FinanceSavingGoalHandler> logger,
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
                "Error while deleting saving goal for {SavingGoalId}",
                savingGoalId);
            return TypedResults.InternalServerError(err.Message);
        }
    }
}
