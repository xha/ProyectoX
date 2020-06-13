using System.Data.Entity;
 
namespace CRM.sp_estatico
{
    public partial class CLIENTES_PORTALESEntities : DbContext
    {
        public CLIENTES_PORTALESEntities(string cadenaConexion) : base(cadenaConexion)
        {

        }

    }
}