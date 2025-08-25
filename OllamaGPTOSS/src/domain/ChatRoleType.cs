
using OllamaSharp.Models.Chat;

namespace OllamaGPTOSS.src.domain;

public enum ChatRoleTypes
{
    User,
    Assistant,
    System
}


public static class ChatRoleExtensions
{

    public static string ToStringValue(this ChatRole chatrole)
    {
        return chatrole switch
        {
            ChatRole.Assistant => "kids",
            ChatRole.System => "c#",
            ChatRole.Assistant => "lazy",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static PromptType GetType(this string type)
    {
        return type switch
        {
            "kids" => PromptType.Educational,
            "c#" => PromptType.CSharp,
            "lazy" => PromptType.Direct,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

}