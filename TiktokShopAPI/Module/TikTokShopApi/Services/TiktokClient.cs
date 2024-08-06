using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TiktokShopAPI.Module.TikTokShopApi.Services;

public class TiktokClient
{
    public string AccessToken;
    public string AppKey;
    public string AppSecret;
    public string Domain;
    public string ShopCipher;
    public string Refreshtoken;
    public string Accesstokenexpirein;
    public string Version = "202309";
    private HttpClient CallHttpClient { get; }

    public TiktokClient(string appKey, string appSecret, string accessToken, string shopCipher,string refreshtoken, string accesstokenexpirein = "", bool isProduction = true)
    {
        Domain = isProduction ? "https://open-api.tiktokglobalshop.com" : "https://open-api-sandbox.tiktokglobalshop.com";
        AccessToken = accessToken;
        AppKey = appKey;
        AppSecret = appSecret;
        ShopCipher = shopCipher;
        Refreshtoken = refreshtoken;
        Accesstokenexpirein = accesstokenexpirein;
        CallHttpClient = CreateHttpClient();
        CheckNgayHetHan();
    }

    private void CheckNgayHetHan()
    {
        if (Accesstokenexpirein == "")return;
      
        long timestamp = long.Parse(Accesstokenexpirein);
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        DateTime dateTime = dateTimeOffset.DateTime;

        Console.WriteLine("Ngày Hết Hạn Token: " + dateTime.ToString("dd/MM/yyyy HH:mm:ss"));
    }

    private HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(Domain)
        };
        //if (Options.ContainsKey("timeout")) httpClient.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(Options["timeout"]));
        return httpClient;
    }
    
    private void PrepareSignature(HttpRequestMessage request, ref JObject parameters)
    {
        var stringToBeSigned = "";
        var paramsToBeSigned = new JObject(parameters.Properties().OrderBy(p => p.Name));


        foreach (var property in paramsToBeSigned.Properties())
        {
            if (!string.IsNullOrEmpty(property.Value.ToString()))
            {
                stringToBeSigned += $"{property.Name}{property.Value}";
            }
        }

        stringToBeSigned = request.RequestUri.OriginalString + stringToBeSigned;

        if (request.Method != HttpMethod.Get && !request.Content.Headers.ContentType.MediaType.Equals("multipart/form-data"))
        {
            stringToBeSigned += request.Content.ReadAsStringAsync().Result;
        }

        stringToBeSigned = AppSecret + stringToBeSigned + AppSecret;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(AppSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToBeSigned));
        parameters["sign"] = BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public async Task<string> SendRequestAsync(HttpRequestMessage request, JObject parameters)
    {
        var endpoint = $"{request.RequestUri.OriginalString}";
        parameters["app_key"] = AppKey;
        parameters["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();


        // shop_cipher is not allowed in some api
        parameters["shop_cipher"] = ShopCipher;
        if (endpoint.StartsWith("/authorization/") || endpoint.StartsWith("/seller/")) parameters.Remove("shop_cipher");

        //parameters["sign"] = GenerateSignature(endpoint, parameters);
        PrepareSignature(request, ref parameters);
        
        request.Headers.Add("x-tts-access-token", AccessToken);

        var queryString = string.Join("&", parameters.Properties().Select(p => $"{p.Name}={p.Value}"));
        var requestUrl = $"{Domain}{endpoint}?{queryString}";
        request.RequestUri = new Uri(requestUrl);

        var response = await CallHttpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
    
}