using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Aspotus.Filestore.Api.Services
{
    public class S3AccountService : BackgroundService
    {
        private readonly S3AccountSettings _settings;
        private readonly S3Account _accountToInit;
        private readonly ILogger<S3AccountService> _logger;

        public S3AccountService(IOptions<S3AccountSettings> options, S3Account accountToInit,
            ILogger<S3AccountService> logger) 
        { 
            _settings = options.Value;
            _accountToInit = accountToInit;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation("Чтение данных аккаунта начато.");
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    YandexIamClient yandexIamClient = new YandexIamClient();
                    string iamToken = await yandexIamClient.GetIamToken();
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {iamToken}");
                    _accountToInit.PublicId = await GetSecretValueAsync(httpClient, _settings.CredentialPublic, token);
                    _accountToInit.PrivateId = await GetSecretValueAsync(httpClient, _settings.CredentialPrivate, token);
                    _accountToInit.BucketName = await GetSecretValueAsync(httpClient, _settings.BucketId, token);
                }
            }
            catch (Exception x)
            {
                _logger.LogCritical(x, "Не удалось прочеть данные аккаунта.");
            }
            _logger.LogInformation("Чтение данных аккаунта завершено.");
        }

        private static async Task<string> GetSecretValueAsync(HttpClient httpClient, string secretId, CancellationToken token)
        {
            var response = await httpClient.GetAsync($"https://lockbox.api.cloud.yandex.net/lockbox/v1/secrets/{secretId}", token);
            response.EnsureSuccessStatusCode();

            var payloadContent = await response.Content.ReadAsStringAsync(token);
            var payloadDoc = JsonDocument.Parse(payloadContent);
            JsonElement currentVersion = payloadDoc.RootElement.GetProperty("currentVersion");

            if (currentVersion.TryGetProperty("payloadEntryKeys", out var payload))
            {
                return payload.EnumerateArray().First().GetString();
            }

            return null;
        }

    }
}
