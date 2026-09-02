namespace Aspotus.Filestore.Api.Services
{
    public record S3AccountSettings
    {
        public const string SectionName = "S3Account";
        public required string CredentialPublic { get; set; }
        public required string CredentialPrivate { get; set; }
        public required string BucketId { get; set; }
    }
}
