using System.Data.Entity;

namespace CRM.sp_dinamic
{
    public partial class BDWENCOEntities : DbContext
    {
        public BDWENCOEntities(string cadenaConexion) : base(cadenaConexion)
        {

        }

    }
}