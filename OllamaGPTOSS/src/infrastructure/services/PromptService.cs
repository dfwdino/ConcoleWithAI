using OllamaGPTOSS.src.domain;


namespace OllamaGPTOSS.src.infrastructure.services;

public class PromptService
{
    public string GetPrompt(PromptType type) => type switch
    {
        PromptType.Educational => PromptTemplates.Educational,
        PromptType.CSharp => PromptTemplates.CSharp,
        PromptType.Direct => PromptTemplates.Direct,
        _ => PromptTemplates.Educational
    };
}