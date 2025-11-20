using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

namespace MovieApp.Components.Services;

public interface IKernelFactory
{
    Kernel CreateKernel();
}

public class KernelFactory : IKernelFactory
{
    private readonly IConfiguration _configuration;

    public KernelFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();

        var endpoint = _configuration["AzureOpenAI:Endpoint"]; // expecting configured externally
        var apiKey = _configuration["AzureOpenAI:ApiKey"];    // expecting configured externally
        var deployment = _configuration["AzureOpenAI:DeploymentName"] ?? "gpt-35-turbo";

        if (!string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey))
        {
            builder.AddAzureOpenAIChatCompletion(deployment, endpoint, apiKey);
        }

        return builder.Build();
    }
}
