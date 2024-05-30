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

            ret = CONSULTA_CURSO();

            return 0;
        }

        private static int CONSULTA_CURSO() // metodo para consultar no site da alura
        {
            int ret = 0;

            var driver = Utilities.AbreChrome(); //abre chrome com selenium
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt32(Utilities.GetParameters("Wait")))); // variavel para daley com 30 segundos

            driver.Navigate().GoToUrl(Utilities.GetParameters("urlSiteAlura")); // abrir site da alura, capturando o link do app config

            ret = Utilities.VerificaAcesso(driver, Utilities.GetParameters("CaminhoElementoVerificacao"), wait);
            if (ret != 0) return -1; // em caso negativo, finaliza o processo

            return 0;
        }
    }
}
