using XMLAutoTagger.Application;
using XMLAutoTagger.Infrastructure;
using XMLAutoTagger.Infrastructure.Persistence;

// Enable flexible DateTime handling for PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName ?? type.Name);

    var securityScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT token directly. Swagger will automatically attach 'Bearer '."
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(_ => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5079/";
builder.Services.AddHttpClient<XMLAutoTagger.WebAPI.Services.ApiClients.IUserManagementApiClient, XMLAutoTagger.WebAPI.Services.ApiClients.UserManagementApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Initialize database schema and seed data cleanly on startup
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<XMLAutoTagger.Application.Common.Interfaces.IApplicationDbInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LegalXML API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapGet("/logout", (HttpContext context) =>
{
    context.Response.Cookies.Delete("jwt_token");
    context.Response.Cookies.Delete("user_name");
    context.Response.Cookies.Delete("user_role");
    context.Response.Cookies.Delete("user_initials");
    return Results.Redirect("/login");
});

app.Run();
