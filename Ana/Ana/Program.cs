using Ana.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

app.MapPost("/api/funcionario/cadastrar", ([FromBody] Funcionario funcionario, [FromServices] AppDataContext cxt) =>
{
    cxt.Funcionario.Add(funcionario);
    cxt.SaveChanges();
    return Results.Created($"/produto/{funcionario.Id}", funcionario);
});

app.MapGet("/api/funcionario/listar", ([FromServices] AppDataContext cxt) =>
{
  return Results.Ok(cxt.Funcionario.ToList());
});

app.Run();
