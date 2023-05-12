using Newtonsoft.Json;
using STC.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace STC.Services
{
    public class ServiceApi
    {
        private HttpClient httpClient;

        public ServiceApi()
        {
            string token = "";
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", token);
            this.httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", "v3.football.api-sports.io");
        }

        public async Task Prueba()
        {

            var request = "https://v3.football.api-sports.io/fixtures?league=2&season=2022&round=Round of 16";

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {

                Root myRoot = JsonConvert.DeserializeObject<Root>(jsonResponse);
                //myRoot.response[0].teams.
            }
            else
            {

            }

        }
        public async Task<List<ModelLineUp>> GetLineupsFixture(int idFixture)
        {
            var request = "https://v3.football.api-sports.io/fixtures/lineups?fixture=" + idFixture;

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                RootLineUp root= JsonConvert.DeserializeObject< RootLineUp>(jsonResponse);
                List<ModelLineUp> alineaciones = new List<ModelLineUp>();
                foreach (ResponseLineUp res in root.response)
                {
                    ModelLineUp model = new ModelLineUp();
                    LineupB Lineup = new LineupB();
                    Lineup.IdPartido = idFixture;
                    Lineup.IdEquipo = res.team.id;
                    Lineup.Esquema = res.formation;
                    List<LineupPlayer> titulares = new List<LineupPlayer>();
                    List<LineupPlayer> suplentes = new List<LineupPlayer>();

                    foreach(StartXI player in res.startXI)
                    {
                        LineupPlayer miJugador= new LineupPlayer();
                        miJugador.IdPartido = idFixture;
                        miJugador.IdEquipo = res.team.id;
                        miJugador.Nombre = player.player.Name;
                        if (player.player.number == null)
                        {
                            miJugador.Numero =0;
                        }
                        else
                        {
                            miJugador.Numero = (int)player.player.number;
                        }
                        //Hay algunos jugadores que no tienen posición, probablemente sean canteranos
                        if (player.player.pos == null)
                        {
                            miJugador.Posicion = "";
                        }
                        else
                        {
                            miJugador.Posicion = player.player.pos;
                        }
                       
                        if (player.player.grid == null)
                        {
                            miJugador.Grid = "";
                        }
                        else
                        {
                            miJugador.Grid= player.player.grid;
                        }
                        titulares.Add(miJugador);
                    }

                    foreach (Substitutes player in res.substitutes)
                    {
                        LineupPlayer miJugador = new LineupPlayer();
                        miJugador.IdPartido = idFixture;
                        miJugador.IdEquipo = res.team.id;
                        miJugador.Nombre = player.player.Name;
                        if (player.player.number == null)
                        {
                            miJugador.Numero = 0;
                        }
                        else
                        {
                            miJugador.Numero = (int)player.player.number;
                        }

                        //Hay algunos jugadores que no tienen posición, probablemente sean canteranos
                        if (player.player.pos == null)
                        {
                            miJugador.Posicion = "";
                        }
                        else
                        {
                            miJugador.Posicion = player.player.pos;
                        }
                        if (player.player.grid == null)
                        {
                            miJugador.Grid = "";
                        }
                        else
                        {
                            miJugador.Grid = player.player.grid;
                        }
                        suplentes.Add(miJugador);
                    }
                    model.Lineup = Lineup;
                    model.Titulares = titulares;
                    model.Suplentes = suplentes;

                    alineaciones.Add(model);
                }
                return alineaciones;
            }
            else
            {
                return null;
            }
                
        }
        public async Task<ModelStatsPartido> GetStatsFixture(int idFixture)
        {
            var request = "https://v3.football.api-sports.io/fixtures/statistics?fixture=" + idFixture;

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                RootMatchStats root = JsonConvert.DeserializeObject<RootMatchStats>(jsonResponse);
              ModelStatsPartido modelStats = new ModelStatsPartido();
                int cnt = 1;
                foreach(ResponseMatchStats res in root.response)
                {
                    List<StatsPartido> stats = new List<StatsPartido>();
                       foreach(Statistic stat in res.statistics)
                    {
                        StatsPartido miStat = new StatsPartido();
                        miStat.IdPartido = idFixture;
                        miStat.Tipo = stat.type;
                        miStat.IdEquipo = res.team.id;
                        if (stat.value == null)
                        {
                            miStat.Valor = "0";
                        }else if(stat.GetType() == typeof(int))
                        {
                            miStat.Valor = ((int)stat.value).ToString();
                        }
                        else
                        {
                            miStat.Valor+= stat.value.ToString();
                        }
                       stats.Add(miStat);
                    }
                    if (cnt == 1)
                    {
                        modelStats.statsLocal = stats;
                    }
                    else
                    {
                        modelStats.statsVisitante = stats;
                    }
                    cnt++;
                }
                
                return modelStats;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<Evento>> GetEventsFixture(int idFixture)
        {
            var request = "https://v3.football.api-sports.io/fixtures/events?fixture="+idFixture;

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                RootEvent root= JsonConvert.DeserializeObject<RootEvent>(jsonResponse);
                List<Evento> eventos = new List<Evento>();
                foreach(ResponseEvent res in root.Response)
                {

                    Evento evento = new Evento();
                    evento.Elapsed = res.Time.Elapsed;
                    if (res.Time.Extra == null)
                    {
                        evento.Extra = 0;
                    }
                    else
                    {
                        evento.Extra = (int)res.Time.Extra;
                    }
                  
                    evento.IdPartido = root.Parameters.Fixture;
                    evento.IdEquipo = res.Team.id;
                    evento.NombreJugador = res.Player.Name;
                    if (res.Assist.Name == null)
                    {
                        evento.NombreAsistencia = "";
                    }
                    else
                    {
                        evento.NombreAsistencia = res.Assist.Name;
                    }
                  
                    evento.Tipo = res.Type;
                    evento.Detalles = res.Detail;
                    if (res.Comments == null)
                    {
                        evento.Comentarios ="";
                    }
                    else
                    {
                        evento.Comentarios = (string)res.Comments;
                    }
                   
                    eventos.Add(evento);
                }
                return eventos;
            }
            else
            {
                return null;
            }

        }
        public async Task<Root> GetFixturesDia(string fecha)
        {
            var request = "https://v3.football.api-sports.io/fixtures?date="+fecha;

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if(jsonResponse!=null)
            {
                Root myRoot = JsonConvert.DeserializeObject<Root>(jsonResponse);
                return myRoot;
            }
            else
            {
                return null;
            }
        }
     
        public async Task<List<ModelPartidoCompleto>> GetAllFixturesCompletoCompeticion(int idcomp,int season)
        {
            var request = "https://v3.football.api-sports.io/fixtures?league=" + idcomp + "&season=" + season;

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                Root myRoot = JsonConvert.DeserializeObject<Root>(jsonResponse);
                List<ModelPartidoCompleto> partidos = new List<ModelPartidoCompleto>();
                int cnt = 0;
                foreach (Response resp in myRoot.response)
                {
                    if (cnt >= 350 && cnt<400)
                    {

                    
                    ModelPartidoCompleto modelo = new ModelPartidoCompleto();
                    Partido partido = new Partido();
                    partido.IdPartido = resp.fixture.id;
                    partido.IdCompeticion = resp.league.id;
                    partido.IdTemporada = resp.league.season;
                        if (resp.fixture.referee == null)
                        {
                            partido.Arbitro = "Sin definir";
                        }
                        else
                        {
                            partido.Arbitro = resp.fixture.referee;
                        }
                          
                    partido.FechaInicio = resp.fixture.date;
                    partido.Timezone = resp.fixture.timezone;
                    partido.IdLocal = resp.teams.home.id;
                    partido.NombreLocal = resp.teams.home.name;
                    partido.IdVisitante = resp.teams.away.id;
                    partido.NombreVisitante = resp.teams.away.name;
                    partido.IdVenue = (int)resp.fixture.venue.id;
                    partido.NombreVenue = resp.fixture.venue.name;
                    //partido.GolesLocalHf = resp.score.halftime.home.Value;
                    //partido.GolesVisitanteHf = resp.score.halftime.away.Value;
                    //partido.GolesLocalFt = resp.score.fulltime.home.Value;
                    //partido.GolesVisitanteFt = resp.score.fulltime.away.Value;
                    partido.LogoLocal = resp.teams.home.logo;
                    partido.LogoVisitante = resp.teams.away.logo;
                    var a = resp.score.extratime;


                    if (resp.score.halftime.home == null)
                    {
                        partido.GolesLocalHf = 0;
                    }
                    else
                    {
                        partido.GolesLocalHf = resp.score.halftime.home.Value;
                    }
                    if (resp.score.halftime.away == null)
                    {
                        partido.GolesVisitanteHf = 0;
                    }
                    else
                    {
                        partido.GolesVisitanteHf = resp.score.halftime.away.Value;
                    }


                    //
                    if (resp.score.fulltime.home == null)
                    {
                        partido.GolesLocalFt = 0;
                    }
                    else
                    {
                        partido.GolesLocalFt = resp.score.fulltime.home.Value;
                    }
                    if (resp.score.fulltime.away == null)
                    {
                        partido.GolesVisitanteFt = 0;
                    }
                    else
                    {
                        partido.GolesVisitanteFt = resp.score.fulltime.away.Value;
                    }
                    //

                    if (resp.score.extratime.home == null)
                    {
                        partido.GolesLocalEt = 0;
                    }
                    else
                    {
                        partido.GolesLocalEt = resp.score.extratime.home.Value;
                    }
                    if (resp.score.extratime.away == null)
                    {
                        partido.GolesVisitanteEt = 0;
                    }
                    else
                    {
                        partido.GolesVisitanteEt = resp.score.extratime.away.Value;
                    }



                    if (resp.score.penalty.home == null)
                    {
                        partido.GolesLocalPenalty = 0;
                    }
                    else
                    {
                        partido.GolesLocalPenalty = resp.score.penalty.home.Value;
                    }
                    if (resp.score.penalty.away == null)
                    {
                        partido.GolesVisitantePenalty = 0;
                    }
                    else
                    {
                        partido.GolesVisitantePenalty = resp.score.penalty.away.Value;

                    }

                    partido.Ronda = resp.league.round;
                    partido.Estado = resp.fixture.status.@long;
                    partido.EstadoFinal = resp.fixture.status.@short;
                    if (resp.fixture.status.elapsed == null)
                    {
                        partido.Elapsed =0;
                    }
                    else
                    {
                        partido.Elapsed = resp.fixture.status.elapsed.Value;
                    }
                
                    partido.Pais = resp.league.country;
                    partido.NombreCompeticion = resp.league.name;
                    partido.PaisImagen = "";

                    //recojo los eventos del partido
                    if(partido.EstadoFinal.Equals("FT"))
                    {
                        List<Evento> misEventos = await GetEventsFixture(partido.IdPartido);
                        List<ModelLineUp> lineups = await GetLineupsFixture(partido.IdPartido);
                         ModelStatsPartido estadisticasPartido = await GetStatsFixture(partido.IdPartido);
                        modelo.eventos = misEventos;
                        modelo.LineUps = lineups;
                        modelo.EstadisticasPartido = estadisticasPartido;

                    }
                    else
                    {

                    }
              
                    modelo.partido = partido;
                    partidos.Add(modelo);
                       
                    }
                    cnt++;
                }
                return partidos;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<Partido>> GetFixturesDiaComp(string fecha,int idcomp,int season)
        {
            var request = "https://v3.football.api-sports.io/fixtures?date=" + fecha+"&league="+idcomp+"&season="+season;

            var response = await this.httpClient.GetAsync(request);
            int cnt = 1;
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                Root myRoot = JsonConvert.DeserializeObject<Root>(jsonResponse);
                List<Partido> partidos = new List<Partido>();
                   foreach(Response resp in myRoot.response)
                {
                    Partido partido = new Partido();
                    partido.IdPartido = resp.fixture.id;
                    partido.IdCompeticion = resp.league.id;
                    partido.IdTemporada = resp.league.season;
                    partido.Arbitro = resp.fixture.referee;
                    partido.FechaInicio = resp.fixture.date;
                    partido.Timezone = resp.fixture.timezone;
                    partido.IdLocal = resp.teams.home.id;
                    partido.NombreLocal = resp.teams.home.name;
                    partido.IdVisitante = resp.teams.away.id;
                    partido.NombreVisitante = resp.teams.away.name;
                    partido.IdVenue = (int)resp.fixture.venue.id;
                    partido.NombreVenue = resp.fixture.venue.name;
                    partido.GolesLocalHf = resp.score.halftime.home.Value;
                    partido.GolesVisitanteHf = resp.score.halftime.away.Value;
                    partido.GolesLocalFt = resp.score.fulltime.home.Value;
                    partido.GolesVisitanteFt = resp.score.fulltime.away.Value;
                    partido.LogoLocal = resp.teams.home.logo;
                    partido.LogoVisitante= resp.teams.away.logo;
                 

                    if (resp.score.extratime.home == null)
                    {
                        partido.GolesLocalEt = 0;
                    }
                    else
                    {
                        partido.GolesLocalEt = resp.score.extratime.home.Value;
                    }
                    if (resp.score.extratime.away == null)
                    {
                        partido.GolesVisitanteEt = 0;
                    }
                    else
                    {
                        partido.GolesVisitanteEt = resp.score.extratime.away.Value;
                    }


                   
                    if (resp.score.penalty.home == null)
                    {
                        partido.GolesLocalPenalty = 0;
                    }
                    else
                    {
                        partido.GolesLocalPenalty = resp.score.penalty.home.Value;
                    }
                    if (resp.score.penalty.away == null)
                    {
                        partido.GolesVisitantePenalty = 0;
                    }
                    else
                    {
                        partido.GolesVisitantePenalty = resp.score.penalty.away.Value;

                    }

                        partido.Ronda = resp.league.round;
                    partido.Estado = resp.fixture.status.@long;
                    partido.EstadoFinal = resp.fixture.status.@short;
                    partido.Elapsed = (int)resp.fixture.status.elapsed;

                    partidos.Add(partido);
                }
                return partidos;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<ModelPartidoCompleto>> GetFixturesCompletoDiaComp(string fecha, int idcomp, int season)
        {
            var request = "https://v3.football.api-sports.io/fixtures?date=" + fecha + "&league=" + idcomp + "&season=" + season;

            var response = await this.httpClient.GetAsync(request);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                Root myRoot = JsonConvert.DeserializeObject<Root>(jsonResponse);
                List<ModelPartidoCompleto> partidos = new List<ModelPartidoCompleto>();
                foreach (Response resp in myRoot.response)
                {
                    ModelPartidoCompleto modelo = new ModelPartidoCompleto();
                    Partido partido = new Partido();
                    partido.IdPartido = resp.fixture.id;
                    partido.IdCompeticion = resp.league.id;
                    partido.IdTemporada = resp.league.season;
                    partido.Arbitro = resp.fixture.referee;
                    partido.FechaInicio = resp.fixture.date;
                    partido.Timezone = resp.fixture.timezone;
                    partido.IdLocal = resp.teams.home.id;
                    partido.NombreLocal = resp.teams.home.name;
                    partido.IdVisitante = resp.teams.away.id;
                    partido.NombreVisitante = resp.teams.away.name;
                    partido.IdVenue = (int)resp.fixture.venue.id;
                    partido.NombreVenue = resp.fixture.venue.name;
                    partido.GolesLocalHf = resp.score.halftime.home.Value;
                    partido.GolesVisitanteHf = resp.score.halftime.away.Value;
                    partido.GolesLocalFt = resp.score.fulltime.home.Value;
                    partido.GolesVisitanteFt = resp.score.fulltime.away.Value;
                    partido.LogoLocal = resp.teams.home.logo;
                    partido.LogoVisitante = resp.teams.away.logo;
                    var a = resp.score.extratime;

                    if (resp.score.extratime.home == null)
                    {
                        partido.GolesLocalEt = 0;
                    }
                    else
                    {
                        partido.GolesLocalEt = resp.score.extratime.home.Value;
                    }
                    if (resp.score.extratime.away == null)
                    {
                        partido.GolesVisitanteEt = 0;
                    }
                    else
                    {
                        partido.GolesVisitanteEt = resp.score.extratime.away.Value;
                    }



                    if (resp.score.penalty.home == null)
                    {
                        partido.GolesLocalPenalty = 0;
                    }
                    else
                    {
                        partido.GolesLocalPenalty = resp.score.penalty.home.Value;
                    }
                    if (resp.score.penalty.away == null)
                    {
                        partido.GolesVisitantePenalty = 0;
                    }
                    else
                    {
                        partido.GolesVisitantePenalty = resp.score.penalty.away.Value;

                    }

                    partido.Ronda = resp.league.round;
                    partido.Estado = resp.fixture.status.@long;
                    partido.EstadoFinal = resp.fixture.status.@short;
                    partido.Elapsed = (int)resp.fixture.status.elapsed;
                    partido.Pais = resp.league.country;
                    partido.NombreCompeticion = resp.league.name;
                    partido.PaisImagen = "";

                    //recojo los eventos del partido
                    List<Evento> misEventos = await GetEventsFixture(partido.IdPartido);
                    List<ModelLineUp> lineups = await GetLineupsFixture(partido.IdPartido);
                   ModelStatsPartido estadisticasPartido = await GetStatsFixture(partido.IdPartido);
                    modelo.partido = partido;
                    modelo.eventos = misEventos;
                    modelo.LineUps = lineups;
                    modelo.EstadisticasPartido = estadisticasPartido;
                    partidos.Add(modelo);
                }
                return partidos;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Equipo>> GetEquiposApiRoot(Root root)
        {
            List<Equipo> listaEquipos = new List<Equipo>();

            Equipo team =await ApiFindEquipo(root.response[0].teams.home.id);
            listaEquipos.Add(team);
            return listaEquipos;
        }

        public async Task<List<TeamRespuesta>> GetEquiposPorComp(int idCompeticion,int season)
        {
            List<TeamRespuesta> lista = new List<TeamRespuesta>();
            var request = "https://v3.football.api-sports.io/teams?league=" + idCompeticion + "&season=" + season;
            var response = await this.httpClient.GetAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                RootTeam rootTeam = JsonConvert.DeserializeObject<RootTeam>(jsonResponse);
                int tamanio = rootTeam.response.Count();
             
                foreach(TeamResponse res in rootTeam.response)
                {
                    TeamRespuesta tr = new TeamRespuesta();

                    Equipo eq = new Equipo();
                    eq.Pais = res.team.country;
                    eq.IdEquipo= res.team.id;
                    eq.Nombre= res.team.name;
                    eq.Abreviatura = res.team.code;
                    eq.Logo= res.team.logo;
                    eq.IdVenue = (int)res.venue.id;

                    tr.equipo= eq;
                    VenueB ven = new VenueB();
                    ven.Capacidad = res.venue.capacity;
                    ven.IdVenue = (int)res.venue.id;
                    ven.Nombre = res.venue.name;
                    ven.Ciudad = res.venue.city;
                    ven.Pais = res.team.country;

                    tr.venue= ven;
                    lista.Add(tr);
                }
            }


            return lista;
        }

        public async Task<EquipoCompStats> GetStatsEquipoComp(int idequipo,int idliga,int season,int posicion,int puntos)
        {
            var request = "https://v3.football.api-sports.io/teams/statistics?season="+season+"&team="+ idequipo + "&league="+idliga;
            var response = await this.httpClient.GetAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {

                RootTeamStats resStats =JsonConvert.DeserializeObject<RootTeamStats>(jsonResponse);
               
                EquipoCompStats misStats= new EquipoCompStats();
                misStats.IdCompeticion = resStats.response.league.id;
                misStats.IdEquipo = resStats.response.team.id;
                misStats.IdTemporada = resStats.response.league.season;
                misStats.Nombre = resStats.response.team.name;

                //PUNTOS
                misStats.Puntos= puntos;
                misStats.Posicion= posicion;

                misStats.logo=resStats.response.team.logo;

               string formaReves= resStats.response.form.Substring(resStats.response.form.Length -4);

                string formaFinal = "";
                char[] chars= formaReves.ToCharArray();
                for (int i = 3; i >= 0; i--)
                {
                    formaFinal+= chars[i];
                }
                misStats.forma = formaFinal;
                misStats.PartidosJugados = resStats.response.fixtures.played.total;
                misStats.PartidosJugadosLocal = resStats.response.fixtures.played.home;
                misStats.PartidosJugadosVisitante = resStats.response.fixtures.played.away;

                misStats.Victorias = resStats.response.fixtures.wins.total;
                misStats.VictoriasLocal = resStats.response.fixtures.wins.home;
                misStats.VictoriasVisitante = resStats.response.fixtures.wins.away;

                misStats.Empates = resStats.response.fixtures.draws.total;
                misStats.EmpatesLocal = resStats.response.fixtures.draws.home;
                misStats.EmpatesVisitante = resStats.response.fixtures.draws.away;

                misStats.Derrotas=resStats.response.fixtures.loses.total;
                misStats.DerrotasLocal = resStats.response.fixtures.loses.home;
                misStats.DerrotasVisitante = resStats.response.fixtures.loses.away;

                misStats.GolesMarcados = resStats.response.goals.@for.total.total;
                misStats.GolesMarcadosLocal = resStats.response.goals.@for.total.home;
                misStats.GolesMarcadosVisitante = resStats.response.goals.@for.total.away;

                misStats.GolesEncajados=resStats.response.goals.against.total.total;
                misStats.GolesEncajadosLocal=resStats.response.goals.against.total.home;
                misStats.GolesEncajadosVisitante=resStats.response.goals.against.total.away;
                int ctotalamarillas = 0;
                foreach (KeyValuePair<string,MinuteCards> test in resStats.response.cards.yellow)
                {
                    if (test.Value.total != null)
                    {
                        string a=test.Key;
                        if (test.Key.Equals("0-15"))
                        {
                            misStats.TarjetasAmarillas0_15= (int)test.Value.total;
                        }
                        else if (test.Key.Equals("16-30"))
                        {
                            misStats.TarjetasRojas16_30 = (int)test.Value.total;
                        }
                        else if (test.Key.Equals("31-45"))
                        {
                            misStats.TarjetasRojas31_45 = (int)test.Value.total;
                        }
                        else if (test.Key.Equals("46-60"))
                        {
                            misStats.TarjetasRojas46_60 = (int)test.Value.total;
                        }
                        else if (test.Key.Equals("61-75"))
                        {
                            misStats.TarjetasRojas61_75 = (int)test.Value.total;
                        }
                        else if (test.Key.Equals("76-90"))
                        {
                            misStats.TarjetasRojas76_90 = (int)test.Value.total;

                        }
                        else if (test.Key.Equals("91-105"))
                        {
                            misStats.TarjetasRojas91_105 = (int)test.Value.total;
                        }
                        else if (test.Key.Equals("106-120"))
                        {
                            misStats.TarjetasRojas106_120 = (int)test.Value.total;
                        }
                        ctotalamarillas += (int)test.Value.total;
                    }
                }

                misStats.TarjetasAmarillas= ctotalamarillas;
                int ctotalrojas = 0;
                foreach (KeyValuePair<string, MinuteCards> test in resStats.response.cards.red)
                {
                    if (test.Value.total != null)
                    {
                        if (test.Key.Equals("0-15"))
                        {

                        }
                        else if (test.Key.Equals("16-30"))
                        {

                        }
                        else if (test.Key.Equals("31-45"))
                        {

                        }
                        else if (test.Key.Equals("46-60"))
                        {

                        }
                        else if (test.Key.Equals("61-75"))
                        {

                        }
                        else if (test.Key.Equals("76-90"))
                        {

                        }
                        else if (test.Key.Equals("91-105"))
                        {

                        }
                        else if (test.Key.Equals("106-120"))
                        {

                        }
                        ctotalrojas += (int)test.Value.total;
                    }
                  
                }
                misStats.TarjetasRojas = ctotalrojas;

                misStats.GolesMarcados=resStats.response.goals.@for.total.total;
                misStats.GolesMarcadosLocal=resStats.response.goals.@for.total.home;
                misStats.GolesMarcadosVisitante=resStats.response.goals.@for.total.away;

                foreach(KeyValuePair<string,MinuteGoals> goles in resStats.response.goals.@for.minute) 
                {
                    if (goles.Value.total != null)
                    {
                        if (goles.Key.Equals("0-15"))
                        {
                            misStats.GolesFavor0_15 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("16-30"))
                        {
                            misStats.GolesFavor16_30 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("31-45"))
                        {
                            misStats.GolesFavor31_45 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("46-60"))
                        {
                            misStats.GolesFavor46_60 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("61-75"))
                        {
                            misStats.GolesFavor61_75 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("76-90"))
                        {
                            misStats.GolesFavor76_90 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("91-105"))
                        {
                            misStats.GolesFavor91_105 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("106-120"))
                        {
                            misStats.GolesFavor106_120 = (int)goles.Value.total;
                        }

                    }
                }
                misStats.GolesEncajados = resStats.response.goals.against.total.total;
                misStats.GolesEncajadosLocal = resStats.response.goals.against.total.home;
                misStats.GolesEncajadosVisitante = resStats.response.goals.against.total.away;

                foreach (KeyValuePair<string, MinuteGoals> goles in resStats.response.goals.against.minute)
                {
                    if (goles.Value.total != null)
                    {
                        if (goles.Key.Equals("0-15"))
                        {
                            misStats.GolesContra0_15 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("16-30"))
                        {
                            misStats.GolesContra16_30 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("31-45"))
                        {
                            misStats.GolesContra31_45 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("46-60"))
                        {
                            misStats.GolesContra46_60 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("61-75"))
                        {
                            misStats.GolesContra61_75 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("76-90"))
                        {
                            misStats.GolesContra76_90 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("91-105"))
                        {
                            misStats.GolesContra91_105 = (int)goles.Value.total;
                        }
                        else if (goles.Key.Equals("106-120"))
                        {
                            misStats.GolesContra106_120 = (int)goles.Value.total;
                        }

                    }
                }

                return misStats;
            }
            return null;
         }

        public async Task<Dictionary<int, Resumen>> GetPosicionesComp(int idComp, int season)
        {
            var request = "https://v3.football.api-sports.io/standings?league=" + idComp+"&season="+season;
            var response = await this.httpClient.GetAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                RootStandings rootStanding= JsonConvert.DeserializeObject<RootStandings>(jsonResponse);


                Dictionary<int, Resumen> Posiciones = new Dictionary<int, Resumen>();
                foreach(Standing stand in rootStanding.response[0].league.standings[0])
                {
                    Resumen resumen = new Resumen();
                    resumen.Puntos = stand.points;
                    resumen.Diferencia = stand.goalsDiff;
                    resumen.Posicion = stand.rank;
                    Posiciones.Add(stand.team.id, resumen);
                }
                return Posiciones;

            }


            return null;
        }

            public async Task<Equipo> ApiFindEquipo(int idTeam)
        {
            
        var request = "https://v3.football.api-sports.io/teams?id=" + idTeam;
         var response = await this.httpClient.GetAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (jsonResponse != null)
            {
                RootTeam rootTeam=JsonConvert.DeserializeObject<RootTeam>(jsonResponse);
                Equipo equipo = new Equipo();
                equipo.IdEquipo = rootTeam.response[0].team.id;
                equipo.Logo = rootTeam.response[0].team.logo;
                equipo.Abreviatura = rootTeam.response[0].team.code;
                equipo.Nombre = rootTeam.response[0].team.name;
                equipo.Pais = rootTeam.response[0].team.country;
                equipo.IdVenue = (int)rootTeam.response[0].venue.id;
                return equipo;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<Competicion>> GetCompeticionesApi(Root root)
        {
           
            //var request = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri("https://v3.football.api-sports.io/fixtures?date=2023-03-10"),

            //    Headers =
            //    {
            //        {"x-rapidapi-key", "9dbcd3b27b1169a084ab6a480be5af6d" },
            //        {"x-rapidapi-host", "v3.football.api-sports.io" }
            //    },
            //};

            //var response = await httpClient.SendAsync(request);

            //response.EnsureSuccessStatusCode();

  
                List<int> listaLeagues= new List<int>();
                List<Competicion> compesInsert= new List<Competicion>();
                foreach(Response res in root.response)
                {
                    listaLeagues.Add(res.league.id);                
                }
                var listaSinDuplicados = listaLeagues.Distinct();

                int cnt = 0;
                foreach (int i in listaSinDuplicados)
                {
                    if (cnt < 299)
                    {

                        Competicion compeinsert = await ApiFindCompeticionAsync(i);
                        compesInsert.Add(compeinsert);
                    }

                    cnt++;
                }
                return compesInsert;
                

            
        }

        public async Task<Competicion> ApiFindCompeticionAsync(int idLeague)
        {
            var request = ("https://v3.football.api-sports.io/leagues?id=") + idLeague;

            var response = await this.httpClient.GetAsync(request);

                string jsonResponse = await response.Content.ReadAsStringAsync();
                if (jsonResponse != null)
                {
                    RootLeague rootLeague = JsonConvert.DeserializeObject<RootLeague>(jsonResponse);

                    Competicion compe = new Competicion();
                    compe.IdCompeticion = rootLeague.response[0].league.id;
                    compe.Nombre = rootLeague.response[0].league.name;
                    compe.Pais = rootLeague.response[0].country.name;
                    compe.Logo = rootLeague.response[0].league.logo;
                    compe.Type = rootLeague.response[0].league.type;
                    compe.BanderaPais = rootLeague.response[0].country.flag.ToString();
                    return compe;
                }
                else
                {
                    return null;
                }

            
        }

        public class ComparadorCompeticion : IEqualityComparer<Competicion>
        {
            public bool Equals(Competicion x, Competicion y)
            {
                return x.IdCompeticion == y.IdCompeticion
                    && x.Nombre == y.Nombre
                    && x.Type == y.Type
                    && x.Logo == y.Logo
                    && x.Pais == y.Pais;
            }

            public int GetHashCode(Competicion obj)
            {
                return obj.IdCompeticion.GetHashCode();
            }
        }
    }
}
