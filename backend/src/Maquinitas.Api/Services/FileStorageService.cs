namespace Maquinitas.Api.Services;

public class FileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration configuration, IWebHostEnvironment env)
    {
        var relative = configuration["FileStorage:BasePath"] ?? "App_Data/uploads";
        _basePath = Path.Combine(env.ContentRootPath, relative);
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> GuardarAsync(string subCarpeta, IFormFile archivo)
    {
        var carpeta = Path.Combine(_basePath, subCarpeta);
        Directory.CreateDirectory(carpeta);

        var extension = Path.GetExtension(archivo.FileName);
        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        await using var stream = File.Create(rutaCompleta);
        await archivo.CopyToAsync(stream);

        return Path.Combine(subCarpeta, nombreArchivo).Replace('\\', '/');
    }

    public (Stream stream, string contentType) Abrir(string rutaRelativa)
    {
        var rutaCompleta = Path.Combine(_basePath, rutaRelativa);
        var contentType = Path.GetExtension(rutaCompleta).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
        return (File.OpenRead(rutaCompleta), contentType);
    }
}
