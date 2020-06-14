using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.IO;
using TesteNovaVidaTI.Models;

namespace TesteNovaVidaTI.ORM
{
    public class DB
    {
        private string _strConn;
        public int _tempoEsperaImportacao;
        public DB()
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(HttpContext.Current.Server.MapPath("~"), "appsettings.json")));

            _strConn = config["dbConnection"]["default"].ToString();
            _tempoEsperaImportacao = (int)config["appSettings"]["tempoEsperaImportacao"];
        }

        public List<Professor> ListarProfessores()
        {
            try
            {
                List<Professor> professores = new List<Professor>();

                using (SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();

                    professores = conn.Query<Professor>("dbo.sp_ListarProfessores", null, null, false, 60, CommandType.StoredProcedure).ToList();

                    conn.Close();
                }

                return professores;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel retornar lista");
            }
        }

        public List<Aluno> ListarAlunos(int IdProfessor)
        {
            try
            {
                List<Aluno> alunos = new List<Aluno>();

                using (SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("IdProfessor", IdProfessor);

                    alunos = conn.Query<Aluno>("dbo.sp_ListarAlunos", param, null, false, 60, CommandType.StoredProcedure).ToList();

                    conn.Close();
                }

                return alunos;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel retornar lista");
            }
        }

        public void CadastrarProfessor(Professor professor)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();
                    using(SqlCommand command = new SqlCommand("dbo.spAddProfessor", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Nome", professor.Nome);

                        if (command.ExecuteNonQuery() < 1)
                            throw new Exception("Falha ao cadastrar");
                    }
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel cadastrar");
            }
        }

        public bool PermiteImportacao(int IdProfessor)
        {
            try
            {
                bool bPermite = false;
                using(SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();
                    using(SqlCommand command = new SqlCommand("dbo.sp_PermiteImportacao", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Tempo", _tempoEsperaImportacao);
                        command.Parameters.AddWithValue("@IdProfessor", IdProfessor);

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                            bPermite = Convert.ToBoolean(reader[0]);

                    }
                    conn.Close();
                }
                return bPermite;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel consultar ultima importação");
            }
        }

        public DateTime DataUltimaImportacao(int IdProfessor)
        {
            try
            {
                DateTime ultima = DateTime.MinValue;
                using (SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand("dbo.sp_DataUltimaImportacao", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdProfessor", IdProfessor);

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                            ultima = Convert.ToDateTime(reader[0]);

                    }
                    conn.Close();
                }
                return ultima;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel consultar ultima importação");
            }
        }

        public bool ImportarAlunos(List<Aluno> alunos)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();

                    var dt = new DataTable();

                    using (var adapter = new SqlDataAdapter(string.Format("SELECT TOP 0 * FROM {0}", "dbo.Aluno"), conn))
                    {
                        adapter.Fill(dt);
                    };

                    foreach(var aluno in alunos)
                    {
                        var row = dt.NewRow();

                        row["IdProfessor"] = aluno.IdProfessor;
                        row["Nome"] = aluno.Nome;
                        row["Mensalidade"] = aluno.Mensalidade;
                        row["DataVencimento"] = aluno.DataVencimento;

                        dt.Rows.Add(row);
                    }

                    using (var transaction = conn.BeginTransaction())
                    {
                        using (var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulk.DestinationTableName = "dbo.Aluno";
                            bulk.BatchSize = dt.Rows.Count;
                            bulk.BulkCopyTimeout = 0;
                            dt.TableName = "dbo.Aluno";
                            try
                            {
                                bulk.WriteToServer(dt);
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                    conn.Close();

                    return true;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel importar os alunos");
            }
        }

        public void ImportarArquivo(int IdProfessor, string NomeArquivo, int Tamanho, int QtdLinhas, int QtdImportado)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();
                    using(SqlCommand command = new SqlCommand("dbo.sp_AddArquivoImportacao", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdProfessor", IdProfessor);
                        command.Parameters.AddWithValue("@NomeArquivo", NomeArquivo);
                        command.Parameters.AddWithValue("@Tamanho", Tamanho);
                        command.Parameters.AddWithValue("@QtdLinhas", QtdLinhas);
                        command.Parameters.AddWithValue("@QtdImportado", QtdImportado);

                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel registrar o arquivo de importação");
            }
        }

        public void RemoverAluno(int IdAluno)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_strConn))
                {
                    conn.Open();
                    using(SqlCommand command = new SqlCommand("dbo.sp_RemoverAluno", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@IdAluno", IdAluno);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2812)
                    throw new Exception("Erro interno do servidor. Por favor, entre em contato com o Administrador do Sistema");
                else
                    throw new Exception("Não foi possivel remover o aluno");
            }
        }
    }
}