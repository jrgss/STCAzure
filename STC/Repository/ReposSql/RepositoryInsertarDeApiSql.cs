using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using STC.Data;
using STC.Models;
using STC.Repository.Interfaces;
using STC.Services;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace STC.Repository.ReposSql
{
    public class RepositoryInsertarDeApiSql :IRepositoryInsertarDeApi
    {
        private StcContext context;
        //private ServiceApi api;
        public RepositoryInsertarDeApiSql(StcContext context)
        {
            this.context = context;
            //this.api=new ServiceApi();
        }

        //public async void InsertarPartidosDia()
        //{
        //    Competicion compeapi = await this.api.GetCompeticionApi();
        //    InsertarCompeticionesAsync(compeapi);

        //    //InsertarCompeticionesAsync(new Competicion());

        //}

        public void InsertarCompeticion(Competicion competicion)
        {
            //InsertarCompeticionesAsync(competicion);
        }
        public async Task InsertarCompeticionesAsync(List<Competicion> compes)
        {
            foreach(Competicion compe in compes)
            {
                if (BaseFindCompeticion(compe.IdCompeticion) == null)
                {

                    this.context.Competiciones.Add(compe);
                    await this.context.SaveChangesAsync();
                }
                else
                {

                }

            }

            //Competicion compeprueba = new Competicion();
            //compeprueba.IdCompeticion = 2004;
            //compeprueba.Nombre = "TESTMETODO";
            //compeprueba.Pais = "SPAIN";
            //compeprueba.Logo = "ADAWE";
            //compeprueba.Type = "CUP";
             

            //this.context.SaveChanges();
            //}
        }

        public async Task InsertarEquiposYVenues(List<TeamRespuesta> lista)
        {
           
            foreach(TeamRespuesta tr in lista)
            {

                if (await BaseFindEquipo(tr.equipo.IdEquipo) == null)
                {
                    if (tr.venue.IdVenue != null )
                    {
                        if(await BaseFindVenue(tr.venue.IdVenue) == null)
                        {

                            this.context.Venues.Add(tr.venue);
                            await this.context.SaveChangesAsync();
                        }
                    }

                    this.context.Equipos.Add(tr.equipo);
                    await this.context.SaveChangesAsync();
                    
                }
            }
        }

        public async Task InsertarListaPartidos(List<Partido> partidos)
        {
           
                foreach (Partido partido in partidos)
                {
                if (BaseFindPartido(partido.IdLocal,partido.IdVisitante,partido.IdCompeticion,partido.IdTemporada)==null )
                    {
                    this.context.Partidos.Add(partido);
                    await this.context.SaveChangesAsync();
                     }   
                }
            
           
        }
        public async Task InsertarListaPartidosCompleto(List<ModelPartidoCompleto> partidoseventos)
        {
            foreach (ModelPartidoCompleto modelo in partidoseventos)
            {

                if (BaseFindPartido(modelo.partido.IdLocal, modelo.partido.IdVisitante, modelo.partido.IdCompeticion, modelo.partido.IdTemporada) == null)
                {
                    this.context.Partidos.Add(modelo.partido);
                    await this.context.SaveChangesAsync();
                    if (modelo.partido.Estado.Equals("Match Finished"))
                    {

                    
                    foreach (Evento evento in modelo.eventos)
                    {
                        evento.IdEvento = GetMaximoIdEvento();
                        this.context.Eventos.Add(evento);
                        await this.context.SaveChangesAsync();
                    }

                    foreach(ModelLineUp modelLineUp in modelo.LineUps)
                    {
                        //añado a la table LineUp el esquema,partido y equipo
                        this.context.Lineups.Add(modelLineUp.Lineup);
                        await this.context.SaveChangesAsync();

                        //Para cada titular lo añado
                        foreach(LineupPlayer playerLineUp in modelLineUp.Titulares)
                        {
                            this.context.LineupPlayers.Add(playerLineUp);
                            await this.context.SaveChangesAsync();
                        } 
                        //Para cada suplente lo añado
                        foreach(LineupPlayer playerLineUp in modelLineUp.Suplentes)
                        {
                            this.context.LineupPlayers.Add(playerLineUp);
                            await this.context.SaveChangesAsync();
                        }

                    }

                    //Para cada equipo hay una lista de estadísticas
                  
                        //Para cada stat la inserto
                        foreach(StatsPartido miStat in modelo.EstadisticasPartido.statsLocal)
                        {
                            this.context.StatsPartidos.Add(miStat);
                            await this.context.SaveChangesAsync();
                        }
                        foreach (StatsPartido miStat in modelo.EstadisticasPartido.statsVisitante)
                        {
                            this.context.StatsPartidos.Add(miStat);
                            await this.context.SaveChangesAsync();
                        }
                    }


                }
            }
        }

        public async Task<List<ModelPartidoCompleto>> BaseGetPartidosYEventosDiaComp( DateTime dia, int idcomp)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.FechaInicio.Date == dia.Date && datos.IdCompeticion == idcomp
                           select datos;

            List<Partido>partidos= await consulta.ToListAsync();

            List<ModelPartidoCompleto> listaModelo = new List<ModelPartidoCompleto>();
            foreach (Partido partido in partidos)
            {
                ModelPartidoCompleto model = new ModelPartidoCompleto();
                model.partido = partido;
                List<Evento> eventos = await GetEventsFixture(partido.IdPartido);
                model.eventos = eventos;
                listaModelo.Add(model);
            }
            return listaModelo;
        }

        public async Task<ModelPartidoCompleto> BaseFindModeloPartidoYEventos(int idPartido)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.IdPartido==idPartido
                           select datos;
            Partido partido= await consulta.FirstOrDefaultAsync();
            ModelPartidoCompleto model = new ModelPartidoCompleto();
            model.partido = partido;
            List<Evento> eventos = await GetEventsFixture(partido.IdPartido);
            model.eventos = eventos;

            ModelStatsPartido modelstats = new ModelStatsPartido();
             List<StatsPartido> statsLocal = await GetStatsEquipoPartido(partido.IdPartido,partido.IdLocal);
             List<StatsPartido> statsVisitante = await GetStatsEquipoPartido(partido.IdPartido,partido.IdVisitante);

            modelstats.statsLocal = statsLocal;
            modelstats.statsVisitante = statsVisitante;
            model.EstadisticasPartido = modelstats;


            List<ModelLineUp> listaModelsLineups = await GetLineupsPartido(partido.IdPartido, partido.IdLocal,partido.IdVisitante);

            model.LineUps=listaModelsLineups;

         

           

            return model;

        }
        public async Task<List<ModelLineUp>> GetLineupsPartido(int idpartido,int idlocal,int idvisitante)
        {
            List<ModelLineUp> modelo = new List<ModelLineUp>();
            ModelLineUp modelLocal = new ModelLineUp();
            ModelLineUp modelVisitante = new ModelLineUp();

            var consultaLineupB = from datos in this.context.Lineups
                                  where datos.IdPartido== idpartido && datos.IdEquipo== idlocal
                                  select datos;
            LineupB lineupLocal = await consultaLineupB.FirstOrDefaultAsync();
            var consultaJugadoresLocal=from datos in this.context.LineupPlayers
                                       where datos.IdPartido == idpartido && datos.IdEquipo == idlocal
                                       select datos;
            List<LineupPlayer> jugadoresLocal = await consultaJugadoresLocal.ToListAsync();
            List<LineupPlayer> titularesLocal = new List<LineupPlayer>();
            List<LineupPlayer> suplentesLocal = new List<LineupPlayer>();

            foreach(LineupPlayer jugador in jugadoresLocal)
            {
                if (jugador.Grid.Equals("")){
                    suplentesLocal.Add(jugador);
                }
                else
                {
                    titularesLocal.Add(jugador);
                }
            }

            modelLocal.Lineup = lineupLocal;
            modelLocal.Titulares= titularesLocal;
            modelLocal.Suplentes = suplentesLocal;

            //MODELO DEL VISITANTE
            var consultaLineupBVisitante = from datos in this.context.Lineups
                                  where datos.IdPartido == idpartido && datos.IdEquipo == idvisitante
                                  select datos;
            LineupB lineupVisitante = await consultaLineupBVisitante.FirstOrDefaultAsync();

            var consultaJugadoresVisitante = from datos in this.context.LineupPlayers
                                         where datos.IdPartido == idpartido && datos.IdEquipo == idvisitante
                                             select datos;
            List<LineupPlayer> jugadoresVisitante = await consultaJugadoresVisitante.ToListAsync();
            List<LineupPlayer> titularesVisitante = new List<LineupPlayer>();
            List<LineupPlayer> suplentesVisitante = new List<LineupPlayer>();

            foreach (LineupPlayer jugador in jugadoresVisitante)
            {
                if (jugador.Grid.Equals(""))
                {
                    suplentesVisitante.Add(jugador);
                }
                else
                {
                    titularesVisitante.Add(jugador);
                }
            }
            modelVisitante.Lineup = lineupVisitante;
            modelVisitante.Titulares = titularesVisitante;
            modelVisitante.Suplentes = suplentesVisitante;
            modelo.Add(modelLocal);
            modelo.Add(modelVisitante);

            return modelo;

        }
        public async Task InsertarOUpdatearStatsEquipo(EquipoCompStats equipin)
        {
           
            EquipoCompStats existe = BaseFindStatsEquipo(equipin.IdEquipo, equipin.IdCompeticion, equipin.IdTemporada);
            if(existe == null)
            {

                this.context.EquiposCompStats.Add(equipin);
                await this.context.SaveChangesAsync();
            }
            else
            {
                existe.Nombre = equipin.Nombre;
                existe.Puntos= equipin.Puntos;
                existe.PartidosJugados = equipin.PartidosJugados;
                existe.PartidosJugadosLocal = equipin.PartidosJugadosLocal;
                existe.PartidosJugadosVisitante = equipin.PartidosJugadosVisitante;
                existe.Victorias= equipin.Victorias;
                existe.VictoriasLocal= equipin.VictoriasLocal;
                existe.VictoriasVisitante = equipin.VictoriasVisitante;
                existe.Empates= equipin.Empates;
                existe.EmpatesLocal= equipin.EmpatesLocal;
                existe.EmpatesVisitante= equipin.EmpatesVisitante;
                existe.Derrotas= equipin.Derrotas;
                existe.DerrotasLocal= equipin.DerrotasLocal;
                existe.DerrotasVisitante = equipin.DerrotasVisitante;
                existe.GolesMarcados = equipin.GolesMarcados;
                existe.GolesMarcadosLocal = equipin.GolesMarcadosLocal;
                existe.GolesMarcadosVisitante = equipin.GolesMarcadosVisitante;
                existe.GolesEncajados=equipin.GolesEncajados;
                existe.GolesEncajadosLocal = equipin.GolesEncajadosLocal;
                existe.GolesEncajadosVisitante = equipin.GolesEncajadosVisitante;
                existe.TarjetasAmarillas= equipin.TarjetasAmarillas;
                existe.TarjetasAmarillas0_15 = equipin.TarjetasAmarillas0_15;
                existe.TarjetasAmarillas16_30 = equipin.TarjetasAmarillas16_30;
                existe.TarjetasAmarillas31_45 = equipin.TarjetasAmarillas31_45;
                existe.TarjetasAmarillas46_60 = equipin.TarjetasAmarillas46_60;
                existe.TarjetasAmarillas61_75 = equipin.TarjetasAmarillas61_75;
                existe.TarjetasAmarillas76_90 = equipin.TarjetasAmarillas76_90;
                existe.TarjetasAmarillas91_105 = equipin.TarjetasAmarillas91_105;
                existe.TarjetasAmarillas106_120 = equipin.TarjetasAmarillas106_120;
                existe.TarjetasRojas=equipin.TarjetasRojas;
                existe.TarjetasRojas0_15 = equipin.TarjetasRojas0_15;
                existe.TarjetasRojas16_30 = equipin.TarjetasRojas16_30;
                existe.TarjetasRojas31_45 = equipin.TarjetasRojas31_45;
                existe.TarjetasRojas46_60 = equipin.TarjetasRojas46_60;
                existe.TarjetasRojas61_75 = equipin.TarjetasRojas61_75;
                existe.TarjetasRojas76_90 = equipin.TarjetasRojas76_90;
                existe.TarjetasRojas91_105 = equipin.TarjetasRojas91_105;
                existe.TarjetasRojas106_120 = equipin.TarjetasRojas106_120;
                existe.GolesFavor0_15 = equipin.GolesFavor0_15;
                existe.GolesFavor16_30=equipin.GolesFavor16_30;
                existe.GolesFavor31_45 = equipin.GolesFavor31_45;
                existe.GolesFavor46_60 = equipin.GolesFavor46_60;
                existe.GolesFavor61_75 = equipin.GolesFavor61_75;
                existe.GolesFavor76_90 = equipin.GolesFavor76_90;
                existe.GolesFavor91_105 = equipin.GolesFavor91_105;
                existe.GolesFavor106_120 = equipin.GolesFavor106_120;
                existe.GolesContra0_15 = equipin.GolesContra0_15;
                existe.GolesContra16_30 = equipin.GolesContra16_30;
                existe.GolesContra31_45 = equipin.GolesContra31_45;
                existe.GolesContra46_60 = equipin.GolesContra46_60;
                existe.GolesContra61_75 = equipin.GolesContra61_75;
                existe.GolesContra76_90 = equipin.GolesContra76_90;
                existe.GolesContra91_105 = equipin.GolesContra91_105;
                existe.GolesContra106_120 = equipin.GolesContra106_120;
                existe.logo=equipin.logo;
                existe.forma = equipin.forma;
                existe.Posicion=equipin.Posicion;
                
                await this.context.SaveChangesAsync();
            }
        }


        //public async Task<string> TestApiTeamAsync(Root root)
        //{
        //    //string json = await ApiFindCompeticionAsync(root.response[0].league.id); 
        //    string json = await ApiFindCompeticionStringAsync(root.response[0].league.id);
        //    return json;
        //}
       


        //public async void InsertarVenue(Venue venue)
        //{
        //    if (FindVenue(venue) == null &&venue.id!=null)
        //    {
        //        VenueB mivenue = new VenueB();
        //        mivenue.Ciudad = venue.city;
        //        mivenue.IdVenue = (int)venue.id;
        //        mivenue.Nombre = venue.name;
        //        this.context.Venues.Add(mivenue);
        //        await this.context.SaveChangesAsync();

        //    }
        //}
        //public VenueB FindVenue(Venue venue)
        //{

        //    var consulta = from datos in this.context.Venues
        //                   where datos.IdVenue ==venue.id
        //                   select datos;
        //    return consulta.FirstOrDefault();

        //}

        //public VenueB ApiFindVenue(int idVenue)
        //{

        //}

      
        public Partido BaseFindPartido(int idLocal,int idVisitante,int idCompeticion, int idTemporada)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.IdLocal == idLocal && datos.IdVisitante == idVisitante && datos.IdCompeticion == idCompeticion && datos.IdTemporada == idTemporada
                           select datos;

            return consulta.FirstOrDefault();
        }
        public Competicion BaseFindCompeticion(int idLiga)
        {

            var consulta = from datos in this.context.Competiciones
                           where datos.IdCompeticion == idLiga
                           select datos;
            return consulta.FirstOrDefault();
        }

        public async Task<Equipo> BaseFindEquipo(int id)
        {
            var consulta = from datos in this.context.Equipos
                           where datos.IdEquipo == id
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<VenueB> BaseFindVenue(int id)
        {
            var consulta = from datos in this.context.Venues
                           where datos.IdVenue == id
                           select datos;
            
              return await consulta.FirstOrDefaultAsync();
            //return await this.context.Venues.FirstOrDefaultAsync(x => x.IdVenue == id);


        }
        public async Task<List<StatsPartido>> GetStatsEquipoPartido(int idFixture,int idEquipo)
        {
            var consulta = from datos in this.context.StatsPartidos
                           where datos.IdPartido == idFixture &&  datos.IdEquipo==idEquipo
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<List<Evento>> GetEventsFixture(int idFixture)
        {
            var consulta = (from datos in this.context.Eventos
                             where datos.IdPartido == idFixture
                             select datos).OrderBy(x=>x.Elapsed).ThenBy(x=>x.Extra);
            return await consulta.ToListAsync() ;
        }
        public EquipoCompStats BaseFindStatsEquipo(int idEquipo,int idComp,int temporada)
        {
            var consulta = from datos in this.context.EquiposCompStats
                           where datos.IdEquipo == idEquipo && datos.IdCompeticion==idComp && datos.IdTemporada==temporada
                           select datos;
            if (consulta.FirstOrDefault() == null)
            {
                return null;
            }
            else
            {
                return consulta.FirstOrDefault();
            }
           
        }
        public List<Competicion> PruebaGetCompe() 
        {
            var consulta = from datos in this.context.Competiciones 
                           select datos;
            List<Competicion> compes = new List<Competicion>();
            compes = consulta.ToList();
            return compes;
            //return new List<Competicion>();
        }

        public async Task<List<Partido>> GetPartidosDiaComp(DateTime dia,int idcomp)
        {
            var consulta = from datos in this.context.Partidos
                           where datos.FechaInicio.Date==dia.Date && datos.IdCompeticion==idcomp
                           select datos;

            return await consulta.ToListAsync() ;
        }
        public int GetMaximoIdEvento()
        {
            if (this.context.Eventos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Eventos.Max(z => z.IdEvento) + 1;
            }
                
        }
    }
}
