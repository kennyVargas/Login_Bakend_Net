using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Apitransac.Helpers
{
    public class RequestHash
    {
        public static string Create<T>(T request)
        {
            var json = JsonSerializer.Serialize(request);

            var bytes = Encoding.UTF8.GetBytes(json);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
