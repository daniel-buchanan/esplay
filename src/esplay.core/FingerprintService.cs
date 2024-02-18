using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace esplay.core;

public interface IFingerprintService
{
    Task<string> Calculate<T>(T item);
}

public class FingerprintService : IFingerprintService
{
    public async Task<string> Calculate<T>(T item)
    {
        var type = typeof(T);
        var prop = type.GetProperty("KeyProperties");
        var input = string.Empty;

        if (prop != null)
        {
            var keyProperties = prop.GetValue(item) as IEnumerable<Expression<Func<T, object>>>;
            if (keyProperties == null) return await GetHash(JsonConvert.SerializeObject(item));
            foreach (var kp in keyProperties)
            {
                var func = kp.Compile();
                var val = func(item);
                input = string.Concat(input, "|", val?.ToString());
            }

            input = string.Concat(input, "|");
        }

        if (!string.IsNullOrWhiteSpace(input))
            return await GetHash(input);

        input = JsonConvert.SerializeObject(item);
        return await GetHash(input);
    }

    private Task<string> GetHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            // Convert the input string to a byte array
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Compute the hash
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Convert the byte array to a hexadecimal string
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return Task.FromResult(hash);
        }
    }
}