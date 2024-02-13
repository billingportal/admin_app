global using BillingPortalClient.ModelViews;
global using BillingPortalClient.Components;
global using BillingPortalClient.Hubs;
global using BillingPortalClient.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder( args );

// Add services to the container.
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

// Register OracleApiServices
builder.Services.AddScoped<OracleApiServices>();

builder.Services.AddLocalization( options => options.ResourcesPath = "Resources" );
builder.Services.AddControllersWithViews().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
  .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>( options =>
 {
   var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar"),
    };

   options.DefaultRequestCulture = new RequestCulture( "en" );
   options.SupportedCultures = supportedCultures;
   options.SupportedUICultures = supportedCultures;
 } );

builder.Services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme ).
  AddCookie( options =>
   {
     options.LoginPath = "/Authenticationz/Login";
     options.ExpireTimeSpan = TimeSpan.FromMinutes( 20 );
   } );
builder.Services.AddSession( options =>
 {
   options.IdleTimeout = TimeSpan.FromMinutes( 10 );
 } );

builder.Services.AddSignalR( e => {
  e.MaximumReceiveMessageSize = 102400000;
} );

builder.Services.AddScoped<InvoiceViewModel, InvoiceViewModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if( !app.Environment.IsDevelopment() )
{
  app.UseExceptionHandler( "/Home/Error" );
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization( app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value );
app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}" );

app.UseEndpoints( endpoints =>
 {
   endpoints.MapHub<ChatHub>( "/chatHub" );
 } );

app.MapHub<ChatHub>("/chatHub");

app.MapBlazorHub();
app.Run();
