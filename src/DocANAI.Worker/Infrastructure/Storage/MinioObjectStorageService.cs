using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace DocANAI.Worker.Infrastructure.Storage;

public sealed class MinioObjectStorageService : IObjectStorageService
{
    private readonly ILogger<MinioObjectStorageService> _logger;
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioObjectStorageService(IConfiguration configuration, ILogger<MinioObjectStorageService> logger)
    {
        _logger = logger;

        var endpoint = configuration["MinIO:Endpoint"]
            ?? throw new InvalidOperationException("MinIO:Endpoint is missing");
        var accessKey = configuration["MinIO:AccessKey"]
            ?? throw new InvalidOperationException("MinIO:AccessKey is missing");
        var secretKey = configuration["MinIO:SecretKey"]
            ?? throw new InvalidOperationException("MinIO:SecretKey is missing");
        _bucketName = configuration["MinIO:Bucket"]
            ?? throw new InvalidOperationException("MinIO:Bucket is missing");

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    public async Task<Stream> DownloadAsync(string objectName, CancellationToken ct = default)
    {
        try
        {
            await EnsureBucketExistsAsync(ct).ConfigureAwait(false);

            var memoryStream = new MemoryStream();
            var getArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(getArgs, ct).ConfigureAwait(false);

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (ObjectNotFoundException)
        {
            throw new FileNotFoundException($"Object '{objectName}' not found in bucket '{_bucketName}'");
        }
        catch (MinioException ex)
        {
            throw new ApplicationException($"MinIO error: {ex.Message}", ex);
        }
    }

    public async Task<string> UploadAsync(
        string objectName,
        Stream content,
        string contentType,
        CancellationToken ct = default)
    {
        try
        {
            await EnsureBucketExistsAsync(ct).ConfigureAwait(false);

            var putArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putArgs, ct).ConfigureAwait(false);

            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for object {ObjectName}", objectName);
            throw;
        }
    }

    private async Task EnsureBucketExistsAsync(CancellationToken ct)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);

        if (!await _minioClient.BucketExistsAsync(bucketExistsArgs, ct).ConfigureAwait(false))
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, ct).ConfigureAwait(false);
        }
    }
}
