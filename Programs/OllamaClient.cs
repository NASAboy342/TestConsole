using System;
using System.Text;
using Newtonsoft.Json;

namespace TestConsole.Programs;

public class OllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _modelUrl = "http://localhost:11434/api/chat";
    private readonly string _modelName = "qwen3:14b";
    private const int _contextLength = 40960;
    private int _promptEvalCount = 0;
    private int _evalCount = 0;
    private List<OllamaMessage> _messages = new List<OllamaMessage>();

    public OllamaClient()
    {
        _httpClient = new HttpClient();
    }
    public async Task Run()
    {
        while(true)
        {
            var userInput = GetUserInput();
            var questionRequest = GenerateUserQuestionRequest(userInput);
            _messages.Add(await GetAnswer(questionRequest));
            if (IsNearContextLimit()) await SummarizeConversation();
        }
    }

    private async Task SummarizeConversation()
    {
        var summaryPrompt = new List<OllamaMessage>
        {
            new OllamaMessage
            {
                Role = "system",
                Content = "You are a helpful assistant for summarizing conversations. Please provide a concise summary of the following conversation history, focusing on the main points and key information. The original goal that the user wanted to achieve is to be included and emphasized in the summary."
            }
        };
        summaryPrompt.AddRange(_messages);
        summaryPrompt.Add(new OllamaMessage
        {
            Role = "user",
            Content = "Please summarize the above conversation history."
        });

        var summaryRequest = new
        {
            model = _modelName,
            messages = summaryPrompt,
            stream = true
        };

        Console.WriteLine("\nSummarizing conversation history...");
        var summaryResponse = await GetAnswer(summaryRequest);

        _messages = new List<OllamaMessage>
        {
            new OllamaMessage
            {
                Role = "system",
                Content = $"Conversation summary: {summaryResponse.Content}"
            }
        };
    }

    private static string? GetUserInput()
    {
        Console.Write(">>: ");
        var userInput = Console.ReadLine();
        return userInput;
    }

    private global::System.Object GenerateUserQuestionRequest(string? input)
    {
        _messages.Add(new OllamaMessage
        {
            Role = "user",
            Content = input ?? string.Empty
        });
        return new
        {
            model = _modelName,
            messages = _messages,
            stream = true
        };
    }

    private async Task<OllamaMessage> GetAnswer(global::System.Object request, bool isShow = true)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var requestSend = new HttpRequestMessage(HttpMethod.Post, _modelUrl);
        requestSend.Content = content;

        var response = await _httpClient.SendAsync(requestSend, HttpCompletionOption.ResponseHeadersRead);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var responseMessage = new OllamaMessage();

        Console.Write("<<: ");

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk = JsonConvert.DeserializeObject<OllamaResponse>(line);

            if (!string.IsNullOrEmpty(chunk?.Message?.Content))
            {
                responseMessage.Content += chunk.Message.Content;
                Console.ForegroundColor = ConsoleColor.White;
                Write(chunk.Message.Content);
                Console.ResetColor();
            }
            else if (!string.IsNullOrEmpty(chunk?.Message?.Thinking))
            {
                responseMessage.Thinking += chunk.Message.Thinking;
                Console.ForegroundColor = ConsoleColor.Gray;
                Write(chunk.Message.Thinking);
                Console.ResetColor();
            }

            if (chunk?.Done == true)
            {
                responseMessage.Role = chunk.Message.Role;
                _promptEvalCount = chunk.PromptEvalCount;
                _evalCount = chunk.EvalCount;
                break;
            }
        }
        Console.WriteLine();
        return responseMessage;
    }

    private void Write(string content)
    {
        Console.Write(content);
    }

    public bool IsNearContextLimit(int contextLength = _contextLength, double threshold = 0.3)
    {
        var estimatedTokens = _promptEvalCount + _evalCount;

        var usage = (double)estimatedTokens / contextLength * 100;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"Estimated tokens: {estimatedTokens} ↑{_promptEvalCount} ↓{_evalCount}, Usage {usage:F2}%, Context length: {contextLength}, Threshold: {threshold * 100}%");
        Console.ResetColor();

        return estimatedTokens >= contextLength * threshold;
    }
}

public class OllamaResponse
{
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("message")]
    public OllamaMessage Message { get; set; }

    [JsonProperty("done")]
    public bool Done { get; set; }

    [JsonProperty("done_reason")]
    public string DoneReason { get; set; }

    [JsonProperty("total_duration")]
    public long TotalDuration { get; set; }

    [JsonProperty("load_duration")]
    public long LoadDuration { get; set; }

    [JsonProperty("prompt_eval_count")]
    public int PromptEvalCount { get; set; }

    [JsonProperty("prompt_eval_duration")]
    public long PromptEvalDuration { get; set; }

    [JsonProperty("eval_count")]
    public int EvalCount { get; set; }

    [JsonProperty("eval_duration")]
    public long EvalDuration { get; set; }

    [JsonProperty("logprobs")]
    public List<OllamaLogprob> Logprobs { get; set; }
}

public class OllamaMessage
{
    [JsonProperty("role")]
    public string Role { get; set; } = string.Empty;

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;

    [JsonProperty("thinking")]
    public string Thinking { get; set; } = string.Empty;

    [JsonProperty("tool_calls")]
    public List<OllamaToolCall> ToolCalls { get; set; }

    [JsonProperty("images")]
    public List<string> Images { get; set; }
}

public class OllamaToolCall
{
    [JsonProperty("function")]
    public OllamaFunction Function { get; set; }
}

public class OllamaFunction
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("arguments")]
    public object Arguments { get; set; }
}

public class OllamaLogprob
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("logprob")]
    public double Logprob { get; set; }

    [JsonProperty("bytes")]
    public List<int> Bytes { get; set; }

    [JsonProperty("top_logprobs")]
    public List<OllamaTopLogprob> TopLogprobs { get; set; }
}

public class OllamaTopLogprob
{
    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("logprob")]
    public double Logprob { get; set; }

    [JsonProperty("bytes")]
    public List<int> Bytes { get; set; }
}
