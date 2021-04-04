using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace efcore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            HealthCheckBD();
        }

        //Logo que executa a aplicação ele executa o banco de dados , caso ele não exista
        static void EnsureCreate(){
            using var db = new Curso.Data.ApplicatioContext();
            db.Database.EnsureCreated();
        }
        //Deleta a base inteira no momento da execução
        static void EnsureDeleted(){
            using var db = new Curso.Data.ApplicatioContext();
            db.Database.EnsureDeleted();        
        }
        //Resolve o problema de se usa mais de um contexto e força a criação da base
        static void GapDoEnsureCreatedComMultiplosContextos(){
            using var db1 = new Curso.Data.ApplicatioContext();
            using var db2 = new Curso.Data.ApplicatioContextCidade();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        static void HealthCheckBD(){
            using var db = new Curso.Data.ApplicatioContext();
            var canConnect = db.Database.CanConnect();

            if(canConnect){
                Console.WriteLine("Conexão Liberada");
            }else{
                Console.WriteLine("Conexão não Liberada");
            }
        }
    }
}
