using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineShoppingPlatform.Business.DataProtection;
using OnlineShoppingPlatform.Business.Operations.Order;
using OnlineShoppingPlatform.Business.Operations.Product;
using OnlineShoppingPlatform.Business.Operations.User;
using OnlineShoppingPlatform.DataAccess;
using OnlineShoppingPlatform.DataAccess.Entities;
using OnlineShoppingPlatform.DataAccess.Entities.Services;
using OnlineShoppingPlatform.DataAccess.Logging;
using OnlineShoppingPlatform.DataAccess.Maintenance;
using OnlineShoppingPlatform.DataAccess.Repositories;
using OnlineShoppingPlatform.DataAccess.UnitOfWork;
using OnlineShoppingPlatform.Presentation.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add(new TimeRestrictedAccessFilter("09:00", "17:00")); // Tüm API'ler için geçerli
//});

// JWT Configuration
//var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//var secretKey = jwtSettings.GetValue<string>("SecretKey");

//if (secretKey == null)
//{
//    // Handle the null value (e.g., throw an exception or set a default value)
//    throw new ArgumentNullException("SecretKey", "Secret key cannot be null.");
//}

//var key = Encoding.UTF8.GetBytes(secretKey);
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
//            ValidAudience = jwtSettings.GetValue<string>("Audience"),
//            IssuerSigningKey = new SymmetricSecurityKey(key)
//        };
//    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
//});

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(builder =>
//    {
//        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
//    });
//});
//jwt son

// Register Services for Dependency Injection
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IOrderService, OrderManager>();



// Configure logging middleware
builder.Services.AddLogging();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Name = "Jwt Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT Bearer Token on Textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }


    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// Add Data Protection Service
builder.Services.AddScoped<IDataProtection, DataProtection>();

var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath,"App_Data","Keys"));

builder.Services.AddDataProtection()
    .SetApplicationName("OnlineShoppingPlatform")
    .PersistKeysToFileSystem(keysDirectory);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options =>
         {
             options.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuer = true, // Appsettingsteki Issuer alaný ile eþleþiyor mu ? kontrol et
                 ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

                 ValidateAudience = true,
                 ValidAudience = builder.Configuration["JwtSettings:Audience"],

                 ValidateLifetime = true, // süresi dolan tokeni kabul etme

                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)

             )};
         });


// DbContext için baðlantý dizesi
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Identity ile kullanýcýlarý yapýlandýrýyoruz
//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();


// UnitOfWork ve Repository için DI yapýlandýrmasý
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


var app = builder.Build();

app.UseRouting();

// Enable CORS
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseMiddleware<LoggingMiddleware>(); // Ýstekler önce loglanýr
//app.UseMiddleware<MaintenanceMiddleware>(); // Bakým modunu kontrol eden middleware

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



//var scope = app.Services.CreateScope();
//var services = scope.ServiceProvider;
//var userManager = services.GetRequiredService<UserManager<User>>();
//var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//await SeedDatabase(userManager, roleManager); // Veritabanýný seed etme

//async Task SeedDatabase(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
//{
//    var role = await roleManager.FindByNameAsync("Admin");
//    if (role == null)
//    {
//        await roleManager.CreateAsync(new IdentityRole("Admin"));
//    }

//    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
//    if (adminUser == null)
//    {
//        adminUser = new User
//        {
//            UserName = "adminko",
//            Email = "admin@example.com"
//        };
//        await userManager.CreateAsync(adminUser, "AdminPassword123!");
//    }

//    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
//    {
//        await userManager.AddToRoleAsync(adminUser, "Admin");
//    }
//}

app.Run();
