using AutoMapper;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Eventos.Commands;
using Eventos.IO.Domain.Eventos.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Application.Services
{
  public class EventoAppService : IEventoAppService
  {
    private readonly IBus _bus;
    private readonly IMapper _mapper;
    private readonly IEventoRepository _eventoRepository;

    public EventoAppService(IBus bus, IMapper mapper, IEventoRepository eventoRepository)
    {
      _bus = bus;
      _mapper = mapper;
      _eventoRepository = eventoRepository;
    }

    public void Registrar(EventoViewModel eventoViewModel)
    {
      var registroCommand = _mapper.Map<RegistrarEventoCommand>(eventoViewModel);
      _bus.SendCommand(registroCommand);
    }

    public IEnumerable<EventoViewModel> ObterTodos()
    {
      return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterTodos());
    }

    public IEnumerable<EventoViewModel> ObterEventoPorOrganizador(Guid organizadorId)
    {
      return _mapper.Map<IEnumerable<EventoViewModel>>(_eventoRepository.ObterEventoPorOrganizador(organizadorId));
    }

    public EventoViewModel ObterPorId(Guid id)
    {
      return _mapper.Map<EventoViewModel>(_eventoRepository.ObterPorId(id));
    }

    public void Atualizar(EventoViewModel eventoViewModel)
    {
      // TODO: Validar se o organizador é dono do evento

      var atualizarEventoCommand = _mapper.Map<AtualizarEventoCommand>(eventoViewModel);
      _bus.SendCommand(atualizarEventoCommand);
    }

    public void Excluir(Guid id)
    {
      _bus.SendCommand(new ExcluirEventoCommand(id));
    }


    public void AdicionarEndereco(EnderecoViewModel enderecoViewModel)
    {
      throw new NotImplementedException();
    }

    public void AtualizarEndereco(EnderecoViewModel enderecoViewModel)
    {
      throw new NotImplementedException();
    }

    public EnderecoViewModel ObterEnderecoPorId(Guid id)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      _eventoRepository.Dispose();
    }
  }
}
