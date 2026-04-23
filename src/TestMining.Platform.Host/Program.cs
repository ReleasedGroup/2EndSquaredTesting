using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TestMining.Platform.Host.Auth;
using TestMining.Platform.Host.Components;
using TestMining.Platform.Host.Navigation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<LocalDevelopmentAuthOptions>()
    .Bind(builder.Configuration.GetSection(LocalDevelopmentAuthOptions.SectionName))
    .ValidateOnStart();
builder.Services.AddSingleton<
    Microsoft.Extensions.Options.IValidateOptions<LocalDevelopmentAuthOptions>,
    LocalDevelopmentAuthOptionsValidator>();
builder.Services.AddSingleton<LocalDevelopmentUserDirectory>();
builder.Services.AddScoped<ShellNavigationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/sign-in";
        options.AccessDeniedPath = "/access-denied";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost(
        "/auth/local-sign-in",
        async Task<IResult> (
            HttpContext httpContext,
            LocalDevelopmentUserDirectory userDirectory,
            [FromForm] string userId,
            [FromForm] string? returnUrl) =>
        {
            var principal = userDirectory.CreatePrincipal(userId);
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Results.Redirect(RedirectTarget.ForReturnUrl(returnUrl));
        })
    .DisableAntiforgery();

app.MapPost(
        "/auth/sign-out",
        async Task<IResult> (HttpContext httpContext, [FromForm] string? returnUrl) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect(RedirectTarget.ForReturnUrl(returnUrl, defaultPath: "/sign-in"));
        })
    .DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program;
