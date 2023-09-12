using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PISEOAuthSarlaft.Models;

namespace PISEOAuthSarlaft.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("get/the/authorization/code")]
    public IActionResult GetTheCode()
    {
        string Authorization_Endpoint = _configuration["OAuth:Authorization_Endpoint"];
        string Response_Type = "code";
        string Client_Id = _configuration["OAuth:Client_ID"];
        string Client_RedirectURI = _configuration["OAuth:Redirect_URI"];
        string Scope = _configuration["OAuth:Scope"];
        const string State = "ThisIsMyStateValue";

        string URL = $"{Authorization_Endpoint}?" +
            $"response_type={Response_Type}&" +
            $"client_id={Client_Id}&" +
            $"redirect_uri={Client_RedirectURI}&" +
            $"scope={Scope}&state={State}";

        return Redirect(URL);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpGet("/authentication/login-back")]
    public IActionResult LoginCallBack([FromQuery] string code,string state) {
        return View((code, state));
    }


    [HttpGet("/exchange/the/authorization/code/token")]
    public async Task<IActionResult> ExchangeTheAuthorizationCodeForTokenAccess(string code, string state)
    {
        const string Grant_Type = "authorization_code";
        string Token_EndPoint = _configuration["OAuth:Token_Endpoint"];
        string Authorization_Endpoint = _configuration["OAuth:Authorization_Endpoint"];
        string Response_Type = "code";
        string Client_Id = _configuration["OAuth:Client_ID"];
        string Client_Secret = _configuration["OAuth:Client_Secret"];
        string Scope = _configuration["OAuth:Scope"];
        const string State = "ThisIsMyStateValue";
        string Client_RedirectURI = _configuration["OAuth:Redirect_URI"];

        Dictionary<string, string> BodyData = new Dictionary<string, string> {

            {"gran_type",Grant_Type },
            {"code",code },
            {"redirect_uri",Client_RedirectURI },
            {"client_id",Client_Id },
            {"client_secret",Client_Secret },
            {"scope",Client_Secret }
        };

        HttpClient httpClient = new HttpClient();
        var Body = new FormUrlEncodedContent(BodyData);

        var Response = await httpClient.PostAsync(Token_EndPoint, Body);
        var Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
        var JsonContent = await Response.Content.ReadFromJsonAsync<JsonElement>();
        var PrettyPrintJson = JsonSerializer.Serialize(JsonContent, new JsonSerializerOptions { WriteIndented = true });


        return View((Status,PrettyPrintJson,Response.IsSuccessStatusCode));
    }


}

