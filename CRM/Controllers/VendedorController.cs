using CRM.Models;
using System.Linq;
using System.Web.Mvc;
using CRM.Conexion;
using CRM.sp_dinamic;
using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity;
namespace CRM.Controllers
{
    public class VendedorController : Controller
    {
        private conexion CD = new conexion();
        public JsonResult modifict_user(string user, string id, string tip)
        {

            try
            {
                BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                JArray user_array = JArray.Parse(user);
                decimal ids = Convert.ToDecimal(id);
                CRM_Usuarios_Rol crm_user = BD.CRM_Usuarios_Rol.Where(x => x.ID == ids).First();
                User u = new User();
                string codigo = Convert.ToString(Session["codigo"] != null ? Session["codigo"] : " ");
                string TIPO = Convert.ToString(Session["identificador"] != null ? Session["identificador"] : " ");

                foreach (JObject i in user_array)
                {

                    crm_user.nombreUser = i.GetValue("nombre").ToString();
                    crm_user.alias = i.GetValue("usuario").ToString();
                    crm_user.email = i.GetValue("email").ToString();

                    if (i.GetValue("password").ToString().Count() > 0)
                    {
                        crm_user.contrase = u.CODIFICA(i.GetValue("password").ToString(), 5);
                    }
                    if (i.GetValue("rol").ToString().Count() > 0)
                    {
                        crm_user.Rol = crm_user.Rol = i.GetValue("rol").ToString();
                    }

                    if (i.GetValue("codigo_empresa").ToString().Count() > 0)
                    {
                        crm_user.codigoEmpresa = i.GetValue("codigo_empresa").ToString();
                    }

                }

                BD.Entry(crm_user).State = EntityState.Modified;
                BD.SaveChanges();

                var jss = new System.Web.Script.Serialization.JavaScriptSerializer();

                if (tip == "1")
                {
                    Session["user_logueado"] = crm_user;
                    var jsonData = new
                    {
                        error = false,
                        user = crm_user,
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var jsonData = new
                    {
                        error = false,
                        mensaje = "Usuario modificado con exito",
                        users = TIPO == "RP" ? jss.Serialize(BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa })) : jss.Serialize(BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Where(x => x.codigoEmpresa == codigo).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa }))
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }


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

        public ActionResult save_user(string user)
        {

            try
            {
                BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                JArray user_array = JArray.Parse(user);
                CRM_Usuarios_Rol crm_user = new CRM_Usuarios_Rol();
                User u = new User();
                foreach (JObject i in user_array)
                {
                    crm_user.nombreUser = i.GetValue("nombre").ToString();
                    crm_user.Rol = i.GetValue("rol").ToString();
                    crm_user.alias = i.GetValue("usuario").ToString();
                    crm_user.email = i.GetValue("email").ToString();
                    crm_user.contrase = u.CODIFICA(i.GetValue("password").ToString(), 5);
                    crm_user.codigoEmpresa = i.GetValue("codigo_empresa").ToString();
                }
                BD.CRM_Usuarios_Rol.Add(crm_user);
                BD.SaveChanges();
                var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                string codigo = Convert.ToString(Session["codigo"] != null ? Session["codigo"] : " ");
                string TIPO = Convert.ToString(Session["identificador"] != null ? Session["identificador"] : " ");

                var jsonData = new
                {
                    error = false,
                    mensaje = "El usuario se ha guardado con exito",
                    users = TIPO=="RP"? jss.Serialize(BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa })) : jss.Serialize(BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Where(x => x.codigoEmpresa == codigo).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa }))
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

        public ActionResult eliminar_usuario(decimal id)
        {
            try
            {
                BDWENCOEntities BD = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                BD.Database.ExecuteSqlCommand("Delete CRM_Usuarios_Rol where ID = {0}", new object[] { id });
                var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                string codigo = Convert.ToString(Session["codigo"] != null ? Session["codigo"] : " ");
                string TIPO = Convert.ToString(Session["identificador"] != null ? Session["identificador"] : " ");

                var jsonData = new
                {
                    error = false,
                    inesperado = false,
                    mensaje = "El registro se ha eliminado ",
                    users = TIPO == "RP" ? jss.Serialize(BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa })) : jss.Serialize(BD.CRM_Usuarios_Rol.OrderByDescending(x => x.ID).Where(x => x.codigoEmpresa == codigo).Select(x => new { x.ID, x.Rol, x.nombreUser, x.alias, x.email, x.codigoEmpresa }))
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
