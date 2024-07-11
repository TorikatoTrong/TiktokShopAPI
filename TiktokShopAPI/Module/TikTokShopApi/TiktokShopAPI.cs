using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TiktokShopAPI.Module.TikTokShopApi;


public class TiktokShopApi(TiktokShopAuthorized auth)
{
    private readonly string _version = "202309";

    public async Task<string> GetProductCategories()
    {
        var endpoint = $"/product/{_version}/categories";
        var parameters = new JObject();
        return await auth.GetRequestAsync(endpoint, parameters);
    }

    public async Task<string> GetBrandsAsync(int pageSize = 100)
    {
        var endpoint = $"/product/{_version}/brands";
        var parameters = new JObject
        {
            { "page_size", pageSize }
        };

        return await auth.GetRequestAsync(endpoint, parameters);
    }

    public async Task<string> GetAuthorizedShops()
    {
        var endpoint = $"/authorization/{_version}/shops";
        var parameters = new JObject();
        return await auth.SendRequestAsync(endpoint, HttpMethod.Get, parameters);
    }
}