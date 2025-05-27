using FluentValidation;
using GestaoEventos.API.DTO;
using GestaoEventos.API.Utils;
using GestaoEventos.API.Helpers;

namespace GestaoEventos.API.Validators
{
    /// <summary>
    /// Validador para compra de ingressos
    /// </summary>
    public class ComprarIngressoValidator : AbstractValidator<ComprarIngressoDto>
    {
        public ComprarIngressoValidator()
        {
            RuleFor(x => x.EventoId)
                .GreaterThan(0)
                .WithMessage("ID do evento deve ser maior que zero");

            RuleFor(x => x.NomeComprador)
                .NotEmpty()
                .WithMessage("Nome do comprador é obrigatório")
                .MaximumLength(100)
                .WithMessage("Nome deve ter no máximo 100 caracteres")
                .Must(nome => !string.IsNullOrWhiteSpace(nome) && nome.Trim().Contains(' '))
                .WithMessage("Nome deve conter nome e sobrenome");

            RuleFor(x => x.EmailComprador)
                .NotEmpty()
                .WithMessage("Email é obrigatório")
                .MaximumLength(Constants.Validation.EMAIL_MAX_LENGTH)
                .WithMessage($"Email deve ter no máximo {Constants.Validation.EMAIL_MAX_LENGTH} caracteres")
                .Must(GestaoEventos.API.Helpers.Helpers.IsValidPhoneBR)
                .WithMessage("Email deve ter formato válido");

            RuleFor(x => x.TelefoneComprador)
                .NotEmpty()
                .WithMessage("Telefone é obrigatório")
                .MaximumLength(Constants.Validation.TELEFONE_MAX_LENGTH)
                .WithMessage($"Telefone deve ter no máximo {Constants.Validation.TELEFONE_MAX_LENGTH} caracteres")
                .Must(GestaoEventos.API.Helpers.Helpers.IsValidPhoneBR)
                .WithMessage("Telefone deve ter formato válido (brasileiro)");
                

        }
    }

    /// <summary>
    /// Validador para devolução de ingressos
    /// </summary>
    public class DevolverIngressoValidator : AbstractValidator<DevolverIngressoDto>
    {
        public DevolverIngressoValidator()
        {
            RuleFor(x => x.MotivoDevolucao)
                .NotEmpty()
                .WithMessage("Motivo da devolução é obrigatório")
                .MaximumLength(Constants.Validation.MOTIVO_DEVOLUCAO_MAX_LENGTH)
                .WithMessage($"Motivo deve ter no máximo {Constants.Validation.MOTIVO_DEVOLUCAO_MAX_LENGTH} caracteres")
                .MinimumLength(10)
                .WithMessage("Motivo deve ter pelo menos 10 caracteres");
        }
    }
}