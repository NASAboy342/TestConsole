using System;
using System.IO;
using System.Net.Http;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TestConsole.Programs;

public class YoutubeService
{
    private readonly YoutubeClient _youtube;
    private readonly string _downloadPath;

    public YoutubeService()
    {
        _youtube = new YoutubeClient();
        // Set download path to user's Downloads folder
        _downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        
        // Create downloads directory if it doesn't exist
        if (!Directory.Exists(_downloadPath))
        {
            Directory.CreateDirectory(_downloadPath);
        }
    }

    internal async Task Run()
    {
        Console.WriteLine("=== YouTube Video Downloader ===");
        Console.WriteLine($"Downloads will be saved to: {_downloadPath}");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("Enter YouTube video URL (or 'exit' to quit): ");
            var videoUrl = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                Console.WriteLine("Invalid URL. Please try again.");
                continue;
            }

            if (videoUrl.ToLower() == "exit")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            try
            {
                await DownloadVideo(videoUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    private async Task DownloadVideo(string videoUrl)
    {
        Console.WriteLine("Fetching video information...");
        
        // Get video metadata
        var video = await _youtube.Videos.GetAsync(videoUrl);
        Console.WriteLine($"Video ID: {video.Id}");
        Console.WriteLine($"Title: {video.Title}");
        Console.WriteLine($"Author: {video.Author.ChannelTitle}");
        Console.WriteLine($"Duration: {video.Duration}");
        Console.WriteLine($"Video URL: {video.Url}");
        Console.WriteLine();

        // Get available streams with retry mechanism
        StreamManifest streamManifest;
        try
        {
            Console.WriteLine("Extracting stream information...");
            streamManifest = await GetStreamManifestWithRetry(video.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to extract streams: {ex.Message}");
            Console.WriteLine("\nThis might be due to:");
            Console.WriteLine("- Age-restricted content");
            Console.WriteLine("- Private/unlisted video");
            Console.WriteLine("- Geographic restrictions");
            Console.WriteLine("- YouTube's anti-bot measures");
            Console.WriteLine("- Recent YouTube API changes");
            
            Console.WriteLine("\n💡 Troubleshooting suggestions:");
            Console.WriteLine("1. Try a different video URL");
            Console.WriteLine("2. Try again in a few minutes (YouTube may be blocking)");
            Console.WriteLine("3. Use a video that's not age-restricted");
            Console.WriteLine("4. Try a shorter/simpler video URL");
            
            if (ex.Message.Contains("cipher"))
            {
                Console.WriteLine("\n🔧 Cipher-specific suggestions:");
                Console.WriteLine("- YouTube has updated their anti-bot protection");
                Console.WriteLine("- Try using: youtube-dl or yt-dlp as alternatives");
                Console.WriteLine("- This library may need an update from the developers");
            }
            
            return;
        }
        
        Console.WriteLine("Available download options:");
        Console.WriteLine("1. Best Quality Video (MP4)");
        Console.WriteLine("2. Audio Only (WebM/M4A - native format)");
        Console.WriteLine("3. Audio Only + Convert to MP3 (requires ffmpeg/avconv)");
        Console.WriteLine("4. Cancel");
        Console.Write("Choose option (1-4): ");
        
        var choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1":
                await DownloadVideoWithAudio(streamManifest, video);
                break;
            case "2":
                await DownloadAudioOnly(streamManifest, video, false);
                break;
            case "3":
                await DownloadAudioOnly(streamManifest, video, true);
                break;
            case "4":
                Console.WriteLine("Download cancelled.");
                return;
            default:
                Console.WriteLine("Invalid choice. Download cancelled.");
                return;
        }
    }

    private async Task DownloadVideoWithAudio(StreamManifest streamManifest, YoutubeExplode.Videos.Video video)
    {
        // Get the best quality muxed stream (video + audio)
        var streamInfo = streamManifest.GetMuxedStreams()
            .Where(s => s.Container == YoutubeExplode.Videos.Streams.Container.Mp4)
            .GetWithHighestVideoQuality();

        if (streamInfo == null)
        {
            Console.WriteLine("No suitable video stream found.");
            return;
        }

        var fileName = SanitizeFileName($"{video.Title}.{streamInfo.Container}");
        var filePath = Path.Combine(_downloadPath, fileName);

        Console.WriteLine($"Downloading: {streamInfo.VideoQuality} - {streamInfo.Container}");
        Console.WriteLine("Please wait...");

        // Download the stream
        await _youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

        Console.WriteLine($"✅ Download completed: {fileName}");
        Console.WriteLine($"File saved to: {filePath}");
    }

    private async Task DownloadAudioOnly(StreamManifest streamManifest, YoutubeExplode.Videos.Video video, bool convertToMp3 = false)
    {
        // Get the best quality audio stream
        var streamInfo = streamManifest.GetAudioOnlyStreams()
            .GetWithHighestBitrate();

        if (streamInfo == null)
        {
            Console.WriteLine("No suitable audio stream found.");
            return;
        }

        // Use the actual container format instead of forcing .mp3
        var extension = GetAudioExtension(streamInfo.Container);
        var fileName = SanitizeFileName($"{video.Title}.{extension}");
        var filePath = Path.Combine(_downloadPath, fileName);

        Console.WriteLine($"Downloading audio: {streamInfo.Bitrate} - {streamInfo.Container}");
        Console.WriteLine($"Format: {extension.ToUpper()} container");
        Console.WriteLine("Please wait...");

        // Download the stream
        await _youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

        Console.WriteLine($"✅ Download completed: {fileName}");
        Console.WriteLine($"File saved to: {filePath}");

        // Convert to MP3 if requested
        if (convertToMp3)
        {
            Console.WriteLine();
            var mp3Path = await ConvertToMp3Async(filePath);
            if (mp3Path != null && mp3Path != filePath)
            {
                Console.WriteLine($"🎵 MP3 file ready: {Path.GetFileName(mp3Path)}");
            }
        }
        else
        {
            Console.WriteLine($"📂 You can play this file with VLC, QuickTime, or any media player that supports {extension.ToUpper()}");
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove invalid characters from filename
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }
        
        // Limit length to avoid issues
        if (fileName.Length > 200)
        {
            var extension = Path.GetExtension(fileName);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            fileName = nameWithoutExt.Substring(0, 200 - extension.Length) + extension;
        }

        return fileName;
    }

    private static string GetAudioExtension(YoutubeExplode.Videos.Streams.Container container)
    {
        return container.Name.ToLower() switch
        {
            "webm" => "webm",
            "mp4" => "m4a",  // MP4 audio container is typically .m4a
            "3gpp" => "3gp",
            _ => container.Name.ToLower()
        };
    }

    private async Task<StreamManifest> GetStreamManifestWithRetry(YoutubeExplode.Videos.VideoId videoId, int maxRetries = 5)
    {
        Exception? lastException = null;
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (attempt > 1)
                {
                    Console.WriteLine($"Attempt {attempt}/{maxRetries} to extract streams...");
                    // Use exponential backoff with jitter
                    var delay = (int)(Math.Pow(2, attempt - 1) * 1000) + new Random().Next(500, 1500);
                    await Task.Delay(delay);
                }
                
                // Try different approaches for each attempt
                YoutubeClient tempClient;
                switch (attempt)
                {
                    case 1:
                        // Standard approach
                        tempClient = new YoutubeClient();
                        break;
                    case 2:
                        // Try with HTTP client configuration
                        var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Add("User-Agent", 
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                        tempClient = new YoutubeClient(httpClient);
                        break;
                    case 3:
                        // Try with different user agent
                        var httpClient2 = new HttpClient();
                        httpClient2.DefaultRequestHeaders.Add("User-Agent", 
                            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Safari/605.1.15");
                        tempClient = new YoutubeClient(httpClient2);
                        break;
                    case 4:
                        // Try with mobile user agent
                        var httpClient3 = new HttpClient();
                        httpClient3.DefaultRequestHeaders.Add("User-Agent", 
                            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1");
                        tempClient = new YoutubeClient(httpClient3);
                        break;
                    default:
                        // Last attempt with original client
                        tempClient = _youtube;
                        break;
                }
                
                var manifest = await tempClient.Videos.Streams.GetManifestAsync(videoId);
                
                if (attempt > 1)
                {
                    Console.WriteLine("✅ Stream extraction successful on retry!");
                }
                return manifest;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (attempt < maxRetries)
                {
                    Console.WriteLine($"⚠️ Attempt {attempt} failed: {ex.Message}");
                    if (ex.Message.Contains("cipher"))
                    {
                        Console.WriteLine($"   Trying different approach...");
                    }
                }
            }
        }
        
        // If all retries failed, throw the last exception
        throw lastException ?? new Exception("Stream extraction failed after all retries");
    }

    /// <summary>
    /// Converts audio file to MP3 format using available system tools
    /// </summary>
    /// <param name="inputPath">Path to the input audio file</param>
    /// <param name="outputPath">Path for the output MP3 file (optional, will use input path with .mp3 extension if not provided)</param>
    /// <param name="deleteOriginal">Whether to delete the original file after successful conversion</param>
    /// <returns>Path to the converted MP3 file, or null if conversion failed</returns>
    public static async Task<string?> ConvertToMp3Async(string inputPath, string? outputPath = null, bool deleteOriginal = true)
    {
        try
        {
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"❌ Input file not found: {inputPath}");
                return null;
            }

            // Generate output path if not provided
            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Path.ChangeExtension(inputPath, ".mp3");
            }

            Console.WriteLine($"🔄 Converting to MP3: {Path.GetFileName(inputPath)} → {Path.GetFileName(outputPath)}");

            // Try different conversion methods in order of preference
            bool converted = false;

            // Method 1: Try ffmpeg (most reliable)
            string ffmpegArgs = $"-i \"{inputPath}\" -acodec mp3 -ab 128k \"{outputPath}\"";
            if (await TryConvertWithCommand("ffmpeg", ffmpegArgs))
            {
                converted = true;
            }
            // Method 2: Try avconv (alternative to ffmpeg)
            else 
            {
                string avconvArgs = $"-i \"{inputPath}\" -acodec mp3 -ab 128k \"{outputPath}\"";
                if (await TryConvertWithCommand("avconv", avconvArgs))
                {
                    converted = true;
                }
                // Method 3: Try using macOS built-in tools (afconvert)
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    string afconvertArgs = $"\"{inputPath}\" \"{outputPath}\" -f mp3f";
                    if (await TryConvertWithCommand("afconvert", afconvertArgs))
                    {
                        converted = true;
                    }
                }
            }

            if (converted)
            {
                Console.WriteLine($"✅ Conversion successful: {Path.GetFileName(outputPath)}");
                
                // Delete original file if requested and conversion was successful
                if (deleteOriginal && File.Exists(outputPath))
                {
                    try
                    {
                        File.Delete(inputPath);
                        Console.WriteLine($"🗑️  Original file deleted: {Path.GetFileName(inputPath)}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️  Could not delete original file: {ex.Message}");
                    }
                }

                return outputPath;
            }
            else
            {
                Console.WriteLine("❌ No suitable audio conversion tool found on system.");
                Console.WriteLine("💡 To enable MP3 conversion, install one of the following:");
                Console.WriteLine("   • ffmpeg: brew install ffmpeg");
                Console.WriteLine("   • libav: brew install libav");
                Console.WriteLine($"📁 Original file kept: {inputPath}");
                return inputPath; // Return original file if conversion fails
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Conversion error: {ex.Message}");
            return inputPath; // Return original file if conversion fails
        }
    }

    /// <summary>
    /// Try to convert using a specific command-line tool
    /// </summary>
    private static async Task<bool> TryConvertWithCommand(string command, string arguments)
    {
        try
        {
            using var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            await process.WaitForExitAsync();

            return process.ExitCode == 0;
        }
        catch (Exception)
        {
            // Command not found or failed to execute
            return false;
        }
    }
}
