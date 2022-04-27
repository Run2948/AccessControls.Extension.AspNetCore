using AccessControlDemo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
builder.Services.AddSingleton<PermissionContext>();
builder.Services.AddAccessControl<ResourceAccessStrategy, ControlAccessStrategy>(options =>
{
    options.UseAsDefaultPolicy = true;
    //options.AccessKeyResolver = context => context.RequestServices.GetRequiredService<AccessKeyResolver>().GetAccessKey(context.Request.Path);
    options.AccessKeyResolver = context => $"{(context.GetRouteData().DataTokens["area"] == null ? "" : $"{context.GetRouteData().DataTokens["area"]}:")}{context.GetRouteValue("controller")}:{context.GetRouteValue("action")}";
});
//builder.Services.AddSingleton<AccessKeyResolver>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
//app.UseAccessControl();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
