using CRM.Models;
using System.Linq;
using System.Web.Mvc;
using CRM.Conexion;
using CRM.sp_dinamic;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace CRM.Controllers
{
    public class AdministradorController : Controller
    {
        private conexion CD = new conexion();
        // GET: Administrador
        public ActionResult Index()
        {
            try {

                BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                ViewBag.oportunidades = (from num in db.PROSPECTO select num).Count();
                ViewBag.clientes = (from num in db.MAECLI select num).Count();
                ViewBag.potencial_cliente = (from num in db.POTENCIALCLI select num).Count();
                string codigo = Session["codigo"].ToString();
                ViewBag.vendedores = BD.CRM_Usuarios_Rol.Where(x => x.codigoEmpresa == codigo).Count();
                int cron_retraso = (from num in db.CRONOGRAMA where num.estatus == 2 select num).Count();
                if (cron_retraso < 1) cron_retraso = 0;
                int cron_pendiente = (from num in db.CRONOGRAMA where num.estatus == 0 select num).Count();
                if (cron_pendiente < 1) cron_pendiente = 0;
                decimal cron_sumn = Convert.ToDecimal((from num in db.PROSPECTO where num.id_estatusProspecto == 1 && num.codigo_tipoMon == "MN" select num.ingreso).Sum());
                decimal cron_sume = Convert.ToDecimal((from num in db.PROSPECTO where num.id_estatusProspecto == 1 && num.codigo_tipoMon == "ME" select num.ingreso).Sum());
                if (cron_sumn < 1) cron_sumn = 0;
                if (cron_sume < 1) cron_sume = 0;
                ViewBag.cron_retraso = cron_retraso;
                ViewBag.cron_pendiente = cron_pendiente;
                ViewBag.cron_sume = cron_sume;
                ViewBag.cron_sumn = cron_sumn;
                ViewBag.flujo = "active";
                ViewBag.s = Session.Timeout;
                ViewBag.ListadoCronograma = ListarDatosReportes();
                return View();
            } catch {
                return View();
            }          
        }
     
       
        public ActionResult Usuarios()
        {
            BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.user = "active";
            string codigo = Session["codigo"].ToString();         
            var usuario =  BD.CRM_Usuarios_Rol.OrderByDescending(x =>x.ID).Where(x =>x.codigoEmpresa == codigo).Select( x=> new { x.ID, x.Rol, x.nombreUser, x.alias, x.email,x.codigoEmpresa });
       
            ViewBag.Usuarios = usuario;
            return View();

        }
        public ActionResult Clientes() {
            BDWENCOEntities bd = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            Entidades BD = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.cliente = "active";
            ViewBag.open = "menu-open";
            string pattern = @"\[\\``"".,\s]";
            string replacement = "";
            string input = "$16.32 12.19 £16.29 €18.29  €18,29";
            string result = Regex.Replace(input, pattern, replacement);
          
            var cliente2 = BD.MAECLI.OrderByDescending(x => x.CCODCLI).Select(x => new { x.CCODCLI, x.CNOMCLI, x.CDIRCLI, x.CTELEFO, x.CNUMRUC, x.CEMAIL }).ToList();
            var cliente = (from e in cliente2
                           select new
                                {
                                    CNOMCLI = Regex.Replace(e.CNOMCLI, pattern, replacement),
                               e.CCODCLI,
                               e.CDIRCLI,
                               e.CTELEFO,
                               e.CEMAIL,
                               e.CNUMRUC
                  

                           }).ToList();

            ViewBag.Usuarios= cliente;
            return View();
        }


        public ActionResult ClientesPotenciales ()
        {

            Entidades BD = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.clientePotenciales = "active";
            ViewBag.open = "menu-open";
            ViewBag.UsuariosPotenciales = BD.CONTACTO_POTENCIAL.OrderByDescending(x => x.COD_CLIENTE).Select(x => new { x.COD_CLIENTE, x.CONTACTO, x.TELEFONO, x.CORREO, x.CARGO});
            return View();
        }
        public ActionResult Prospecto()
        {

            Entidades BD = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.Oportunidad = "active";
            ViewBag.open = "menu-open";
            var p = BD.POTENCIALCLI.OrderByDescending(x => x.CCODCLI).Select(x => new { x.CNOMCLI, x.CDIRCLI, x.CTELEFO });
            ViewBag.Oportunidades = p;
            return View();
        }
       


        public ActionResult Actualizar(string codigo)
        {
            BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            SP_EMPRESA_USUARIO_CRM_Result empresa = BD.SP_EMPRESA_USUARIO_CRM().Where(x => x.EMP_CODIGO == codigo).FirstOrDefault();
            if (Session["identificador"].ToString() == "CRM") {
                string alias = Session["alias"].ToString();
                CRM_Usuarios_Rol usuario = BD.CRM_Usuarios_Rol.Where(x => x.codigoEmpresa == codigo).Where(x => x.alias == alias).First();
                Session["nombre_admin"] = usuario.nombreUser;
            }
            Session["codigo"] = empresa.EMP_CODIGO;
            Session["empresa"] = empresa;
            Session["catalog_user"] = codigo + "BDCOMUN";
            return RedirectToAction("Index", "Administrador");
        }

        public JsonResult ListarDatosReportes()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var anio = DateTime.Now.ToString("yyyy");
            string fecha_inicial = anio+"0101";
            string fecha_final = anio+"1231";

            string sql = "select estatus 'Id', CASE  WHEN estatus = 0 THEN 'Pendiente' WHEN estatus = 1 THEN 'Cerrado' WHEN estatus=2 THEN 'Caducado' ELSE 'Eliminado' END as 'Descripcion',CAST(count(estatus) AS numeric) 'Total' from CRONOGRAMA WHERE fecha_inicial between '" + fecha_inicial + "' and '" + fecha_final + "' GROUP BY estatus";

            var listado = db.Database.SqlQuery<ListadoReporte>(sql).ToList();

            return Json(listado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TipoActividad() {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.tipoActividad = db.TIPO_ACTIVIDAD.OrderByDescending(x => x.id_tipoActividad).Select(x => new { x.id_tipoActividad, x.color, x.descripcion }).ToList();
            ViewBag.openAts = "menu-open";
            ViewBag.actividades = "active";
            return View();
        }

        public ActionResult planificadas()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.openAts = "menu-open";
            ViewBag.planificadas = "active";
            
            return View();
        }

        public ActionResult save_tipoActividad(string tipoActividad) {

            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                JArray actividad_array = JArray.Parse(tipoActividad);
                TIPO_ACTIVIDAD TipoActividad = new TIPO_ACTIVIDAD();
                foreach (JObject i in actividad_array)
                {
                    TipoActividad.descripcion = i.GetValue("descripcion").ToString();
                    TipoActividad.color = i.GetValue("color").ToString();
                    TipoActividad.estado = Convert.ToBoolean(1);
                }
                db.TIPO_ACTIVIDAD.Add(TipoActividad);
                db.SaveChanges();
                var jss = new System.Web.Script.Serialization.JavaScriptSerializer();

                var jsonData = new
                {
                    error = false,
                    mensaje = "El usuario se ha guardado con exito",
                    users = jss.Serialize(db.TIPO_ACTIVIDAD.OrderByDescending(x => x.id_tipoActividad).Select(x => new { x.id_tipoActividad, x.color, x.descripcion}))
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception e)
            {
                var jsonData = new
                {
                    error = true,
                    mensaje = "La actividad ingresada ya existe"
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult modificar_tipoActividad(string tipoActividad, string id)
        {

            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                JArray actividad_array = JArray.Parse(tipoActividad);
                decimal ids = Convert.ToDecimal(id);
                TIPO_ACTIVIDAD TipoActividad = db.TIPO_ACTIVIDAD.Where(x =>x.id_tipoActividad == ids).First();
                foreach (JObject i in actividad_array)
                {
                    TipoActividad.descripcion = i.GetValue("descripcion").ToString();
                    TipoActividad.color = i.GetValue("color").ToString();
                    TipoActividad.estado = Convert.ToBoolean(1);
                }
                db.Entry(TipoActividad).State = EntityState.Modified;
                db.SaveChanges();

                var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonData = new
                    {
                        error = false,
                        mensaje = "Tipo de actividad modificada con exito",
                        users = jss.Serialize(db.TIPO_ACTIVIDAD.OrderByDescending(x => x.id_tipoActividad).Select(x => new { x.id_tipoActividad, x.color, x.descripcion }))

                };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
             


            }
            catch (Exception e)
            {
                var jsonData = new
                {
                    error = true,
                    mensaje = "Ha ocurrido un error" + e.Message
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult eliminar_actividad(decimal id)
        {
            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                db.Database.ExecuteSqlCommand("Delete TIPO_ACTIVIDAD where id_tipoActividad = {0}", new object[] { id });
                var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jsonData = new
                {
                    error = false,
                    inesperado = false,
                    mensaje = "El registro se ha eliminado ",
                    users = jss.Serialize(db.TIPO_ACTIVIDAD.OrderByDescending(x => x.id_tipoActividad).Select(x => new { x.id_tipoActividad, x.color, x.descripcion }))
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var jsonData = new
                {
                    error = true,
                    mensaje = "Ha ocurrido un error" + e.Message
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
