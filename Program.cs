using Microsoft.EntityFrameworkCore;
using ExpenseSharing.Data;
using ExpenseSharing.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<ExpenseSharingContext>(options =>
    options.UseSqlite("Data Source=expenses.db"));

// Add services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<BalanceService>();
builder.Services.AddScoped<DataSeedService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(); // Move CORS before other middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ExpenseSharingContext>();
    var seedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
    
    // Delete and recreate database to reset all data
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    
    await seedService.SeedDataAsync();
}

app.Urls.Add($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "5000"}");
app.Run();