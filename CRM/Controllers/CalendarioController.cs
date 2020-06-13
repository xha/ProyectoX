using CRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using CRM.Conexion;
using CRM.sp_dinamic;

namespace CRM.Controllers
{
    public class CalendarioController : Controller
    {
        private conexion CD = new conexion();

        [HttpGet]
        public ActionResult Index(string propuestas)
        {
            ViewBag.vcalendario = "active";
            ViewBag.usuario_actual = Session["alias"].ToString();
            ViewBag.rol = Session["rol"].ToString();
            ViewBag.Actividades = ListarActividades("");
            ViewBag.ListarUsuarios = ListarUsuarios();
            ViewBag.parametro = propuestas;
            return View();
        }

        [HttpGet]
        public JsonResult ListarDatos()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;
            var tipoActividad = db.TIPO_ACTIVIDAD.Where(m => m.estado == true).OrderBy(m => m.descripcion).ToList();
            string rol = Session["rol"].ToString();
            string usuario = Session["alias"].ToString();

            if (rol == "ADMINISTRADOR")
            {
                var propuesta = db.PROSPECTO.SqlQuery("select p.id_oportunidad,p.nombre,p.ingreso,p.id_estadoOporunidad,p.idCliente,p.id_prioridad,p.color,p.id_usuario,p.cierre_previsto,p.id_categoria,p.id_planificacion,p.tipoCliente,p.codigo_tipoMon,p.porcentajeGanar,(case when p.tipoCliente = 1 then i.CNOMCLI else pt.CNOMCLI end) as 'notas',p.id_estatusProspecto, p.fecha_inicial, p.fecha_final, p.monto_logrado from PROSPECTO p left join MAECLI i on p.idCliente = i.CCODCLI left join POTENCIALCLI pt on p.idCliente = pt.CCODCLI").ToList();
                var jsonData = new
                {
                    tipoActividad,
                    propuesta,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var propuesta = db.PROSPECTO.SqlQuery("select p.id_oportunidad,p.nombre,p.ingreso,p.id_estadoOporunidad,p.idCliente,p.id_prioridad,p.color,p.id_usuario,p.cierre_previsto,p.id_categoria,p.id_planificacion,p.tipoCliente,p.codigo_tipoMon,p.porcentajeGanar,(case when p.tipoCliente = 1 then i.CNOMCLI else pt.CNOMCLI end) as 'notas',p.id_estatusProspecto, p.fecha_inicial, p.fecha_final, p.monto_logrado from PROSPECTO p left join MAECLI i on p.idCliente = i.CCODCLI left join POTENCIALCLI pt on p.idCliente = pt.CCODCLI WHERE id_usuario='" + usuario + "'").ToList();
                var jsonData = new
                {
                    tipoActividad,
                    propuesta,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ListadoEspecial()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;
            string rol = Session["rol"].ToString();
            string usuario = Session["alias"].ToString();
            var anio = DateTime.Now.ToString("yyyy");
            DateTime fecha_desde = Convert.ToDateTime("01/01/" + anio);
            DateTime fecha_hasta = Convert.ToDateTime("31/12/" + anio);

            var propuesta = db.PROSPECTO.SqlQuery("select p.id_oportunidad,p.nombre,p.ingreso,p.id_estadoOporunidad,p.idCliente,p.id_prioridad,p.color,p.id_usuario,p.cierre_previsto,p.id_categoria,p.id_planificacion,p.tipoCliente,p.codigo_tipoMon,p.porcentajeGanar,(case when p.tipoCliente = 1 then i.CNOMCLI else pt.CNOMCLI end) as 'notas',p.id_estatusProspecto, p.fecha_inicial, p.fecha_final, p.monto_logrado from PROSPECTO p left join MAECLI i on p.idCliente = i.CCODCLI left join POTENCIALCLI pt on p.idCliente = pt.CCODCLI").ToList();
            var tipoActividad = db.TIPO_ACTIVIDAD.Where(m => m.estado == true).OrderBy(m => m.descripcion).ToList();
            var cronograma = db.CRONOGRAMA.Where(m => m.fecha_inicial >= fecha_desde && m.fecha_final <= fecha_hasta).ToList();

            var res = from e1 in cronograma
                      join e2 in tipoActividad on e1.id_tipoActividad equals e2.id_tipoActividad
                      join e3 in propuesta on e1.id_oportunidad equals e3.id_oportunidad
                      select new
                      {
                          e1.id_cronograma,
                          e1.observacion,
                          e1.id_tipoActividad,
                          e1.descripcion,
                          e1.id_oportunidad,
                          e1.fecha_inicial,
                          e1.fecha_final,
                          e1.estatus,
                          e1.USU_CODIGO,
                          e1.respuesta,
                          Tipo_Actividad = e2.descripcion,
                          Color_actividad = e2.color,
                          Propuesta = e3.nombre,
                          e3.ingreso,
                          e3.id_estadoOporunidad,
                          e3.idCliente,
                          e3.id_prioridad,
                          e3.cierre_previsto,
                          e3.id_categoria,
                          e3.id_planificacion,
                          e3.tipoCliente,
                          e3.codigo_tipoMon,
                          e3.porcentajeGanar,
                          Nombre_Cliente = e3.notas,
                          e3.id_estatusProspecto,
                          FechaI_Prospecto = e3.fecha_inicial,
                          FechaF_Prospecto = e3.fecha_final,
                          e3.monto_logrado,
                          Nombre_Oportunidad = e3.nombre
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarUsuarios()
        {
            BDWENCOEntities db = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;
            string empresa_actual = Session["codigo"].ToString();
            List<CRM_Usuarios_Rol> listado = db.CRM_Usuarios_Rol.Where(m => m.codigoEmpresa == empresa_actual).OrderBy(m => m.alias).ToList();
            var Resultado = (from N in listado
                             select new { N.ID, N.alias, N.nombreUser, N.Rol });

            return Json(Resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarActividades(string user)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            string rol = Session["rol"].ToString();
            db.Configuration.ProxyCreationEnabled = false;
            var anio = DateTime.Now.ToString("yyyy");
            string usuario = "";
            if (user == "")
            {
                usuario = Session["alias"].ToString();
            } else
            {
                usuario = user;
                rol = "";
            }
            DateTime fecha_desde = Convert.ToDateTime("01/01/" + anio);
            DateTime fecha_hasta = Convert.ToDateTime("31/12/" + anio);

            db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");
            if (rol == "ADMINISTRADOR")
            {
                var actividad = db.CRONOGRAMA.Where(m => m.fecha_inicial >= fecha_desde && m.fecha_final <= fecha_hasta && m.estatus < 3).ToList();
                return Json(actividad, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var actividad = db.CRONOGRAMA.Where(m => m.USU_CODIGO == usuario && m.fecha_inicial >= fecha_desde && m.fecha_final <= fecha_hasta && m.estatus < 3).ToList();
                return Json(actividad, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Guardar(int id_cronograma, int tipo_actividad, string actividad, DateTime fecha_desde, DateTime fecha_hasta, string observacion, string propuestas, int estatus, string usuario, string respuesta)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var jsonData = new
            {
                resultado = true,
                id = 0,
                error = "",
            };

            try
            {
                bool bandera = ValidaActividad(fecha_desde, fecha_hasta, usuario, id_cronograma);
                bandera = true;
                if (bandera)
                {
                    CRONOGRAMA Modelo = new CRONOGRAMA();
                    PROSPECTO ModeloProspecto = db.PROSPECTO.Find(propuestas);
                    Modelo.id_cronograma = id_cronograma;
                    Modelo.id_oportunidad = propuestas;
                    Modelo.id_tipoActividad = tipo_actividad;
                    if (observacion == null) observacion = " ";
                    Modelo.observacion = observacion;
                    Modelo.descripcion = actividad;
                    Modelo.fecha_inicial = fecha_desde;
                    Modelo.fecha_final = fecha_hasta;
                    Modelo.estatus = estatus;
                    Modelo.respuesta = respuesta;
                    Modelo.USU_CODIGO = usuario;
                    
                    ModeloProspecto.id_usuario = usuario;
                    db.Entry(ModeloProspecto).State = EntityState.Modified;
                    db.CRONOGRAMA.Add(Modelo);

                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");

                    jsonData = new
                    {
                        resultado = true,
                        id = Modelo.id_cronograma,
                        error = "",
                    };
                }
                else
                {
                    jsonData = new
                    {
                        resultado = false,
                        id = 0,
                        error = "Fecha comprometida para el usuario " + Session["alias"].ToString(),
                    };
                }

                return Json(jsonData);
            }
            catch (Exception e)
            {
                jsonData = new
                {
                    resultado = false,
                    id = 0,
                    error = "No se pudo grabar la actividad",

                };

                return Json(jsonData);
            }
        }

        public bool ValidaActividad(DateTime fecha_desde, DateTime fecha_hasta, string usuario, int id_cronograma)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var bandera = false;

            try
            {
                int contador = db.CRONOGRAMA.Where(m => m.USU_CODIGO == usuario && m.id_cronograma != id_cronograma && m.estatus < 3 && ((m.fecha_inicial >= fecha_desde && m.fecha_inicial <= fecha_hasta) || (m.fecha_final >= fecha_desde && m.fecha_inicial <= fecha_hasta))).Count();

                if (contador == 0) bandera = true;
            }
            catch (Exception e)
            {
                bandera = false;
            }

            return bandera;
        }

        [HttpPost]
        public JsonResult Eliminar(int id_cronograma)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var jsonData = new
            {
                resultado = true,
                id = 0,
                error = "",
                calendarios = Json("")
            };

            try
            {
                CRONOGRAMA crono = db.CRONOGRAMA.Find(id_cronograma);
                /*db.CRONOGRAMA.Remove(crono);
                db.SaveChanges();*/
                crono.estatus = 3;

                db.Entry(crono).State = EntityState.Modified;
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");

                jsonData = new
                {
                    resultado = true,
                    id = 0,
                    error = "",
                    calendarios = ListarActividades("")
                };

                return Json(jsonData);
            }
            catch (Exception e)
            {
                jsonData = new
                {
                    resultado = false,
                    id = 0,
                    error = "No se pudo grabar la actividad",
                    calendarios = Json("")
                };

                return Json(jsonData);
            }
        }

        [HttpPost]
        public JsonResult Actualizar(int id_cronograma, int tipo_actividad, string actividad, DateTime fecha_desde, DateTime fecha_hasta, string observacion, string propuestas, int estatus, string usuario, string respuesta)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;
            var jsonData = new
            {
                resultado = true,
                id = 0,
                error = "",
                calendarios = Json("")
            };

            try
            {
                bool bandera = ValidaActividad(fecha_desde, fecha_hasta, usuario, id_cronograma);
                bandera = true;
                if (bandera)
                {
                    CRONOGRAMA Modelo = db.CRONOGRAMA.Find(id_cronograma);
                    PROSPECTO ModeloProspecto = db.PROSPECTO.Find(propuestas);
                    Modelo.id_cronograma = id_cronograma;
                    Modelo.id_oportunidad = propuestas;
                    Modelo.id_tipoActividad = tipo_actividad;
                    if (observacion == null) observacion = " ";
                    Modelo.observacion = observacion;
                    Modelo.descripcion = actividad;
                    Modelo.fecha_inicial = fecha_desde;
                    Modelo.fecha_final = fecha_hasta;
                    Modelo.estatus = estatus;
                    if (usuario != "0")
                    {
                        Modelo.USU_CODIGO = usuario;
                        ModeloProspecto.id_usuario = usuario;
                        db.Entry(ModeloProspecto).State = EntityState.Modified;
                    }
                    
                    Modelo.respuesta = respuesta;

                    db.Entry(Modelo).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");

                    jsonData = new
                    {
                        resultado = true,
                        id = Modelo.id_cronograma,
                        error = "",
                        calendarios = ListarActividades("")
                    };
                }
                else
                {
                    jsonData = new
                    {
                        resultado = false,
                        id = 0,
                        error = "Fecha comprometida para el usuario " + Session["alias"].ToString(),
                        calendarios = Json("")
                    };
                }

                return Json(jsonData);
            }
            catch (Exception e)
            {
                jsonData = new
                {
                    resultado = false,
                    id = 0,
                    error = "No se pudo grabar la actividad",
                    calendarios = Json("")
                };

                return Json(jsonData);
            }
        }

        public JsonResult cambiarEstatusCronograma(int id_cronograma, string id_oportunidad, string tipo, string respuesta, string usuario)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var jsonData = new
            {
                Cambio = true,
            };
            try
            {
                CRONOGRAMA cRONOGRAMA = db.CRONOGRAMA.Find(id_cronograma);

                cRONOGRAMA.estatus = 1;
                cRONOGRAMA.respuesta = respuesta;
                if(usuario != null && usuario != "")
                {
                cRONOGRAMA.USU_CODIGO = usuario;
                }

                // Crear el Historico de la Oportunidad
                HISTORICO Historico = new HISTORICO();

                Historico.id_oportunidad = id_oportunidad;
                Historico.fecha = @DateTime.Now;
                Historico.titulo = "Completó la actividad: " + tipo + "";
                Historico.descripcion = "";
                Historico.id_usuario = Convert.ToString(Session["alias"]);


                if (ModelState.IsValid)
                {
                    db.HISTORICO.Add(Historico);
                    db.Entry(cRONOGRAMA).State = EntityState.Modified;
                    db.SaveChanges();
                    jsonData = new
                    {
                        Cambio = true,
                    };
                }



            }

            catch (Exception)
            {
                jsonData = new
                {
                    Cambio = false,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult editarAsignadoCronograma(int id_cronograma, string id_oportunidad, string asignado, string respuesta)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var jsonData = new
            {
                Cambio = true,
            };
            try
            {
                CRONOGRAMA cRONOGRAMA = db.CRONOGRAMA.Find(id_cronograma);

                cRONOGRAMA.USU_CODIGO = asignado;
                cRONOGRAMA.respuesta = respuesta;
                // Crear el Historico de la Oportunidad
                HISTORICO Historico = new HISTORICO();

                Historico.id_oportunidad = id_oportunidad;
                Historico.fecha = @DateTime.Now;
                Historico.titulo = "la actividad: " + cRONOGRAMA.TIPO_ACTIVIDAD.descripcion + "";
                Historico.descripcion = "se reasigno a: " + asignado + "";
                Historico.id_usuario = Convert.ToString(Session["alias"]);


                if (ModelState.IsValid)
                {
                    db.HISTORICO.Add(Historico);
                    db.Entry(cRONOGRAMA).State = EntityState.Modified;
                    db.SaveChanges();
                    jsonData = new
                    {
                        Cambio = true,
                    };
                }



            }

            catch (Exception)
            {
                jsonData = new
                {
                    Cambio = false,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


    }
}