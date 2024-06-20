using Microsoft.EntityFrameworkCore;

namespace Ana.Models;

public class AppDataContext : DbContext
{
  //Quais classes vão representar as tabelas no banco
  public DbSet<Funcionario> Funcionario { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ana_ana.db");
    }

}

