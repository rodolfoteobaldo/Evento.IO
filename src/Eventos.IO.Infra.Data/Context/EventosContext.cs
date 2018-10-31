using Eventos.IO.Domain.Eventos;
using Eventos.IO.Domain.Organizadores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Eventos.IO.Infra.Data.Context
{
  public class EventosContext : DbContext
  {
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Organizador> Organizadores { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      #region FlunetAPI

      #region Evento

      modelBuilder.Entity<Evento>()
        .Property(e => e.Nome)
        .HasColumnType("varchar(150)")
        .IsRequired();

      modelBuilder.Entity<Evento>()
        .Property(e => e.DescricaoCurta)
        .HasColumnType("varchar(150)");

      modelBuilder.Entity<Evento>()
        .Property(e => e.DescricaoLonga)
        .HasColumnType("varchar(max)");

      modelBuilder.Entity<Evento>()
        .Property(e => e.NomeEmpresa)
        .HasColumnType("varchar(150)")
        .IsRequired();

      modelBuilder.Entity<Evento>()
        .Ignore(e => e.ValidationResult);

      modelBuilder.Entity<Evento>()
        .Ignore(e => e.Tags);

      modelBuilder.Entity<Evento>()
        .Ignore(e => e.CascadeMode);

      modelBuilder.Entity<Evento>()
        .ToTable("Eventos");

      modelBuilder.Entity<Evento>()
        .HasOne(e => e.Organizador)
        .WithMany(o => o.Eventos)
        .HasForeignKey(e => e.OrganizadorId);

      modelBuilder.Entity<Evento>()
        .HasOne(e => e.Categoria)
        .WithMany(o => o.Eventos)
        .HasForeignKey(e => e.CategoriaId)
        .IsRequired(false);

      #endregion

      #region Endereco

      modelBuilder.Entity<Endereco>()
        .HasOne(c => c.Evento)
        .WithOne(c => c.Endereco)
        .HasForeignKey<Endereco>(c => c.EventoId)
        .IsRequired(false);

      modelBuilder.Entity<Endereco>()
        .Ignore(e => e.ValidationResult);

      modelBuilder.Entity<Endereco>()
        .Ignore(e => e.CascadeMode);

      modelBuilder.Entity<Endereco>()
        .ToTable("Enderecos");

      #endregion

      #region Categoria

      modelBuilder.Entity<Categoria>()
       .Ignore(e => e.ValidationResult);

      modelBuilder.Entity<Categoria>()
        .Ignore(e => e.CascadeMode);

      modelBuilder.Entity<Categoria>()
        .ToTable("Categorias");

      #endregion

      #region Organizador

      modelBuilder.Entity<Organizador>()
       .Ignore(e => e.ValidationResult);

      modelBuilder.Entity<Organizador>()
        .Ignore(e => e.CascadeMode);

      modelBuilder.Entity<Organizador>()
        .ToTable("Organizadores");

      #endregion

      #endregion

      base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

      optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }
  }
}
