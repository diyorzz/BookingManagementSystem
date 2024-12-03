using ActualLab.Fusion;
using ActualLab.Fusion.Blazor;
using ActualLab.Fusion.EntityFramework;
using ActualLab.Fusion.EntityFramework.Npgsql;
using ActualLab.Fusion.Extensions;
using BookingSystem.Database;
using BookingSystem.Service;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


//connect with NgpSql
builder.Services.AddDbContextServices<BookingManagementDbContext>(db =>
{
    db.AddOperations(operations =>
    {
        operations.ConfigureOperationLogReader(_ => new()
        {

            CheckPeriod = TimeSpan.FromSeconds(5),
        });
        operations.ConfigureEventLogReader(_ => new()
        {
            CheckPeriod = TimeSpan.FromSeconds(5),
        });
        operations.AddNpgsqlOperationLogWatcher();
    });
    db.Services.AddTransientDbContextFactory<BookingManagementDbContext>((c, db) =>
    {
        db.UseNpgsql(builder.Configuration.GetConnectionString("BookingConnection"), npgsql =>
        {
            npgsql.EnableRetryOnFailure(0);
        });
        db.UseNpgsqlHintFormatter();
    });
});

//service configurations
var fusion = builder.Services.AddFusion();
fusion.AddBlazor();
fusion.AddFusionTime();
fusion.AddService<CounterService>();
fusion.AddService<TicketService>();
fusion.AddOperationReprocessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
