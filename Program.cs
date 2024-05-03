using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using trb_prefs;

var builder = WebApplication.CreateBuilder(args);

// Firebase
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.GetApplicationDefault(),
    ProjectId = "trb-officer-android",
});

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "trb-prefs",
        Version = "v1",
        Description = "Backend API"
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// DB
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));

// -----------------------------------------------------------------------------------------------------

var app = builder.Build();

// Auto Migration
using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
context?.Database.Migrate();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();