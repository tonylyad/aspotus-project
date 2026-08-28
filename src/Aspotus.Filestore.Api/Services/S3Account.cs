namespace Aspotus.Filestore.Api.Services
{
    /// <summary>
    /// Данные пользователя в Yandex Cloud.
    /// </summary>
    public sealed class S3Account
    {
        /// <summary>
        /// Открытый ключ.
        /// </summary>
        public string PublicId { get; set; } = string.Empty;

        /// <summary>
        /// Закрытый ключ.
        /// </summary>
        public string PrivateId { get; set; } = string.Empty;

        /// <summary>
        /// Имя контейнера, хранящего файлы.
        /// </summary>
        public string BucketName { get; set; } = string.Empty;
    }
}
