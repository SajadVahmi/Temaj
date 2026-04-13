using Idp.Domain.UserAggregate;
using Idp.Domain.UserAggregate.Contracts;
using Idp.Infrastructure.Persistence._Shared;
using Microsoft.AspNetCore.Identity;

namespace Idp.Infrastructure.Persistence.UserAggregate;

public class UserRepository(IdpDbContext context,UserManager<UserDataModel> userManager) : IUserRepository
{
    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var dataModel =await userManager.FindByIdAsync(id.ToString());

        return dataModel == null
            ? null
            : User.FromSnapshot(dataModel.GetSnapshot());
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var dataModel =await userManager.FindByNameAsync(phoneNumber);

        return dataModel == null
            ? null
            : User.FromSnapshot(dataModel.GetSnapshot());
    }

    
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var dataModel = await userManager.FindByIdAsync(user.Id.ToString());
        if (dataModel is null)
        {
            dataModel = new UserDataModel();
            dataModel.ApplySnapshot(user.GetSnapshot());
            dataModel.ApplyEvents(user.GetEvents());
            await userManager.CreateAsync(dataModel);
        }
        else
        {
            dataModel.ApplySnapshot(user.GetSnapshot());
            dataModel.ApplyEvents(user.GetEvents());
            await userManager.UpdateAsync(dataModel);
        }
        
    }
}