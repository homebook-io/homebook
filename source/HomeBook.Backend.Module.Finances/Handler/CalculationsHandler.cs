using HomeBook.Backend.Module.Finances.Contracts;
using HomeBook.Backend.Module.Finances.Enums;
using HomeBook.Backend.Module.Finances.Mappings;
using HomeBook.Backend.Module.Finances.Models;
using HomeBook.Backend.Module.Finances.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeBook.Backend.Module.Finances.Handler;

public class CalculationsHandler
{
    /// <summary>
    /// calculates the savings based on the provided parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="financeCalculationsService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleCalculateSavings([FromBody] CalculateSavingRequest request,
        [FromServices] ILogger<CalculationsHandler> logger,
        [FromServices] IFinanceCalculationsService financeCalculationsService,
        CancellationToken cancellationToken)
    {
        try
        {
            SavingCalculationResult result = request.InterestRateOption switch
            {
                InterestRateOptions.MONTHLY => financeCalculationsService.CalculateMonthlySavings(
                    request.TargetAmount,
                    request.TargetDate,
                    request.InterestRate,
                    request.TargetSimpleRate),

                InterestRateOptions.YEARLY => financeCalculationsService.CalculateYearlySavings(
                    request.TargetAmount,
                    request.TargetDate,
                    request.InterestRate,
                    request.TargetSimpleRate),

                InterestRateOptions.NONE => financeCalculationsService.CalculateSavings(
                    request.TargetAmount,
                    request.TargetDate),

                _ => throw new ArgumentOutOfRangeException(nameof(request.InterestRateOption),
                    "Invalid interest rate option")
            };

            return TypedResults.Ok(result.ToResponse());
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while calculating savings");
            return TypedResults.InternalServerError(err.Message);
        }
    }
}
