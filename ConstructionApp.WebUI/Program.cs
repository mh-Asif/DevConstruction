using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// Add this in the service registration section
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddTransient<EmailService>();
//builder.Services.AddTransient<ClientMail>(); // Register your ClientMail class if you want to inject it elsewhere

builder.Services.AddResponseCompression(options =>
{
    //options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/javascript" });
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();

    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
   {
        "image/svg+xml", "application/javascript", "application/json"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 209715200; // 100 MB
});
// Register IHttpClientFactory
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Home/ErrorMessage";
            options.LoginPath = "/Home/Index";
        });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 30 days
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public, max-age=2592000"); // 30 days
    }
});
app.Use((context, next) =>
{  


    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 209715200;
    return next();
});

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

//app.Use(async (context, next) =>
//{
//    var userId = context.Session.GetString("UserId");
//    if (context.Session.GetString("UserId") == null &&
//        !context.Request.Path.StartsWithSegments("/Home/Index"))
//    {
//        context.Response.Redirect("/Home/Index");
//        return;
//    }

//    await next.Invoke();
//});

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();
//app.UseResponseCaching();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
