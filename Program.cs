using CapstoneProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CapstoneProject.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using System.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using SmartBreadcrumbs.Extensions;

var builder = WebApplication.CreateBuilder(args);
IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
IConfigurationSection facebookAuthNSection = builder.Configuration.GetSection("Authentication:Facebook");

builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly(), options => {
    options.TagName = "nav";
    options.TagClasses = "";
    options.OlClasses = "breadcrumb";
    options.LiClasses = "breadcrumb-item";
    options.ActiveLiClasses = "breadcrumb-item active";
});

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions => {
        googleOptions.ClientId = googleAuthNSection["ClientId"];
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
        googleOptions.CallbackPath = "/signin-google";
        googleOptions.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
        googleOptions.SaveTokens = true;
        googleOptions.Events.OnCreatingTicket = ctx => {
            List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();
            tokens.Add(new AuthenticationToken() {
                Name = "TicketCreated",
                Value = DateTime.UtcNow.ToString()
            });
            ctx.Properties.StoreTokens(tokens);
            return Task.CompletedTask;
        };

    })
    .AddFacebook(facebookOptions => {
        facebookOptions.AppId = facebookAuthNSection["AppId"];
        facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
        facebookOptions.Scope.Remove("email");
        facebookOptions.CallbackPath = "/signin-facebook";
    });


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<RecipeOrganizerContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RecipeOrganizerContext>();

builder.Services.Configure<IdentityOptions>(options => {
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCookiePolicy(new CookiePolicyOptions() {
    MinimumSameSitePolicy = SameSiteMode.Lax
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();