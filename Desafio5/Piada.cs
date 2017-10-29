using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desafio5
{
    public class Piada
    {
        public string Nome { get; set; }
        public string Url { get; set; }
        public string Corpo { get; set; }
        public string Categoria { get; set; }
        public DateTime DataPublicacao { get; set; }
        public int VotosPositivos { get; set; }
        public int VotosNegativos { get; set; }
        public int VotosMedios { get; set; }
    }
}
