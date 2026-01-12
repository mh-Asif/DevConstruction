using ConstructionApp.Core.Repository;
using ConstructionApp.Services.Configurations;
using ConstructionApp.Services.DBContext;
using ConstructionApp.Services.Repository;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ConstDbContext>(options => options.UseSqlServer(
  builder.Configuration.GetSection("ConnectionStrings:ConnectionDB").Value,
  sqlServerOptions => sqlServerOptions.CommandTimeout(180))
);

builder.Services.AddScoped<ICountryMasterRepository, CountryMasterRepository>();
builder.Services.AddScoped<ICityMasterRepository, CityMasterRepository>();
builder.Services.AddScoped<IStateMasterRepository, StateMasterRepository>();
builder.Services.AddScoped<IPriorityMasterRepository, PriorityMasterRepository>();
builder.Services.AddScoped<IStatusMasterRepository, StatusMasterRepository>();
builder.Services.AddScoped<IUsersMasterRepository, UsersMasterRepository>();
builder.Services.AddScoped<DapperDBContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddResponseCaching();

builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("ExposeResponseHeaders",
//        builder =>
//        {
//            builder.WithOrigins("https://localhost:7128",
//                                "http://localhost:7013",
//                                 "https://localhost:7013",
//                                 "https://localhost:7128",
//                                 "http://208.110.72.220:91",
//                                 "https://api.mimicogroupinc.ca:85"

//                             )
//                   .WithExposedHeaders("X-Pagination")
//                   .AllowAnyHeader()
//                   .AllowCredentials()
//                   .WithMethods("POST", "PUT", "PATCH", "GET", "DELETE")
//                   .SetIsOriginAllowed(host => true);
//        });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI", policy =>
        policy.WithOrigins("https://localhost:7128", "https://cms.hrclicks.com",
        "http://154.61.69.36:81", "https://app.mimicogroupinc.ca") // your WebUI port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});
// and in app.UseCors("AllowWebUI");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowWebUI");
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod());
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseResponseCaching();
app.UseResponseCompression();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    //c.DefaultModelsExpandDepth(-1);
    c.SwaggerEndpoint($"v1/swagger.json", "Construction Master APIs");
    //c.RoutePrefix = "swagger";
});
app.MapControllers();

app.Run();
