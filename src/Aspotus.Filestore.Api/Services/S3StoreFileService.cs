using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Aspotus.Filestore.Api.Infrastructure;
using System.Net;

namespace Aspotus.Filestore.Api.Services
{
    public class S3StoreFileService : IFileService
    {
        private string _bucketName;
        private readonly AmazonS3Client _s3client;

        public S3StoreFileService(S3Account account)
        {
            _bucketName = account.BucketName;
            var credentials = new BasicAWSCredentials(account.PublicId, account.PrivateId);
            var config = new AmazonS3Config
            {
                ServiceURL = "https://s3.yandexcloud.net" // Для Yandex Object Storage
            };

            _s3client = new AmazonS3Client(credentials, config);
        }

        public async Task<HttpStatusCode> DeleteFileAsync(string key, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var response = await _s3client.DeleteObjectAsync(request, cancellationToken);
                return response.HttpStatusCode;
            });
        }

        public async Task<byte[]> DownloadFileAsync(string key, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                using (GetObjectResponse response = await _s3client.GetObjectAsync(request, cancellationToken))
                {
                    return StreamToArray(response.ResponseStream);
                }
            });
        }

        private static byte[] StreamToArray(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<List<FileItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    MaxKeys = 500
                };

                var response = await _s3client.ListObjectsV2Async(request, cancellationToken);
                return GetFileItems(response.S3Objects);
            });
        }

        private List<FileItem> GetFileItems(List<S3Object> s3Objects)
        {
            List<S3Object> result = s3Objects ?? new List<S3Object>();
            return result.Select(s3Object => GetFileItem(s3Object.Key, s3Object.Size))
                .ToList();
        }

        public async Task<FileItem> UploadFileAsync(string key, byte[] content, CancellationToken cancellationToken)
        {
            return await Execute(async () =>
            {
                using (MemoryStream stream = new MemoryStream(content))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        InputStream = stream
                    };

                    var response = await _s3client.PutObjectAsync(request, cancellationToken);
                    return GetFileItem(key, content.LongLength);
                }
            });
        }

        private FileItem GetFileItem(string key, long? size)
        {
            return new FileItem
            {
                Key = key,
                Size = size.GetValueOrDefault(),
                Url = $"https://{_bucketName}.storage.yandexcloud.net/{key}"
            };
        }

        private async Task<T> Execute<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (AmazonS3Exception x)
            {
                throw new StatusCodeException(x.StatusCode, x.Message);
            }
        }

    }
}
