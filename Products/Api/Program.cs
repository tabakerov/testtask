
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

var categoryServiceHost = Environment.GetEnvironmentVariable("CATEGORY_SERVICE_HOST")!;
var categryServicePort = int.Parse(Environment.GetEnvironmentVariable("CATEGORY_SERVICE_PORT")!);

builder.Services.AddSingleton(new Api.ServiceConfiguration(categoryServiceHost, categryServicePort));
builder.Services.AddSingleton<Api.IService, Api.Service>();
builder.Services.AddControllers();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "Swagger", Version = "v1" });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add resilience handler to HttpClient, so we handle products<->categories communication errors
builder.Services.AddHttpClient<Api.Service>().AddStandardResilienceHandler();
builder.Services.AddResiliencePipeline("default", x =>
{
    x.AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            Delay = TimeSpan.FromSeconds(2),
            MaxRetryAttempts = 2,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        })
        .AddTimeout(TimeSpan.FromSeconds(30));
});

var app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment())
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.MapControllers();



app.Run();