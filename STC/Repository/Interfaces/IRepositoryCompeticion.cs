using STC.Models;

namespace STC.Repository.Interfaces
{
    public interface IRepositoryCompeticion
    {
        public List<Competicion> GetCompeticionesPais(int IdPais);
        //public List<Competicion> GetCompeticionTemporadas(int IdCompeticion);
        //public Competicion GetCompeticion(int IdCompeticion,int idTemporada);
        public List<Partido> GetUltimosPartidosComp(int idComp,int cantidad);
        public List<VistaEquiposCompeticion> GetEquiposComp(int idComptemp);
        public Competicion BaseFindCompeticion(int idLiga);
        public  Task<List<EquipoCompStats>> GetCompeticionStandings(int idComp, int season);
        public Task<ModelCompeticionPartidos> GetUltimosPartidosComp(int idcomp, int season, int cantidad, int posicion);
        public Task<List<Competicion>> GetCompeticiones();
        public  Task<Partido> GetUltimoPartidoDisputado(int idcomp, int season);
        public  Task<List<Partido>> GetProximosPartidos(int idcomp, int season,int cantidad);

    }
}
