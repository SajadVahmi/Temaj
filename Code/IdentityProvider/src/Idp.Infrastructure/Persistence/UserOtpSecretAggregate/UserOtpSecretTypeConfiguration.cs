using Framework.Infrastructure.Persistence.ValueConvertors;
using Idp.Domain.UserOtpSecretAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Idp.Infrastructure.Persistence.UserOtpSecretAggregate;

public class UserOtpSecretTypeConfiguration:IEntityTypeConfiguration<UserOtpSecret>
{
    public void Configure(EntityTypeBuilder<UserOtpSecret> builder)
    {
        builder.ToTable("UserOtpSecret");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.SecretKey)
            .HasConversion(new EncryptedStringConverter())
            .HasMaxLength(64) 
            .IsRequired();

        builder.Property(c => c.UserId).IsRequired();

    }
}