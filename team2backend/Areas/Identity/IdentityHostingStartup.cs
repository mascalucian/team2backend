using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using team2backend.Areas.Identity.Data;

[assembly: HostingStartup(typeof(team2backend.Areas.Identity.IdentityHostingStartup))]
namespace team2backend.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<team2backendContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("team2backendContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<team2backendContext>();
            });
        }
    }
}