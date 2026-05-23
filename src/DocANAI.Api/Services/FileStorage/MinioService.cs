using Minio;
using Minio.Exceptions;
using Minio.DataModel.Args;

namespace DocANAI.Api.Services.FileStorage;

public class MinioService : IMinioService
{
    private readonly ILogger<MinioService> _logger;

    private readonly IMinioClient _minioClient;
    private readonly IMinioClient _minioPublicClient;

    private readonly string _bucketName;

    public MinioService(IConfiguration configuration, ILogger<MinioService> logger)
    {
        _logger = logger;

        var privateEndpoint = configuration["MinIO:Endpoint"] ?? throw new InvalidOperationException("MinIO:Endpoint missing");
        var publicEndpoint = configuration["MinIO:PublicEndpoint"] ?? throw new InvalidOperationException("MinIO:PublicEndpoing missing");
        var accessKey = configuration["MinIO:AccessKey"] ?? throw new InvalidOperationException("MinIO:AccessKey missing");
        var secretKey = configuration["MinIO:SecretKey"] ?? throw new InvalidOperationException("MinIO:SecretKey missing");
        _bucketName = configuration["MinIO:Bucket"] ?? throw new InvalidOperationException("MinIO:Bucket missing");

        _minioClient = new MinioClient()
            .WithEndpoint(privateEndpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false) // TODO: local - false, production - true
            .Build();
        
        _minioPublicClient = new MinioClient()
            .WithEndpoint(publicEndpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false) // TODO: local - false, production - true
            .Build();
    }

    public async Task<string> UploadFileAsync(IFormFile file, string? objectName = null)
    {
        try
        {
            await EnsureBucketExistsAsync().ConfigureAwait(false);

            objectName ??= $"{Guid.NewGuid()}/{file.FileName}";
            await using var stream = file.OpenReadStream();

            var putArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType);
            
            await _minioClient.PutObjectAsync(putArgs).ConfigureAwait(false);

            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload failed for file {FileName}", file.FileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string objectName)
    {
        try
        {
            await EnsureBucketExistsAsync().ConfigureAwait(false);

            var memoryStream = new MemoryStream();
            var getArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));
            
            await _minioClient.GetObjectAsync(getArgs).ConfigureAwait(false);
            
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

    public async Task<bool> DeleteFileAsync(string objectName)
    {
        await EnsureBucketExistsAsync().ConfigureAwait(false);

        try
        {
            var statArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);
            
            await _minioClient.StatObjectAsync(statArgs).ConfigureAwait(false);
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
        
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName);
        
        await _minioClient.RemoveObjectAsync(removeArgs).ConfigureAwait(false);

        return true;
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, int expiryMinutes = 5)
    {
        await EnsureBucketExistsAsync().ConfigureAwait(false);

        var presignedArgs = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithExpiry(expiryMinutes * 60);

        return await _minioPublicClient.PresignedGetObjectAsync(presignedArgs).ConfigureAwait(false);
    }

    private async Task EnsureBucketExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);

        if (!await _minioClient.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false))
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
        }
    }
}