using EmailServiceLibrary;
using EmailServiceLibrary.Model;
using EmailServiceLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApiRoleBasedAuthorization.Data;


var builder = WebApplication.CreateBuilder(args);

// Add the EmailSender service
var emailconfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailconfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Add services to the container.vis
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});
builder.Services.AddControllersWithViews(); ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'SQLServerIdentityConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(connectionString));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 5;    
}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
options.TokenLifespan = TimeSpan.FromHours(2)
);


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
    builder =>
    {
        builder.WithOrigins("https://localhost:7254")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();


    });
});


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(Options =>
{
    Options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    Options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*app.UseCors("MyPolicy");*/
app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();