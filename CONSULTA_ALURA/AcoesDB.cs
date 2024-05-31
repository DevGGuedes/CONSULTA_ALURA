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

//Classe para realizar ações com banco de dados

using MySql.Data.MySqlClient;
using System;

namespace CONSULTA_ALURA
{
    public class AcoesDB
    {
        private static MySqlConnection cn = new MySqlConnection($"Server={Utilities.GetParameters("server")};DataBase={Utilities.GetParameters("dataBase")};User={Utilities.GetParameters("user")};pwd={Utilities.GetParameters("pwd")}");

        // Metodo para inserir os registros no banco, recebendo os dados da classe DadosCurso
        public static void DB_INSERE_DADOS(DadosCurso dados)
        {
            try
            {
                MySqlCommand my = new MySqlCommand("INSERT INTO DADOS_CURSO (TITULO_CURSO, PROFESSOR_CURSO, CARGA_HORARIA_CURSO, DESCRICAO_CURSO) " +
                    "values (@nomeCurso, @nomeInstrutor, @cargaHoraria, @descricao)", MyConectarBD());

                my.Parameters.Add("@nomeCurso", MySqlDbType.VarChar).Value = dados.nomeCurso;
                my.Parameters.Add("@nomeInstrutor", MySqlDbType.VarChar).Value = dados.nomeInstrutor;
                my.Parameters.Add("@cargaHoraria", MySqlDbType.VarChar).Value = dados.cargaHoraria;
                my.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = dados.descricao;

                my.ExecuteNonQuery();
                MyDesconectarBD();

                Utilities.LOG(1, $"Dado inserido com sucesso!");

            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Erro ao inserir registro no Bando de Dados {ex}");
            }
        }

        // Metodo para conectar com o Mysql
        private static MySqlConnection MyConectarBD()
        {

            try
            {
                cn.Open();
            }
            catch (Exception ex)
            {
                Utilities.LOG(1, $"Ocorreu um erro ao se conectar {ex}");
            }
            return cn;
        }

        // Metodo para desconectar com o Mysql
        private static MySqlConnection MyDesconectarBD()
        {

            try
            {
                cn.Close();
            }

            catch (Exception ex)
            {
                Utilities.LOG(1, $"Ocorreu um erro ao se desconectar {ex}");
            }
            return cn;
        }
    }
}
