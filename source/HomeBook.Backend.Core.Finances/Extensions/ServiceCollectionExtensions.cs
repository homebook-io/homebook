using FluentValidation;
using HomeBook.Backend.Abstractions;
using HomeBook.Backend.Core.Finances.Contracts;
using HomeBook.Backend.Core.Finances.Validators;
using HomeBook.Backend.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeBook.Backend.Core.Finances.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendCoreFinances(this IServiceCollection services,
        IConfiguration configuration,
        InstanceStatus instanceStatus)
    {
        services.AddScoped<ISavingGoalsProvider, SavingGoalsProvider>();
        services.AddScoped<IFinanceCalculationsService, FinanceCalculationsService>();

        services.AddSingleton<IValidator<SavingGoal>, SavingGoalValidator>();

        return services;
    }
}
