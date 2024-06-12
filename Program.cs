using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

Console.WriteLine("Inicia lab azure blobs");
await ProcessAsync();
Console.WriteLine("Finalizo..");
Console.ReadLine();

static async Task ProcessAsync()
{
    string containerName= "contblob"; //+ Guid.NewGuid().ToString();
    string storageConn = "<<COLOCAR AQUI CONNECTION STRING>>";
    BlobServiceClient blobServiceClient= new BlobServiceClient(storageConn);
    var container = blobServiceClient.GetBlobContainerClient(containerName);

    BlobContainerClient blobContainerClient= container.Exists() ? container :
        await blobServiceClient.CreateBlobContainerAsync(containerName);
    FileData fileData= await createFile();
    BlobClient blobClient=blobContainerClient.GetBlobClient(fileData.Name);
    Console.WriteLine($"{containerName} blob container client");
    Console.WriteLine($"URL Blob storage: \n\t{blobClient.Uri}");

    try
    {
        using(FileStream uploadFileStream = File.OpenRead(fileData.FilePath))
        {
            await blobClient.UploadAsync(uploadFileStream);
            uploadFileStream.Close();
        }

        Console.WriteLine("El archivo se cargó correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error subiendo archivo: \n\t" + ex.Message);
    }

    Console.WriteLine("Listando Blobs disponibles..");

    await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
    {
        Console.WriteLine("\t" + blobItem.Name);
    }

    string downloadFilePath = fileData.FilePath.Replace(".txt", "-downloaded.txt");
    Console.WriteLine($"Descargando el blob a:\n\t {downloadFilePath}");
    await blobClient.DownloadToAsync(downloadFilePath);
    Console.WriteLine("Finaliza descarga..");
}

static async Task<FileData> createFile()
{
    string localPath ="./data/";
    string fileName = "labfile-" + Guid.NewGuid().ToString() + ".txt";
    string localFilePath = Path.Combine(localPath, fileName);
    await File.WriteAllTextAsync(localFilePath, "Este es un ejemplo de archivo a guardar en un blob");
    return new FileData(){
        FilePath = localFilePath,
        Name = fileName
    };
}

public class FileData
{
    public string Name { get; set; }
    public string FilePath { get; set;}
}