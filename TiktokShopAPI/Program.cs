using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TiktokShopAPI.Module.TikTokShopApi;
using TiktokShopAPI.Module.TikTokShopApi.Services;

namespace TiktokShopAPI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        #region Private Data
        var appKey = "";
        var appSecret = "";
        var accessToken = "";
        var refreshtoken = "";
        var shopCipher = "";
        var accesstokenexpirein = "";
        #endregion

        TiktokShopApi.Client = new TiktokClient(appKey, appSecret, accessToken, shopCipher, refreshtoken, accesstokenexpirein);

        var jsonData = await TiktokAuth.GetShops();
        //var jsonData = await PromotionApi.Detail("");
        SaveJsonToFile(jsonData, "Promotion", "");
        Console.ReadKey();
    }

    public static void SaveJsonToFile(JObject jsonData, string subFolder, string filename = "TiktokData")
    {
        var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
        var fullPath = Path.Combine(@"D:\TiktokData", subFolder, $"{filename}.json");
        File.WriteAllText(fullPath, json);
        Console.WriteLine($"Data has been saved to {fullPath}");
    }
}