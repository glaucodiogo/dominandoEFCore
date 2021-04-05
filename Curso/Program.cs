using System;
using System.Collections.Generic;
using System.Linq;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace efcore
{
    class Program
    {
        static void Main(string[] args)
        {
            MigracoesPendentes();

        }

        //Logo que executa a aplicação ele executa o banco de dados , caso ele não exista
        static void EnsureCreate(){
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureCreated();
        }
        //Deleta a base inteira no momento da execução
        static void EnsureDeleted(){
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();        
        }
        //Resolve o problema de se usa mais de um contexto e força a criação da base
        static void GapDoEnsureCreatedComMultiplosContextos(){
            using var db1 = new Curso.Data.ApplicationContext();
            using var db2 = new Curso.Data.ApplicatioContextCidade();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        static void HealthCheckBD(){
            using var db = new Curso.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if(canConnect){
                Console.WriteLine("Conexão Liberada");
            }else{
                Console.WriteLine("Conexão não Liberada");
            }
        }

static int _count;
        //Realizar a gestao da conexao é muito melhor que deixar na mão do ef. 
        static void GerenciarEstadoDaConexao(bool gerenciarEstado){
            using var db = new Curso.Data.ApplicatioContextCidade();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_,__) => ++_count;

            if(gerenciarEstado){
                conexao.Open();            
            }

            for(var i=0; i < 400;i++){
                db.Cidades.AsNoTracking().Any();
            }

            time.Stop();

            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstado}, Numero Conexões abertas:{_count}";
            Console.WriteLine(mensagem);
        }
        static void ExecuteSql(){
            //Primeira opçao
            using var db = new Curso.Data.ApplicationContext();
            using(var cmd = db.Database.GetDbConnection().CreateCommand()){
                cmd.CommandText= "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            //Segunda opção
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("UPDATE departamentos SET descricao={0} WHERE ID=1",descricao);

            //Terceira Opçao
            db.Database.ExecuteSqlInterpolated($"UPDATE departamentos SET descricao={descricao} WHERE ID=1");
        }


        static void SqlInjection(){
            using var db = new Curso.Data.ApplicationContext();
            EnsureDeleted();
            EnsureDeleted();

            var descricao = "Teste ' or 1='1";
            db.Database.ExecuteSqlRaw($"UPDATE departamentos SET descricao='AtaqueInjection' WHERE descricao='{descricao}'");
            foreach(var departamento in db.Departamentos.AsNoTracking()){
                //Console.WriteLine($"id: {departamento.Id}, Descricao: {departamento.Name}");
            }
        }

        static void DriblandoSqlInjection(){
            using var db = new Curso.Data.ApplicationContext();
            var descricao = "nova";
            db.Database.ExecuteSqlRaw("UPDATE departamentos SET descricao='{0}' WHERE Id=1",descricao);
            
        }
        //1 - dotnet add package Microsoft.EntityFrameworkCore.Design --version 5.0.0
        //2 - dotnet tool install --global dotnet-ef --version 5.0.0
        //3 - dotnet ef migrations add initial --context ApplicationContext
        static void MigracoesPendentes(){
            using var db = new Curso.Data.ApplicationContext();

            var migracoesPendentes= db.Database.GetPendingMigrations();
            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach(var migracao in migracoesPendentes){
                Console.WriteLine($"Migração: {migracao}");
            }
        }
        //Para testar pode-se usar o comando para deletar a base
        //dotnet ef database drop --context ApplicationContext
        static void AplicarMigracaoEmTempoDeExecucao(){
            using var db = new Curso.Data.ApplicationContext();
            db.Database.Migrate();// verifica todas migracoes pendentes e aplica em tempo de execucao
        }

        //1- Adiciona nova propriedade na entidade
        //2- Dotnet ef migrations add RG --context ApplicationContext
        //3- Automaticamente o migrações é gerado baseado na alteração em dominio
        static void RecuperaTodasMigracoes(){
            using var db = new Curso.Data.ApplicationContext();
            
            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes){
                Console.WriteLine($"Migração: {migracao}");
            }
        }

         static void MigracoesJaAplicadas(){
            using var db = new Curso.Data.ApplicationContext();
            
            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes){
                Console.WriteLine($"Migração: {migracao}");
            }
        }
        //gera o script de todo o banco de dados
        static void ScriptGeralDoBancoDeDados(){
            using var db = new Curso.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();
        }
        //Neste tipo de carregamento, é recomendável utilizar para carregamento de poucos campos
        //pois o left join carrega a tabela toda de funcionarios, podendo acarretar lentidão
        static void CarregamentoAdiantado(){
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            //Toda vez que for carregar departamento, será carregado também funcionários.Todos funcionários são carregados
            var departamentos = db.Departamentos
                                  .Include(p=> p.Funcionarios);

            foreach(var departamento in departamentos){
                Console.WriteLine("---------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if(departamento.Funcionarios?.Any() ?? false){
                    foreach(var funcionario in departamento.Funcionarios){
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }else{
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }


        static void SetupTiposCarregamentos(Curso.Data.ApplicationContext db){
            if(!db.Departamentos.Any()){
                db.Departamentos.AddRange(
                    new Departamento{
                        Descricao = "Departamento 01",
                        Funcionarios = new List<Funcionario>{
                            new Funcionario{
                                Nome = "Rafael Oliveira",
                                Cpf  = "78946513245",
                                Rg   = "123456"
                            },
                             new Funcionario{
                                Nome = "Maria Oliveira",
                                Cpf  = "7866663245",
                                Rg   = "12000"
                            }
                        }
                    },
                    new Departamento{
                        Descricao = "Departamento 02",
                        Funcionarios = new List<Funcionario>{
                            new Funcionario{
                                Nome = "João Oliveira",
                                Cpf  = "12346513245",
                                Rg   = "444444"
                            }
                        }
                    }
                );

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }


        }
    }
  
}
