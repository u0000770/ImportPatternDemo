using Domain;
using Microsoft.EntityFrameworkCore;
using SqlItems;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using noSqlItems;
using GenRepository;
using MongoDB.Driver;
using Repository;

namespace PatternDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Sql Context
           builder.Services.AddDbContext<MyDbContext>();
           builder.Services.AddScoped<Repository.IRepository<Item>, GenRepository.Repository<Item> >();

            // Build and Register the MongoDB client
           //  BuildMongoContext(builder);
           //  builder.Services.AddScoped<IRepository<Item>, MongoRepository<Item>>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void BuildMongoContext(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient("mongodb://localhost:27017"));

            // Register the IMongoDatabase (you may already have this registered)
            builder.Services.AddScoped<IMongoDatabase>(sp =>
            {
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                return mongoClient.GetDatabase("JustItems");
            });

            ////// Register the IMongoCollection<T>
            builder.Services.AddScoped<IMongoCollection<Item>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Item>("Items");
            });
        }
    }
}
