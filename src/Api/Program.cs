using Agents.RegisterServices;
using Api.Application.Interface;
using Api.Application.Service;
using Database;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddLogging();

builder.Services.AddAgents();

builder.Services.AddSingleton<IChatClient>(sp =>
{
    IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
    string url = Environment.GetEnvironmentVariable("LLM_BASE_URL")!;
    string model = Environment.GetEnvironmentVariable("LLM_MODEL")!;
    string cred = Environment.GetEnvironmentVariable("LLM_CRED")!;

    OpenAIClient client = new (
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

builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IWorkflowExecutionService, WorkflowExecutionService>();

builder.Services.AddAgentDB(Environment.GetEnvironmentVariable("POSTGRES")!);

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(
        Environment.GetEnvironmentVariable("POSTGRES")));
});

builder.Services.AddHangfireServer();

WebApplication app = builder.Build();

// exposed intentionally
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        Status = "InfraPilot API Running"
    });
});

// serving ui
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();