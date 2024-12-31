// using System.Data.Common;
// using System.Data.SQLite;
// using System.Net;
// using System.Security.Cryptography;
// using System.Text;
// using System.Text.Json;
// using Newtonsoft.Json;
// using Serilog;
// using TrackYourLife.Modules.Common.Application.Core.Abstractions.Services;
// using TrackYourLife.SharedLib.Domain.Errors;
// using TrackYourLife.SharedLib.Domain.Results;

// namespace TrackYourLife.Modules.Common.Infrastructure.Services;

// public class CookiesReader(ILogger logger) : ICookiesReader
// {
//     private const int AesGcmTagSize = 16;

//     private sealed class LocalStateDto
//     {
//         [JsonProperty("os_crypt")]
//         public OsCrypt OsCrypt { get; set; } = new OsCrypt();
//     }

//     private sealed class OsCrypt
//     {
//         [JsonProperty("encrypted_key")]
//         public string EncryptedKey { get; set; } = string.Empty;
//     }

//     public async Task<Result<List<Cookie>>> GetCookiesAsync(
//         byte[] cookieFileStream,
//         byte[] localStateFileStream,
//         string[] cookieDomains,
//         CancellationToken cancellationToken
//     )
//     {
//         var keyResult = GetKey(localStateFileStream);
//         if (keyResult.IsFailure)
//         {
//             return Result.Failure<List<Cookie>>(keyResult.Error);
//         }

//         string cookieFilePath = await CreateTempFileAsync(cookieFileStream, cancellationToken);

//         var cookiesResult = await ReadFromDbFileAsync(
//             cookieFilePath,
//             keyResult.Value,
//             cookieDomains,
//             cancellationToken
//         );

//         if (cookiesResult.IsFailure)
//         {
//             return Result.Failure<List<Cookie>>(cookiesResult.Error);
//         }

//         return cookiesResult;
//     }

//     private Result<byte[]> GetKey(byte[] localStateFileStream)
//     {
//         try
//         {
//             string localStateContent = Encoding.UTF8.GetString(localStateFileStream);
//             LocalStateDto? localState = JsonConvert.DeserializeObject<LocalStateDto>(
//                 localStateContent
//             );
//             string? encryptedKey = localState?.OsCrypt?.EncryptedKey;

//             if (string.IsNullOrEmpty(encryptedKey))
//                 throw new ArgumentNullException(encryptedKey, "Encrypted key is null or empty");

//             var keyWithPrefix = Convert.FromBase64String(encryptedKey);
//             var key = keyWithPrefix[5..];

// #pragma warning disable CA1416 // Validate platform compatibility
//             var masterKey = ProtectedData.Unprotect(key, null, DataProtectionScope.CurrentUser);
// #pragma warning restore CA1416 // Validate platform compatibility

//             return Result.Success(masterKey);
//         }
//         catch (Exception ex)
//         {
//             logger.Error(
//                 ex,
//                 "CookieReader failed reading the cookie file with exception {Exception}",
//                 ex
//             );

//             return Result.Failure<byte[]>(InfrastructureErrors.CookieReader.FailedToGetKey);
//         }
//     }

//     private static async Task<string> CreateTempFileAsync(
//         byte[] file,
//         CancellationToken cancellationToken
//     )
//     {
//         string tempFilePath = Path.GetRandomFileName();

//         await File.WriteAllBytesAsync(tempFilePath, file, cancellationToken);

//         return tempFilePath;
//     }

//     private async Task<Result<List<Cookie>>> ReadFromDbFileAsync(
//         string filePath,
//         byte[] key,
//         string[] cookieDomains,
//         CancellationToken cancellationToken
//     )
//     {
//         List<Cookie> result = [];

//         using SQLiteConnection connection = new($"Data Source={filePath}");

//         try
//         {
//             await connection.OpenAsync(cancellationToken);

//             SQLiteCommand command = connection.CreateCommand();

//             command.CommandText =
//                 $"SELECT name, encrypted_value, path, host_key FROM Cookies WHERE host_key LIKE '%{string.Join("%' OR host_key LIKE '%", cookieDomains)}%'";

//             using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

//             while (await reader.ReadAsync(cancellationToken))
//             {
//                 string? name = reader["name"].ToString();
//                 if (name is null)
//                 {
//                     continue;
//                 }
//                 string? path = reader["path"].ToString();
//                 string? domain = reader["host_key"].ToString();
//                 byte[] encrypted_value = (byte[])reader["encrypted_value"];

//                 string value = DecryptCookie(key, encrypted_value);

//                 Cookie cookie = new(name, value, path, domain);
//                 result.Add(cookie);
//             }

//             return Result.Success(result);
//         }
//         catch (SQLiteException exception)
//         {
//             logger.Error(
//                 exception,
//                 "CookieReader failed reading the cookie file with exception {Exception}",
//                 exception
//             );

//             return Result.Failure<List<Cookie>>(
//                 InfrastructureErrors.CookieReader.FailedToReadCookies
//             );
//         }
//         catch (Exception exception)
//         {
//             logger.Error(
//                 exception,
//                 "CookieReader failed reading the cookie file with exception {Exception}",
//                 exception
//             );

//             return Result.Failure<List<Cookie>>(
//                 InfrastructureErrors.CookieReader.FailedToReadCookies
//             );
//         }
//     }

//     private static string DecryptCookie(byte[] masterKey, byte[] cookie)
//     {
//         if (cookie.Length == 0)
//         {
//             return string.Empty;
//         }

//         byte[] nonce = cookie[3..15];
//         byte[] cipherText = cookie[15..(cookie.Length - 16)];
//         byte[] tag = cookie[(cookie.Length - 16)..];

//         byte[] resultBytes = new byte[cipherText.Length];

//         using AesGcm aesGcm = new(masterKey, AesGcmTagSize);
//         try
//         {
//             aesGcm.Decrypt(nonce, cipherText, tag, resultBytes);
//         }
//         catch
//         {
//             return string.Empty;
//         }
//         string cookieValue = Encoding.UTF8.GetString(resultBytes);
//         return cookieValue;
//     }
// }
