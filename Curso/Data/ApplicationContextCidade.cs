using System;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicatioContextCidade : DbContext
    {
        public DbSet<Cidade> Cidades {get;set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data source=localhost; Initial Catalog=dominandoEfCore;User ID=sa;Password=<YourStrong@Passw0rd>;Pooling=true;";
            optionsBuilder.UseSqlServer(strConnection)
                          .EnableSensitiveDataLogging()
                          .LogTo(Console.WriteLine,LogLevel.Information);
        }
    }
}