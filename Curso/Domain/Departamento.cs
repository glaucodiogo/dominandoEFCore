using System;
using System.Collections.Generic;
using System.Linq;
namespace Curso.Domain
{
    public class Departamento
    {
        public int Id {get;set;}
        public string Descricao { get; set; }
        public List<Funcionario> Funcionarios { get; set; }
        public bool Ativo { get; set;}
    }
}