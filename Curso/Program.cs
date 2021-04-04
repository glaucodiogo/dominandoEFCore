using System;

namespace efcore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            EnsureCreate();
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
    }
}
