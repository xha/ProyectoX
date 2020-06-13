using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.EntityClient;

namespace CRM.Conexion
{
    public class conexion
    {
        
             public string ConexDinamicaEntidades(string datasour, string catalog, string user, string password)
        {
            string providerName = "System.Data.SqlClient";
            EntityConnectionStringBuilder sqlBuilders = new EntityConnectionStringBuilder();
            sqlBuilders.Provider = providerName;
            sqlBuilders.ProviderConnectionString = "data source=" + datasour + ";initial catalog=" + catalog + ";user id=" + user + ";password=" + password + ";MultipleActiveResultSets=True;App=EntityFramework";
            sqlBuilders.Metadata = "res://*/Models.CRM.csdl|res://*/Models.CRM.ssdl|res://*/Models.CRM.msl";

            return sqlBuilders.ToString();
        }


        public string ConexDinamica(string datasour, string catalog, string user, string password)
        {
            string providerName = "System.Data.SqlClient";
            EntityConnectionStringBuilder sqlBuilders = new EntityConnectionStringBuilder();
            sqlBuilders.Provider = providerName;
            sqlBuilders.ProviderConnectionString = "data source=" + datasour + ";initial catalog=" + catalog + ";user id=" + user + ";password=" + password + ";MultipleActiveResultSets=True;App=EntityFramework";
            sqlBuilders.Metadata = "res://*/sp_dinamic.Model1.csdl|res://*/sp_dinamic.Model1.ssdl|res://*/sp_dinamic.Model1.msl";
 
            return sqlBuilders.ToString();
        }

        public bool ConsultaPost(string ruta, string ruc, string modulo)
        {
            HttpClient client = new HttpClient(new HttpClientHandler());
            client.BaseAddress = new Uri("https://www.starsoftweb.com/ServicioLicenciaPortales/Api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _CREDENCIALES);
            HttpResponseMessage respuesta = new HttpResponseMessage();

            JObject cadena = new JObject();
            cadena.Add(new JProperty("ruc", ruc));
            cadena.Add(new JProperty("modulo", modulo));

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(cadena), Encoding.UTF8, "application/json");

            var response = client.PostAsync(client.BaseAddress + ruta, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;

                if (data.Contains("IsSuccess\":true"))
                {
                    DateTime Hoy = DateTime.Today;
                    dynamic Listado = JsonConvert.DeserializeObject(data);

                    string[] arreglo_fecha = Listado.fechaVigencia.ToString().Split(' ');

                    DateTime fechaVigencia = Convert.ToDateTime(arreglo_fecha[0] + ' ' + arreglo_fecha[1]);

                    if (fechaVigencia > Hoy)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string ConexFIJA(string datasour, string catalog, string user, string password)
        {
            string providerName = "System.Data.SqlClient";
            EntityConnectionStringBuilder sqlBuilders = new EntityConnectionStringBuilder();
            sqlBuilders.Provider = providerName;
            sqlBuilders.ProviderConnectionString = "data source=" + datasour + ";initial catalog=" + catalog + ";user id=" + user + ";password=" + password + ";MultipleActiveResultSets=True;App=EntityFramework";
            sqlBuilders.Metadata = "res://*/sp_estatico.Model1.csdl|res://*/sp_estatico.Model1.ssdl|res://*/sp_estatico.Model1.msl";
            return sqlBuilders.ToString();
        }
      
    }
}
