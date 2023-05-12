
using STC.Models;
using System.Net.Http.Headers;

namespace STC.Services
{
    public class ServiceApiSTC
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;
        private ServiceStorageBlobs serviceBlob;
        public ServiceApiSTC(IConfiguration configuration,ServiceStorageBlobs sblob)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiSTC");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.serviceBlob = sblob;
        }
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        //public async Task<string> GetTokenAsync(string username, string password)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        string request = "/api/auth/login";
        //        client.BaseAddress = new Uri(this.UrlApi);
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(this.Header);
        //        LoginModel model = new LoginModel
        //        {
        //            UserName = username,
        //            Password = password
        //        };
        //        string jsonModel = JsonConvert.SerializeObject(model);
        //        StringContent content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
        //        HttpResponseMessage response = await client.PostAsync(request, content);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string data = await response.Content.ReadAsStringAsync();
        //            JObject jsonObject = JObject.Parse(data);
        //            string token = jsonObject.GetValue("response").ToString();
        //            return token;
        //        }
        //        else
        //        {
        //            return "ERROR " + response.StatusCode;
        //        }
        //    }
        //}
        public async Task<Partido> GetPartido(int idLocal, int idVisitante, int idCompeticion, int idTemporada)
        {
            string request = "/api/Partidos/BaseFindPartido/" + idLocal + "/" + idVisitante + "/" + idCompeticion + "/" + idTemporada;
            Partido partido = await CallApiAsync<Partido>(request);
            partido.LogoLocal = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoLocal);
            partido.LogoVisitante = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoVisitante);
            return partido;
        }
        public async Task<List<Partido>> GetPartidosDiaComp(string fecha,int idComp)
        {
            string request = "/api/Partidos/GetPartidosDiaComp/" + idComp + "/" + fecha;
            List<Partido> partidos = await CallApiAsync<List<Partido>>(request);
            foreach(Partido partido in partidos)
            {
                partido.LogoLocal = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoLocal);
                partido.LogoVisitante = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoVisitante);
            }
            return partidos;
        }
        public async Task<Competicion>GetCompeticion(int idComp)
        {
            string request = "/api/Competicion/GetCompeticion/" + idComp;
            Competicion compe = await CallApiAsync<Competicion>(request);
            compe.Logo = await serviceBlob.GetBlobUriAsync("containerpublico", compe.Logo);
            compe.BanderaPais = await serviceBlob.GetBlobUriAsync("containerpublico", compe.BanderaPais);
            return compe;
        }
        public async Task<VenueB>GetVenue(int idVenue)
        {
            string request = "/api/Partidos/BaseFindVenue/" + idVenue;
            VenueB venue= await CallApiAsync<VenueB>(request);
            return venue;
        }
        public async Task<ModelPartidoCompleto> BaseFindModeloPartidoYEventos(int idpartido)
        {
            string request = "/api/Partidos/BaseFindModeloPartidoYEventos/" + idpartido;
            ModelPartidoCompleto modelo = await CallApiAsync<ModelPartidoCompleto>(request);
            return modelo;
        }
        public async Task<List<EquipoCompStats>> GetCompeticionStandings(int idComp,int season)
        {
            string request = "/api/Competicion/GetCompeticionStandings/" + idComp + "/" + season ;
            List<EquipoCompStats> Equipos=await CallApiAsync<List<EquipoCompStats>>(request);
            return Equipos;
        }
        public async Task<ModelCompeticionPartidos> GetUltimosPartidosComp(int idComp,int season,int cantidad,int posicion)
        {
            string request = "/api/Competicion/GetUltimosPartidosComp/" + idComp + "/" + season + "/" + cantidad + "/" + posicion;
            ModelCompeticionPartidos modelo = await CallApiAsync<ModelCompeticionPartidos>(request);
            return modelo;
        }
        public async Task<Partido> GetUltimoPartidoDisputado(int idComp,int season)
        {
            string request = "/api/Competicion/GetUltimoPartidoDisputado/" + idComp + "/" + season;
            Partido partido=await CallApiAsync<Partido>(request);
            return partido;
        }
        public async Task<List<Partido>> GetProximosPartidos(int idComp,int season,int cantidad)
        {
            string request = "/api/Competicion/GetProximosPartidos/" + idComp + "/" + season + "/" + cantidad;
            List<Partido> partidos = await CallApiAsync<List<Partido>>(request);
            return partidos;
        }
    }
}
