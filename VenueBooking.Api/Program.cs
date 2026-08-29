using Microsoft.EntityFrameworkCore;
using VenueBooking.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VenueBookingContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.Run();
