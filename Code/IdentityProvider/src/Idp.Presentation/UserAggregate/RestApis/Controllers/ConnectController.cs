//using Idp.Presentation.UserAggregate.RestApis.Requests;
//using Idp.Presentation.UserAggregate.RestApis.Responses;
//using Microsoft.AspNetCore.Mvc;

//namespace Idp.Presentation.UserAggregate.RestApis.Controllers;

//[ApiController]
//[Route("connect")]
//public class ConnectController : ControllerBase
//{

//    [HttpPost("token")]
//    [Consumes("application/x-www-form-urlencoded")]
//    [Produces("application/json")]
//    [ProducesResponseType(typeof(TokenResponseBody), 200)]
//    public IActionResult Token([FromForm] TokenRequestBody request) => throw new NotImplementedException();


//    [HttpGet("authorize")]
//    public IActionResult Authorize([FromQuery] AuthorizeRequestBody body) => throw new NotImplementedException();


//    [HttpGet("userinfo")]
//    [Produces("application/json")]
//    [ProducesResponseType(typeof(UserInfoResponseBody), 200)]
//    public IActionResult UserInfo() => throw new NotImplementedException();


//    [HttpPost("logout")]
//    [Produces("application/json")]
//    public IActionResult Logout() => throw new NotImplementedException();
//}