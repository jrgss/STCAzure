using STC.Models;

namespace STC.Repository.Interfaces
{
    public interface IRepositoryPartido
    {
        public List<Partido> GetUltimosPartidosComp(int idComptemp);
    }
}
