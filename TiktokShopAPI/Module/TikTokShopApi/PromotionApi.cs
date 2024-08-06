using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TiktokShopAPI.Module.TikTokShopApi.Services;

namespace TiktokShopAPI.Module.TikTokShopApi;

public class PromotionApi : TiktokShopApi
{
    private static readonly string Category = "promotion";
    internal static async Task<JObject> GetList(string pageToken = "")
    {
        var parameters = new JObject();
        var body = new JObject
        {
            { "page_size", 100 },
            { "status", "ONGOING" }
        };
        if (pageToken != "") body.Add("page_token", pageToken);
       
        return await CallAsync(Category, "activities/search", HttpMethod.Post, parameters, body);
    }
    internal static async Task<JObject> Detail(string activityid = "")
    {
        var parameters = new JObject();
        var body = new JObject();
        string action = $"activities/{activityid}";
        return await CallAsync(Category, action, HttpMethod.Get, parameters, body);
    }
}