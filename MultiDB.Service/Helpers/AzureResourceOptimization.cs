using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.IO;
using System.Threading.Tasks;

public class AzureResourceOptimization
{
  private readonly BlobServiceClient _blobServiceClient;
  private readonly IDistributedCache _redisCache;

  public AzureResourceOptimization(BlobServiceClient blobServiceClient, IDistributedCache redisCache)
  {
    _blobServiceClient = blobServiceClient;
    _redisCache = redisCache;
  }

  #region Blob Storage Methods

  public async Task UploadFileAsync(string clientName, string blobName, Stream fileStream)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(clientName);
    await containerClient.CreateIfNotExistsAsync();
    var blobClient = containerClient.GetBlobClient(blobName);
    await blobClient.UploadAsync(fileStream, overwrite: true);
  }

  public async Task<Stream> RetrieveFileAsync(string clientName, string blobName)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(clientName);
    var blobClient = containerClient.GetBlobClient(blobName);
    if (await blobClient.ExistsAsync())
    {
      return await blobClient.OpenReadAsync();
    }
    return null;
  }

  public async Task DeleteFileAsync(string clientName, string blobName)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(clientName);
    var blobClient = containerClient.GetBlobClient(blobName);
    await blobClient.DeleteIfExistsAsync();
  }

  #endregion

  #region Redis Cache Methods

  public async Task CacheDataAsync(string key, string data, TimeSpan expiration)
  {
    await _redisCache.SetStringAsync(key, data, new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = expiration
    });
  }

  public async Task<string> GetCachedDataAsync(string key)
  {
    return await _redisCache.GetStringAsync(key);
  }

  public async Task RemoveCachedDataAsync(string key)
  {
    await _redisCache.RemoveAsync(key);
  }

  #endregion

  #region Caching Strategy with Fallback

  public async Task<string> GetDataWithFallbackAsync(string key, Func<Task<string>> fetchFromDb)
  {
    var cachedData = await GetCachedDataAsync(key);
    if (!string.IsNullOrEmpty(cachedData))
    {
      return cachedData;
    }

    // Fallback to database
    var dataFromDb = await fetchFromDb();
    if (!string.IsNullOrEmpty(dataFromDb))
    {
      await CacheDataAsync(key, dataFromDb, TimeSpan.FromMinutes(10));
    }

    return dataFromDb;
  }

  #endregion
}
