﻿using System.Text;
using HealthCareABApi.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HealthCareABApi.Repositories.Implementations;
using HealthCareABApi.Repositories.Interfaces;
using HealthCareABApi.Services.Implementations;
using HealthCareABApi.Services.Interfaces;
using Email.Net;
using Email.Net.Channel.Smtp;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
if (builder.Environment.IsDevelopment())
{
    // get from secrets.json
    builder.Configuration.AddUserSecrets<Program>();
    builder.Services.Configure<MongoDBSettings>(
        builder.Configuration.GetSection("MONGODB")
    );
}
else
{
    // get from appsettings
    builder.Services.Configure<MongoDBSettings>(
        builder.Configuration.GetSection("MongoDBSettings")
    );
}

// register Email.Net service with configuration for Dependency Injection to EmailService
builder.Services.AddEmailNet(options =>
{
    options.PauseSending = builder.Environment.IsDevelopment(); // pause sending in dev mode
    options.DefaultFrom = new System.Net.Mail.MailAddress(
        address: "test@example.com",
        displayName: "Test Sender");
    options.DefaultEmailDeliveryChannel = SmtpEmailDeliveryChannel.Name;
});

// Register MongoDB context
builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();

// Register repositories
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

// Register custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
// this vv might cause problems since we have an empty constructor present in EmailService
// TODO - verify that the DI is working correctly here
builder.Services.AddScoped<HealthCareABApi.Services.Implementations.EmailService>();

// Add shared service
builder.Services.AddScoped<IAppointmentAvailabilityService, AppointmentAvailabilityService>();

// Add other services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger to use JWT authentication
    // Tells Swagger to send the JWT token with API requests
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT settings
// Retrieve the "JwtSettings" section from the app's configuration (e.g., appsettings.json).
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Add authentication services to the application, specifying JWT Bearer as the default authentication scheme.
builder.Services.AddAuthentication(options =>
{
    // This means that by default, the application will use JWT tokens for authentication.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // This is used when an unauthenticated request is made, prompting the app to challenge the user for JWT authentication.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Configure JWT Bearer authentication with specific options.
.AddJwtBearer(options =>
{
    // Read token from cookie
    // Add an event to handle when a JWT token is received.
    options.Events = new JwtBearerEvents
    {
        // This event is triggered when a request with a JWT token is received.
        // It allows custom logic to determine where the token is read from.
        OnMessageReceived = context =>
        {
            // Check if the request contains a "jwt" cookie.
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                // If the "jwt" cookie exists, set the token in the context from the cookie.
                // This allows the application to authenticate requests based on the token stored in cookies.
                context.Token = context.Request.Cookies["jwt"];
            }
            // Complete the task with no further action.
            return Task.CompletedTask;
        }
    };
    // Configure token validation parameters for JWT Bearer authentication.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // This ensures that the token was issued by a trusted source.
        ValidateIssuer = true,
        // This should match the "Issuer" value from the "JwtSettings" configuration.
        ValidIssuer = jwtSettings["Issuer"],
        // This ensures that the token is intended for the correct audience.
        ValidateAudience = true,
        // This should match the "Audience" value from the "JwtSettings" configuration.
        ValidAudience = jwtSettings["Audience"],
        // This ensures that the token was signed by a trusted source and has not been tampered with.
        ValidateIssuerSigningKey = true,
#pragma warning disable CS8604 // Possible null reference argument.
        // The key is created using the "Secret" value from the "JwtSettings" configuration, encoded in UTF-8.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])),
#pragma warning restore CS8604 // Possible null reference argument.
        // This ensures that expired tokens will be rejected.
        ValidateLifetime = true,
        // By default, a small amount of clock skew is allowed to account for minor time differences between systems.
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS (Cross-Origin Resource Sharing) services to the application.
builder.Services.AddCors(options =>
{
    // Define a CORS policy named "AllowReactApp".
    options.AddPolicy("AllowReactApp", policy =>
    {

        // Specify the allowed origins for this policy.
        // Only requests coming from "http://localhost:5173" will be allowed.
        // You can add more origins here if needed.
        policy.WithOrigins("http://localhost:5173")
              // Allow any HTTP method (e.g., GET, POST, PUT, DELETE) for cross-origin requests.
              .AllowAnyMethod()
              // Allow any HTTP header in the requests (e.g., Content-Type, Authorization).
              .AllowAnyHeader()
              // Required for cross-origin cookies
              // This is necessary when you want to send cookies, like JWT tokens in cookies, across origins.
              .AllowCredentials();
    });
});


var app = builder.Build();

// Enable the CORS policy for the app.
// By calling `UseCors("AllowReactApp")`, the app will use the "AllowReactApp" policy defined above.
// This allows the specified origins, methods, headers, and credentials in cross-origin requests.
// I kept the name "AllorReactApp" for simplicity since Iset this API up to be used with React as well...
app.UseCors("AllowReactApp");

// Apply HTTPS redirection only in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Important that UseAuthentication is above UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();