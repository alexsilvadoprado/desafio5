using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Desafio5
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://www.osvigaristas.com.br/piadas/";

            WebClient webClient = new WebClient();

            string resposta = webClient.DownloadString(url);

            int indiceInicial = resposta.IndexOf("<article");
            int indiceFinal = resposta.LastIndexOf("</article>");

            string piadas = resposta.Substring(indiceInicial, indiceFinal - indiceInicial + 10);
            piadas = Regex.Replace(piadas, ">(\\s)*<", "><");
            DeserializarPiadas(piadas);
        }

        private static void DeserializarPiadas(string piadas)
        {
            string[] split = Regex.Split(piadas, "article>");
            foreach (var s in split)
            {
                Piada piada = new Piada();
                int indiceInicioUrl = s.IndexOf("<h4><a href=");
                int indiceFimUrl = s.IndexOf(".html");
                piada.Url = s.Substring(indiceInicioUrl + 13, indiceFimUrl - indiceInicioUrl - 8);

                int indiceFimNome = s.IndexOf("</a></h4>");
                piada.Nome = s.Substring(indiceFimUrl + 7, indiceFimNome - indiceFimUrl - 7);

                int indiceInicioCorpo = s.IndexOf("<p>");
                int indiceFimCorpo = s.LastIndexOf("</p>");
                string corpo = s.Substring(indiceInicioCorpo, indiceFimCorpo - indiceInicioCorpo + 4);
                string[] splitCorpo = Regex.Split(corpo, "><");
                corpo = "";
                foreach (var p in splitCorpo)
                {
                    int indiceInicial = p.IndexOf("p>");
                    int indiceFinal = p.IndexOf("</");
                    corpo += p.Substring(indiceInicial + 2, indiceFinal - indiceInicial - 2) + "\n";
                }
                piada.Corpo = corpo;

                int indiceInicioCategoria = s.IndexOf("<li>");
                int indiceFimCategoria = s.LastIndexOf("</li>");
                string categorias = s.Substring(indiceInicioCategoria, indiceFimCategoria - indiceInicioCategoria + 5);
                string[] splitCategorias = Regex.Split(categorias, "</a></li>");
                categorias = "";
                foreach (var cat in splitCategorias)
                {
                    int indiceNomeCategoria = cat.LastIndexOf(">");
                    categorias += cat.Substring(indiceNomeCategoria + 1, cat.Length - indiceNomeCategoria - 1) + "\n";
                }
                piada.Categoria = categorias;
            }
        }
    }
}
