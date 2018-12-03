using System;
using Microsoft.AspNetCore.Mvc;
using Eventos.IO.Application.ViewModels;
using Eventos.IO.Application.Interfaces;
using Eventos.IO.Domain.Core.Notifications;
using Eventos.IO.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Eventos.IO.Site.Controllers
{
  public class EventosController : BaseController
  {
    private readonly IEventoAppService _eventoAppService;

    public EventosController(IEventoAppService eventoAppService,
                             IDomainNotificationHandler<DomainNotification> notifications,
                             IUser user) : base(notifications, user)
    {
      _eventoAppService = eventoAppService;
    }

    public IActionResult Index()
    {
      return View(_eventoAppService.ObterTodos());
    }

    [Route("meus-eventos")]
    [Authorize(Policy = "PodeLerEventos")]
    public IActionResult MeusEventos()
    {
      return View(_eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
    }

    public IActionResult Details(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

      if (eventoViewModel == null)
      {
        return NotFound();
      }

      return View(eventoViewModel);
    }

    public IActionResult Create()
    {
      return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(EventoViewModel eventoViewModel)
    {
      if (!ModelState.IsValid) return View(eventoViewModel);

      eventoViewModel.OrganizadorId = OrganizadorId;
      _eventoAppService.Registrar(eventoViewModel);

      ViewBag.RetornoPost = OperacaoValida() ? "success,Evento registrado com sucesso!" : "error,Evento não registrado! Verique as mensagens";

      return View(eventoViewModel);
    }

    [Authorize]
    public IActionResult Edit(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

      if (eventoViewModel == null)
      {
        return NotFound();
      }

      if (ValidarAutoridadeEvento(eventoViewModel))
      {
        return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
      }

      return View(eventoViewModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(EventoViewModel eventoViewModel)
    {
      if (ValidarAutoridadeEvento(eventoViewModel))
      {
        return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
      }

      if (!ModelState.IsValid) return View(eventoViewModel);

      eventoViewModel.OrganizadorId = OrganizadorId;
      _eventoAppService.Atualizar(eventoViewModel);

      ViewBag.RetornoPost = OperacaoValida() ? "success,Evento atualizado com sucesso!" : "error,Evento n�o ser atualizado! Verifique as mensagens";

      if (_eventoAppService.ObterPorId(eventoViewModel.Id).Online)
      {
        eventoViewModel.Endereco = null;
      }
      else
      {
        eventoViewModel = _eventoAppService.ObterPorId(eventoViewModel.Id);
      }

      return View(eventoViewModel);
    }

    [Authorize]
    public IActionResult Delete(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoAppService.ObterPorId(id.Value);

      if (ValidarAutoridadeEvento(eventoViewModel))
      {
        return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
      }

      if (eventoViewModel == null)
      {
        return NotFound();
      }

      return View(eventoViewModel);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(Guid id)
    {
      if (ValidarAutoridadeEvento(_eventoAppService.ObterPorId(id)))
      {
        return RedirectToAction("MeusEventos", _eventoAppService.ObterEventoPorOrganizador(OrganizadorId));
      }

      _eventoAppService.Excluir(id);
      return RedirectToAction("Index");
    }

    [Authorize(Policy = "PodeGravar")]
    [Route("incluir-endereco/{id:guid}")]
    public IActionResult IncluirEndereco(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoAppService.ObterPorId(id.Value);
      return PartialView("_IncluirEndereco", eventoViewModel);
    }

    [Authorize(Policy = "PodeGravar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("incluir-endereco/{id:guid}")]
    public IActionResult IncluirEndereco(EventoViewModel eventoViewModel)
    {
      ModelState.Clear();
      eventoViewModel.Endereco.EventoId = eventoViewModel.Id;
      _eventoAppService.AdicionarEndereco(eventoViewModel.Endereco);

      if (OperacaoValida())
      {
        var url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
        return Json(new { success = true, url = url });
      }

      return PartialView("_IncluirEndereco", eventoViewModel);
    }

    [Authorize(Policy = "PodeGravar")]
    [Route("atualizar-endereco/{id:guid}")]
    public IActionResult AtualizarEndereco(Guid? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var eventoViewModel = _eventoAppService.ObterPorId(id.Value);
      return PartialView("_AtualizarEndereco", eventoViewModel);
    }

    [Authorize(Policy = "PodeGravar")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("atualizar-endereco/{id:guid}")]
    public IActionResult AtualizarEndereco(EventoViewModel eventoViewModel)
    {
      ModelState.Clear();
      _eventoAppService.AtualizarEndereco(eventoViewModel.Endereco);

      if (OperacaoValida())
      {
        var url = Url.Action("ObterEndereco", "Eventos", new { id = eventoViewModel.Id });
        return Json(new { success = true, url = url });
      }

      return PartialView("_AtualizarEndereco", eventoViewModel);
    }

    [Route("listar-endereco/{id:guid}")]
    public IActionResult ObterEndereco(Guid id)
    {
      return PartialView("_DetalhesEndereco", _eventoAppService.ObterPorId(id));
    }

    private bool ValidarAutoridadeEvento(EventoViewModel eventoViewModel)
    {
      return eventoViewModel.OrganizadorId != OrganizadorId;
    }
  }
}
