using Ana.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<AppDataContext>();
        var app = builder.Build();

        app.MapPost("/api/funcionario/cadastrar", ([FromBody] Funcionario funcionario, [FromServices] AppDataContext cxt) =>
        {
            cxt.Funcionarios.Add(funcionario);
            cxt.SaveChanges();
            return Results.Created($"/produto/{funcionario.Id}", funcionario);
        });

        app.MapGet("/api/funcionario/listar", ([FromServices] AppDataContext cxt) =>
        {
            return Results.Ok(cxt.Funcionarios.ToList());
        });

        app.MapPost("/api/folha/cadastrar", ([FromBody] Folha folha, [FromServices] AppDataContext cxt) =>
        {
            //Validar se o funcionario existe 
           Funcionario? funcionario = 
                cxt.Funcionarios.Find(folha.FuncionarioId);

            if (funcionario is null)
              return Results.NotFound("Funcionario não encontrado");

            folha.Funcionario = funcionario;

            //Calcular salario bruto 
            folha.SalarioBruto = folha.Quantidade * folha.Valor;

            //calcular IRRF
            if (folha.SalarioBruto <= 1903.98)
                folha.ImpostoIRRF = 0;
            if (folha.SalarioBruto <= 2826.65)
                folha.ImpostoIRRF = folha.SalarioBruto * .075 - 142.80;
            if (folha.SalarioBruto <= 3751.05)
                folha.ImpostoIRRF = folha.SalarioBruto * .15 - 354.80;
            if (folha.SalarioBruto <= 4664.68)
                folha.ImpostoIRRF = folha.SalarioBruto * .225 - 636.13;
            else
                folha.ImpostoIRRF = folha.SalarioBruto * .275 - 869.36;

            //Calcular INSS
            if (folha.SalarioBruto <= 1693.72)
                folha.ImpostoINSS = folha.SalarioBruto * .08;
            if (folha.SalarioBruto <= 2822.90)
                folha.ImpostoINSS = folha.SalarioBruto * .09;
            if (folha.SalarioBruto <= 5645.80)
                folha.ImpostoINSS = folha.SalarioBruto * .11;
            else
                folha.ImpostoINSS = 621.04;
 
            //calcular o FGTS
            folha.ImpostoFGTS = folha.SalarioBruto * .08;

            //calcular o salario liquido
            folha.SalarioLiquido = folha.SalarioBruto - folha.ImpostoIRRF - folha.ImpostoINSS;

            cxt.Folhas.Add(folha);
            cxt.SaveChanges();
            return Results.Created($"/produto/{folha.Id}", folha);
        });

        app.MapGet("/api/folha/listar", ([FromServices] AppDataContext cxt) =>
        {
            return Results.Ok(cxt.Folhas.Include(x => x.Funcionario).ToList());
        });

        app.MapGet("/api/folha/buscar/{cpf}/{mes}/{ano}", ([FromServices] AppDataContext cxt, [FromRoute] int mes, [FromRoute] int ano, [FromRoute] string cpf) =>
        {
            Folha? folha = cxt.Folhas.
            Include(x => x.Funcionario).
            FirstOrDefault(f => f.Funcionario.CPF == cpf && f.Mes == mes && f.Ano == ano);
            if (folha is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(folha);
        });

        app.Run();
    }
}