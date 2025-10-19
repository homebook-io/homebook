using HomeBook.Frontend.Module.Finances.Models;
using HomeBook.Frontend.Module.Finances.ViewModels;

namespace HomeBook.Frontend.Module.Finances.Mappings;

public static class SavingGoalMappings
{
    public static SavingGoalDto ToDto(this HomeBook.Client.Models.FinanceSavingGoalResponse savingGoal) =>
        new(
            savingGoal.Id!.Value,
            savingGoal.Name!,
            "",
            savingGoal.Color!,
            Convert.ToDecimal(savingGoal.TargetAmount),
            Convert.ToDecimal(savingGoal.CurrentAmount),
            savingGoal.TargetDate?.UtcDateTime ?? null);


    public static SavingGoalDto ToDto(this SavingGoalDetailViewModel vm) =>
        new(vm.Id,
            vm.Name,
            vm.IconName,
            vm.Color,
            vm.TargetAmount,
            vm.CurrentAmount,
            vm.TargetDate);


    public static SavingGoalDetailViewModel ToDetailViewModel(this SavingGoalDto dto) =>
        new()
        {
            Name = dto.Name,
            Color = dto.Color,
            TargetAmount = dto.TargetAmount,
            CurrentAmount = dto.CurrentAmount,
            TargetDate = dto.TargetDate
        };


    public static SavingGoalOverviewViewModel ToOverviewViewModel(this SavingGoalDto dto) =>
        new(dto.Id,
            dto.Name,
            dto.IconName,
            dto.Color,
            dto.TargetAmount,
            dto.CurrentAmount,
            dto.TargetDate,
            dto.Percentage);
}
