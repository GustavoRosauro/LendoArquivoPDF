using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class ArquivoController : Controller
    {
        // GET: Arquivo
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Exibidados()
        {
            var lista = retornaArquivo(0);
            return View(lista);
        }
        public ActionResult SalvaArquvivo(HttpPostedFileBase arquivo)
        {
            byte[] data;

            using (Stream inputStream = arquivo.InputStream)
            {
                MemoryStream memoryStream = arquivo.InputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
                InserirArquivo(arquivo.FileName,data);
                return RedirectToAction("Exibidados");
            }
        }
        public void InserirArquivo(string nome, byte[]arquivo)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=USUARIO-PC\;Initial Catalog=C#;Integrated Security=True");
            conn.Open();
            string query = @"INSERT INTO [dbo].[Salvar_Arquivo]
                              ([NomeArquivo]
                              ,[Arquivo])
                              VALUES(@nome,@arquivo)";
            SqlCommand cmd = new SqlCommand(query,conn);
            cmd.Parameters.AddWithValue("@nome",nome);
            cmd.Parameters.AddWithValue("@arquivo",arquivo);
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                throw;
            }
        }
        public List<arquivo> retornaArquivo(int id)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=USUARIO-PC\;Initial Catalog=C#;Integrated Security=True");
            conn.Open();
            List<arquivo> arquivos = new List<arquivo>();
            string query = @"SELECT [Id]
                            ,[NomeArquivo]
                            ,[Arquivo]
                        FROM [dbo].[Salvar_Arquivo]";
            if (id != 0)
            {
                query += "WHERE Id = "+id+"";
            }
            SqlCommand cmd = new SqlCommand(query,conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    arquivo arquivo = new arquivo();
                    arquivo.id = Convert.ToInt32(reader["Id"]);
                    arquivo.nome = reader["NomeArquivo"].ToString();
                    arquivo.arquivos =  (byte[])(reader["Arquivo"]);
                    arquivos.Add(arquivo);
                }
            }
                return arquivos;
        }
        public FileResult MostraArquivo(int id)
        {
            var lista = retornaArquivo(id).First();
            MemoryStream ms = new MemoryStream(lista.arquivos);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=labtest.pdf");
            Response.Buffer = true;
            ms.WriteTo(Response.OutputStream);
            Response.End();
            return new FileStreamResult(ms, "application/pdf");
        }
        }
    }