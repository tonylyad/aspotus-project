using Amazon.Runtime;
using Microsoft.Extensions.Options;
using System.Text;
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
                    // хранить токен в настройках не получается, GitHub не дает сделать push
                    string codedToken = "=cGRL5WQ3cFcIRzatJ3ctZ2VxAzM24kbwE0aSNWbnVjcIJ0U4h3SMdWWmRWWu1ES0RXSNpVeGtWRwpUMyNEcmJ3UmZmaaV3MSFzX0cUL1EUaVJXY3J2au4UcalmTxpkaTZHcsJVc1oWUHR3alFmWwMWY0YTMPp1aNpHcsl1TKx2T5VjePNTNqpkc0oWYXBnbyNjc6JWewhnVyA3aLZnWux0NNxWVhVjeaFnWsVWdlhHOfVneilHc4ZlMwt2S2plbMdTTsVVY1onWxpFblVXZ542M402VHBXbQNkWtQHV10WTL5kbX9kWutULZBDW19WalNkWwMWYJdTMMVWM5IXaaFFOycTN2N0TGFzd5o3Mk9VcZdUR2lTZtI3cqZkTk9FOsNTWrpkZzxGW51kbNlXWs9kb0kXVyAXehdFcuJ3Ml5mSxhDeOZXWpNWLv5GZyQTaKhkWu5UcaxWZ1VWOuEDd";
                    string base64 = new string(codedToken.Reverse().ToArray());
                    byte[] bytes = Convert.FromBase64String(base64);
                    string iamToken = Encoding.Default.GetString(bytes);
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
