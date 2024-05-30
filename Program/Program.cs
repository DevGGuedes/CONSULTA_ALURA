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

using System;
using System.IO;
using CONSULTA_ALURA; // Importação projeto com classes proprias

namespace Program
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Para rodar no EXE - em caso de local deixar 
                args = new string[] { "0", "-log" }; //item forçado para testes. Não utilizar em modo comum de execução

                CHECA_INTEGRIDADE_PASTAS();

                string returned_arg = VALIDA_ARGUMENTOS_EXE(args);

                switch (returned_arg)
                {
                    case "0":
                        string nomeCurso = VALIDA_ENTRADA_DADOS();
                        break;
                }

            }
            catch (Exception ex)
            {
                Utilities.LOG(3000, ex.ToString());
            }
        }

        private static string VALIDA_ENTRADA_DADOS()
        {
            string nomeCurso = string.Empty;

            while (string.IsNullOrEmpty(nomeCurso))
            {
                Console.WriteLine("\nInforme o nome do Curso desejado: ");
                nomeCurso = Console.ReadLine();
            }

            return nomeCurso;
        }

        //Valida argumentos passados a este EXE
        private static string VALIDA_ARGUMENTOS_EXE(string[] args)
        {
            string returned_arg = "-1";

            //Cenário onde nenhum argumento foi fornecido
            if (args.Length == 0)
            {
                string arg_msg_standard = Utilities.GetParameters("arg_msg_standard");
                Console.WriteLine(arg_msg_standard);
                Utilities.LOG(1, arg_msg_standard);

                Environment.Exit(1);
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {

                    switch (args[i].ToLower())
                    {

                        case "-h": //help
                            string arg_msg_help = Utilities.GetParameters("arg_msg_help");
                            Console.WriteLine(arg_msg_help);
                            Environment.Exit(1);
                            break;
                        case "-log": //gera arquivo de log
                            Console.WriteLine(">> LOG habilitado");
                            Utilities.FILE_LOG = true;
                            Utilities.LOG(0, "");
                            break;
                        case "-ver": //exibe versão atual do código
                            Console.WriteLine(Utilities.GetParameters("ver"));
                            Environment.Exit(1);
                            break;
                        default:
                            if (i == 0)
                            {
                                returned_arg = args[i];
                            }
                            break;
                    }
                }
            }

            return returned_arg;
        }

        //Metodo criado para valiudação e criação de pastas para uso do sistema
        private static void CHECA_INTEGRIDADE_PASTAS()
        {
            string[] sPath = new string[]   { "CaminhoLog" };
            string subPath;

            for (int i = 0; i < sPath.Length; i++)
            {
                subPath = Utilities.GetParameters(sPath[i]);

                bool exists = Directory.Exists(subPath);

                if (!exists)
                    Directory.CreateDirectory(subPath);
            }
        }
    }
}
