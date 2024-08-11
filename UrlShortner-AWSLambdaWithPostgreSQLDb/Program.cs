using Microsoft.EntityFrameworkCore;
using UrlShortner_AWSLambdaWithPostgreSQLDb;
using UrlShortner_AWSLambdaWithPostgreSQLDb.Entities;
using UrlShortner_AWSLambdaWithPostgreSQLDb.Extensions;
using UrlShortner_AWSLambdaWithPostgreSQLDb.Models;
using UrlShortner_AWSLambdaWithPostgreSQLDb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));
builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}
app.MapGet("", () => "Welcome to my APP");
app.MapPost("api/shorten", async (ShortenUrlRequest request,
    UrlShorteningService urlShorteningService,
    ApplicationDbContext dbContext,
    HttpContext httpContext)=>
{
    if(!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("The specified Url is invalid.");
    }
    var code = await urlShorteningService.GenerateUniqueCode();
    var shortenedUrl = new ShortenedUrl
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        CreatedOnUTC= DateTime.UtcNow
    };
    dbContext.ShortenedUrl.Add(shortenedUrl);
    await dbContext.SaveChangesAsync();
    return Results.Ok(shortenedUrl.ShortUrl);
});
app.MapGet("api/{code}", async (string code, ApplicationDbContext dbcontext) =>
{
    var shortenedUrl = await dbcontext.ShortenedUrl
    .FirstOrDefaultAsync(s => s.Code == code);
    if(shortenedUrl == null)
    {
        return Results.NotFound();
    }
    return Results.Redirect(shortenedUrl.LongUrl);
});
app.UseHttpsRedirection();



app.Run();

