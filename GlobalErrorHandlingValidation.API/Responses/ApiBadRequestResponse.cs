namespace GlobalErrorHandlingValidation.API.Responses;

public sealed record ApiBadRequestResponse (int Status, string Title, ErrorDetail Detail);

public sealed record ErrorDetail (PathString Instance, string Errors);
