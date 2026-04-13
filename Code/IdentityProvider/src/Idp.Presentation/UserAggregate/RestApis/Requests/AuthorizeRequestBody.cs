using Microsoft.AspNetCore.Mvc;

namespace Idp.Presentation.UserAggregate.RestApis.Requests;

public class AuthorizeRequestBody
{
    [FromQuery(Name = "client_id")]
    public string ClientId { get; set; } = "";

    [FromQuery(Name = "redirect_uri")]
    public string RedirectUri { get; set; } = "";

    [FromQuery(Name = "response_type")]
    public string ResponseType { get; set; } = "code";

    [FromQuery(Name = "scope")]
    public string Scope { get; set; } = "openid profile";

    [FromQuery(Name = "code_challenge")]
    public string CodeChallenge { get; set; }

    [FromQuery(Name = "code_challenge_method")]
    public string CodeChallengeMethod { get; set; } = "S256";
}