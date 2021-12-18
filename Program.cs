using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ComputerContext>(o => o.UseInMemoryDatabase("Computers"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Olá Visual Studio Summit!").ExcludeFromDescription();

app.MapGet("/computers", async (ComputerContext context) => await context.Computers.ToListAsync())
    .Produces<List<Computer>>(StatusCodes.Status200OK);

app.MapGet("/computers/{id:int}", async (int id, ComputerContext context) =>  await context.Computers.FirstOrDefaultAsync(c => c.Id == id));

app.MapPost("/computers", async (Computer computer, ComputerContext context, HttpContext httpContext) => 
{
    context.Computers.Add(computer);
    await context.SaveChangesAsync();
    return Results.Created($"/computers/{computer.Id}", computer);
});

app.MapPut("/computers/{id:int}", async (Computer computer, ComputerContext context) =>
{
    context.Entry(computer).State = EntityState.Modified;
    await context.SaveChangesAsync();
    return computer;
});

app.MapDelete("/computers/{id:int}", async (int id, ComputerContext context) => 
{
    var computer = context.Computers.FirstOrDefaultAsync(c => c.Id == id);
    context.Remove(computer);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public class Computer
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string Brand { get; set; }
}

public class ComputerContext : DbContext
{
    public DbSet<Computer> Computers { get; set; }

    public ComputerContext(DbContextOptions options) : base(options) { }
}