using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TesteNovaVidaTI.Models
{
    public class Professor
    {
        public int IdProfessor { get; set; }
        public string Nome { get; set; }
        
        public DateTime DataInclusao { get; set; }

        public Professor()
        {

        }
    }
}