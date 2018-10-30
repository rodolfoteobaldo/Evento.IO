﻿using Eventos.IO.Domain.CommandHandlers;
using Eventos.IO.Domain.Core.Bus;
using Eventos.IO.Domain.Core.Events;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Eventos.Events;
using Eventos.IO.Domain.Eventos.Repository;
using Eventos.IO.Domain.Interfaces;
using System;

namespace Eventos.IO.Domain.Eventos.Commands
{
  public class EventoCommandHandler : CommandHandler,
      IHandler<RegistrarEventoCommand>,
      IHandler<AtualizarEventoCommand>,
      IHandler<ExcluirEventoCommand>

  {
    private readonly IEventoRepository _eventoRepository;
    private readonly IBus _bus;

    public EventoCommandHandler(IEventoRepository eventoRepository,
                                IUnitOfWork uow,
                                IBus bus,
                                IDomainNotificationHandler<DomainNotification> notifications) : base(uow, bus, notifications)
    {
      _eventoRepository = eventoRepository;
      _bus = bus;
    }

    public void Handle(RegistrarEventoCommand message)
    {
      var evento = Evento.EventoFactory.NovoEventoCompleto(message.Id, message.Nome, message.DescricaoCurta,
          message.DescricaoLonga, message.DataInicio, message.DataFim, message.Gratuito, message.Valor,
          message.Online, message.NomeEmpresa, message.OrganizadorId, message.Endereco, message.Categoria.Id);

      if (!EventoValido(evento)) return;

      // TODO:
      // Validacoes de negocio!
      // Organizador pode registrar evento?

      _eventoRepository.Adicionar(evento);

      if (Commit())
      {
        _bus.RaiseEvent(new EventoRegistradoEvent(evento.Id, evento.Nome, evento.DataInicio, evento.DataFim, evento.Gratuito, evento.Valor, evento.Online, evento.NomeEmpresa));
      }
    }

    public void Handle(AtualizarEventoCommand message)
    {
      if (!EventoExistente(message.Id, message.MessageType)) return;

      var eventoAtual = _eventoRepository.ObterPorId(message.Id);

      var evento = Evento.EventoFactory.NovoEventoCompleto(message.Id, message.Nome, message.DescricaoCurta,
          message.DescricaoLonga, message.DataInicio, message.DataFim, message.Gratuito, message.Valor,
          message.Online, message.NomeEmpresa, message.OrganizadorId, eventoAtual.Endereco, message.Categoria.Id);

      if (!EventoValido(evento)) return;

      _eventoRepository.Atualizar(evento);

      if (Commit())
      {
        _bus.RaiseEvent(new EventoAtualizadoEvent(evento.Id, evento.Nome, evento.DescricaoCurta, evento.DescricaoLonga, evento.DataInicio, evento.DataFim, evento.Gratuito, evento.Valor, evento.Online, evento.NomeEmpresa));
      }
    }

    public void Handle(ExcluirEventoCommand message)
    {
      if (!EventoExistente(message.Id, message.MessageType)) return;

      _eventoRepository.Remover(message.Id);

      if (Commit())
      {
        _bus.RaiseEvent(new EventoExcluidoEvent(message.Id));
      }
    }

    private bool EventoValido(Evento evento)
    {
      if (evento.EhValido()) return true;

      NotificarValidacoesErro(evento.ValidationResult);
      return false;
    }

    private bool EventoExistente(Guid id, string messageType)
    {
      var evento = _eventoRepository.ObterPorId(id);

      if (evento != null) return true;

      _bus.RaiseEvent(new DomainNotification(messageType, "Evento não encontrado."));
      return false;
    }
  }
}