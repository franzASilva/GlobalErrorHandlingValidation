using GlobalErrorHandlingValidation.API.Commands.Requests;
using GlobalErrorHandlingValidation.Domain.Models;
using MediatR;

namespace GlobalErrorHandlingValidation.API.Commands.Handlers;

internal class CreateDummyCommandHandler : IRequestHandler<CreateDummyCommand, DummyModel>
{
    public async Task<DummyModel> Handle(CreateDummyCommand request, CancellationToken ct)
    {
        var task = Task.Run(() =>
        {
            return new DummyModel(request.Name, request.Value);
        });

        return await task.WaitAsync(ct);
    }
}
