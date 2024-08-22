using FluentValidation;
using GlobalErrorHandlingValidation.API.Commands.Behaviors;
using GlobalErrorHandlingValidation.API.Commands.Requests;
using GlobalErrorHandlingValidation.API.Exceptions;
using GlobalErrorHandlingValidation.API.Responses;
using GlobalErrorHandlingValidation.Domain.Models;
using MediatR;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Global Error Handling Validation API",
        Description = "An ASP.NET Core Web API for exemplify inputs validation with FluentValidation, MediatR and Global Error Handling",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.DefaultModelsExpandDepth(-1); // removes schemas
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapPost("/dummy", async (CreateDummyCommand request, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(request, ct);
    
    if (result is not null)
    {
        return Results.Ok(result);
    }

    return Results.BadRequest("dummy wasn't saved");
})
.WithName("CreateDummy")
.WithTags("Dummy API")
.WithOpenApi(operation => new(operation)
{
    Summary = "This is a summary",
    Description = "This is a description"
})
.Produces<DummyModel>(StatusCodes.Status200OK)
.Produces<ApiBadRequestResponse>(StatusCodes.Status400BadRequest);

app.MapHealthChecks("/healthcheck");
app.Run();
