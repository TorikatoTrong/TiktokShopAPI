using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TiktokShopAPI.Module.TikTokShopApi;

public class TiktokShopAuthorized(string appKey, string appSecret, string accessToken, string shopCipher, bool isProduction = true)
{
    private readonly string _domain = isProduction ? "https://open-api.tiktokglobalshop.com" : "https://open-api-sandbox.tiktokglobalshop.com";

    #region AccessToken - Use For Get AccessToken And RefreshAccessToken

    public async Task<string> RefreshAccessToken(string refreshToken)
    {
        var endpoint = $"{_domain}/token/refresh?app_key={appKey}&app_secret={appSecret}&refresh_token={refreshToken}&grant_type=refresh_token";
        return await CallRequest(endpoint, HttpMethod.Get, new JObject());
    }

    public async Task<string> GetAccessToken(string authCode)
    {
        var endpoint = $"{_domain}/token/get?app_key={appKey}&app_secret={appSecret}&auth_code={authCode}&grant_type=authorized_code";
        return await CallRequest(endpoint, HttpMethod.Get, new JObject());
    }

    private async Task<string> CallRequest(string endpoint, HttpMethod method, JObject parameters)
    {
        using var httpClient = new HttpClient();
        var request = new HttpRequestMessage(method, endpoint);
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
    #endregion

    public async Task<string> SendRequestAsync(string endpoint, HttpMethod method, JObject parameters, string body = null)
    {
        parameters["app_key"] = appKey;
        parameters["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var sign = GenerateSignature(endpoint, parameters, appSecret);
        parameters["sign"] = sign;

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-tts-access-token", accessToken);
        var queryString = string.Join("&", parameters.Properties().Select(p => $"{p.Name}={p.Value}"));
        var requestUrl = $"{_domain}{endpoint}?{queryString}";

        var request = new HttpRequestMessage(method, requestUrl);
        if (method == HttpMethod.Post || method == HttpMethod.Put) request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    private string GenerateSignature(string path, JObject parameters, string appSecret)
    {
        var sortedParams = parameters.Properties().OrderBy(p => p.Name);
        var concatenatedParams = string.Join("", sortedParams.Select(p => $"{p.Name}{p.Value}"));
        var stringToSign = $"{appSecret}{path}{concatenatedParams}{appSecret}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public async Task<string> GetRequestAsync(string endpoint, JObject parameters)
    {
        parameters["shop_cipher"] = shopCipher;
        return await SendRequestAsync(endpoint, HttpMethod.Get, parameters);
    }

}