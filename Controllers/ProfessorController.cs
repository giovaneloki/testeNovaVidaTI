using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TesteNovaVidaTI.Models;
using TesteNovaVidaTI.ORM;

namespace TesteNovaVidaTI.Controllers
{
    public class ProfessorController : Controller
    {
        // GET: Professor
        DB db = new DB();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Professor professor)
        {
            string msg = "";
            try
            {
                db.CadastrarProfessor(professor);
                msg = "Cadastro realizado com sucesso!";
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { message = msg, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}