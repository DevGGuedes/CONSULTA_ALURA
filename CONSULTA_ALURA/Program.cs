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
using OpenQA.Selenium.Support.UI;
using System;

namespace CONSULTA_ALURA
{
    public class Program
    {
        // Classe criada para estrutura de dados
        private static DadosCurso dados = new DadosCurso();

        public static void Main(string[] args)
        {

        }

        // Metodo principal para realizar o processo
        public static int CONSULTA_PRINC_EXECUTA_AUTOMACAO(string nomeCurso)
        {
            int ret = 0; // variavel para tratativa de erro

            ret = Utilities.VerificaChrome(); // Validação chrome
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            ret = CONSULTA_CURSO(nomeCurso);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            Utilities.LOG(1, $"Consulta do curso realizada com sucesso");

            return 0;
        }

        // Metodo principal para realizar consultas
        private static int CONSULTA_CURSO(string nomeCurso) // metodo para consultar no site da alura
        {
            // Variavel para controle de erro
            int ret = 0;

            // Abre chrome
            var driver = Utilities.AbreChrome();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt32(Utilities.GetParameters("Wait")))); // variavel para daley com 30 segundos

            // Abrir site da alura, capturando o link do app config
            driver.Navigate().GoToUrl(Utilities.GetParameters("urlSiteAlura"));

            // Validação acesso ao site + validação campo busca
            ret = Utilities.VerificaAcesso(driver, Utilities.GetParameters("idCampoBuscaCurso"), wait);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            // Metodo para realizar a consulta no site
            ret = REALIZA_CONSULTA(driver, nomeCurso);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            // Metodo para validação apos realizar a consulta do curso
            ret = VALIDA_RESULTADO_CONSULTA(driver);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            // Metodo principal para capturar os dados
            ret = CAPTURA_DADOS_CURSO(driver);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            AcoesDB.DB_INSERE_DADOS(dados);

            driver.Quit();

            return 0;
        }

        // Metodo para realizar a consulta no site
        private static int REALIZA_CONSULTA(IWebDriver driver, string nomeCurso)
        {
            try
            {
                // Buscar curso
                Utilities.EnviaValor(driver, By.Id(Utilities.GetParameters("idCampoBuscaCurso")), nomeCurso);

                // Envia click para busca
                Utilities.EnviaClick(driver, By.XPath(Utilities.GetParameters("CaminhoElementoBtnBusca")));

                // Esperar para a proxima pagina ser carregada com os resultados
                Utilities.WaitForElement(driver, By.Id(Utilities.GetParameters("idCampoBuscaResultados")));

                return 0;
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao realizar consulta {ex}");
                return -1;
            }
        }

        // Metodo para capturar todos os dados necessario
        private static int CAPTURA_DADOS_CURSO(IWebDriver driver)
        {
            // Variavel para controle de erro
            var ret = 0;

            try
            {
                ret = CAPTURA_NOME_CURSO(driver);
                if (ret != 0) return -1; // em caso negativo, retorna erro
                ret = CAPTURA_INSTRUTORES(driver);
                if (ret != 0) return -1; // em caso negativo, retorna erro
                ret = CAPTURA_CARGA_HORARIA(driver);
                if (ret != 0) return -1; // em caso negativo, retorna erro
                ret = CAPTURA_DESCRICAO(driver);
                if (ret != 0) return -1; // em caso negativo, retorna erro

                return 0;
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao capturar dados {ex}");
                return -1;
            }
        }

        // Metodo para capturar os instrutores
        private static int CAPTURA_INSTRUTORES(IWebDriver driver)
        {
            try
            {
                // Verificar se existe a lista de instrutores
                string todosInstrutores = string.Empty;
                var listaInstrutores1 = Utilities.FindElemests(driver, By.XPath(Utilities.GetParameters("CaminhoElementoListaInstrutores")));
                var listaInstrutores2 = Utilities.FindElemests(driver, By.XPath(Utilities.GetParameters("CaminhoElementoListaInstrutores1")));

                if (listaInstrutores1 != null || listaInstrutores2 != null)
                {
                    IWebElement listaInstrutores = listaInstrutores1 ?? listaInstrutores2;

                    var instrutores = listaInstrutores.FindElements(By.TagName("li"));
                    foreach (var instrutor in instrutores)
                    {
                        string nome = instrutor.Text;

                        // Em alguns casos a lista vem em branco, so ira adicionar a string quando houver texto
                        if (!string.IsNullOrEmpty(nome))
                        {
                            // Validação para verificar se tem quebra de linhas
                            if (nome.Contains("\r"))
                            {
                                nome = nome.Substring(0, nome.IndexOf("\r"));
                            }

                            todosInstrutores += $"{nome},";
                        }
                    }

                    // Remover a ultima "," da string com todos os instrutores
                    todosInstrutores = todosInstrutores.Substring(0, todosInstrutores.Length - 1);
                }

                // Quando é somente um instrutor é em outro html
                var instrutor1 = Utilities.FindElemests(driver, By.XPath(Utilities.GetParameters("CaminhoElementoListaInstrutor")));
                if (instrutor1 != null)
                {
                    todosInstrutores = instrutor1.Text;
                }

                dados.nomeInstrutor = todosInstrutores;

                return 0;
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao capturar dados dos instrutores {ex}");
                return -1;
            }
        }

        // Metodo para capturar nome do curso
        private static int CAPTURA_NOME_CURSO(IWebDriver driver)
        {
            try
            {
                // Localiza o elemento "section" dos resultados
                var section = driver.FindElement(By.Id(Utilities.GetParameters("idCampoResultadosPesquisa")));

                // Encontra todos os elementos LI dentro da "section"
                var listItems = section.FindElements(By.TagName("li"));
                string nomeCompletoCurso = listItems[0].Text;
                dados.nomeCurso = nomeCompletoCurso.Substring(0, nomeCompletoCurso.IndexOf("\r"));
                var links = listItems[0].FindElements(By.TagName("a")); // captura TAG HTML A dentro de cada LI encontrada no resultado
                string linkCurso = links[0].GetAttribute("href"); // captura href com o link para abrir dados do curso

                driver.Navigate().GoToUrl(linkCurso); //acessa link curso

                return 0;
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao capturar o nome do curso {ex}");
                return -1;
            }
        }

        // Metodo para capturar a carga horaria
        private static int CAPTURA_CARGA_HORARIA(IWebDriver driver)
        {
            try
            {
                // Captura carga horaria do curso
                // Encontra o elemento que contém o texto específico para consultar carga horaria
                var element = driver.FindElement(By.XPath($"//*[contains(text(), 'Para conclusão')]"));

                // Sobe na árvore do DOM até encontrar o <div> pai, div acima do elemento desejado
                IWebElement parentDiv = element;
                while (parentDiv != null && parentDiv.TagName.ToLower() != "div")
                {
                    parentDiv = parentDiv.FindElement(By.XPath(".."));
                }

                string divContent = parentDiv.GetAttribute("innerHTML");
                dados.cargaHoraria = divContent.Substring(divContent.IndexOf(">"));
                dados.cargaHoraria = dados.cargaHoraria.Substring(0, 4).Replace(">", "").Replace("<", "");

                if (dados.cargaHoraria == "")
                {
                    Utilities.LOG(1, $"Não foi possivel localiar a carga horaria do curso");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao capturar carga horaria {ex}");
                return -1;
            }
        }

        // Metodo para capturar a descrição
        private static int CAPTURA_DESCRICAO(IWebDriver driver)
        {
            try
            {
                // Captura descrição do curso
                // Dependedo, a estrutura aonde se encontra a descrição muda
                var elementoDescricao = Utilities.FindElemests(driver, By.XPath(Utilities.GetParameters("CaminhoElementoDescricaoCurso")));
                var elementoDescricao1 = Utilities.FindElemests(driver, By.XPath(Utilities.GetParameters("CaminhoElementoDescricaoCurso1")));
                if (elementoDescricao != null)
                {
                    dados.descricao = elementoDescricao.Text;
                }
                else if (elementoDescricao1 != null)
                {
                    dados.descricao = elementoDescricao1.Text;
                }

                // Em casos que a automação nao consegue achar a descrições
                if (dados.descricao == null)
                {
                    Utilities.LOG(1, $"Não foi possivel localiar a descrição do curso");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao capturar a descrição do curso {ex}");
                return -1;
            }
        }

        // Metodo para validação apos realizar a consulta do curso
        private static int VALIDA_RESULTADO_CONSULTA(IWebDriver driver)
        {
            try
            {
                var resultadoConsulta = Utilities.FindElemests(driver, By.XPath(Utilities.GetParameters("CaminhoElementoResultadosBusca")));
                var textoVisivel = resultadoConsulta.Displayed; //true para elementos visiveis na pagina / false para elementos escondidos

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
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao consultar resultados {ex}");
                return -1;
            }
        }
    }
}
