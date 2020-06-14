using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TesteNovaVidaTI.Models
{
    public class Aluno
    {
        public int IdAluno { get; set; }
        public int IdProfessor { get; set; }
        public string Nome { get; set; }
        public double Mensalidade { get; set; }
        public DateTime DataVencimento { get; set; }

        public Aluno()
        {

        }
    }
}