using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Idp.Infrastructure.Persistence.UserAggregate;

public class UserDataModelEntityConfiguration:IEntityTypeConfiguration<UserDataModel>
{
    public void Configure(EntityTypeBuilder<UserDataModel> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.AuthenticatorTwoFactorEnabledAt).IsRequired(false);
    }
}
