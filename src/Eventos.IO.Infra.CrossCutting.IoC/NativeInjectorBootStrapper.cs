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
using Eventos.IO.Infra.CrossCutting.Bus;
using Eventos.IO.Infra.Data.Context;
using Eventos.IO.Infra.Data.Repository;
using Eventos.IO.Infra.Data.UoW;
using Microsoft.Extensions.DependencyInjection;

namespace Eventos.IO.Infra.CrossCutting.IoC
{
  public class NativeInjectorBootStrapper
  {

    public static void RegisterServices(IServiceCollection services)
    {
      //Application
      services.AddSingleton(Mapper.Configuration);
      services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
      services.AddScoped<IEventoAppService, EventoAppService>();

      //Domain - Commands
      services.AddSingleton<IHandler<RegistrarEventoCommand>, EventoCommandHandler>();
      services.AddSingleton<IHandler<AtualizarEventoCommand>, EventoCommandHandler>();
      services.AddSingleton<IHandler<ExcluirEventoCommand>, EventoCommandHandler>();

      //Domain - Eventos
      services.AddScoped<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();
      services.AddSingleton<IHandler<EventoRegistradoEvent>, EventoEventHandler>();
      services.AddSingleton<IHandler<EventoAtualizadoEvent>, EventoEventHandler>();
      services.AddSingleton<IHandler<EventoExcluidoEvent>, EventoEventHandler>();

      //Infra - Data
      services.AddScoped<IEventoRepository, EventoRepository>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<EventosContext>();

      //Infra - Bus
      services.AddSingleton<IBus, InMemoryBus>();
    }
  }
}
