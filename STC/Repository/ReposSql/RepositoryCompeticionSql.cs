using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using STC.Data;
using STC.Models;
using STC.Repository.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics.Metrics;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Scaffolding;
using NuGet.Packaging;

namespace STC.Repository.ReposSql
{
    #region
    //    CREATE PROCEDURE SP_PROCEDURE_COMPETICION_PAIS(@IDPAIS INT)
    //AS

    //    SELECT* FROM COMPETICION WHERE IDPAIS=@IDPAIS
    //GO


    //    CREATE PROCEDURE SP_ULTIMOS_PARTIDOS(@IDCOMPETICION INT,@CANTIDAD INT)
    //    AS
    //SELECT IDPARTIDO, IDLOCAL, IDVISITANTE, PARTIDO.IDCOMPTEMP,IDARBITRO,FECHAINICIO FROM PARTIDO INNER JOIN COMPETICION_DURANTE_TEMPORADA ON PARTIDO.IDCOMPTEMP=COMPETICION_DURANTE_TEMPORADA.IDCOMPTEMP WHERE IDCOMPETICION= @IDCOMPETICION ORDER BY FECHAINICIO ASC offset 0 rows fetch next @CANTIDAD rows only
    //GO


    //ALTER PROCEDURE SP_PROCEDURE_PARTIDOS_COMP(@IDCOMPETICION INT, @TEMPORADA INT, @POSICION INT, @CANTIDAD INT, @NUMREGISTROS INT OUT)
    //AS
    //SELECT @NUMREGISTROS = COUNT(IDPARTIDO) FROM PARTIDO WHERE IDCOMPETICION = @IDCOMPETICION AND IDTEMPORADA = @TEMPORADA AND ESTADO = 'Match Finished'
    //SELECT IDPARTIDO, IDCOMPETICION, IDTEMPORADA, ARBITRO, TIMEZONE, STARTDATE, IDVENUE, IDLOCAL, IDVISITANTE, GOLESLOCALHF, GOLESVISITANTEHF, GOLESLOCALFT, GOLESVISITANTEFT, GOLESLOCALET, GOLESVISITANTEET, GOLESLOCALPENALTY, GOLESVISITANTEPENALTY, RONDA, ESTADO, ESTADOFINAL, ELAPSED, NOMBRELOCAL, NOMBREVISITANTE, LOGOLOCAL, LOGOVISITANTE, PAIS, PAISIMAGEN, NOMBRECOMPETICION, NOMBREVENUE FROM(
    //SELECT CAST(ROW_NUMBER() OVER(ORDER BY STARTDATE)AS INT) AS POSICION,
    //IDPARTIDO, IDCOMPETICION, IDTEMPORADA, ARBITRO, TIMEZONE, STARTDATE, IDVENUE, IDLOCAL, IDVISITANTE, GOLESLOCALHF, GOLESVISITANTEHF, GOLESLOCALFT, GOLESVISITANTEFT, GOLESLOCALET, GOLESVISITANTEET, GOLESLOCALPENALTY, GOLESVISITANTEPENALTY, RONDA, ESTADO, ESTADOFINAL, ELAPSED, NOMBRELOCAL, NOMBREVISITANTE, LOGOLOCAL, LOGOVISITANTE, PAIS, PAISIMAGEN, NOMBRECOMPETICION, NOMBREVENUE
    //FROM PARTIDO WHERE IDCOMPETICION = @IDCOMPETICION AND IDTEMPORADA = @TEMPORADA AND ESTADO = 'Match Finished' ) AS QUERY
    //WHERE QUERY.POSICION >@NUMREGISTROS-@POSICION-@CANTIDAD AND QUERY.POSICION<=(@NUMREGISTROS-@POSICION) ORDER BY QUERY.STARTDATE DESC
    //GO


    //    CREATE PROCEDURE SP_COMPETICIONES
    //AS
    //SELECT* FROM COMPETICION WHERE IDCOMPETICION=135 OR IDCOMPETICION = 140 OR IDCOMPETICION = 39 OR IDCOMPETICION = 61 OR IDCOMPETICION = 78
    //GO
    #endregion
    public class RepositoryCompeticionSql : IRepositoryCompeticion
    {
        private StcContext context;
        public RepositoryCompeticionSql(StcContext context)
        {
            this.context = context;
        }

        public List<Competicion> GetCompeticionesPais(int IdPais)
        {
            string sql = "SP_PROCEDURE_COMPETICION_PAIS @IDPAIS";
            SqlParameter pamid = new SqlParameter("@IDPAIS", IdPais);
            
            var consulta = this.context.Competiciones.FromSqlRaw(sql,pamid);
            List<Competicion> competiciones = consulta.AsEnumerable().ToList();
            return competiciones;
        }

        public List<VistaEquiposCompeticion> GetEquiposComp(int idComptemp)
        {
            throw new NotImplementedException();
        }

        public List<Partido> GetUltimosPartidosComp(int idComp, int cantidad)
        {
            string sql = "SP_ULTIMOS_PARTIDOS @IDCOMPETICION,@CANTIDAD";
            SqlParameter pamidcomp = new SqlParameter("@IDCOMPETICION", idComp);
            SqlParameter pamcantidad = new SqlParameter("@CANTIDAD", cantidad);
            var consulta = this.context.Partidos.FromSqlRaw(sql, pamidcomp, pamcantidad);
            List<Partido> partidos = consulta.AsEnumerable().ToList();
            return partidos;
        }

        public async Task<List<Competicion>> GetCompeticiones()
        {
            string sql = "SP_COMPETICIONES";
            var consulta = this.context.Competiciones.FromSqlRaw(sql);
            List<Competicion> compes = consulta.AsEnumerable().ToList();
            return compes;
        }

        public async Task<List<EquipoCompStats>> GetCompeticionStandings(int idComp,int season)
        {
            var consulta = (from datos in this.context.EquiposCompStats
                           where datos.IdCompeticion == idComp && datos.IdTemporada == season
                           select datos).OrderBy(x=>x.Posicion);

            return await consulta.ToListAsync();
        }
        public Competicion BaseFindCompeticion(int idLiga)
        {

            var consulta = from datos in this.context.Competiciones
                           where datos.IdCompeticion == idLiga
                           select datos;
            return consulta.FirstOrDefault();
        }

        public async Task<ModelCompeticionPartidos> GetUltimosPartidosComp(int idcomp,int season,int cantidad,int posicion)
        {
            string sql = "SP_PROCEDURE_PARTIDOS_COMP @IDCOMPETICION ,@TEMPORADA, @POSICION , @CANTIDAD , @NUMREGISTROS OUT";
            SqlParameter pamidcomp = new SqlParameter("@IDCOMPETICION", idcomp);
            SqlParameter pamseason = new SqlParameter("@TEMPORADA", season);
            SqlParameter pamposicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamcantidad = new SqlParameter("@CANTIDAD", cantidad);
            SqlParameter pamregistros = new SqlParameter("@NUMREGISTROS", -1);
            pamregistros.Direction = ParameterDirection.Output;
            var consulta = this.context.Partidos.FromSqlRaw(sql,pamidcomp,pamseason,pamposicion,pamcantidad,pamregistros);
            List<Partido> partidos = await consulta.ToListAsync();


            int registros = (int)pamregistros.Value;

            ModelCompeticionPartidos modelo = new ModelCompeticionPartidos();
            modelo.partidos = partidos;
            Competicion compe = BaseFindCompeticion(idcomp);
            modelo.competicion = compe;
            modelo.registros = registros;
            return modelo;
        }
        public async Task<Partido> GetUltimoPartidoDisputado(int idcomp,int season)
        {
            var consulta=(from datos in this.context.Partidos
                         where datos.IdCompeticion==idcomp && datos.IdTemporada==season && datos.Estado=="Match Finished"
                         select datos).OrderByDescending(x => x.FechaInicio);
            Partido partido=await consulta.FirstOrDefaultAsync();
            return partido;
        }
        public async Task<List<Partido>> GetProximosPartidos(int idcomp,int season,int cantidad)
        {
            var consulta = (from datos in this.context.Partidos
                            where datos.IdCompeticion == idcomp && datos.IdTemporada == season && datos.Estado != "Match Finished" && datos.FechaInicio.Date > new DateTime()
                            select datos).OrderByDescending(x => x.FechaInicio);

            List<Partido> proximosPartidos = await consulta.ToListAsync();
            return proximosPartidos;
        }
        public async Task<ResumenPartidosCompeticion> GetResumenCompeticion(int idComp,int season)
        {
            ResumenPartidosCompeticion resumen = new ResumenPartidosCompeticion();
            var consulta = (from datos in this.context.Partidos
                            where datos.IdCompeticion==idComp && datos.IdTemporada==season && datos.EstadoFinal=="FT"
                            select datos).OrderByDescending(x => x.FechaInicio);

            Partido partido = await consulta.FirstOrDefaultAsync();
            resumen.UltimoPartido = partido;

            //var consulta= from datos in this.context.Partidos
            List<Partido> UltimosPartidos;



            return resumen;
            
        }

    }
}
