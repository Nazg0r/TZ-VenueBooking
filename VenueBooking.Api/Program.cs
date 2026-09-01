using VenueBooking.Api.MinimalAPIs;
using VenueBooking.BusinessLogic;
using VenueBooking.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Підключення шару DataAccess
builder.Services.AddDataAccess(builder.Configuration);
// Підключення шару BusinessLogic
builder.Services.AddBusinessLogic();
// Підключення ProblemDetails для стандартизованого оброблення помилок
builder.Services.AddProblemDetails();
// Підключення Validation для валідації DTO через анотації
builder.Services.AddValidation();
// Підключення OpenAPI для генерації документації API
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware для глобального оброблення помилок
app.UseExceptionHandler();
// Middleware для переадресації HTTP на HTTPS
app.UseHttpsRedirection();

// Маршрут для OpenAPI документації та Swagger UI
app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "VenueBooking API v1"));

// Маршрути для VenueBooking API 
app.MapApiEndpoints();

app.Run();