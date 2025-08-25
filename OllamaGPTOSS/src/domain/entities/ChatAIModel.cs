
namespace OllamaGPTOSS.src.domain.entities;

public class ChatAIModel{
    public string BrandUri {get; private set;}
    public string ModelName {get; private set;}
          
    public ChatAIModel(string brandUri, string modelName){
        BrandUri=brandUri;
        ModelName=modelName;
    }

}