using ElectroHogar.Config;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace ElectroHogar.Persistencia
{
    public abstract class BaseWS
    {
        protected readonly string _adminId;
        public string AdminId => _adminId;

        protected BaseWS()
        {
            _adminId = ConfigHelper.GetValue("AdminId") ?? throw new ArgumentNullException(nameof(_adminId));
        }

        protected T DeserializarRespuesta<T>(HttpResponseMessage response)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(responseBody);
            }

            throw new Exception(responseBody);
        }
    }
}
