using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Student_Info_Api.Data;
using Students_Info_Api.AuthData.Configuration;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AutoMapper;
using Students_Info_Api.Middleware;
using Students_Info_Api.Repository.CourseRepo;
using Students_Info_Api.Repository.Interfaces;
using Students_Info_Api.Repository.StudentRepo;
using Students_Info_Api.Repository.UserRepoFolder;
using Students_Info_Api.Service.CourseServiceF;
using Students_Info_Api.Service.EmailServiceFolder;
using Students_Info_Api.Service.ImageServiceFolder;
using Students_Info_Api.Service.Interfaces;
using Students_Info_Api.Service.JwtTokenServiceFolderr;
using Students_Info_Api.Service.StudentService;
using Students_Info_Api.Service.UserService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSerilog(logConfig);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { 
    
        Description = "Enter Jwt Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
    { 
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }

    } );
    
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<EmailSMTP>(builder.Configuration.GetSection("EmailSMTP"));

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("StudentConnection")));

builder.Services.AddScoped<IStudentPoints,RepoStudentPoints>();

builder.Services.AddScoped<ICoursePoints,RepoCoursePoints>();

builder.Services.AddScoped<IStudentService,StudentService>();

builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IPasswordHasher<User_DM>, PasswordHasher<User_DM>>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {

        options.TokenValidationParameters = new TokenValidationParameters { 
        
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))

        

        };

    });

builder.Services.AddAutoMapper(cfg => {

    cfg.AddProfile<MappingProfile>();

});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();


