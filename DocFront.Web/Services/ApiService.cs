using System.Net.Http;
using System.Net.Http.Json;
using DocFront.Utils;

public class ApiService
{
    protected readonly HttpClient _http;

    public ApiService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DocApi");
    }

    protected async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _http.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return ApiResponse<T>.Fail(response.StatusCode.ToString());

            var content = await response.Content.ReadFromJsonAsync<T>();

            if (content is null)
                return ApiResponse<T>.Fail("Empty response");

            return ApiResponse<T>.Ok(content);
        }
        catch (Exception ex)
        {
            return ApiResponse<T>.Fail(ex.Message);
        }
    }
    protected async Task<ApiResponse<bool>> PostAsync<T>(string endpoint, T data)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, data);

            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.Fail(response.StatusCode.ToString());

            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

    protected async Task<ApiResponse<bool>> PutAsync<T>(string endpoint, T data)
    {
        try
        {
            var response = await _http.PutAsJsonAsync(endpoint, data);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.Fail(response.StatusCode.ToString());

            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

    protected async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _http.DeleteAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.Fail(response.StatusCode.ToString());

            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

}

