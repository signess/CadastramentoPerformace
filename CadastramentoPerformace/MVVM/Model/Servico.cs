using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastramentoPerformace.MVVM.Model
{
    public class Servico
    {
        public string NomeLocal { get; set; }
        public int CodigoOS { get; set; }
        public string DescricaoOS { get; set; }
        public bool Improdutivo { get; set; }
        public int NumeroEquipe { get; set; }
        public string Executor { get; set; }
        public DateTime Data { get; set; }
        public int Quantidade { get; set; }
        public float TempoExecucao { get; set; }

    }
}
