using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "MyCompany.MyProduct.MyService";
var serviceVersion = "1.0.0";


var greeterMeter = new Meter("otel-test", "1.0.0");
var countGreetings = greeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");

builder.Services.AddDbContext<UserDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(serviceName));

builder.Services.AddOpenTelemetry()
    .WithTracing(trace => 
    {
        trace 
        .AddSource(serviceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
        )
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter();
    })
    .WithMetrics(metric => {
        metric
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter()
        .AddMeter(greeterMeter.Name);
    });

builder.Logging.AddOpenTelemetry(logging =>
{
  logging.IncludeScopes = true;
  logging.AddOtlpExporter();
});


var app = builder.Build();

app.MapGet("/users", async (UserDb db, Tracer tracer, ILogger<Program> logger) =>
{
    using var span = tracer.StartActiveSpan("hello-span");
    span.SetAttribute("customer.project", "Getlink");
    span.AddEvent("Get users");

    var users = await db.Users.ToListAsync();
    
    return Results.Ok(users);

    countGreetings.Add(1);

    logger.LogInformation("Get All Users success !!");
});



app.MapGet("/user/{id}", async (int id, UserDb db, Tracer tracer, ILogger<Program> logger) =>
{
    using var span = tracer.StartActiveSpan("hello-span");
    span.SetAttribute("customer.project", "Getlink");
    span.AddEvent("Get user by id");

    if (await db.Users.FindAsync(id) is User user) {
        return Results.Ok(user);
    }
            
    return Results.NotFound();

    countGreetings.Add(2);
    
    logger.LogInformation("Get User success !!");
});

app.MapPost("/user/create", async (User user, UserDb db, Tracer tracer, ILogger<Program> logger) =>
{
    using var span = tracer.StartActiveSpan("hello-span");
    span.SetAttribute("customer.project", "Getlink");
    span.AddEvent("Creating new user");

    db.Users.Add(user);
    await db.SaveChangesAsync();

    countGreetings.Add(3);

    logger.LogInformation("User create success !!");

    return Results.Created($"/useritems/{user.Id}", user);
});

app.MapPut("/user/update/{id}", async (int id, User inputUser, UserDb db, Tracer tracer, ILogger<Program> logger) =>
{
    using var span = tracer.StartActiveSpan("hello-span");
    span.SetAttribute("customer.project", "Getlink");
    span.AddEvent("Update user info");

    var user = await db.Users.FindAsync(id);

    if (user is null) return Results.NotFound();

    user.FirstName = inputUser.FirstName;
    user.LastName = inputUser.LastName;
    user.Company = inputUser.Company; 
    user.IsComplete = inputUser.IsComplete;

    await db.SaveChangesAsync();

    countGreetings.Add(4);

    logger.LogInformation("User update success !!");

    return Results.NoContent();
});

app.MapDelete("/user/delete/{id}", async (int id, UserDb db, Tracer tracer, ILogger<Program> logger) =>
{
    using var span = tracer.StartActiveSpan("hello-span");
    span.SetAttribute("customer.project", "Getlink");
    span.AddEvent("Delete user");
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    countGreetings.Add(5);

    logger.LogInformation("User delete success !!");

    return Results.NotFound();
});

app.Run();