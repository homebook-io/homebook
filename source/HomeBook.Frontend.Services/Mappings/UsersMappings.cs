using HomeBook.Client.Models;
using HomeBook.Frontend.Abstractions.Models;
using HomeBook.Frontend.Abstractions.Models.System;

namespace HomeBook.Frontend.Services.Mappings;

public static class UsersMappings
{
    public static PagedList<UserData> ToPagedResult(this GetUsersResponse response)
    {
        PagedList<UserData> page = new()
        {
            Page = response.Page ?? 0,
            PageSize = response.PageSize ?? 0,
            TotalCount = response.TotalCount ?? 0,
            TotalPages = response.TotalPages ?? 0,
            Items = response.Users.Select(x => x.ToUserData()).ToList()
        };

        return page;
    }

    public static UserData ToUserData(this UserResponse response)
    {
        return new UserData(response.Id.Value,
            response.Username!,
            response.Created.Value.UtcDateTime,
            response.Disabled?.UtcDateTime,
            response.IsAdmin.Value);
    }
}
