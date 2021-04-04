﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace efcore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            _count =0;
            GerenciarEstadoDaConexao(false);
            _count =0;
            GerenciarEstadoDaConexao(true);
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

        
    }
}
