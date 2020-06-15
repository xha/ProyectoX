using CRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CRM.Conexion;
using System.Web.Services;
using Newtonsoft.Json;
using CRM.sp_dinamic;

namespace CRM.Controllers
{

    public class FlujoController : Controller
    {
        private conexion CD = new conexion();

        //Clientes Fidelizados de MAECLI
        public void cargarClientesFi()
        {
            List<POTENCIALCLI> data = new List<POTENCIALCLI>();
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            ViewBag.ListaClientesFi = db.POTENCIALCLI.Select(m => new { m.CCODCLI, m.CTIPO_DOCUMENTO, m.CDOCIDEN, m.CAPELLIDO_PATERNO, m.CAPELLIDO_MATERNO, m.CPRIMER_NOMBRE, m.CNOMCLI, m.CTELEFO, m.CEMAIL, m.CHOST, m.TCL_CODIGO, m.CDIRCLI, m.CPROV, m.CPAIS, m.CDISTRI }).ToList();

            var clientes = (from m in db.POTENCIALCLI
                            select new { m.CCODCLI, m.CNOMCLI, m.CTELEFO, m.CEMAIL }).ToList();
            foreach (var s in clientes)
            {
                data.Add(new POTENCIALCLI() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI, CTELEFO = s.CTELEFO, CEMAIL = s.CEMAIL });
            }
            ViewBag.ListaClientesFi2 = data;
        }

        [HttpGet]
        public JsonResult cargarPotencial()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;

            var potencial = db.POTENCIALCLI.Select(m => new { m.CCODCLI, m.CNOMCLI }).ToList();

            var datos = new
            {
                potencial,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult cargarFidelizado()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;

            var fidelizado = db.POTENCIALCLI.Select(m => new { m.CCODCLI, m.CNOMCLI }).ToList();

            var datos = new
            {
                fidelizado,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult cargarEstado()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;

            var estados = db.ESTADO_PROSPECTO.Select(m => new { m.id_estadoOportunidad, m.decripcion }).ToList();

            var datos = new
            {
                estados,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult cargarMoneda()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;

            var moneda = db.TIPO_MONEDA.Select(m => new { m.TIPOMON_CODIGO, m.TIPOMON_SIMBOLO }).ToList();

            var datos = new
            {
                moneda,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult cargarDistrito()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;

            var distrito = db.TABAYU.Where(p => p.TCOD == "13").Select(m => new { m.TCLAVE, m.TDESCRI }).ToList();

            var datos = new
            {
                distrito,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult cargarHistorico(string id)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;

            var historico = db.HISTORICO.Where(p => p.id_oportunidad == id).OrderByDescending(p => p.id_historico).Select(m => new { m }).ToList();

            var datos = new
            {
                historico,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult cargarActividad(int id_cronograma, string id_oportunidad, int tipo)
        {

            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");
            PROSPECTO pROSPECTO = db.PROSPECTO.Find(id_oportunidad);
            CRONOGRAMA cRONOGRAMA = db.CRONOGRAMA.Find(id_cronograma);
            var inicial = cRONOGRAMA.fecha_inicial.ToString("dd/MM/yyyy");
            var final = cRONOGRAMA.fecha_final.ToString("dd/MM/yyyy");
            var usuarios = ListarUsuarios();

            if (pROSPECTO.tipoCliente == 0)
            {
                var actividad = db.CRONOGRAMA.Where(m => m.id_cronograma == id_cronograma && m.id_oportunidad == id_oportunidad).Select(m => new { Id_cronograma = m.id_cronograma, Id_oportunidad = m.id_oportunidad, fecha_inicio = m.fecha_inicial, fecha_final = m.fecha_final, complemento = m.descripcion, Titulo = m.observacion, Tipo_Actividad = m.TIPO_ACTIVIDAD.descripcion, respuesta = m.respuesta, usuarioAsig = m.USU_CODIGO, estatus = m.estatus}).ToList();
                var cliente = db.POTENCIALCLI.Where(m => m.CCODCLI == pROSPECTO.idCliente).Select(m => new { nombreCliente = m.CNOMCLI, telefono = m.CTELEFO, correo = m.CEMAIL }).ToList();
                var datos = new
                {
                    usuarios,
                    inicial,
                    final,
                    actividad,
                    cliente,
                };

                return Json(datos, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var actividad = db.CRONOGRAMA.Where(m => m.id_cronograma == id_cronograma && m.id_oportunidad == id_oportunidad).Select(m => new { Id_cronograma = m.id_cronograma, Id_oportunidad = m.id_oportunidad, fecha_inicio = m.fecha_inicial, fecha_final = m.fecha_final, complemento = m.descripcion, Titulo = m.observacion, Tipo_Actividad = m.TIPO_ACTIVIDAD.descripcion, respuesta = m.respuesta }).ToList();
                var cliente = db.POTENCIALCLI.Where(m => m.CCODCLI == pROSPECTO.idCliente).Select(m => new { nombreCliente = m.CNOMCLI, telefono = m.CTELEFO, correo = m.CEMAIL }).ToList();
                var datos = new
                {
                    usuarios,
                    inicial,
                    final,
                    actividad,
                    cliente,
                };

                return Json(datos, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult devuelveCalendario()
        {
            return RedirectToAction("Index", "Calendario");
        }

        //Nuevo Contacto Para clientes Fidelizados
        public void nuevoContactoFi([Bind(Include = "COD_CLIENTE,COD_CONTACTO,CONTACTO,TELEFONO,CORREO,AREA,CARGO,CELULAR")] CONTACTO_VENTA cONTACTO_VENTA)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            if (ModelState.IsValid)
            {
                var cONTACTO = from datos in db.CONTACTO_VENTA where datos.COD_CLIENTE == cONTACTO_VENTA.COD_CLIENTE select datos;

                int valor = cONTACTO.Count();
                int codigo = 0;
                if (valor == 0)
                {
                    codigo = 0;
                    codigo = (codigo + 1);
                }
                else
                {
                    int ultOp = int.Parse(db.CONTACTO_VENTA
                    .Where(p => p.COD_CLIENTE == cONTACTO_VENTA.COD_CLIENTE)
                   .OrderByDescending(p => p.COD_CONTACTO)
                   .Select(r => r.COD_CONTACTO)
                   .First().ToString());

                    codigo = Convert.ToInt32(ultOp);
                    codigo = (codigo + 1);
                }

                cONTACTO_VENTA.COD_CONTACTO = codigo;

                db.CONTACTO_VENTA.Add(cONTACTO_VENTA);
                db.SaveChanges();

            }
        }

        public void editarContactoFi([Bind(Include = "COD_CLIENTE,COD_CONTACTO,CONTACTO,TELEFONO,CORREO,AREA,CARGO,CELULAR")] CONTACTO_VENTA cONTACTO_VENTA)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));


            if (ModelState.IsValid)
            {


                db.Entry(cONTACTO_VENTA).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        public void EliminarContactoFi(string COD_CLIENTE, decimal COD_CONTACTO)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            CONTACTO_VENTA Contacto = db.CONTACTO_VENTA.Find(COD_CLIENTE, COD_CONTACTO);
            db.CONTACTO_VENTA.Remove(Contacto);
            db.SaveChanges();



        }


        //Clientes Fidelizados de POTENCIALCLI
        public JsonResult editarFidelizado(string CCODCLI, string tipoDoc, string CDOCIDEN, string CAPELLIDO_PATERNO, string CAPELLIDO_MATERNO, string CPRIMER_NOMBRE, string CSEGUNDO_NOMBRE, string CNOMCLI, string CTELEFO, string CEMAIL, string CHOST, string tipoEmpresa, string CDIRCLI, string CPROV, string CPAIS, string distrito)
        {


            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

                POTENCIALCLI mAECLI = db.POTENCIALCLI.Find(CCODCLI);

                mAECLI.CCODCLI = CCODCLI;
                mAECLI.CTIPO_DOCUMENTO = tipoDoc;
                mAECLI.CDOCIDEN = CDOCIDEN;
                mAECLI.CAPELLIDO_PATERNO = CAPELLIDO_PATERNO;
                mAECLI.CAPELLIDO_MATERNO = CAPELLIDO_MATERNO;
                mAECLI.CPRIMER_NOMBRE = CPRIMER_NOMBRE;
                mAECLI.CNOMCLI = CNOMCLI;
                mAECLI.CTELEFO = CTELEFO;
                mAECLI.CEMAIL = CEMAIL;
                mAECLI.CHOST = CHOST;
                mAECLI.TCL_CODIGO = tipoEmpresa;
                mAECLI.CDIRCLI = CDIRCLI;
                mAECLI.CPROV = CPROV;
                mAECLI.CPAIS = CPAIS;
                mAECLI.CDISTRI = distrito;


                if (ModelState.IsValid)
                {

                    db.Entry(mAECLI).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var cONTACTO = from datos in db.CONTACTO_VENTA where datos.COD_CLIENTE == mAECLI.CCODCLI select datos;
                var jsonData = new
                {
                    resultado1 = true,
                    mAECLI.CNOMCLI,
                    mAECLI.CTELEFO,
                    mAECLI.CEMAIL,
                    mAECLI.CCODCLI,
                    cONTACTO,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var jsonData = new
                {

                    resultado1 = false,

                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        }

        //Clientes Potenciales
        public JsonResult editarPot(string CCODCLI, string tipoDoc, string CDOCIDEN, string CAPELLIDO_PATERNO, string CAPELLIDO_MATERNO, string CPRIMER_NOMBRE, string CSEGUNDO_NOMBRE, string CNOMCLI, string CTELEFO, string CEMAIL, string CHOST, string tipoEmpresa, string CDIRCLI, string CPROV, string CPAIS, string distrito)
        {

            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));


                POTENCIALCLI potencial = db.POTENCIALCLI.Find(CCODCLI);

                potencial.CCODCLI = CCODCLI;
                potencial.CTIPO_DOCUMENTO = tipoDoc;
                potencial.CDOCIDEN = CDOCIDEN;
                potencial.CAPELLIDO_PATERNO = CAPELLIDO_PATERNO;
                potencial.CAPELLIDO_MATERNO = CAPELLIDO_MATERNO;
                potencial.CPRIMER_NOMBRE = CPRIMER_NOMBRE;
                potencial.CNOMCLI = CNOMCLI;
                potencial.CTELEFO = CTELEFO;
                potencial.CEMAIL = CEMAIL;
                potencial.CHOST = CHOST;
                potencial.TCL_CODIGO = tipoEmpresa;
                potencial.CDIRCLI = CDIRCLI;
                potencial.CPROV = CPROV;
                potencial.CPAIS = CPAIS;
                potencial.CDISTRI = distrito;


                if (ModelState.IsValid)
                {

                    db.Entry(potencial).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var cONTACTO = from datos in db.CONTACTO_POTENCIAL where datos.COD_CLIENTE == potencial.CCODCLI select datos;

                var jsonData = new
                {
                    resultado1 = true,
                    potencial.CNOMCLI,
                    potencial.CTELEFO,
                    potencial.CEMAIL,
                    potencial.CCODCLI,
                    cONTACTO ,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var jsonData = new
                {
                    resultado1 = false,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

        }


        //Clientes Potenciales
        [HttpPost]
        public JsonResult crearPotencial(string CONTACTO_COBRANZA, string CCODCLAS, string CCODTRANS, string CFLAGPRIN, string CCODCLI, string tipoDoc, string CDOCIDEN, string CAPELLIDO_PATERNO, string CAPELLIDO_MATERNO, string CPRIMER_NOMBRE, string CSEGUNDO_NOMBRE, string CNOMCLI, string CTELEFO, string CEMAIL, string CHOST, string tipoEmpresa, string CDIRCLI, string CPROV, string CPAIS, string distrito, POTENCIALCLI Potencial, List<CONTACTO_POTENCIAL> ListadoContactoPot, CONTACTO_POTENCIAL ContactoPot)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var jsonData = new
            {
                resultado1 = true,
                resultado2 = true,
                id = "",
                error = "",
            };

            try
            {
                POTENCIALCLI prueba = db.POTENCIALCLI.Find(CCODCLI);
                if (prueba == null)
                {
                    Potencial.CFLAGPRIN = Convert.ToBoolean(CFLAGPRIN);
                    Potencial.CONTACTO_COBRANZA = CONTACTO_COBRANZA;
                    Potencial.CCODTRANS = CCODTRANS;
                    Potencial.CCODCLAS = CCODCLAS;
                    Potencial.CCODCLI = CCODCLI;
                    Potencial.CTIPO_DOCUMENTO = tipoDoc;
                    Potencial.CDOCIDEN = CDOCIDEN;
                    Potencial.CAPELLIDO_PATERNO = CAPELLIDO_PATERNO;
                    Potencial.CAPELLIDO_MATERNO = CAPELLIDO_MATERNO;
                    Potencial.CPRIMER_NOMBRE = CPRIMER_NOMBRE;
                    Potencial.CNOMCLI = CNOMCLI;
                    Potencial.CTELEFO = CTELEFO;
                    Potencial.CEMAIL = CEMAIL;
                    Potencial.CHOST = CHOST;
                    Potencial.TCL_CODIGO = tipoEmpresa;
                    Potencial.CDIRCLI = CDIRCLI;
                    Potencial.CPROV = CPROV;
                    Potencial.CPAIS = CPAIS;
                    Potencial.CDISTRI = distrito;

                    if (ModelState.IsValid)
                    {

                        db.POTENCIALCLI.Add(Potencial);
                        db.SaveChanges();
                    }
                    jsonData = new
                    {
                        resultado1 = true,
                        resultado2 = true,
                        id = CCODCLI,
                        error = "",
                    };

                }
                else
                {
                    jsonData = new
                    {
                        resultado1 = false,
                        resultado2 = false,
                        id = "",
                        error = "El codigo de Cliente ya existe",
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                jsonData = new
                {
                    resultado1 = false,
                    resultado2 = false,
                    id = "",
                    error = "No se pudo crear el Cliente",
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (ListadoContactoPot == null)
                {

                }
                else
                {
                    foreach (var data in ListadoContactoPot)
                    {

                        ContactoPot.COD_CLIENTE = CCODCLI;
                        ContactoPot.COD_CONTACTO = Convert.ToDecimal(data.COD_CONTACTO);
                        ContactoPot.AREA = data.AREA;
                        ContactoPot.CARGO = data.CARGO;
                        ContactoPot.CORREO = data.CORREO;
                        ContactoPot.CONTACTO = data.CONTACTO;
                        ContactoPot.CELULAR = data.CELULAR;
                        ContactoPot.TELEFONO = data.TELEFONO;

                        if (ModelState.IsValid)
                        {

                            db.CONTACTO_POTENCIAL.Add(ContactoPot);
                            db.SaveChanges();

                        }

                        ContactoPot = new CONTACTO_POTENCIAL();

                        jsonData = new
                        {
                            resultado1 = true,
                            resultado2 = true,
                            id = CCODCLI,
                            error = "",
                        };

                    }
                }
            }
            catch (Exception)
            {
                jsonData = new
                {
                    resultado1 = true,
                    resultado2 = false,
                    id = "",
                    error = "Hubo un error al crear el contacto",
                };


                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }



            db.Configuration.ProxyCreationEnabled = false;
            var potencial = db.POTENCIALCLI.Select(m => new { m.CCODCLI, m.CNOMCLI }).ToList();

            var datos = new
            {
                potencial,
                jsonData,
            };


            return Json(datos, JsonRequestBehavior.AllowGet);

        }

        //Nuevo Contacto Para clientes Fidelizados
        public void nuevoContactoPot([Bind(Include = "COD_CLIENTE,COD_CONTACTO,CONTACTO,TELEFONO,CORREO,AREA,CARGO,CELULAR")] CONTACTO_POTENCIAL cONTACTO_POTENCIAL)
        {
            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));


                if (ModelState.IsValid)
                {
                    var cONTACTO = from datos in db.CONTACTO_VENTA where datos.COD_CLIENTE == cONTACTO_POTENCIAL.COD_CLIENTE select datos;

                    int valor = cONTACTO.Count();
                    int codigo = 0;
                    if (valor == 0)
                    {
                        codigo = 0;
                        codigo = (codigo + 1);
                    }
                    else
                    {
                        int ultOp = int.Parse(db.CONTACTO_POTENCIAL
                        .Where(p => p.COD_CLIENTE == cONTACTO_POTENCIAL.COD_CLIENTE)
                       .OrderByDescending(p => p.COD_CONTACTO)
                       .Select(r => r.COD_CONTACTO)
                       .First().ToString());

                        codigo = Convert.ToInt32(ultOp);
                        codigo = (codigo + 1);
                    }

                    cONTACTO_POTENCIAL.COD_CONTACTO = codigo;

                    db.CONTACTO_POTENCIAL.Add(cONTACTO_POTENCIAL);
                    db.SaveChanges();

                }
            }
            catch
            {

            }

        }

        public void editarContactoPot([Bind(Include = "COD_CLIENTE,COD_CONTACTO,CONTACTO,TELEFONO,CORREO,AREA,CARGO,CELULAR")] CONTACTO_POTENCIAL cONTACTO_POTENCIAL)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));


            if (ModelState.IsValid)
            {


                db.Entry(cONTACTO_POTENCIAL).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        public void EliminarContactoPot(string COD_CLIENTE, string COD_CONTACTO)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            CONTACTO_POTENCIAL cONTACTO_POTENCIAL = db.CONTACTO_POTENCIAL.Find(COD_CLIENTE, Convert.ToDecimal(COD_CONTACTO));
            db.CONTACTO_POTENCIAL.Remove(cONTACTO_POTENCIAL);
            db.SaveChanges();



        }

        //Clientes Potenciales de POTENCIALCLI
        public void cargarClientesPot()
        {
            
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            var clientes = from m in db.POTENCIALCLI
                           select m;

            List<POTENCIALCLI> data = clientes.ToList();

            ViewBag.ListaClientesPot = data;
        }


        //Clientes Potenciales de POTENCIALCLI
        public void cargarTipoMoneda()
        {
            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

                var TipoMon = from m in db.TIPO_MONEDA
                              select m;

                List<TIPO_MONEDA> data = TipoMon.ToList();

                ViewBag.ListaTipoMoneda = data;
            }
            catch (Exception)
            {
                Session["Errores"] = ("No se encontro la tabla tipo Moneda");
            }

        }


        public JsonResult ListarUsuarios()
        {
            BDWENCOEntities db = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;
            string empresa = Session["codigo"].ToString();

            List<CRM_Usuarios_Rol> listado = db.CRM_Usuarios_Rol.Where(m => m.codigoEmpresa == empresa).OrderBy(m => m.alias).ToList();
            var Resultado = (from N in listado
                             select new { N.ID, N.alias, N.nombreUser, N.Rol });

            return Json(Resultado, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult infoCliente(string cliente, int tipoCli)
        {


            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            if (tipoCli == 0)
            {

                var consultaContactoPot = from datos in db.CONTACTO_POTENCIAL where datos.COD_CLIENTE == cliente select datos;
                List<Potencial_campos> data = new List<Potencial_campos>();

                var clientes = (from m in db.POTENCIALCLI
                                where m.CCODCLI == cliente
                                select new { m.CCODCLI, m.CTIPO_DOCUMENTO, m.CDOCIDEN, m.CAPELLIDO_PATERNO, m.CAPELLIDO_MATERNO, m.CPRIMER_NOMBRE, m.CNOMCLI, m.CTELEFO, m.CEMAIL, m.CHOST, m.TCL_CODIGO, m.CDIRCLI, m.CPROV, m.CPAIS, m.CDISTRI, m.CSEGUNDO_NOMBRE }).ToList();

                foreach (var s in clientes)
                {
                    data.Add(new Potencial_campos() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI, CTELEFO = s.CTELEFO, CEMAIL = s.CEMAIL, CTIPO_DOCUMENTO = s.CTIPO_DOCUMENTO, CDOCIDEN = s.CDOCIDEN, CAPELLIDO_PATERNO = s.CAPELLIDO_PATERNO, CAPELLIDO_MATERNO = s.CAPELLIDO_MATERNO, CPRIMER_NOMBRE = s.CPRIMER_NOMBRE, CHOST = s.CHOST, TCL_CODIGO = s.TCL_CODIGO, CDIRCLI = s.CDIRCLI, CPROV = s.CPROV, CPAIS = s.CPAIS, CDISTRI = s.CDISTRI, CSEGUNDO_NOMBRE = s.CSEGUNDO_NOMBRE });
                }

                List<CONTACTO_POTENCIAL> data2 = consultaContactoPot.ToList();
                ViewBag.Distrito = new SelectList(db.TABAYU.Where(p => p.TCOD == "13"), "TCLAVE", "TDESCRI", data.First().CDISTRI);
                ViewBag.clientePot = data;
                ViewBag.contactoPot = data2;
                ViewBag.tipoCliente = "modal_editarClientePot";


                return PartialView("_Cliente");
            }
            else
            {

                var consultaContactoFi = from datos in db.CONTACTO_VENTA where datos.COD_CLIENTE == cliente select datos;
                List<POTENCIALCLI> data = new List<POTENCIALCLI>();

                var clientes = (from m in db.POTENCIALCLI
                                where m.CCODCLI == cliente
                                select new { m.CCODCLI, m.CTIPO_DOCUMENTO, m.CDOCIDEN, m.CAPELLIDO_PATERNO, m.CAPELLIDO_MATERNO, m.CPRIMER_NOMBRE, m.CNOMCLI, m.CTELEFO, m.CEMAIL, m.CHOST, m.TCL_CODIGO, m.CDIRCLI, m.CPROV, m.CPAIS, m.CDISTRI, m.CSEGUNDO_NOMBRE }).ToList();

                foreach (var s in clientes)
                {
                    data.Add(new POTENCIALCLI() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI, CTELEFO = s.CTELEFO, CEMAIL = s.CEMAIL, CTIPO_DOCUMENTO = s.CTIPO_DOCUMENTO, CDOCIDEN = s.CDOCIDEN, CAPELLIDO_PATERNO = s.CAPELLIDO_PATERNO, CAPELLIDO_MATERNO = s.CAPELLIDO_MATERNO, CPRIMER_NOMBRE = s.CPRIMER_NOMBRE, CHOST = s.CHOST, TCL_CODIGO = s.TCL_CODIGO, CDIRCLI = s.CDIRCLI, CPROV = s.CPROV, CPAIS = s.CPAIS, CDISTRI = s.CDISTRI, CSEGUNDO_NOMBRE = s.CSEGUNDO_NOMBRE });
                }

                List<CONTACTO_VENTA> data2 = consultaContactoFi.ToList();
                ViewBag.Distrito = new SelectList(db.TABAYU.Where(p => p.TCOD == "13"), "TCLAVE", "TDESCRI", data.First().CDISTRI);
                ViewBag.clienteFi = data;
                ViewBag.contactoFi = data2;
                ViewBag.tipoCliente = "modal_editarClienteFi";


                return PartialView("_Cliente");
            }


        }


        public JsonResult cambiarEstado(int estadoInicial, int estadoFinal, string idOportunidad)
        {
            var jsonData = new
            {
                Cambio = true,
            };
            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));


                if (estadoInicial == estadoFinal)
                {
                    jsonData = new
                    {
                        Cambio = false,
                    };
                }
                else
                {


                    PROSPECTO pROSPECTO = db.PROSPECTO.Find(idOportunidad);

                    pROSPECTO.id_estadoOporunidad = estadoFinal;


                    ESTADO_PROSPECTO eSTADO = db.ESTADO_PROSPECTO.Find(estadoFinal);

                    // Crear el Historico de la Oportunidad
                    HISTORICO Historico = new HISTORICO();

                    Historico.id_oportunidad = pROSPECTO.id_oportunidad;
                    Historico.fecha = @DateTime.Now;
                    Historico.titulo = "Cambió el prospecto al estado: " + eSTADO.decripcion + "";
                    Historico.descripcion = "";
                    Historico.id_usuario = Convert.ToString(Session["alias"]);



                    if (ModelState.IsValid)
                    {
                        db.HISTORICO.Add(Historico);
                        db.Entry(pROSPECTO).State = EntityState.Modified;
                        db.SaveChanges();
                        jsonData = new
                        {
                            Cambio = true,
                        };
                    }
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

        public JsonResult cambiarColorOp(string color, string idOportunidad)
        {

            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            var jsonData = new
            {
                Cambio = true,
            };


            try
            {

                PROSPECTO pROSPECTO = db.PROSPECTO.Find(idOportunidad);

                pROSPECTO.color = color;

                if (ModelState.IsValid)
                {
                    db.Entry(pROSPECTO).State = EntityState.Modified;
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

        public JsonResult cambiarPosicion(int posicionInicial, int posicionFinal, int idEstadoInicial, int idEstadoFinal)
        {
            var jsonData = new
            {
                resultado = true,

            };
            try
            {

                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

                ESTADO_PROSPECTO eSTADO_INICIAL = db.ESTADO_PROSPECTO.Find(idEstadoInicial);
                ESTADO_PROSPECTO eSTADO_FINAL = db.ESTADO_PROSPECTO.Find(idEstadoFinal);

                if (eSTADO_INICIAL.id_estadoOportunidad == eSTADO_FINAL.id_estadoOportunidad)
                {

                    throw new ApplicationException();
                }
                else
                {
                    eSTADO_INICIAL.posicion = posicionFinal;
                    eSTADO_FINAL.posicion = posicionInicial;
                }


                // Crear el Historico de la Oportunidad
                HISTORICO Historico = new HISTORICO();

                Historico.id_EstadoProspecto = eSTADO_INICIAL.id_estadoOportunidad;
                Historico.fecha = @DateTime.Now;
                Historico.titulo = "Cambiando la posicion del estado:";
                Historico.descripcion = "";
                Historico.id_usuario = "001";
                if (ModelState.IsValid)
                {
                    db.Entry(eSTADO_INICIAL).State = EntityState.Modified;
                    db.Entry(eSTADO_FINAL).State = EntityState.Modified;
                    db.HISTORICO.Add(Historico);
                    db.SaveChanges();

                }
            }
            catch (Exception)
            {
                jsonData = new
                {
                    resultado = false,

                };


                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }

            jsonData = new
            {
                resultado = true,

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int? Estatus, int? tipoCliente, int? id_prioridad, string usuario)
        {
            try {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");


                ViewBag.Distrito = new SelectList(db.TABAYU.Where(p => p.TCOD == "13"), "TCLAVE", "TDESCRI");

                cargarClientesFi();
                cargarClientesPot();
                cargarTipoMoneda();

                var dato = from s in db.ESTADO_PROSPECTO select s;

                var oportunidades = from s in db.PROSPECTO select s;
                if (Estatus != null)
                {
                    oportunidades = oportunidades.Where(m => m.id_estatusProspecto == Estatus);
                    dato = dato.Where(m => m.PROSPECTO == oportunidades);
                }
                if (tipoCliente != null)
                {
                    oportunidades = oportunidades.Where(m => m.tipoCliente == tipoCliente);
                    dato = dato.Where(m => m.PROSPECTO == oportunidades);
                }
                if (id_prioridad != null)
                {


                    oportunidades = oportunidades.Where(m => m.id_prioridad == id_prioridad);
                    dato = dato.Where(m => m.PROSPECTO == oportunidades);

                }
                if (usuario != null)
                {


                    oportunidades = oportunidades.Where(m => m.id_usuario == usuario);
                    dato = dato.Where(m => m.PROSPECTO == oportunidades);

                }


                ViewBag.estados = dato.ToList();
                return View(db.ESTADO_PROSPECTO.ToList());

            }
            catch {

                return RedirectToAction("Login", "Autentificacion");
            }
        }

        public ActionResult Lista(int? Estatus, int? tipoCliente, int? id_prioridad, string usuario)
        {
            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");
                var estados = from m in db.ESTADO_PROSPECTO.OrderBy(p => p.posicion)
                              select m;

                List<ESTADO_PROSPECTO> data = estados.ToList();


                var oportunidades = from s in db.PROSPECTO select s;

                ViewBag.estados = data;
                cargarClientesFi();
                cargarClientesPot();
                cargarTipoMoneda();
                ViewBag.Distrito = new SelectList(db.TABAYU.Where(p => p.TCOD == "13"), "TCLAVE", "TDESCRI");
                if (Estatus != null)
                {
                    oportunidades = oportunidades.Where(m => m.id_estatusProspecto == Estatus);
                }
                if (tipoCliente != null)
                {
                    oportunidades = oportunidades.Where(m => m.tipoCliente == tipoCliente);
                }
                if (id_prioridad != null)
                {


                        oportunidades = oportunidades.Where(m => m.id_prioridad == id_prioridad);
                    
                }
                if (usuario != null)
                {


                    oportunidades = oportunidades.Where(m => m.id_usuario == usuario);

                }

                return View(oportunidades.ToList());
            }

            catch
            {
                ViewBag.Error("error no se encontraron las tablas");
                return RedirectToAction("Login", "Autentificacion");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult nuevoProspecto(int idEstatus, int idEstadoOp, string idUsuario,int probabilidad, string nombreOp, string clientePot, string clienteFi, int prioridadVal, string ingreso, string tipoMoneda, string vista, int tipContacto)
        {
         
            try
            {
   Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            ESTADO_PROSPECTO estado = db.ESTADO_PROSPECTO.Find(idEstadoOp);
            string nombreEstado = estado.decripcion;
                //buscar el ultimo registro del codigo de la tabla OPORTUNIDAD
                
                if(ingreso == "")
                {
                    ingreso = "0.00";
                }
                ingreso = ingreso.Replace(",", ".");
                decimal nvo_ingreso = Convert.ToDecimal(ingreso);

                var oport = from m in db.PROSPECTO
                            select m;
                int valor = oport.Count();
                int codigo1 = 0;
                if (valor == 0)
                {
                    codigo1 = 0;
                    codigo1 = (codigo1 + 1);
                }
                else
                {
                    int ultOp = int.Parse(db.PROSPECTO
                                   .OrderByDescending(p => p.id_oportunidad)
                                   .Select(r => r.id_oportunidad)
                                   .First().ToString());

                    codigo1 = Convert.ToInt32(ultOp);
                    codigo1 = (codigo1 + 1);
                }

                PROSPECTO oPORTUNIDAD = new PROSPECTO();
                if (codigo1 < 10)
                {
                    oPORTUNIDAD.id_oportunidad = "0000" + codigo1.ToString();
                }
                if (codigo1 < 100 && codigo1 > 9)
                {
                    oPORTUNIDAD.id_oportunidad = "000" + codigo1.ToString();
                }
                if (codigo1 < 1000 && codigo1 > 99)
                {
                    oPORTUNIDAD.id_oportunidad = "00" + codigo1.ToString();
                }
                if (codigo1 < 10000 && codigo1 > 999)
                {
                    oPORTUNIDAD.id_oportunidad = "0" + codigo1.ToString();
                }
                
                oPORTUNIDAD.id_estatusProspecto = idEstatus;
                oPORTUNIDAD.nombre = nombreOp;
                oPORTUNIDAD.porcentajeGanar = probabilidad;
                oPORTUNIDAD.codigo_tipoMon = tipoMoneda;
                oPORTUNIDAD.ingreso = nvo_ingreso;
                oPORTUNIDAD.tipoCliente = tipContacto;
                oPORTUNIDAD.fecha_inicial = @DateTime.Now;

                if (tipContacto == 1)
                {
                    oPORTUNIDAD.idCliente = Convert.ToString(clienteFi);
                }
                else
                {
                    oPORTUNIDAD.idCliente = Convert.ToString(clientePot);
                }

                oPORTUNIDAD.id_estadoOporunidad = idEstadoOp;

                if (prioridadVal == 0)
                {
                    oPORTUNIDAD.id_prioridad = 0;
                }
                else
                {
                    oPORTUNIDAD.id_prioridad = prioridadVal;
                }
                oPORTUNIDAD.id_usuario = idUsuario;

                // Crear el Historico de la Oportunidad
                HISTORICO Historico = new HISTORICO();

                Historico.id_oportunidad = oPORTUNIDAD.id_oportunidad;
                Historico.fecha = @DateTime.Now;
                Historico.titulo = "Añadio el prospecto a " + nombreEstado + "";
                Historico.id_usuario = idUsuario;
                Historico.id_estatusOp = idEstatus;

                ViewBag.CodEmpresa = "";


                if (ModelState.IsValid)
                {

                    db.HISTORICO.Add(Historico);

                    db.PROSPECTO.Add(oPORTUNIDAD);

                    db.SaveChanges();
                    return RedirectToAction(vista);

                }
            }
            catch
            {

                return View();
            }
            return RedirectToAction(vista);
        }

        [HttpPost]
        public ActionResult CrearEmpresa(string nombre, string celular, string telefono, string correo, string pagina)
        {
            if (nombre != null && celular != null && telefono != null && correo != null && pagina != null)
            {/*
                VentasCN cnper = new VentasCN();
                List<Ventas> per = cnper.UltimoEmp();
                foreach (Ventas ma in per)
                {
                    int codigo = 0;
                    codigo = Convert.ToInt32(ma.Codigo);
                    codigo = codigo + 1;
                    if (codigo < 10)
                    {
                        ma.Codigo = "000" + codigo.ToString();
                    }
                    if (codigo < 100 && codigo > 9)
                    {
                        ma.Codigo = "00" + codigo.ToString();
                    }
                    if (codigo < 1000 && codigo > 99)
                    {
                        ma.Codigo = "0" + codigo.ToString();
                    }
                    txtCodigo.Text = ma.Codigo;
                }*/
                ViewBag.CodEmpresa = "";
                return View();
            }
            return View();
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
        /****************************************************************************************************************************************************/
        //REPORTES
        /****************************************************************************************************************************************************/
        //PIRAMIDE
        [HttpGet]
        public ActionResult RepPiramide(string fecha_inicial, string fecha_final)
        {
            DateTime fechatemp = DateTime.Today;
            ViewBag.vreppiramide = "active";
            if (fecha_final == null)
            {
                fecha_inicial = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month, 1));
                fecha_final = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1));
            }

            fecha_inicial = FechaUniversal(fecha_inicial);
            fecha_final = FechaUniversal(fecha_final);
            ViewBag.ListadoMN = ListarDatosReportes(1, "MN", fecha_inicial, fecha_final);
            ViewBag.ListadoME = ListarDatosReportes(1, "ME", fecha_inicial, fecha_final);
            ViewBag.fecha_inicial = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month, 1));
            ViewBag.fecha_final = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1));

            return View();
        }

        //ESTATUS
        [HttpGet]
        public ActionResult RepEstatus(string fecha_inicial, string fecha_final)
        {
            ViewBag.vrepestatus = "active";
            DateTime fechatemp = DateTime.Today;
            if (fecha_final == null)
            {
                fecha_inicial = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month, 1));
                fecha_final = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1));
            }

            fecha_inicial = FechaUniversal(fecha_inicial);
            fecha_final = FechaUniversal(fecha_final);
            ViewBag.ListadoEP = ListarDatosReportes(2, "", fecha_inicial, fecha_final);
            ViewBag.fecha_inicial = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month, 1));
            ViewBag.fecha_final = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1));

            return View();
        }

        //GANADO
        [HttpGet]
        public ActionResult RepGanado(string fecha_inicial, string fecha_final)
        {
            ViewBag.vrepganado = "active";
            DateTime fechatemp = DateTime.Today;
            if (fecha_final == null)
            {
                fecha_inicial = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month, 1));
                fecha_final = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1));
            }
            fecha_inicial = FechaUniversal(fecha_inicial);
            fecha_final = FechaUniversal(fecha_final);

            ViewBag.ListadoGMN = ListarDatosReportes(3, "MN", fecha_inicial, fecha_final);
            ViewBag.ListadoDMN = ListarDatosReportes(4, "MN", fecha_inicial, fecha_final);
            ViewBag.ListadoGME = ListarDatosReportes(3, "ME", fecha_inicial, fecha_final);
            ViewBag.ListadoDME = ListarDatosReportes(4, "ME", fecha_inicial, fecha_final);
            ViewBag.fecha_inicial = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month, 1));
            ViewBag.fecha_final = Convert.ToString(new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1)); 

            return View();
        }

        public string FechaUniversal(string fecha)
        {
            string[] arreglo1 = fecha.Split(' '); ;
            string[] arreglo2 = arreglo1[0].Split('/'); ;

            return arreglo2[2]+ arreglo2[1] + arreglo2[0];
        }

        public JsonResult ListarDatosReportes(int valor, string condicion, string fecha_inicial, string fecha_final)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            string sql = "";
            switch (valor)
            {
                case 1:
                    sql = "SELECT ep.id_estadoOportunidad 'Id', ep.decripcion 'Descripcion', sum(p.ingreso) 'Total' FROM PROSPECTO p INNER JOIN Estado_Prospecto ep ON ep.id_estadoOportunidad = p.id_estadoOporunidad WHERE p.codigo_tipoMon='"+ condicion + "' and p.fecha_inicial between '"+fecha_inicial+"' and '"+fecha_final+"' group by id_estadoOportunidad, ep.decripcion, ep.posicion order by ep.posicion";
                    break;
                case 2:
                    sql = "SELECT ep.id_estatusProspecto 'Id', ep.descripcion 'Descripcion', CAST(count(p.id_estatusProspecto) AS numeric) 'Total' FROM ESTATUS_PROSPECTO ep left JOIN PROSPECTO p ON ep.id_estatusProspecto = p.id_estatusProspecto  and p.fecha_inicial between '" + fecha_inicial + "' and '" + fecha_final + "' group by ep.id_estatusProspecto, ep.descripcion";
                    break;
                case 3:
                    sql = "SELECT ep.id_estatusProspecto 'Id', ep.descripcion 'Descripcion', ISNULL(sum(p.ingreso),0) 'Total' FROM ESTATUS_PROSPECTO ep left JOIN PROSPECTO p ON ep.id_estatusProspecto = p.id_estatusProspecto and p.codigo_tipoMon='" + condicion + "' and p.fecha_inicial between '" + fecha_inicial + "' and '" + fecha_final + "' group by ep.id_estatusProspecto, ep.descripcion";
                    break;
                case 4:
                    sql = "select CAST(p.id_oportunidad as Int) 'Id',pt.CNOMCLI 'Descripcion',p.ingreso 'Total' from PROSPECTO p left join POTENCIALCLI pt on p.idCliente = pt.CCODCLI WHERE p.fecha_inicial between '" + fecha_inicial + "' and '" + fecha_final + "' and p.id_estatusProspecto=3 and p.codigo_tipoMon='" + condicion + "'";
                    break;
                case 5:
                    sql = "select estatus 'Id', CASE  WHEN estatus = 0 THEN 'Pendiente' WHEN estatus = 1 THEN 'Cerrado' WHEN estatus=2 THEN 'Caducado' ELSE 'Eliminado' END as 'Descripcion',COUNT(estatus) 'Total' from CRONOGRAMA WHERE fecha_inicial between '" + fecha_inicial + "' and '" + fecha_final + "' GROUP BY estatus"; 
                    break;
            }

            var listado = db.Database.SqlQuery<ListadoReporte>(sql).ToList();

            return Json(listado, JsonRequestBehavior.AllowGet);
        }
    }
}