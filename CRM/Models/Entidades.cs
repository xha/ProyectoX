using System.Data.Entity;

namespace CRM.Models
{
    public partial class Entidades : DbContext
    {
        public Entidades(string cadenaConexion) : base(cadenaConexion)
        {

        }
    }
}