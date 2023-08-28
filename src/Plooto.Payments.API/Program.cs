using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Plooto.Payments.API.Filters;
using Plooto.Payments.Application;
using Plooto.Payments.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plooto Payments API", Version = "v1" });

    var filePath = Path.Combine(AppContext.BaseDirectory, "Plooto.Payments.API.xml");
    c.IncludeXmlComments(filePath);

    filePath = Path.Combine(AppContext.BaseDirectory, "Plooto.Payments.Application.xml");
    c.IncludeXmlComments(filePath);
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, config) => 
    config.ReadFrom.Configuration(context.Configuration));

// API Versioning
builder.Services.AddApiVersioning(p =>
{
    p.DefaultApiVersion = new ApiVersion(1, 0);
    p.ReportApiVersions = true;
    p.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(p =>
{
    p.GroupNameFormat = "'v'VVV";
    p.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();
app.UseApiVersioning();
app.UseEndpoints(c => { c.MapControllers(); });

app.Run();
