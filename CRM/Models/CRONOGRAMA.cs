//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CRONOGRAMA
    {
        public int id_cronograma { get; set; }
        public string observacion { get; set; }
        public int id_tipoActividad { get; set; }
        public string descripcion { get; set; }
        public string id_oportunidad { get; set; }
        public System.DateTime fecha_inicial { get; set; }
        public System.DateTime fecha_final { get; set; }
        public int estatus { get; set; }
        public string USU_CODIGO { get; set; }
        public string respuesta { get; set; }

        public virtual TIPO_ACTIVIDAD TIPO_ACTIVIDAD { get; set; }
        public virtual PROSPECTO PROSPECTO { get; set; }
    }
}
