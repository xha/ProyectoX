
using CRM.Controllers;
using CRM.Models;
using System.Web;
using System.Web.Mvc;

namespace CRM.filtros
{
    public class autenticacion : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            var oUser = (User)HttpContext.Current.Session["cliente"];
            if (oUser == null)
            {
                if (filterContext.Controller is AutentificacionController == false)
                {
                    filterContext.HttpContext.Response.Redirect("~/Autentificacion/Login");

                }
            }
            else
            {
                if (oUser.tipo == "RP")
                {
                    if (filterContext.Controller is AutentificacionController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/RP/Usuarios");

                    }
                    if (filterContext.Controller is AdministradorController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/RP/Usuarios");

                    }
                    if (filterContext.Controller is PROSPECTOController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/RP/Usuarios");

                    }
                    if (filterContext.Controller is FlujoController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/RP/Usuarios");

                    }

                }
                else if (oUser.tipo == "CRM")
                {
                    if (filterContext.Controller is AutentificacionController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/Administrador/Index");

                    }

                }
                else {

                    if (filterContext.Controller is AutentificacionController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/Flujo/Index");

                    }
                    if (filterContext.Controller is AdministradorController == true)
                    {
                        filterContext.HttpContext.Response.Redirect("~/Flujo/Index");

                    }
                }
           
           

            }
            base.OnActionExecuting(filterContext);
        }
    }
   
}