using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRM.Models;
using CRM.Conexion;
using CRM.sp_dinamic;

namespace CRM.Controllers
{
    public class PROSPECTOController : Controller
    {
        private conexion CD = new conexion();

        // GET: PROSPECTO
        public ActionResult Index()
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");
            var pROSPECTO = db.PROSPECTO.Include(p => p.ESTADO_PROSPECTO).Include(p => p.PRIORIDAD);
            return View(pROSPECTO.ToList());
        }

        // GET: PROSPECTO/Details/5
        public ActionResult Planificar(string id)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROSPECTO pROSPECTO = db.PROSPECTO.Find(id);
            
            List<TIPO_ACTIVIDAD> tIPO_ACTIVIDAD = db.TIPO_ACTIVIDAD.ToList();

            ViewBag.tIPO_ACTIVIDAD = tIPO_ACTIVIDAD;
            if (pROSPECTO == null)
            {
                return HttpNotFound();
            }
            return View(pROSPECTO);
        }



        // GET: PROSPECTO/Edit/5
        public ActionResult Editar(string id, int tipoCli, string cliente)
        {

            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROSPECTO pROSPECTO = db.PROSPECTO.Find(id);
            if (pROSPECTO == null)
            {
                return HttpNotFound();
            }


            List<TIPO_ACTIVIDAD> tIPO_ACTIVIDAD = db.TIPO_ACTIVIDAD.ToList();

            ViewBag.tIPO_ACTIVIDAD = tIPO_ACTIVIDAD;
            ListarUsuarios();

            ViewBag.id_estadoOporunidad = new SelectList(db.ESTADO_PROSPECTO, "id_estadoOportunidad", "decripcion", pROSPECTO.id_estadoOporunidad);
            ViewBag.id_prioridad = new SelectList(db.PRIORIDAD, "id_prioridad", "descripcion", pROSPECTO.id_prioridad);
            ViewBag.ListaTipoMoneda = new SelectList(db.TIPO_MONEDA, "TIPOMON_CODIGO", "TIPOMON_SIMBOLO", pROSPECTO.codigo_tipoMon);

            if (tipoCli == 0)
            {



                List<Potencial_campos> data = new List<Potencial_campos>();

                var clientes = (from m in db.POTENCIALCLI
                                where m.CCODCLI == cliente
                                select new { m.CCODCLI, m.CTIPO_DOCUMENTO, m.CDOCIDEN, m.CAPELLIDO_PATERNO, m.CAPELLIDO_MATERNO, m.CPRIMER_NOMBRE, m.CNOMCLI, m.CTELEFO, m.CEMAIL, m.CHOST, m.TCL_CODIGO, m.CDIRCLI, m.CPROV, m.CPAIS, m.CDISTRI, m.CSEGUNDO_NOMBRE }).ToList();

                foreach (var s in clientes)
                {
                    data.Add(new Potencial_campos() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI, CTELEFO = s.CTELEFO, CEMAIL = s.CEMAIL, CTIPO_DOCUMENTO = s.CTIPO_DOCUMENTO, CDOCIDEN = s.CDOCIDEN, CAPELLIDO_PATERNO = s.CAPELLIDO_PATERNO, CAPELLIDO_MATERNO = s.CAPELLIDO_MATERNO, CPRIMER_NOMBRE = s.CPRIMER_NOMBRE, CHOST = s.CHOST, TCL_CODIGO = s.TCL_CODIGO, CDIRCLI = s.CDIRCLI, CPROV = s.CPROV, CPAIS = s.CPAIS, CDISTRI = s.CDISTRI, CSEGUNDO_NOMBRE = s.CSEGUNDO_NOMBRE });
                }

                List<Maecli_Select> lista = new List<Maecli_Select>();

                var lis = (from m in db.MAECLI
                                select new { m.CCODCLI, m.CNOMCLI }).ToList();
                foreach (var s in lis)
                {
                    lista.Add(new Maecli_Select() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI });
                }
                var consultaContactoPot = from datos in db.CONTACTO_POTENCIAL where datos.COD_CLIENTE == cliente select datos;
                List<CONTACTO_POTENCIAL> data2 = consultaContactoPot.ToList();
                ViewBag.Distrito = new SelectList(db.TABAYU.Where(p => p.TCOD == "13"), "TCLAVE", "TDESCRI", data.First().CDISTRI);
                ViewBag.clientePot = data;
                ViewBag.contactoPot = data2;
                ViewBag.tipoCliente = "modal_editarClientePot";
                ViewBag.listaFidelizado = new SelectList(lista, "CCODCLI", "CNOMCLI");
                ViewBag.listaPotencial = new SelectList(db.POTENCIALCLI, "CCODCLI", "CNOMCLI", pROSPECTO.idCliente);

                return View(pROSPECTO);
            }
            else
            {
                List<Maecli_campos> data = new List<Maecli_campos>();

                var clientes = (from m in db.MAECLI where m.CCODCLI == cliente
                                select new { m.CCODCLI, m.CTIPO_DOCUMENTO, m.CDOCIDEN, m.CAPELLIDO_PATERNO, m.CAPELLIDO_MATERNO, m.CPRIMER_NOMBRE, m.CNOMCLI, m.CTELEFO, m.CEMAIL, m.CHOST, m.TCL_CODIGO, m.CDIRCLI, m.CPROV, m.CPAIS, m.CDISTRI, m.CSEGUNDO_NOMBRE }).ToList();
                
                foreach (var s in clientes)
                {
                    data.Add(new Maecli_campos() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI, CTELEFO = s.CTELEFO, CEMAIL = s.CEMAIL, CTIPO_DOCUMENTO = s.CTIPO_DOCUMENTO, CDOCIDEN = s.CDOCIDEN, CAPELLIDO_PATERNO = s.CAPELLIDO_PATERNO, CAPELLIDO_MATERNO = s.CAPELLIDO_MATERNO, CPRIMER_NOMBRE = s.CPRIMER_NOMBRE, CHOST = s.CHOST, TCL_CODIGO = s.TCL_CODIGO, CDIRCLI = s.CDIRCLI, CPROV = s.CPROV, CPAIS = s.CPAIS, CDISTRI = s.CDISTRI, CSEGUNDO_NOMBRE = s.CSEGUNDO_NOMBRE });
                }


                List<Maecli_Select> lista = new List<Maecli_Select>();

                var lis = (from m in db.MAECLI
                           select new { m.CCODCLI, m.CNOMCLI }).ToList();
                foreach (var s in lis)
                {
                    lista.Add(new Maecli_Select() { CCODCLI = s.CCODCLI, CNOMCLI = s.CNOMCLI });
                }

                var consultaContactoFi = from datos in db.CONTACTO_VENTA where datos.COD_CLIENTE == cliente select datos;
                List<CONTACTO_VENTA> data2 = consultaContactoFi.ToList();
                ViewBag.Distrito = new SelectList(db.TABAYU.Where(p => p.TCOD == "13"), "TCLAVE", "TDESCRI", data.First().CDISTRI);
                ViewBag.clienteFi = data;
                ViewBag.contactoFi = data2;
                ViewBag.tipoCliente = "modal_editarClienteFi";
                ViewBag.listaFidelizado = new SelectList(lista, "CCODCLI", "CNOMCLI", pROSPECTO.idCliente);
                ViewBag.listaPotencial = new SelectList(db.POTENCIALCLI, "CCODCLI", "CNOMCLI");

                return View(pROSPECTO);
            }

        }
        public ActionResult cambiarEstatus(int estatusFinal, string idProspecto)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));

            PROSPECTO pROSPECTO = db.PROSPECTO.Find(idProspecto);

            pROSPECTO.id_estatusProspecto= estatusFinal;
            pROSPECTO.fecha_final = DateTime.Now;

            ESTATUS_PROSPECTO eSTATUS = db.ESTATUS_PROSPECTO.Find(estatusFinal);


            // Crear el Historico de la Oportunidad
            HISTORICO Historico = new HISTORICO();

            Historico.id_oportunidad = pROSPECTO.id_oportunidad;
            Historico.fecha = @DateTime.Now;
            Historico.titulo = "Cambió el prospecto al estado: " + eSTATUS.descripcion + "";
            Historico.descripcion = "";
            Historico.id_usuario = Convert.ToString(Session["alias"]);
            Historico.id_estatusOp = estatusFinal;


            if (ModelState.IsValid)
            {
                db.HISTORICO.Add(Historico);
                db.Entry(pROSPECTO).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Editar", "PROSPECTO", new { id = pROSPECTO.id_oportunidad, tipoCli = pROSPECTO.tipoCliente, cliente = pROSPECTO.idCliente });
            }

            return RedirectToAction("Editar", "PROSPECTO", new { id = pROSPECTO.id_oportunidad, tipoCli = pROSPECTO.tipoCliente, cliente = pROSPECTO.idCliente });
        }

        // POST: PROSPECTO/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarEditar(string idProspecto, string notas, string cierre_previsto, string nombreEstado, int Estado, string UsuarioAsig, int probabilidad, string nombreOp, string clientePot, string clienteFi, int id_prioridad, string Ingreso, string tipoMoneda, string fechaActual, int tipContacto)
        {

            try
            {
                Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
                Ingreso = Ingreso.Replace(",", ".");
                decimal nvo_ingreso = Convert.ToDecimal(Ingreso);
                PROSPECTO pROSPECTO = db.PROSPECTO.Find(idProspecto);

                pROSPECTO.nombre = nombreOp;
                pROSPECTO.porcentajeGanar = probabilidad;
                pROSPECTO.codigo_tipoMon = tipoMoneda;
                pROSPECTO.ingreso = nvo_ingreso;
                pROSPECTO.tipoCliente = tipContacto;
                pROSPECTO.notas = notas;
                pROSPECTO.cierre_previsto = cierre_previsto;
                if(UsuarioAsig != "" && UsuarioAsig != null)
                {
                pROSPECTO.id_usuario = UsuarioAsig;
                }


                if (tipContacto == 1)
                {
                    pROSPECTO.idCliente = Convert.ToString(clienteFi);
                }
                else
                {
                    pROSPECTO.idCliente = Convert.ToString(clientePot);
                }

                pROSPECTO.id_estadoOporunidad = Estado;

                if (id_prioridad == 0)
                {
                    pROSPECTO.id_prioridad = 0;
                }
                else
                {
                    pROSPECTO.id_prioridad = id_prioridad;
                }


                // Crear el Historico de la Oportunidad
                HISTORICO Historico = new HISTORICO();

                Historico.id_oportunidad = pROSPECTO.id_oportunidad;
                Historico.fecha = @DateTime.Now;
                Historico.titulo = "Editó la oportunidad";
                Historico.id_usuario = Session["alias"].ToString();


                ViewBag.CodEmpresa = "";



                if (ModelState.IsValid)
                {
                    db.HISTORICO.Add(Historico);
                    db.Entry(pROSPECTO).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Editar", "PROSPECTO", new { id = pROSPECTO.id_oportunidad, tipoCli = pROSPECTO.tipoCliente, cliente = pROSPECTO.idCliente });
                }
            }
            catch
            {
                ViewBag.Error("Ocurrio un error al editar la oportunidad");
            }
            ViewBag.Error("");
            if (tipContacto == 1)
            {
                return RedirectToAction("Editar", "PROSPECTO", new { id = idProspecto, tipoCli = tipContacto, cliente = clienteFi });
            }
            else
            {
                return RedirectToAction("Editar", "PROSPECTO", new { id = idProspecto, tipoCli = tipContacto, cliente = clientePot });
            }

        }

        public void ListarUsuarios()
        {
            BDWENCOEntities db = new BDWENCOEntities(CD.ConexDinamica(Session["datasour"].ToString(), Session["catalog"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Configuration.ProxyCreationEnabled = false;
            string empresa = Session["codigo"].ToString();

            List<CRM_Usuarios_Rol> listado = db.CRM_Usuarios_Rol.Where(m => m.codigoEmpresa == empresa).OrderBy(m => m.alias).ToList();
            var Resultado = (from N in listado
                             select new { valor = N.alias, texto = string.Format("{0}--{1}", N.alias, N.Rol )}).ToList();

            ViewBag.usuarios = new SelectList(Resultado, "valor", "texto"); 


        }

        // GET: PROSPECTO/Delete/5
        public ActionResult Delete(string id)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROSPECTO pROSPECTO = db.PROSPECTO.Find(id);
            if (pROSPECTO == null)
            {
                return HttpNotFound();
            }
            return View(pROSPECTO);
        }

        // POST: PROSPECTO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            PROSPECTO pROSPECTO = db.PROSPECTO.Find(id);
            db.PROSPECTO.Remove(pROSPECTO);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult cargarActividad(int id_cronograma, string idOportunidad)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            db.Database.ExecuteSqlCommand("UPDATE CRONOGRAMA SET estatus=2 WHERE fecha_final < GETDATE() and estatus=0");
            db.Configuration.ProxyCreationEnabled = false;

            var actividad = db.CRONOGRAMA.Where(m => m.id_cronograma == id_cronograma && m.id_oportunidad == idOportunidad).Select(m => new { m}).ToList();

            var data = new
            {
                actividad,
            };


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            Entidades db = new Entidades(CD.ConexDinamicaEntidades(Session["datasour"].ToString(), Session["catalog_user"].ToString(), Session["user"].ToString(), Session["password"].ToString()));
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
