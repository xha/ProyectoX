
using System.Linq;
using System.Web.Mvc;
using CRM.Conexion;
using CRM.sp_dinamic;

namespace CRM.Controllers
{
    public class RPController : Controller
    {
        private conexion CD = new conexion();
        // GET: RP
        public ActionResult Usuarios()
        {
            try {
                BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                ViewBag.user = "active";
                var usuario = BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa });

                ViewBag.Usuarios = usuario;
                return View();
            } catch {
                return View();

            }
      
        }

    }
}
