using HackerNews.Api.Services;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<HackerNewsService>();
builder.Services.AddControllers();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", // Replace with your policy name
        policy => {
            policy.WithOrigins("http://localhost:4200", "*") // Replace with allowed origins
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("MyCorsPolicy"); // Use the policy name you defined

app.Run();
