using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CredentialManagement;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CONSULTA_ALURA
{
    public class Utilities
    {
        public static bool FILE_LOG { get; set; } // variavel para controle de escrita de LOG

        public static IWebDriver AbreChrome()
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");

                return new ChromeDriver(options);
            }
            catch (Exception e)
            {
                LOG(3000, "Erro ao abrir chrome");
                return null;
            }
        }

        public static int VerificaAcesso(IWebDriver driver, string objHtml, WebDriverWait wait)
        {
            //espera o elemento ficar acessivel para manipular
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath(objHtml)));

            var textLogin = driver.FindElement(By.XPath(objHtml)).Text;
            if (textLogin != null)
            {
                LOG(1, "Sucesso ao acessar o site");
            }
            else
            {
                LOG(3000, "Não foi possivel capturar o texto apos acessar o site");
                return -1;
            }
            
            return 0;
        }

        //Metodo para verificasr se Chrome existe na maquina
        public static int VerificaChrome()
        {
            var Caminhoexiste = File.Exists(GetParameters("CaminhoExeChrome"));
            var CaminhoExeChorme86 = File.Exists(GetParameters("CaminhoExeChrome86"));
            LOG(4);

            if (Caminhoexiste || CaminhoExeChorme86)
            {
                LOG(1, "Executavel Chrome existe");
            }
            else
            {
                LOG(1, "Executavel Chrome não existe");
                LOG(3000, "Executavel não Chrome existe");
                return -1;
            }
            
            return 0;
        }

        //Metodo para escrita de log e console
        public static void LOG(int msg_code, string msg = null, int mode = 0)
        {
            string line = "";

            if (FILE_LOG == true)
            {
                switch (msg_code)
                {
                    case 0:
                        line = "==============================================================================" + '\n';
                        line = line + $">> LOG habilitado" + '\n';
                        line = line + $"Início: {DateTime.Now}" + '\n';
                        line = line + $"Argumentos: {Environment.CommandLine}" + '\n';
                        line = line + $"Nome da Máquina: {Environment.MachineName}" + '\n';
                        line = line + $"Nome do Usuário: {Environment.UserName}" + '\n';
                        line = line + $"Sistema Operacional: {Environment.OSVersion}" + '\n';
                        line = line + $"Versão da Automação: {GetParameters("ver_short")}" + '\n';
                        line = line + "==============================================================================" + '\n';
                        break;
                    case 1:
                        Console.WriteLine($"[{DateTime.Now}] {msg}");
                        line = $"[{DateTime.Now}] " + msg + '\n';
                        break;
                    case 2:
                        line = $"[{DateTime.Now}] Iniciou Chrome \n";
                        break;

                    case 3000:
                        Console.WriteLine($"[{DateTime.Now}] EXCEÇÃO DETECTADA {msg}");
                        line = $"[{DateTime.Now}] EXCEÇÃO DETECTADA: {msg} " + '\n';
                        break;
                    case 3001:
                        line = $"[{DateTime.Now}] Ocorreu um erro na Verificação do Chrome {msg} \n";
                        break;
                    default: break;
                }

                if (line.Trim() != "")
                {
                    EscreveArquivoLog(line);
                }
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now}] {msg}");
            }
        }

        private static void EscreveArquivoLog(string line_str)
        {
            var CaminhoLog = GetParameters("CaminhoLog");

            CaminhoLog += $"Log_{DateTime.Now.ToString("ddMMyyyy")}.txt";

            if (File.Exists(CaminhoLog))
            {
                StreamWriter w = File.AppendText(CaminhoLog);
                w.WriteAsync(line_str);
                w.Close();
                w.Dispose();
            }
            else
            {
                StreamWriter sw = File.CreateText(CaminhoLog);
                sw.WriteAsync(line_str);
                sw.Close();
                sw.Dispose();
            }
        }

        // Recupera parâmetros do arquivo App.config
        public static String GetParameters(string Parametro)
        {
            string retorno_param = ConfigurationManager.AppSettings[Parametro];
            return retorno_param;
        }
    }
}
