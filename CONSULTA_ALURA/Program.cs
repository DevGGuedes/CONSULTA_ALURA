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
using System.Security.Cryptography.X509Certificates;

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

            return 0;
        }

        private static int CONSULTA_CURSO(string nomeCurso) // metodo para consultar no site da alura
        {
            int ret = 0;

            //abre chrome
            var driver = Utilities.AbreChrome(); //abre chrome com selenium
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt32(Utilities.GetParameters("Wait")))); // variavel para daley com 30 segundos

            // abrir site da alura, capturando o link do app config
            driver.Navigate().GoToUrl(Utilities.GetParameters("urlSiteAlura"));

            //Validação acesso ao site + validação campo busca
            ret = Utilities.VerificaAcesso(driver, Utilities.GetParameters("idCampoBuscaCurso"), wait);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            //buscar curso
            Utilities.EnviaValor(driver, By.Id(Utilities.GetParameters("idCampoBuscaCurso")), nomeCurso);

            //Envia click para busca
            Utilities.EnviaClick(driver, By.XPath(Utilities.GetParameters("CaminhoElementoBtnBusca")));

            //Esperar para a proxima pagina ser carregada com os resultados
            Utilities.WaitForElement(driver, By.Id(Utilities.GetParameters("idCampoBuscaResultados")));

            driver.Quit();

            return 0;
        }
    }
}
