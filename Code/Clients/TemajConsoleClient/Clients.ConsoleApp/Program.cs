using Clients.ConsoleApp.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// === CONFIGURATION ===
const string clientId = "spa";
const string redirectUri = "http://localhost:7154/callback/";
const string authorizationEndpoint = "https://localhost:7154/connect/authorize";
const string tokenEndpoint = "https://localhost:7154/connect/token";
const string scope = "openid profile email offline_access";

// === PKCE HELPER ===
string GenerateCodeVerifier()
{
    var bytes = new byte[32];
    RandomNumberGenerator.Fill(bytes);
    return Base64UrlEncode(bytes);
}

string GenerateCodeChallenge(string codeVerifier)
{
    using var sha256 = SHA256.Create();
    var challengeBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
    return Base64UrlEncode(challengeBytes);
}

string Base64UrlEncode(byte[] arg)
{
    return Convert.ToBase64String(arg)
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");
}

// === MAIN FLOW ===
var codeVerifier = GenerateCodeVerifier();
var codeChallenge = GenerateCodeChallenge(codeVerifier);

var state = Guid.NewGuid().ToString("N");
var url = $"{authorizationEndpoint}?response_type=code&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&code_challenge={codeChallenge}&code_challenge_method=S256&state={state}";

Console.WriteLine("Opening browser for login...");
try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }); }
catch { Console.WriteLine($"Please open: {url}"); }

// Start local HTTP listener for redirect URI
using var listener = new HttpListener();
listener.Prefixes.Add(redirectUri);
listener.Start();
Console.WriteLine($"Listening for authorization code on {redirectUri}");

var context = await listener.GetContextAsync();
var request = context.Request;
var response = context.Response;

var query = request.QueryString;
var code = query["code"];
var incomingState = query["state"];

if (string.IsNullOrEmpty(code) || incomingState != state)
{
    var error = query["error"] ?? "Unknown error";
    var errorMsg = $"Authorization failed: {error}";
    await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(errorMsg));
    response.Close();
    Console.WriteLine(errorMsg);
    return;
}

// Respond to browser
var html = "<html><body>Login complete. You may close this window.</body></html>";
await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(html));
response.Close();

Console.WriteLine($"Received code: {code}");

// === TOKEN REQUEST ===
using var http = new HttpClient();
var tokenReq = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
{
    Content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("code", code),
        new KeyValuePair<string, string>("redirect_uri", redirectUri),
        new KeyValuePair<string, string>("client_id", clientId),
        new KeyValuePair<string, string>("code_verifier", codeVerifier),
    })
};

var tokenResp = await http.SendAsync(tokenReq);
var tokenJson = await tokenResp.Content.ReadAsStringAsync();
if (!tokenResp.IsSuccessStatusCode)
{
    Console.WriteLine($"Token request failed: {tokenResp.StatusCode}\n{tokenJson}");
    return;
}

var tokens = JsonSerializer.Deserialize<TokenResponse>(tokenJson);
Console.WriteLine("Access Token:    " + tokens?.AccessToken);
Console.WriteLine("ID Token:        " + tokens?.IdToken);
Console.WriteLine("Refresh Token:   " + tokens?.RefreshToken);
Console.WriteLine("Token Type:      " + tokens?.TokenType);
Console.WriteLine("Expires In:      " + tokens?.ExpiresIn);
