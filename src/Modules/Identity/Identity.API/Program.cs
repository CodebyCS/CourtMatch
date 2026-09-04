using Identity.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Config. connecion string.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Config. DbContext.
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            // Services Identity.
            builder.Services.AddDataProtection();
            builder.Services.AddSingleton(TimeProvider.System);

            // Config. Identity for to use with Controllers
            builder.Services.AddIdentity<Identity.API.Data.ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<Identity.API.Services.TokenService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("DefaultCors");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Bootstrap: garante que a role Admin existe e promove um admin inicial
            // (ambiente de teste local — sem isso ninguém conseguiria acessar a Gestão).
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                var anyAdmin = await userManager.GetUsersInRoleAsync("Admin");
                if (anyAdmin.Count == 0)
                {
                    var bootstrapAdmin = await userManager.FindByEmailAsync("joao@courtmatch.com");
                    if (bootstrapAdmin != null)
                        await userManager.AddToRoleAsync(bootstrapAdmin, "Admin");
                }
            }

            await app.RunAsync();
        }
    }
}
