using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

public class GoogleCloudStorageService
{
    private readonly string _bucketName = "credirapidbucket"; // Nombre del bucket en línea
    private readonly StorageClient _storageClient;

    public GoogleCloudStorageService()
    {
        // Ruta relativa al archivo de credenciales JSON
        var credentialsPath = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "credentials.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

        // Conectar a Google Cloud Storage
        _storageClient = StorageClient.Create();
    }

    public async Task<string> SubirArchivoAsync(Stream stream, string filePath, string objectName)
    {
        using var fileStream = File.OpenRead(filePath);
        await _storageClient.UploadObjectAsync(_bucketName, objectName, null, fileStream);
        Console.WriteLine($"Archivo subido a {_bucketName}/{objectName}");

        // URL del archivo
        return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
    }

    public void ProbarConexion()
    {
        var projectId = "seraphic-effect-457504-r0"; // ID de tu proyecto
        var buckets = _storageClient.ListBuckets(projectId);
        foreach (var bucket in buckets)
        {
            Console.WriteLine($"Bucket encontrado: {bucket.Name}");
        }
    }
}