using Microsoft.EntityFrameworkCore;
using TxTracker.TransactionsApi.Accounts;
using TxTracker.TransactionsApi.Categories;
using TxTracker.TransactionsApi.Configurations;
using TxTracker.TransactionsApi.Data;
using TxTracker.TransactionsApi.Tags;
using TxTracker.TransactionsApi.Transactions;

namespace TxTracker.TransactionsApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        builder.Services.AddControllers();

        builder.Configuration.Bind(nameof(DbConfig));
        builder.Services.AddDbContext<TransactionsContext>((sp, options) =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DB_CONN_STRING"));
        });
        builder.Services.AddScoped<TransactionsRepository>();
        builder.Services.AddScoped<TagsRepository>();
        builder.Services.AddScoped<CategoriesRepository>();
        builder.Services.AddScoped<AccountsRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
