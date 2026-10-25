using CollaborativeCodeEditor.Application;
using CollaborativeCodeEditor.Infrastructure;
using CollaborativeCodeEditor.Api.Authentication;
using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Api.Endpoints.Workspaces;
using CollaborativeCodeEditor.Api.Common.Errors;
using CollaborativeCodeEditor.Api.Endpoints.Users;
using CollaborativeCodeEditor.Api.Endpoints.Projects;
using CollaborativeCodeEditor.Api.Endpoints.Documents;


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

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

app.MapCreateWorkspace();

app.MapRegisterNewUser();

app.MapLoginEndpoint();

app.MapCreateProject();

app.MapCreateDocument();

app.MapGetDocument();

app.UseAuthentication();
app.UseAuthorization();

// app.UseSwagger();
// app.UseSwaggerUI();

app.Run();