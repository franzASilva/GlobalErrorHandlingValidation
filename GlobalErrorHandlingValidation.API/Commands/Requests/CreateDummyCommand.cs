using GlobalErrorHandlingValidation.Domain.Models;
using MediatR;

namespace GlobalErrorHandlingValidation.API.Commands.Requests;

public sealed record CreateDummyCommand(string Name, int Value) : IRequest<DummyModel>;
