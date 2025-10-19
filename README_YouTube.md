# YouTube Video Downloader

This console application includes a YouTube video downloader using the free YoutubeExplode library.

## Features

- ✅ Download YouTube videos in MP4 format (best quality available)
- ✅ Download audio-only files in MP3 format
- ✅ Automatic file naming with sanitized titles
- ✅ Downloads saved to your Downloads folder
- ✅ Support for all YouTube URLs including music.youtube.com
- ✅ No API keys or payments required

## How to Use

1. Run the application: `dotnet run`
2. Enter a YouTube URL when prompted (e.g., "https://music.youtube.com/watch?v=enrCBI7O_6I&list=RDAMVM5PzQHZLiUPs")
3. Choose your download option:
   - **Option 1**: Best Quality Video (MP4) - Downloads video with audio
   - **Option 2**: Audio Only (MP3) - Downloads just the audio track
   - **Option 3**: Cancel - Cancels the download
4. Wait for the download to complete
5. Files are saved to your Downloads folder

## Supported URLs

- Regular YouTube: `https://www.youtube.com/watch?v=VIDEO_ID`
- YouTube Music: `https://music.youtube.com/watch?v=VIDEO_ID`
- YouTube Shorts: `https://www.youtube.com/shorts/VIDEO_ID`
- Any valid YouTube video URL

## Example Usage

```
=== YouTube Video Downloader ===
Downloads will be saved to: /Users/username/Downloads

Enter YouTube video URL (or 'exit' to quit): 
https://music.youtube.com/watch?v=enrCBI7O_6I

Fetching video information...
Title: Song Title
Author: Artist Name
Duration: 00:03:45

Available download options:
1. Best Quality Video (MP4)
2. Audio Only (MP3)
3. Cancel
Choose option (1-3): 2

Downloading audio: 128 Kbit/s
Please wait...
✅ Download completed: Song_Title.mp3
File saved to: /Users/username/Downloads/Song_Title.mp3
```

## Legal Notice

Please respect YouTube's Terms of Service and copyright laws. This tool should only be used for:
- Personal use
- Educational purposes
- Content you own or have permission to download
- Videos with appropriate licenses

## Dependencies

- YoutubeExplode (MIT License) - Free and open source
- .NET 8.0
