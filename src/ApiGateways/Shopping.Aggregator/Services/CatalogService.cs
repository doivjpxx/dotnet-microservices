using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;

    public CatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CatalogModel>> GetCatalog()
    {
        var res = await _httpClient.GetAsync("api/v1/Catalog");
        return await res.ReadContentAs<List<CatalogModel>>();
    }

    public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
    {
        var res = await _httpClient.GetAsync($"api/v1/Catalog/GetProductByCategory/{category}");
        return await res.ReadContentAs<List<CatalogModel>>();
    }

    public async Task<CatalogModel> GetCatalog(string id)
    {
        var res = await _httpClient.GetAsync($"api/v1/Catalog/{id}");
        return await res.ReadContentAs<CatalogModel>();
    }
}