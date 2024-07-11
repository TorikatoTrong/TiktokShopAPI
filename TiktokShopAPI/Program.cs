using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TiktokShopAPI.Module.TikTokShopApi;

namespace TiktokShopAPI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        #region Private Data

        var appKey = "";
        var appSecret = "";
        var accessToken = "";
        var shopCipher = "";// Use Function GetAuthorizedShops To Get This

        #endregion
        
        var tiktokShopAuthorized = new TiktokShopAuthorized(appKey, appSecret, accessToken, shopCipher);
        var tiktokShopApi = new TiktokShopApi(tiktokShopAuthorized);
        
        var jsonString = await tiktokShopApi.GetAuthorizedShops();
        SaveJsonToFile(JObject.Parse(jsonString), "AuthorizedShops");
        
        Console.ReadKey();
    }
    public static void SaveJsonToFile(JObject jsonData, string filename = "TiktokData")
    {
        var json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
        var fullPath = Path.Combine(@"D:\TiktokData\", $"{filename}.json");
        File.WriteAllText(fullPath, json);
        Console.WriteLine($"Data has been saved to {fullPath}");
    }
}