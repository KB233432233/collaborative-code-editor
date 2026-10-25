namespace CollaborativeCodeEditor.Api.Contracts.Users;

public sealed record LoginRequest(
    string Email,
    string Password);