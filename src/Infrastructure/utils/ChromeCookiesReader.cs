using System.Data.SQLite;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackYourLifeDotnet.Infrastructure.Utils;

public class ChromeCookiesReader
{
    class LocalStateDto
    {
        [JsonPropertyName("os_crypt")]
        public OsCrypt OsCrypt { get; set; } = new OsCrypt();
    }

    class OsCrypt
    {
        [JsonPropertyName("encrypted_key")]
        public string EncryptedKey { get; set; } = string.Empty;
    }

    private const string baseFolder = @"C:\Users\catal\AppData\Local\Microsoft\Edge\User Data";
    private const string CookiesFileName = @"Default\Network\Cookies";
    private const string LocalStateFileName = "Local State";

    public static CookieContainer GetCookies(string[] cookieDOmains)
    {
        byte[] key = GetKey(baseFolder);
        ICollection<Cookie> cookies = ReadFromDb(baseFolder, key, cookieDOmains);

        CookieContainer cookieContainer = new();
        foreach (Cookie cookie in cookies)
        {
            cookieContainer.Add(cookie);
        }

        return cookieContainer;
    }

    private static byte[] GetKey(string baseFolder)
    {
        string file = Path.Combine(baseFolder, LocalStateFileName);
        string localStateContent = File.ReadAllText(file);
        LocalStateDto? localState = JsonSerializer.Deserialize<LocalStateDto>(localStateContent);
        string? encryptedKey = localState?.OsCrypt?.EncryptedKey;

        if (string.IsNullOrEmpty(encryptedKey))
            throw new Exception("Encrypted key is null or empty");

        var keyWithPrefix = Convert.FromBase64String(encryptedKey);
        var key = keyWithPrefix[5..];

#pragma warning disable CA1416 // Validate platform compatibility
        var masterKey = ProtectedData.Unprotect(key, null, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416 // Validate platform compatibility

        return masterKey;
    }

    private static ICollection<Cookie> ReadFromDb(
        string baseFolder,
        byte[] key,
        string[] cookieDomains
    )
    {
        ICollection<Cookie> result = new List<Cookie>();
        string dbFileName = Path.Combine(baseFolder, CookiesFileName);
        using SQLiteConnection connection = new($"Data Source={dbFileName}");
        connection.Open();

        // long expireTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        SQLiteCommand command = connection.CreateCommand();

        command.CommandText =
            $"SELECT name, value, encrypted_value, path, host_key, expires_utc, is_secure, is_httponly FROM Cookies WHERE host_key LIKE '%{string.Join("%' OR host_key LIKE '%", cookieDomains)}%'";

        // command.Parameters.AddWithValue("$expireTime", expireTime);
        using (SQLiteDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string? name = reader["name"].ToString();
                if (name is null)
                {
                    continue;
                }
                string? path = reader["path"].ToString();
                string? domain = reader["host_key"].ToString();
                byte[] encrypted_value = (byte[])reader["encrypted_value"];

                string value = DecryptCookie(key, encrypted_value);

                Cookie cookie = new(name, value, path, domain);
                result.Add(cookie);
            }
        }

        return result;
    }

    private static string DecryptCookie(byte[] masterKey, byte[] cookie)
    {
        byte[] nonce = cookie[3..15];
        byte[] ciphertext = cookie[15..(cookie.Length - 16)];
        byte[] tag = cookie[(cookie.Length - 16)..(cookie.Length)];

        byte[] resultBytes = new byte[ciphertext.Length];

        using AesGcm aesGcm = new(masterKey);
        aesGcm.Decrypt(nonce, ciphertext, tag, resultBytes);
        string cookieValue = Encoding.UTF8.GetString(resultBytes);
        return cookieValue;
    }
}
