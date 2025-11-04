using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Models;
using HomeBook.Backend.DTOs.Requests.Finances;
using HomeBook.Backend.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace HomeBook.Backend.Handler;

public class FinanceCalculationsHandler
{
    /// <summary>
    /// calculates the savings based on the provided parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="logger"></param>
    /// <param name="financeCalculationsService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleCalculateSavings([FromBody] FinanceCalculatedSavingRequest request,
        [FromServices] ILogger<FinanceCalculationsHandler> logger,
        [FromServices] IFinanceCalculationsService financeCalculationsService,
        CancellationToken cancellationToken)
    {
        try
        {
            SavingCalculationResult result = request.InterestRateOption switch
            {
                DTOs.Enums.InterestRateOptions.MONTHLY => financeCalculationsService.CalculateMonthlySavings(
                    request.TargetAmount,
                    request.TargetDate,
                    request.InterestRate,
                    request.TargetSimpleRate),

                DTOs.Enums.InterestRateOptions.YEARLY => financeCalculationsService.CalculateYearlySavings(
                    request.TargetAmount,
                    request.TargetDate,
                    request.InterestRate,
                    request.TargetSimpleRate),

                DTOs.Enums.InterestRateOptions.NONE => financeCalculationsService.CalculateSavings(
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
