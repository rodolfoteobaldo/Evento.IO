﻿using Eventos.IO.Domain.Eventos;
using Eventos.IO.Domain.Organizadores;
using Eventos.IO.Infra.Data.Extensions;
using Eventos.IO.Infra.Data.Mappings;
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
      modelBuilder.AddConfiguration(new EventoMapping());
      modelBuilder.AddConfiguration(new CategoriaMapping());
      modelBuilder.AddConfiguration(new EnderecoMapping());
      modelBuilder.AddConfiguration(new OrganizadorMapping());

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
