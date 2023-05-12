using STC.Models;

namespace STC.Repository.Interfaces
{
    public interface IRepositoryInsertarDeApi
    {
        //public void InsertarPartidosDia();
        public void InsertarCompeticion(Competicion competicion);
        public  Task InsertarCompeticionesAsync(List<Competicion> compes);
        //public Task<string> TestApiTeamAsync(Root root);
        public Task InsertarEquiposYVenues(List<TeamRespuesta> lista);
        public List<Competicion> PruebaGetCompe();
        public Task InsertarOUpdatearStatsEquipo(EquipoCompStats equipin);
        public Task InsertarListaPartidos(List<Partido> partidos);
        public Task InsertarListaPartidosCompleto(List<ModelPartidoCompleto> partidoseventos);
        public  Task<List<Partido>> GetPartidosDiaComp(DateTime dia,int idcomp);
        public Competicion BaseFindCompeticion(int idLiga);
        public Task<Equipo> BaseFindEquipo(int id);
        public  Task<VenueB> BaseFindVenue(int id);
        public Task<List<ModelPartidoCompleto>> BaseGetPartidosYEventosDiaComp(DateTime dia, int idcomp);
        public  Task<ModelPartidoCompleto> BaseFindModeloPartidoYEventos(int idPartido);

        //CREATE TABLE EVENTO (ELAPSED INT,EXTRA INT,IDEQUIPO INT,NOMBREJUGADOR nvarchar(100),NOMBREASISTENCIA nvarchar(100),TIPO nvarchar(30),DETALLES nvarchar(40),COMMENTS nvarchar(40))
    }
}
