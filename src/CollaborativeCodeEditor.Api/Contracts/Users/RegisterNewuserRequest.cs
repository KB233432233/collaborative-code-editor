namespace CollaborativeCodeEditor.Api.Contracts.Users;

public sealed record RegisterNewUserRequest(
    string DisplayName,
    string Email);