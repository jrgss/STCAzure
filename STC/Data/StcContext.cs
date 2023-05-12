using Microsoft.EntityFrameworkCore;
using STC.Models;

namespace STC.Data
{
    public class StcContext:DbContext
    {
        public StcContext(DbContextOptions<StcContext> options) : base(options) { }

        public DbSet<Competicion> Competiciones { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<VenueB> Venues { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<EquipoCompStats> EquiposCompStats { get;set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<LineupB> Lineups { get; set; }
        public DbSet<LineupPlayer> LineupPlayers { get; set; }
        public DbSet<StatsPartido> StatsPartidos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquipoCompStats>()
                  .HasKey(m => new { m.IdCompeticion, m.IdEquipo,m.IdTemporada });
            modelBuilder.Entity<Partido>()
                 .HasKey(m => new { m.IdLocal, m.IdVisitante, m.IdCompeticion, m.IdTemporada });
            modelBuilder.Entity<LineupPlayer>()
                .HasKey(m => new { m.IdPartido, m.IdEquipo, m.Nombre });
            modelBuilder.Entity<StatsPartido>()
                .HasKey(m => new { m.IdPartido, m.Tipo, m.IdEquipo });
            modelBuilder.Entity<LineupB>()
                .HasKey(m=> new {m.IdPartido,m.IdEquipo});
        }
        
    }
}
