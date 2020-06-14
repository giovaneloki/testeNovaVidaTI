using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TesteNovaVidaTI.ORM;
using TesteNovaVidaTI.Models;
using System.IO;

namespace TesteNovaVidaTI.Controllers
{
    public class AlunoController : Controller
    {
        // GET: Aluno
        DB db = new DB();
        public ActionResult Index(int IdProfessor)
        {
            ViewBag.Mensagem = "";
            List<Aluno> alunos = new List<Aluno>();
            try
            {
                alunos = db.ListarAlunos(IdProfessor);
                ViewBag.NomeProfessor = db.ListarProfessores().Where(c => c.IdProfessor == IdProfessor).First().Nome;
                ViewBag.IdProfessor = IdProfessor;
            }
            catch (Exception ex)
            {
                ViewBag.Mensagem = ex.Message;
            }
            return View(alunos);
        }

        [HttpPost]
        public ActionResult RemoverAluno(int IdAluno)
        {
            string msg = "";
            try
            {
                db.RemoverAluno(IdAluno);
                msg = "Aluno Removido";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Importacao(int IdProfessor)
        {
            string msg = "";

            try
            {
                var arquivo = Request.Files["arquivo"];
                if (!arquivo.ContentType.Equals("text/plain"))
                    throw new Exception("Tipo de arquivo não suportado para importação.\nTente importando um arquivo .txt no layout:\nNomeAluno||ValorMensalidade||DataVencimento");

                if (!db.PermiteImportacao(IdProfessor))
                {
                    throw new Exception("Foi feito uma importação para o professor recentemente.\nTente novamente após " + db.DataUltimaImportacao(IdProfessor).AddSeconds(db._tempoEsperaImportacao).ToString("dd/MM/yyyy HH:mm:ss"));
                }
                StreamReader reader = new StreamReader(arquivo.InputStream);
                List<Aluno> importacao = new List<Aluno>();

                string line = "";
                int contLinha = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.Equals("NomeAluno||ValorMensalidade||DataVencimento"))
                    {
                        contLinha++;

                        try
                        {
                            var split = line.Replace("||", "|").Split('|');

                            importacao.Add(new Aluno()
                            {
                                IdProfessor = IdProfessor,
                                Nome = split[0].Trim(),
                                Mensalidade = Convert.ToDouble(split[1].Trim()),
                                DataVencimento = Convert.ToDateTime(split[2].Trim())
                            });
                        }
                        catch { /*next*/}
                    }
                }

                if (importacao.Count == 0)
                    throw new Exception("Não foi possivel importar o arquivo, pois o mesmo não contém dados ou está fora do layout.");

                if (db.ImportarAlunos(importacao))
                    db.ImportarArquivo(IdProfessor, arquivo.FileName, arquivo.ContentLength, contLinha, importacao.Count);

                msg = "Importação realizada, " + importacao.Count + " aluno(s) incluídos.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}