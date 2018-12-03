using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.Services;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Eventos.Commands;
using Eventos.IO.Domain.Eventos.Events;
using Eventos.IO.Domain.Eventos.Repository;
using Eventos.IO.Domain.Interfaces;
using Eventos.IO.Domain.Organizadores.Commands;
using Eventos.IO.Domain.Organizadores.Events;
using Eventos.IO.Domain.Organizadores.Repository;
using Eventos.IO.Infra.CrossCutting.Bus;
using Eventos.IO.Infra.CrossCutting.Identity.Models;
using Eventos.IO.Infra.CrossCutting.Identity.Services;
using Eventos.IO.Infra.Data.Context;
using Eventos.IO.Infra.Data.Repository;
using Eventos.IO.Infra.Data.UoW;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Eventos.IO.Infra.CrossCutting.IoC
{
  public class NativeInjectorBootStrapper
  {

    public static void RegisterServices(IServiceCollection services)
    {
      // ASPNET
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      //Application
      services.AddSingleton(Mapper.Configuration);
      services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
      services.AddScoped<IEventoAppService, EventoAppService>();
      services.AddScoped<IOrganizadorAppService, OrganizadorAppService>();

      //Domain - Commands
      services.AddSingleton<IHandler<RegistrarEventoCommand>, EventoCommandHandler>();
      services.AddSingleton<IHandler<AtualizarEventoCommand>, EventoCommandHandler>();
      services.AddSingleton<IHandler<ExcluirEventoCommand>, EventoCommandHandler>();

      services.AddSingleton<IHandler<RegistrarOrganizadorCommand>, OrganizadorCommandHandler>();

      //Domain - Eventos
      services.AddScoped<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();
      services.AddSingleton<IHandler<EventoRegistradoEvent>, EventoEventHandler>();
      services.AddSingleton<IHandler<EventoAtualizadoEvent>, EventoEventHandler>();
      services.AddSingleton<IHandler<EventoExcluidoEvent>, EventoEventHandler>();

      services.AddSingleton<IHandler<OrganizadorRegistradoEvent>, OrganizadorEventHandler>();

      //Infra - Data
      services.AddScoped<IEventoRepository, EventoRepository>();
      services.AddScoped<IOrganizadorRepository, OrganizadorRepository>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<EventosContext>();

      //Infra - Bus
      services.AddSingleton<IBus, InMemoryBus>();

      //Infra - Identity
      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();
      services.AddScoped<IUser, AspNetUser>();
    }
  }
}
