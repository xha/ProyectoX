using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM.Models
{
    public class Maecli_Select
    {
       
        public string CCODCLI { get; set; }
        public string CNOMCLI { get; set; }
        public string CTELEFO { get; set; }
        public string CEMAIL { get; set; }

    }

    public class ListadoReporte
    {

        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Total { get; set; }
    }

    public class Maecli_campos
    {

        public string CCODCLI { get; set; }
        public string CNOMCLI { get; set; }
        public string CTELEFO { get; set; }
        public string CEMAIL { get; set; }
        public string CTIPO_DOCUMENTO { get; set; }
        public string CDOCIDEN { get; set; }
        public string CAPELLIDO_PATERNO { get; set; }
        public string CAPELLIDO_MATERNO { get; set; }
        public string CSEGUNDO_NOMBRE { get; set; }
        public string CPRIMER_NOMBRE { get; set; }
        public string CHOST { get; set; }
        public string TCL_CODIGO { get; set; }
        public string CDIRCLI { get; set; }
        public string CPROV { get; set; }
        public string CPAIS { get; set; }
        public string CDISTRI { get; set; }


    }

    public class Potencial_campos
    {

        public string CCODCLI { get; set; }
        public string CNOMCLI { get; set; }
        public string CTELEFO { get; set; }
        public string CEMAIL { get; set; }
        public string CTIPO_DOCUMENTO { get; set; }
        public string CDOCIDEN { get; set; }
        public string CAPELLIDO_PATERNO { get; set; }
        public string CAPELLIDO_MATERNO { get; set; }
        public string CSEGUNDO_NOMBRE { get; set; }
        public string CPRIMER_NOMBRE { get; set; }
        public string CHOST { get; set; }
        public string TCL_CODIGO { get; set; }
        public string CDIRCLI { get; set; }
        public string CPROV { get; set; }
        public string CPAIS { get; set; }
        public string CDISTRI { get; set; }


    }
}