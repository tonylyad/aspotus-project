using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using YandexCloud.IamJwtCredentials;

namespace Aspotus.Filestore.Api.Services
{
    public class YandexIamClient
    {
        private const string 
            IamEndpoint = "https://iam.api.cloud.yandex.net/iam/v1/tokens",
            ResourceName = "Aspotus.Filestore.Api.Resources.jwt_config.bin";
        private readonly string _jwtConfig;

        public YandexIamClient()
        {
            _jwtConfig = ReadConfig();
        }

        /// <summary>
        /// По сервисному аккаунту получаем IAM-токен.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetIamToken()
        {
            string configData = ReadConfig();
            IamJwtCredentialsConfiguration configuration = JsonSerializer.Deserialize<IamJwtCredentialsConfiguration>(_jwtConfig)!;
            var provider = new IamJwtCredentialsProvider(configuration);

            var jwt = provider.GetJwtToken();

            // Отправляем JWT на обмен на IAM‑токен
            var requestBody = new { jwt = jwt };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(IamEndpoint, content);
                response.EnsureSuccessStatusCode();

                var respJson = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<IamTokenResponse>(respJson);
                return tokenResponse!.iamToken;
            }
        }

        private static string ReadConfig()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            
            using (Stream stream = assembly.GetManifestResourceStream(ResourceName)!)
            {
                byte[] binaryData = new byte[stream.Length];
                stream.Read(binaryData, 0, binaryData.Length);

                using (MemoryStream ms = new MemoryStream(binaryData))
                {
                    using (GZipStream gs = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        using (MemoryStream unzip = new MemoryStream())
                        {
                            gs.CopyTo(unzip);
                            return Encoding.UTF8.GetString(unzip.ToArray());
                        }
                    }
                }
            }
        }

        private class IamTokenResponse
        {
            public string iamToken { get; set; }
            public DateTime expiresAt { get; set; } // необязательно, но полезно
        }
    }
}
