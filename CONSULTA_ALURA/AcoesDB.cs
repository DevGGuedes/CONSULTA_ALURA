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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CONSULTA_ALURA
{
    public class AcoesDB
    {
        private static string msg;
        private static MySqlConnection cn = new MySqlConnection("Server=localhost;DataBase=ALURA;User=root;pwd=pacoca10");

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

        private static MySqlConnection MyConectarBD()
        {

            try
            {
                cn.Open();
            }
            catch (Exception erro)
            {
                msg = "Ocorreu um erro ao se conectar" + erro.Message;
            }
            return cn;
        }

        private static MySqlConnection MyDesconectarBD()
        {

            try
            {
                cn.Close();
            }

            catch (Exception erro)
            {
                msg = "Ocorreu um erro ao se conectar" + erro.Message;
            }
            return cn;
        }
    }
}
