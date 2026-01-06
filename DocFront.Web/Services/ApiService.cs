using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DocFront.Utils;
using DocFront.Utils.Serialization;

namespace DocFront.Services;
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

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            options.Converters.Add(new DateOnlyJsonConverter());
            options.Converters.Add(new JsonStringEnumConverter());
            var json = await response.Content.ReadAsStringAsync();

            var content = JsonSerializer.Deserialize<T>(json, options);


            if (content is null)
                return ApiResponse<T>.Fail("Empty response");

            return ApiResponse<T>.Ok(content);
        }
        catch (Exception ex)
        {
            return ApiResponse<T>.Fail(ex.Message);
        }
    }
    protected async Task<ApiResponse<T>> GetWrappedAsync<T>(string endpoint)
    {
        try
        {
            var response = await _http.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
                return ApiResponse<T>.Fail(response.StatusCode.ToString());

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

            if (apiResponse == null)
                return ApiResponse<T>.Fail("Resposta inválida da API");

            return apiResponse;
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
    protected async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
    string endpoint,
    TRequest data)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, data);

            if (!response.IsSuccessStatusCode)
                return ApiResponse<TResponse>.Fail(response.StatusCode.ToString());

            var result = await response.Content
                .ReadFromJsonAsync<TResponse>();

            return ApiResponse<TResponse>.Ok(result!);
        }
        catch (Exception ex)
        {
            return ApiResponse<TResponse>.Fail(ex.Message);
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

