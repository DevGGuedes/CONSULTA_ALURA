using System;
using System.Configuration;
using System.IO;
using CredentialManagement;

namespace CONSULTA_ALURA
{
    public class Utilities
    {
        public static bool FILE_LOG { get; set; } // variavel para controle de escrita de LOG

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
