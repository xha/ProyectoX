using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CRM.Models;

namespace CRM.Controllers
{
    public class ESTADO_PROSPECTOController : Controller
    {
        private Entidades db = new Entidades();

        // GET: ESTADO_OPORTUNIDAD
        public ActionResult Index()
        {
            return View(db.ESTADO_PROSPECTO.ToList());
        }

        // GET: ESTADO_OPORTUNIDAD/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ESTADO_PROSPECTO eSTADO_PROSPECTO = db.ESTADO_PROSPECTO.Find(id);
            if (eSTADO_PROSPECTO == null)
            {
                return HttpNotFound();
            }
            return View(eSTADO_PROSPECTO);
        }

        // GET: ESTADO_OPORTUNIDAD/Create
        public ActionResult Create()
        {
            return View();
        }


        public void Nuevo([Bind(Include = "id_estadoOportunidad,decripcion,color,posicion")] ESTADO_PROSPECTO eSTADO_PROSPECTO)
        {
            if (ModelState.IsValid)
            {
                int ultOp = int.Parse(db.ESTADO_PROSPECTO
                                   .OrderByDescending(p => p.id_estadoOportunidad)
                                   .Select(r => r.id_estadoOportunidad)
                                   .First().ToString());

                var codigo = Convert.ToInt32(ultOp);
                codigo = (codigo + 1);

                eSTADO_PROSPECTO.id_estadoOportunidad = codigo;

            db.ESTADO_PROSPECTO.Add(eSTADO_PROSPECTO);
                db.SaveChanges();

            }


        }



        public void Editar(int id, string color, string nombre)
        {

            ESTADO_PROSPECTO eSTADO_PROSPECTO = db.ESTADO_PROSPECTO.Find(id);
            if (eSTADO_PROSPECTO == null)
            {
                throw new ApplicationException();

            }


            else
            {
                // Crear el Historico de la Oportunidad
                HISTORICO Historico = new HISTORICO();


                Historico.fecha = @DateTime.Now;
                Historico.titulo = "Editó el estado: " + eSTADO_PROSPECTO.decripcion + " a " + nombre + "";
                Historico.descripcion = "";
                Historico.id_EstadoProspecto = id;
                //colocar el usuario
                Historico.id_usuario = Session["alias"].ToString();

                eSTADO_PROSPECTO.color = color;
                eSTADO_PROSPECTO.decripcion = nombre;

                if (ModelState.IsValid)
                {
                    db.HISTORICO.Add(Historico);
                    db.Entry(eSTADO_PROSPECTO).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }





        }




        public void Eliminar(int id)
        {
            ESTADO_PROSPECTO eSTADO_PROSPECTO = db.ESTADO_PROSPECTO.Find(id);


            // Crear el Historico de la Oportunidad
            HISTORICO Historico = new HISTORICO();


            Historico.fecha = @DateTime.Now;
            Historico.titulo = "Eliminó el estado: " + eSTADO_PROSPECTO.decripcion + "";
            Historico.descripcion = "";
            //colocar el usuario
            Historico.id_usuario = Session["alias"].ToString();

            db.HISTORICO.Add(Historico);
            db.ESTADO_PROSPECTO.Remove(eSTADO_PROSPECTO);
            db.SaveChanges();
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
