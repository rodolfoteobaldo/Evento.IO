﻿using Eventos.IO.Domain.Organizadores;
using Eventos.IO.Domain.Organizadores.Repository;
using Eventos.IO.Infra.Data.Context;

namespace Eventos.IO.Infra.Data.Repository
{
  public class OrganizadorRepository : Repository<Organizador>, IOrganizadorRepository
  {
    public OrganizadorRepository(EventosContext context) : base(context)
    {
    }
  }
}
