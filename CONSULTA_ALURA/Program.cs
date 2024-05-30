/*
 * ########################################################################################################
 * AUTOR: GABRIEL GUEDES
 * 
 * DATA ÚLTIMA MODIFICAÇÃO: 30/05/2024
 * NÚMERO DA VERSÃO: 1.0
 * 
 * DATA ÚLTIMA REVISÃO: 30/05/2024
 * NÚMERO DA REVISÃO: REV.001
 * ########################################################################################################
 */

//Classe para consulta de cursos na alura

using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V123.Network;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace CONSULTA_ALURA
{
    public class Program
    {
        public static void Main(string[] args)
        {

        }

        public static int CONSULTA_PRINC_EXECUTA_AUTOMACAO(string nomeCurso) // metodo principal para realizar consultas
        {
            int ret = 0; // variavel para tratativa de erro

            ret = Utilities.VerificaChrome(); //validação chrome
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            ret = CONSULTA_CURSO(nomeCurso);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            return 0;
        }

        private static int CONSULTA_CURSO(string nomeCurso) // metodo para consultar no site da alura
        {
            // Variavel para controle de erro
            int ret = 0;

            // Abre chrome
            var driver = Utilities.AbreChrome(); //abre chrome com selenium
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt32(Utilities.GetParameters("Wait")))); // variavel para daley com 30 segundos

            // Abrir site da alura, capturando o link do app config
            driver.Navigate().GoToUrl(Utilities.GetParameters("urlSiteAlura"));

            // Validação acesso ao site + validação campo busca
            ret = Utilities.VerificaAcesso(driver, Utilities.GetParameters("idCampoBuscaCurso"), wait);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            // Buscar curso
            Utilities.EnviaValor(driver, By.Id(Utilities.GetParameters("idCampoBuscaCurso")), nomeCurso);

            // Envia click para busca
            Utilities.EnviaClick(driver, By.XPath(Utilities.GetParameters("CaminhoElementoBtnBusca")));

            // Esperar para a proxima pagina ser carregada com os resultados
            Utilities.WaitForElement(driver, By.Id(Utilities.GetParameters("idCampoBuscaResultados")));

            var resultado = Utilities.FindElemests(driver, By.XPath("/html/body/div[2]/div[2]/section/div/h2"));
            var textoVisivel = resultado.Displayed; //true para elementos visiveis na pagina / false para elementos escondidos

            // Se verdadeiro, validação para curso nao encontrado
            if (textoVisivel)
            {
                driver.Quit();
                Utilities.LOG(1, $"Curso consultado mas sem resultados da pagina");
                return -1;
            }
            else
            {
                Utilities.LOG(1, $"Curso consultado com sucesso!");
            }

            // Localiza o elemento "section" dos resultados
            var section = driver.FindElement(By.Id("busca-resultados"));

            // Classe criada para estrutura de dados
            DadosCurso dados = new DadosCurso();

            // Encontra todos os elementos LI dentro da "section"
            var listItems = section.FindElements(By.TagName("li"));
            string nomeCompletoCurso = listItems[0].Text;
            dados.nomeCurso = nomeCompletoCurso.Substring(0, nomeCompletoCurso.IndexOf("\r"));
            var links = listItems[0].FindElements(By.TagName("a")); // captura TAG HTML A dentro de cada LI encontrada no resultado
            string linkCurso = links[0].GetAttribute("href"); // captura href com o link para abrir dados do curso

            driver.Navigate().GoToUrl(linkCurso); //acessa link curso

            // Busca por todos os elementos no documento
            var allElements = driver.FindElements(By.XPath("//*"));

            // captura carga horaria do curso
            // Encontra o elemento que contém o texto específico para consultar carga horaria
            var element1 = driver.FindElement(By.XPath($"//*[contains(text(), 'Para conclusão')]"));
            IWebElement parentDiv = element1;
            while (parentDiv != null && parentDiv.TagName.ToLower() != "div")
            {
                parentDiv = parentDiv.FindElement(By.XPath(".."));
            }
            string divContent = parentDiv.GetAttribute("innerHTML");
            dados.cargaHoraria = divContent.Substring(divContent.IndexOf(">"));
            dados.cargaHoraria = dados.cargaHoraria.Substring(0, 4).Replace(">","").Replace("<", "");

            if (dados.cargaHoraria == "")
            {
                Utilities.LOG(1, $"Não foi possivel localiar a carga horaria do curso");
            }

            // Captura descrição do curso
            // Dependedo, a estrutura aonde se encontra a descrição muda
            var elementoDescricao = Utilities.FindElemests(driver, By.XPath("/html/body/main/section[1]/article/div/div/div[2]/div/h2"));
            var elementoDescricao1 = Utilities.FindElemests(driver, By.XPath("/html/body/section[1]/div/div[1]/p[2]"));
            if (elementoDescricao != null)
            {
                dados.descricao = elementoDescricao.Text;
            }
            else if(elementoDescricao1 != null)
            {
                dados.descricao = elementoDescricao1.Text;
            }
            else
            {
                Utilities.LOG(1, $"Não foi possivel localiar a descrição do curso");
                // Em casos que a automação nao consegue achar a descriçãos
            }

            // Verificar se existe a lista de instrutores
            string todosInstrutores = string.Empty;
            var listaInstrutores = Utilities.FindElemests(driver, By.XPath("/html/body/main/section[2]/section[3]/div/ul"));
            
            if (elementoDescricao != null)
            {
                var instrutores = listaInstrutores.FindElements(By.TagName("li"));
                foreach (var instrutor in instrutores)
                {
                    string inst = instrutor.Text;
                    if (!string.IsNullOrEmpty(inst))  // em alguns casos a lista vem em branco, so ira adicionar a string quando houver texto
                    {
                        if (inst.Contains("\r")) // validação para verificar se tem quebra de linhas
                        {
                            inst = inst.Substring(0, inst.IndexOf("\r"));
                        }
                        
                        todosInstrutores += $"{inst},";
                    }
                }
                todosInstrutores = todosInstrutores.Substring(0, todosInstrutores.Length - 1); // remover a ultima "," da string com todos os instrutores
            }

            // Quando é somente um instrutor é em outro html
            var instrutor1 = Utilities.FindElemests(driver, By.XPath("/html/body/section[2]/div[1]/section/div/div/div/h3"));
            if (instrutor1 != null)
            {
                todosInstrutores = instrutor1.Text;
            }

            dados.nomeInstrutor = todosInstrutores;

            // Insere dados no banco com os valores preenchidos da classe
            AcoesDB.DB_INSERE_DADOS(dados);

            driver.Quit();

            return 0;
        }
    }
}
