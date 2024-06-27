using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Configurations;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BookStoreApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configurazione del DbContext per l'accesso al database
            var connString = builder.Configuration.GetConnectionString("BookStoreAppDbConnection");

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
