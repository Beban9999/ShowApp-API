using System.Net.WebSockets;
using System.Text;
using AppApi.Handlers;
using AppApi.Helper;
using AppApi.Repository;
using AppApi.Repository.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Set JWT configuration
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
var jwtAudience = builder.Configuration.GetSection("Jwt:Audience").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtAudience,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<DbHelper, DbHelper>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppApi.Models.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.Map("/ws", builder =>
{
    builder.UseMiddleware<WebSocketMiddleware>();
});
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path == "/ws")
//    {
//        if (context.WebSockets.IsWebSocketRequest)
//        {
//            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//            await webSocket.SendAsync(Encoding.UTF8.GetBytes("test"), WebSocketMessageType.Text, true, CancellationToken.None);
//        }
//        else
//        {
//            context.Response.StatusCode = StatusCodes.Status400BadRequest;
//        }
//    }
//    else
//    {
//        await next(context);
//    }

//});


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("MyCorsPolicy");

app.Run();
