using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;
using AdvancedMinimalApi.Models;
using AdvancedMinimalApi.Contacts;
using AdvancedMinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add logging using Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation(); // Add FluentValidation for request validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // Automatically register validators

// Register application services
builder.Services.AddSingleton<IProductService, ProductService>();


var app = builder.Build();

// Configure middleware
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//--minimal api controller

app.MapGet("/", () => Results.Ok("Welcome to the Advanced Minimal API"));

app.MapGet("/products", async (IProductService productService) =>
{
    var products = await productService.GetProductsAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (int id, IProductService productService) =>
{
    var product = await productService.GetProductByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/products", async (CreateProductRequest request, IProductService productService, IValidator<CreateProductRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var createdProduct = await productService.CreateProductAsync(request);
    return Results.Created($"/products/{createdProduct.Id}", createdProduct);
});

app.MapPut("/products/{id}", async (int id, UpdateProductRequest request, IProductService productService, IValidator<UpdateProductRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(request);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }

    var updatedProduct = await productService.UpdateProductAsync(id, request);
    return updatedProduct is not null ? Results.Ok(updatedProduct) : Results.NotFound();
});

app.MapDelete("/products/{id}", async (int id, IProductService productService) =>
{
    var deleted = await productService.DeleteProductAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
