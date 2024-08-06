using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TiktokShopAPI.Module.TikTokShopApi.Services;

namespace TiktokShopAPI.Module.TikTokShopApi;


public class TiktokAuth: TiktokShopApi
{
    private readonly HttpClient _httpClient = new();
    private const string AuthHost = "https://auth.tiktok-shops.com";
    internal static async Task<JObject> GetShops()
    {
        string category = "authorization";
        var parameters = new JObject();
        return await CallAsync(category, "shops", HttpMethod.Get, parameters);
    }
    
    public string CreateAuthRequest(string state = null)
    {
        var rand = new Random();
        var parameters = new JObject
        {
            { "app_key", Client.AppKey },
            { "state", state ?? rand.Next(10000, 99999).ToString() }
        };

        var queryString = string.Join("&", parameters.Properties().Select(p => $"{p.Name}={p.Value}"));
        var authUrl = $"{AuthHost}/oauth/authorize?{queryString}";

        return authUrl;
    }

    public async Task<JObject> GetToken(string code)
    {
        var requestUrl = $"{AuthHost}/api/v2/token/get";
        var parameters = new JObject
        {
            { "app_key", Client.AppKey },
            { "app_secret", Client.AppSecret },
            { "auth_code", code },
            { "grant_type", "authorized_code" }
        };

        var response = await _httpClient.GetAsync($"{requestUrl}?{string.Join("&", parameters.Properties().Select(p => $"{p.Name}={p.Value}"))}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseBody);

        if ((int)json["code"] != 0)
        {
            throw new AuthorizationException((string)json["message"], (int)json["code"]);
        }

        return (JObject)json["data"];
    }

    public async Task<JObject> RefreshToken()
    {
        var requestUrl = $"{AuthHost}/api/v2/token/refresh";
        var parameters = new JObject
        {
            { "app_key", Client.AppKey },
            { "app_secret", Client.AppSecret },
            { "refresh_token", Client.Refreshtoken },
            { "grant_type", "refresh_token" }
        };

        var response = await _httpClient.GetAsync($"{requestUrl}?{string.Join("&", parameters.Properties().Select(p => $"{p.Name}={p.Value}"))}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseBody);

        if ((int)json["code"] != 0)
        {
            throw new AuthorizationException((string)json["message"], (int)json["code"]);
        }

        return (JObject)json["data"];
    }
    
}

public class AuthorizationException : Exception
{
    public AuthorizationException(string s, int i)
    {
        throw new NotImplementedException();
    }
}