using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace BookStoreApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("secret.json", optional: true, reloadOnChange: true);

            // Add services to the container.

            // Configurazione del DbContext per l'accesso al database
            var connString = builder.Configuration.GetConnectionString("BookStoreAppDbConnection");

            // identityUser e' la classe che rappresenta l'utente base
            builder.Services.AddIdentityCore<ApiUser>() // specifica la classe che rappresenta l'utente base (personalizzata -> standard = IdentityUser)
                .AddRoles<IdentityRole>() // i ruoli rappresentano i diversi tipi di utenti (es. admin, user e i loro permessi)
                .AddEntityFrameworkStores<BookStoreDbContext>(); // specifica il DbContext da utilizzare per l'accesso al database

            builder.Services.AddDbContext<BookStoreDbContext>(options =>
                options.UseSqlServer(connString));

            // Configurazione di AutoMapper
            builder.Services.AddAutoMapper(typeof(MapperConfig));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Serilog
            builder.Host.UseSerilog((ctx, lc) => 
                lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

            // Configure CORS policy to allow all origins, methods and headers
            // questo permette di accettare richieste da qualsiasi origine
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            //----------------------------------------------------------
            // Configurazione dell'autenticazione JWT
            builder.Services.AddAuthentication(options =>
            {
                // specifica il metodo di autenticazione da utilizzare
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // specifica il metodo di autenticazione da utilizzare per le richieste non autenticate
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                // specifica i parametri per la validazione del token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ci sono molte opzioni per la validazione del token, controllare quali usare
                    // in base alle esigenze del progetto
                    ValidateIssuerSigningKey = true, // controlla se la chiave pubblica e' valida
                    ValidateIssuer = true, // controlla se l'issuer e' valido
                    ValidateAudience = true, // controlla se l'audience e' valida
                    ValidateLifetime = true, // controlla se il token e' scaduto
                    ClockSkew = TimeSpan.Zero, // differenza massima tra l'orario del server e quello del client
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // riporta al file appsettings.json
                    ValidAudience = builder.Configuration["JwtSettings:Audience"], // riporta al file appsettings.json
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])) // riporta al file secret.json
                };
            });
            //----------------------------------------------------------

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // abilita l'utilizzo di CORS policy
            app.UseCors("AllowAll");

            //----------------------------------------------------------
            // abilita la gestione delle richieste HTTP
            app.UseAuthentication();
            //----------------------------------------------------------

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
