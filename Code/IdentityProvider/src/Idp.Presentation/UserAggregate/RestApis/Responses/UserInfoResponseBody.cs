using System.Text.Json.Serialization;

namespace Idp.Presentation.UserAggregate.RestApis.Responses;

public class UserInfoResponseBody
{
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}