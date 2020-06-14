using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TesteNovaVidaTI.ORM;
using TesteNovaVidaTI.Models;

namespace TesteNovaVidaTI.Controllers
{
    public class HomeController : Controller
    {
        DB db = new DB();
        public ActionResult Index()
        {
            List<Professor> professores = new List<Professor>();
            try
            {
                professores = db.ListarProfessores();
            }
            catch(Exception e)
            {
                ViewBag.Mensagem = e.Message;
            }

            return View(professores);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}