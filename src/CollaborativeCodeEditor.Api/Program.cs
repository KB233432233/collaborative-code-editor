using CollaborativeCodeEditor.Application;
using CollaborativeCodeEditor.Infrastructure;
using CollaborativeCodeEditor.Api.Authentication;
using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Api.Endpoints.Workspaces;
using CollaborativeCodeEditor.Api.Common.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

builder.Services
    .AddApplication();

builder.Services
    .AddInfrastructure(
        builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

app.MapCreateWorkspace();

app.Run();