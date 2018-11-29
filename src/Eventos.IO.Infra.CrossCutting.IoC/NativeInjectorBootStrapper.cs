using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Eventos.IO.Infra.CrossCutting.IoC
{
  public class NativeInjectorBootStrapper
  {

    public static void RegisterServices(IServiceCollection services)
    {
      //Application
      services.AddSingleton(Mapper.Configuration);
      services.AddScoped<IEventoAppService, EventoAppService>();

      //Domain - Commands

      //Domain - Eventos

      //Infra - Data
    }
  }
}
