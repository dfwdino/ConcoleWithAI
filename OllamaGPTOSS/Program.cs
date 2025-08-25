using Microsoft.Extensions.AI;
using OllamaGPTOSS.src.domain;
using OllamaGPTOSS.src.domain.entities;
using OllamaGPTOSS.src.infrastructure.services;
using OllamaSharp;

//Pull form DB
ChatAIModel ChatAIModel = new ChatAIModel("http://localhost:11434/", "llama3.2-vision");


// Initialize OllamaApiClient targeting the "gpt-oss:20b" model
IChatClient chatClient = new OllamaApiClient(new Uri(ChatAIModel.BrandUri), ChatAIModel.ModelName);

PromptService promptService = new PromptService();

//Need to add this to the Chat Model selection. For now, hardcoding to Educational and Assistant.
PromptType SelectedType = PromptType.Educational;
ChatRole SelectedRole = ChatRole.Assistant;

Console.WriteLine(PromptTypeExtensions.ToStringValue(PromptType.Educational));
Console.WriteLine(PromptTypeExtensions.GetType("kids").ToString());
Console.WriteLine(SelectedType);

// Maintain conversation history
List<ChatMessage> chatHistory = new()
{
    new ChatMessage(ChatRole.System, promptService.GetPrompt(SelectedType))
};


if (SelectedType.Equals(PromptType.Educational))
{
    Console.WriteLine("Learning Assistant - Type 'exit' to quit");
    Console.WriteLine("I'm here to help you learn! I'll guide you to find the answers.");
    Console.WriteLine();
}
else if (SelectedType.Equals(PromptType.CSharp))
{
    Console.WriteLine("C# Coding Assistant - Type 'exit' to quit");
    Console.WriteLine("I'm here to help you with C# coding questions and tasks.");
    Console.WriteLine();
}
else
{
    Console.WriteLine("Direct Assistant - Type 'exit' to quit");
    Console.WriteLine("I'm here to assist you directly with your questions and tasks.");
    Console.WriteLine();
}

while (true)
{
    Console.Write("You: \n");
    var userInput = Console.ReadLine();
    Console.WriteLine();


    string result = CheckInput(userInput);

    if (result == "break")
    {
        Console.WriteLine("Exiting chat. Goodbye!");
        chatClient.Dispose();
        break;
    }
    else if (result == "continue")
    {
        continue;
    }

    // Add user message to chat history
    chatHistory.Add(new ChatMessage(SelectedRole, userInput));

    var assistantResponse = "";
    using var cts = new CancellationTokenSource();
    var loadingTask = ShowLoadingAnimation(cts.Token);

    try
    {
        await foreach (var update in chatClient.GetStreamingResponseAsync(chatHistory))
        {
            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
                Console.Write("\r" + new string(' ', 20) + "\r"); // Clear loading animation
                Console.Write("Assistant: ");
            }

            Console.Write(update.Text);
            assistantResponse += update.Text;
        }
    }
    finally
    {
        cts.Cancel();
        try
        {
            await loadingTask;
        }
        catch (OperationCanceledException)
        {
            // Expected cancellation, ignore
        }
    }

    // Append assistant message to chat history
    //Need to add only the assistantResponse text and IsReply to DB.
    chatHistory.Add(new ChatMessage(ChatRole.User, assistantResponse));
    Console.WriteLine();
    Console.WriteLine();
}

string CheckInput(string userInput)
{
    string ProcessCommand = string.Empty;

    ProcessCommand = CommandChecking(userInput);

    return ProcessCommand;
}

string CommandChecking(string userInput)
{
    string CommandType = string.Empty;

    if (userInput?.ToLower() == "exit")
    {
        CommandType = "break";
    }

    else if (userInput?.Equals("clear", StringComparison.OrdinalIgnoreCase) == true)
    {
        chatHistory.Clear();
        Console.WriteLine("Chat history cleared.");
        Console.WriteLine();

        CommandType = "continue";
    }

    else if (string.IsNullOrWhiteSpace(userInput)) CommandType = "continue";


    return CommandType;
}


// Add this method to your class
static string FormatThinkingMessage(string frame) =>
    $"\r Assistant Thinking{frame}";

// Then in your ShowLoadingAnimation method:
async Task ShowLoadingAnimation(CancellationToken cancellationToken)
{
    string[] frames = { ".", "..", "..,", "....", ".....", "?", "??", "???" };
    int frameIndex = 0;

    while (!cancellationToken.IsCancellationRequested)
    {
        Console.Write(FormatThinkingMessage(frames[frameIndex]));
        frameIndex = (frameIndex + 1) % frames.Length;
        await Task.Delay(100, cancellationToken);
    }
}















