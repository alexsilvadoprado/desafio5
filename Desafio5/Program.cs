/****************************** Desafio 6 ******************************
 Desafio 5 : API Piadas
 Autor    : Álex Prado
 Data     : 29 de outubro de 2017
 Versão   : 1.0
**********************************************************************/
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

            /** Faz o download do HTML da página */
            string resposta = webClient.DownloadString(url);
            resposta = Encoding.UTF8.GetString(Encoding.Default.GetBytes(resposta));

            int indiceInicial = resposta.IndexOf("<article");
            int indiceFinal = resposta.LastIndexOf("</article>");

            /** Separa o HTML que contém as piadas do HTML da página */
            string piadas = resposta.Substring(indiceInicial, indiceFinal - indiceInicial + 10);
            piadas = Regex.Replace(piadas, ">(\\s)*<", "><");
            List<Piada> listaPiadas = DeserializarPiadas(piadas);
        }

        /// <summary>
        /// Deserializa o HTML de piadas
        /// </summary>
        /// <param name="piadas"></param>
        /// <returns>Lista contendo as piadas do HTML</returns>
        private static List<Piada> DeserializarPiadas(string piadas)
        {
            /** Faz um split para separar as piadas do HTML */
            string[] split = Regex.Split(piadas, "article>");

            /** Lista para armazenar as piadas */
            List<Piada> listPiadas = new List<Piada>();

            /** Percorre o split de piadas */
            foreach (var s in split)
            {
                /** Verifica se é vazia */
                if (string.IsNullOrEmpty(s))
                    continue;

                /** Instancia um objeto de Piada para armazenar as informações deserializadas */
                Piada piada = new Piada();

                #region Deserializa Url
                int indiceInicioUrl = s.IndexOf("<h4><a href=");
                int indiceFimUrl = s.IndexOf(".html");
                piada.Url = s.Substring(indiceInicioUrl + 13, indiceFimUrl - indiceInicioUrl - 8);
                #endregion

                #region Deserializa Nome
                int indiceFimNome = s.IndexOf("</a></h4>");
                piada.Nome = s.Substring(indiceFimUrl + 7, indiceFimNome - indiceFimUrl - 7);
                #endregion

                #region Deserializa Corpo
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
                #endregion

                #region Deserializa Categoria(s)
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
                #endregion

                #region Deserializa Data Publicação
                int indiceInicioTime = s.IndexOf("<time datetime=");
                int indiceFimTime = s.LastIndexOf("</time>");
                string hora = s.Substring(indiceInicioTime + 16, indiceFimTime - indiceInicioTime - 34);
                piada.DataPublicacao = DateTime.ParseExact(hora, "yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture);
                #endregion

                #region Deserializa Votos
                Regex regexVotos = new Regex(@"<div class=""votes"" (.+?)></div></div></div></div></");
                var votos = regexVotos.Matches(s)
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();
                if (votos.Count() > 0)
                {
                    var splitVotos = Regex.Split(votos[0], "><");
                    foreach (var itemVoto in splitVotos)
                    {
                        /** Votos Positivos */
                        if (itemVoto.Contains("stats-up"))
                        {
                            int indiceInicioVoto = itemVoto.IndexOf("div class=");
                            int indiceFimVoto = itemVoto.IndexOf("</div");
                            piada.VotosPositivos = Convert.ToInt32(itemVoto.Substring(indiceInicioVoto + 21, indiceFimVoto - indiceInicioVoto - 21));
                        }
                        /** Votos Médios */
                        else if (itemVoto.Contains("score"))
                        {
                            int indiceInicioVoto = itemVoto.IndexOf("div class=");
                            int indiceFimVoto = itemVoto.IndexOf("</div");
                            piada.VotosMedios = Convert.ToInt32(itemVoto.Substring(indiceInicioVoto + 18, indiceFimVoto - indiceInicioVoto - 18));
                        }
                        /** Votos Negativos */
                        else if (itemVoto.Contains("stats-down"))
                        {
                            int indiceInicioVoto = itemVoto.IndexOf("div class=");
                            int indiceFimVoto = itemVoto.IndexOf("</div");
                            piada.VotosNegativos = Convert.ToInt32(itemVoto.Substring(indiceInicioVoto + 23, indiceFimVoto - indiceInicioVoto - 23));
                        }
                    }
                }
                #endregion

                /** Adiciona piada na lista */
                listPiadas.Add(piada);
            }

            /** Retorna lista de piadas */
            return listPiadas;
        }
    }
}
