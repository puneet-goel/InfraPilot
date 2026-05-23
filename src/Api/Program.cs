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

    string url = configuration["AI:baseUrl"]!;
    string model = configuration["AI:model"]!;
    string cred = configuration["AI:cred"]!;

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

builder.Services.AddAgentDB(builder.Configuration.GetConnectionString("Postgres")!);

builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(
        builder.Configuration.GetConnectionString("Postgres")));
});

builder.Services.AddHangfireServer();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();