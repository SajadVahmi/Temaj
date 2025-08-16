using Framework.Infrastructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Framework.Infrastructure.Persistence.ValueConvertors;

public class EncryptedStringConverter() : ValueConverter<string, string>(plain => EncryptionHelper.Encrypt(plain),
    encrypted => EncryptionHelper.Decrypt(encrypted))
{
   
}