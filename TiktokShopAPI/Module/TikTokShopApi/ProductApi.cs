using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TiktokShopAPI.Module.TikTokShopApi.Services;

namespace TiktokShopAPI.Module.TikTokShopApi;

public class ProductApi: TiktokShopApi
{
    private static string Category = "product";
    internal static async Task<JObject> GetList(string pageToken = "")
    {
        var parameters = new JObject
        {
            { "page_size", 100 }
        };
        if (pageToken != "") parameters.Add("page_token", pageToken);

        var body = new JObject
        {
            { "status", "ACTIVATE" },
            { "category_version", "v2" }
        };
        return await CallAsync(Category, "products/search", HttpMethod.Post, parameters, body);
    }
    internal static async Task<JObject> GetGlobalProduct()
    {
        var parameters = new JObject();
        var body = new JObject();
        return await CallAsync(Category, "global_products", HttpMethod.Get, parameters, body);
    }

    internal static async Task<JObject> GetBrandsAsync()
    {
        Category = "product";
        var parameters = new JObject
        {
            { "page_size", 100 },
            { "is_authorized", true }
        };
        return await CallAsync(Category, "brands", HttpMethod.Get, parameters);
    }
}