using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Presentation;
using IEnumerable.ForEach;
using LibGit2Sharp;
using LLama;
using LLama.Common;
using LLama.Native;
using MiNET.UI;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Crmf;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using TestConsole;
using TestConsole.Helper;
using TestConsole.Programs;
using TestConsole.Tests;
using Path = System.IO.Path;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await Run();
    }

    private static async Task RunSimulate()
    {
        var teststring = $"<|system|>\nYou are a helpful AI coding assistant that would come with code example.\n<|user|>\n<|assistant|>\n<|system|>\nYou are a helpful AI coding assistant that would come with code example.\n<|user|>\n<|assistant|>\n";
        var lastWord = "";
        foreach (var c in teststring)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            if (IsHitBreakIngWord(lastWord, c.ToString(), "<|user|>", out lastWord)) break;
            Console.Write(c + " ");
        }
    }

    private static async Task Run()
    {
        string relativePath = "model/deepseek-coder-6.7b-instruct.Q4_K_M.gguf";
        string basePath = AppContext.BaseDirectory;
        string modelPath = Path.Combine(basePath, relativePath);

        var modelParams = new ModelParams(modelPath)
        {
            ContextSize = 500,
            GpuLayerCount = -1, // Use -1 to offload all layers to GPU, or specify exact number
            UseMemorymap = true, // Enable memory mapping for better performance
            UseMemoryLock = false, // Disable memory locking to allow GPU usage
            MainGpu = 0, // Specify which GPU to use (0 for first GPU)
            BatchSize = 512, // Batch size for GPU processing
        };

        var model = LLamaWeights.LoadFromFile(modelParams);
        using var context = model.CreateContext(modelParams);

        var executor = new InteractiveExecutor(context);
        Console.WriteLine("LLM ready. Type something.");

        var isFirst = true;
        var lastWord = "";
        var breakingToken = "<|user|>";

        while (true)
        {
            Console.Write("> ");
            var prompt = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(prompt)) continue;

            await foreach (var result in executor.InferAsync(isFirst ? $"<|system|>\nYou are a helpful AI coding assistant that would come with code example.\n<|user|>\n{prompt}\n<|assistant|>\n" : $"{prompt}\n<|assistant|>\n"))
            {
                Console.Write(result);
                if (IsHitBreakIngWord(lastWord, result, breakingToken, out lastWord)) break;
            }
            Console.WriteLine();
            isFirst = false;
        }
    }

    private static bool IsHitBreakIngWord(string lastWord1, string result, string breakingToken, out string? lastWord2)
    {
        if(result.Equals(breakingToken))
        {
            lastWord2 = "";
            return true;
        }

        if (result.Contains(breakingToken))
        {
            lastWord2 = "";
            return true;
        }

        lastWord1 += result;
        if (lastWord1.Length > breakingToken.Length)
        {
            lastWord1 = lastWord1[^breakingToken.Length..];
        }
        lastWord2 = lastWord1;

        if (lastWord2.Equals(breakingToken))
        {
            lastWord2 = "";
            return true;
        }

        if (lastWord2.Contains(breakingToken))
        {
            lastWord2 = "";
            return true;
        }

        return false;
    }
}
