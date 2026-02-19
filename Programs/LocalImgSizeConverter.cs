using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace TestConsole.Programs;

public class LocalImgSizeConverter
{
    private string _folderPath = "";
    private string _destinationFolderPath = "";
    private const int _width = 200;
    private const int _height = 200;

    public void GetFolderPath()
    {
        Console.WriteLine("Please enter the folder path containing images:");
        _folderPath = Console.ReadLine() ?? "";
        ValidateFolderPath();
        Console.WriteLine("Please enter the destination folder path for resized images:");
        _destinationFolderPath = Console.ReadLine() ?? "";
        ValidateDestinationFolderPath();
    }

    private void ValidateDestinationFolderPath()
    {
        if (!Directory.Exists(_destinationFolderPath))
        {
            Console.WriteLine("The specified destination folder path does not exist. Please try again.");
            GetFolderPath();
        }
    }

    private void ValidateFolderPath()
    {
        if (!Directory.Exists(_folderPath))
        {
            Console.WriteLine("The specified folder path does not exist. Please try again.");
            GetFolderPath();
        }
    }

    public void ResizeImages()
    {
        var imageFiles = Directory.GetFiles(_folderPath);
        var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        
        foreach (var imageFile in imageFiles)
        {
            var extension = Path.GetExtension(imageFile).ToLowerInvariant();
            if (!supportedExtensions.Contains(extension))
            {
                Console.WriteLine($"Skipping non-image file: {Path.GetFileName(imageFile)}");
                continue;
            }

            try
            {
                using var image = Image.Load(imageFile);
                image.Mutate(x => x.Resize(_width, _height));
                var destinationPath = Path.Combine(_destinationFolderPath, Path.GetFileName(imageFile));
                image.Save(destinationPath);
                Console.WriteLine($"Resized image saved to: {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(imageFile)}: {ex.Message}");
            }
        }
        Console.WriteLine("All images have been resized.");
    }

    public void Run()
    {
        GetFolderPath();
        ResizeImages();
    }

    internal void RunWithGameId()
    {
        GetFolderPath();
        ResizeImagesWithGameId();
    }

    private void ResizeImagesWithGameId()
    {
        var imageFiles = Directory.GetFiles(_folderPath);
        var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        
        foreach (var imageFile in imageFiles)
        {
            var extension = Path.GetExtension(imageFile).ToLowerInvariant();
            if (!supportedExtensions.Contains(extension))
            {
                Console.WriteLine($"Skipping non-image file: {Path.GetFileName(imageFile)}");
                continue;
            }

            try
            {
                using var image = Image.Load(imageFile);
                image.Mutate(x => x.Resize(_width, _height));
                var gameId = Path.GetFileNameWithoutExtension(imageFile).Split('_')[1];
                // create the folder with name of the gameId
                var gameFolderPath = Path.Combine(_destinationFolderPath , gameId);
                if (!Directory.Exists(gameFolderPath))
                {
                    Directory.CreateDirectory(gameFolderPath);
                }
                var destinationPath = Path.Combine(_destinationFolderPath , gameId , Path.GetFileName(imageFile));
                image.Save(destinationPath);
                Console.WriteLine($"Resized image saved to: {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(imageFile)}: {ex.Message}");
            }
        }
        Console.WriteLine("All images have been resized.");
    }

    
}
