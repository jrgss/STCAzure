using Microsoft.EntityFrameworkCore;
using STC.Data;
using STC.Models;

namespace STC.Repository.ReposSql
{
    public class RepositoryEquipoSql
    {
        private StcContext context;
        public RepositoryEquipoSql(StcContext context)
        {
            this.context = context;
        }
        public async Task<EquipoCompStats> GetEquipoCompStatas(int idComp,int season, int idequipo)
        {
            var consulta = from datos in this.context.EquiposCompStats
                           where datos.IdCompeticion == idComp && datos.IdTemporada == season
                          && datos.IdEquipo == idequipo
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

    }
}
