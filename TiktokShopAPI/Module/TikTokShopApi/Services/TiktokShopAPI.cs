using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TiktokShopAPI.Module.TikTokShopApi.Services;

public class TiktokShopApi
{
 
    public static TiktokClient Client ;
    public static string LastMessage;
    public static string LastRequestId;
    
    protected internal static async Task<JObject> CallAsync(string category,string action, HttpMethod method, JObject parameters, JObject body = null)
    {
        var uri = $"/{category}/{Client.Version}/{action.Trim('/')}";
        try
        {
            var request = new HttpRequestMessage(method, uri);
            if (method == HttpMethod.Post || method == HttpMethod.Put) request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var responseBody = await Client.SendRequestAsync(request, parameters);
            var json = JObject.Parse(responseBody);

            LastMessage = json["message"]?.ToString();
            LastRequestId = json["request_id"]?.ToString();

            var code = json["code"]?.Value<int>() ?? -1;
            if (code != 0) HandleErrorResponse(code, json["message"]?.ToString());
            else return (JObject)json["data"];
        }
        catch (HttpRequestException e)
        {
            HandleErrorResponse(0, e.Message);

        }
        return new JObject();
    }

    private static void HandleErrorResponse(int code, string message)
    {
        var errorGroup = code.ToString().Substring(0, 3);

        switch (errorGroup)
        {
            case "105":
            case "360":
                Console.WriteLine($"{code}: {message}");
                break;
            default:
                Console.WriteLine($"{code}: {message}");
                break;
        }
    }
}
