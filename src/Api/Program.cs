using Agents.RegisterServices;
using Api.Application.Interface;
using Api.Application.Service;
using Database;
using Database.Infrastructure.Persistence;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
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

    return client.GetChatClient(model).AsIChatClient();
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

// migrations command
// dotnet ef migrations add InitialCreate --project .\Database\ --startup-project .\Api\ -o .\Infrastructure\Migrations
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        AppDbContext db = services.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Migration complete");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Migration failed");
    }
}

// Rewrite /api/workflow -> /workflow
var rewriteOptions = new RewriteOptions()
    .AddRewrite(
        @"^api/(.*)",
        "$1",
        skipRemainingRules: true);

app.UseRewriter(rewriteOptions);

// exposed intentionally
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();

// serving ui
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();