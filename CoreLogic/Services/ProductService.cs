using System.Net.Http;
using System.Text.Json;

namespace UPB.CoreLogic.Services;

public class ProductService
{
    private string _uri;

    public ProductService(string uri)
    {
        _uri = uri;
    }

    public async Task<double> getRandom()
    {
        HttpClient sharedClient = new()
        {
            BaseAddress = new Uri(_uri),
        };

        //Respuesta
        using HttpResponseMessage response = await sharedClient.GetAsync("");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var element = JsonSerializer.Deserialize<JsonElement>(json);
        var decimalValue = element.GetProperty("decimal").GetDouble();
        return decimalValue;
    }
}