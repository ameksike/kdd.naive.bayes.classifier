using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Classifier.Models.DAO;

namespace Classifier.Services.Startup
{
    public static class StartupDataModel
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration Configuration)
        {
            // load ConnectionString from configuration file: Classifier\appsettings.json
            var DefaultConnection = Configuration.GetConnectionString("DefaultConnection");
            var ConnectionString = Configuration.GetConnectionString(DefaultConnection);

            switch (DefaultConnection)
            {
                case "SQLite":
                    ConnectionString = ConnectionString.Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory") as string);
                    services.AddDbContext<AppDbContext>(options => options.UseSqlite(ConnectionString));
                    break;

                default:
                    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionString));
                    break;
            }
        }
    }
}
