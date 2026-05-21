using Agents.Infrastructure;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<InfrastructureAgent>();

builder.Services.AddSingleton<IChatClient>(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();

    string url = configuration["AI:baseUrl"]!;
    string model = configuration["AI:model"]!;
    string cred = configuration["AI:cred"]!;

    OpenAIClient client = new OpenAIClient(
        new ApiKeyCredential(cred),
        new OpenAIClientOptions
        {
            Endpoint = new Uri(url)
        });

    return client.GetChatClient(model).AsIChatClient()
        .AsBuilder()
        .UseFunctionInvocation()
        .Build();
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();