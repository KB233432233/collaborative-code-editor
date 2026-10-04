using CollaborativeCodeEditor.Application;
using CollaborativeCodeEditor.Infrastructure;
using CollaborativeCodeEditor.Api.Authentication;
using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Api.Endpoints.Workspaces;
using CollaborativeCodeEditor.Api.Common.Errors;
using CollaborativeCodeEditor.Api.Endpoints.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

// builder.Services.AddSwaggerGen();

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

app.MapRegisterNewUser();

// app.UseSwagger();
// app.UseSwaggerUI();

app.Run();