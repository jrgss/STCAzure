
using STC.Models;
using System.Net.Http.Headers;

namespace STC.Services
{
    public class ServiceApiSTC
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;
        private ServiceStorageBlobs serviceBlob;
        public ServiceApiSTC(IConfiguration configuration,ServiceStorageBlobs sblob,string apiurl)
        {

            this.UrlApi = apiurl;
            this.Header = new MediaTypeWithQualityHeaderValue("FGapplication/json");
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
        public async Task<List<Competicion>> GetCompeticiones()
        {
            string request = "/api/Competicion/GetCompeticiones";
            List<Competicion> competiciones = await CallApiAsync<List<Competicion>>(request);
            foreach(Competicion compe in competiciones)
            {
                compe.Logo = await serviceBlob.GetBlobUriAsync("containerpublico", compe.Logo);
                compe.BanderaPais = await serviceBlob.GetBlobUriAsync("containerpublico", compe.BanderaPais);
            }
            return competiciones;
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
            modelo.partido.LogoLocal = await serviceBlob.GetBlobUriAsync("containerpublico", modelo.partido.LogoLocal);
            modelo.partido.LogoVisitante = await serviceBlob.GetBlobUriAsync("containerpublico", modelo.partido.LogoVisitante);
            return modelo;
        }
        public async Task<List<EquipoCompStats>> GetCompeticionStandings(int idComp,int season)
        {
            string request = "/api/Competicion/GetCompeticionStandings/" + idComp + "/" + season ;
            List<EquipoCompStats> Equipos=await CallApiAsync<List<EquipoCompStats>>(request);
            foreach(EquipoCompStats equipo in Equipos)
            {
                equipo.logo = await serviceBlob.GetBlobUriAsync("containerpublico", equipo.logo);
            }

            return Equipos;
        }
        public async Task<ModelCompeticionPartidos> GetUltimosPartidosComp(int idComp,int season,int cantidad,int posicion)
        {
            string request = "/api/Competicion/GetUltimosPartidosComp/" + idComp + "/" + season + "/" + cantidad + "/" + posicion;
            ModelCompeticionPartidos modelo = await CallApiAsync<ModelCompeticionPartidos>(request);

            modelo.competicion.Logo = await serviceBlob.GetBlobUriAsync("containerpublico", modelo.competicion.Logo);
            modelo.competicion.BanderaPais = await serviceBlob.GetBlobUriAsync("containerpublico", modelo.competicion.BanderaPais);

            foreach(Partido partido in modelo.partidos)
            {
                partido.LogoLocal = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoLocal);
                partido.LogoVisitante = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoVisitante);
            }

            return modelo;
        }
        public async Task<Partido> GetUltimoPartidoDisputado(int idComp,int season)
        {
            string request = "/api/Competicion/GetUltimoPartidoDisputado/" + idComp + "/" + season;
            Partido partido=await CallApiAsync<Partido>(request);
            partido.LogoLocal = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoLocal);
            partido.LogoVisitante = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoVisitante);
            return partido;
        }
        public async Task<List<Partido>> GetProximosPartidos(int idComp,int season,int cantidad)
        {
            string request = "/api/Competicion/GetProximosPartidos/" + idComp + "/" + season + "/" + cantidad;
            List<Partido> partidos = await CallApiAsync<List<Partido>>(request);
            foreach (Partido partido in partidos)
            {
                partido.LogoLocal = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoLocal);
                partido.LogoVisitante = await serviceBlob.GetBlobUriAsync("containerpublico", partido.LogoVisitante);
            }
            return partidos;
        }
     
    }
}
