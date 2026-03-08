using BookStoreManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Set up ApplicationDBContext with PostgreSQL (neon)
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(connectionString));

// Set up Conllers and Swagger (test API) 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// bugs in next line
// builder.Services.AddSwaggerGen();

var  app = builder.Build();
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();