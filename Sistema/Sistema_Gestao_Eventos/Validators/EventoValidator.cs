using FluentValidation;
using GestaoEventos.API.DTO;
using GestaoEventos.API.Utils;

namespace GestaoEventos.API.Validators
{
    /// <summary>
    /// Validador para criação de eventos
    /// </summary>
    public class CriarEventoValidator : AbstractValidator<CriarEventoDto>
    {
        public CriarEventoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome é obrigatório")
                .MaximumLength(Constants.Validation.NOME_MAX_LENGTH)
                .WithMessage($"Nome deve ter no máximo {Constants.Validation.NOME_MAX_LENGTH} caracteres");

            RuleFor(x => x.Descricao)
                .MaximumLength(Constants.Validation.DESCRICAO_MAX_LENGTH)
                .WithMessage($"Descrição deve ter no máximo {Constants.Validation.DESCRICAO_MAX_LENGTH} caracteres");

            RuleFor(x => x.DataEvento)
                .NotEmpty()
                .WithMessage("Data do evento é obrigatória")
                .Must(Helpers.IsFutureDate)
                .WithMessage("Data do evento deve ser futura");

            RuleFor(x => x.Local)
                .NotEmpty()
                .WithMessage("Local é obrigatório")
                .MaximumLength(Constants.Validation.LOCAL_MAX_LENGTH)
                .WithMessage($"Local deve ter no máximo {Constants.Validation.LOCAL_MAX_LENGTH} caracteres");

            RuleFor(x => x.PrecoIngresso)
                .GreaterThanOrEqualTo(Constants.Validation.PRECO_MINIMO)
                .WithMessage($"Preço deve ser maior ou igual a {Constants.Validation.PRECO_MINIMO:C}");

            RuleFor(x => x.CapacidadeMaxima)
                .GreaterThanOrEqualTo(Constants.Validation.CAPACIDADE_MINIMA)
                .WithMessage($"Capacidade deve ser maior ou igual a {Constants.Validation.CAPACIDADE_MINIMA}");
        }
    }

    /// <summary>
    /// Validador para atualização de eventos
    /// </summary>
    public class AtualizarEventoValidator : AbstractValidator<AtualizarEventoDto>
    {
        public AtualizarEventoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome é obrigatório")
                .MaximumLength(Constants.Validation.NOME_MAX_LENGTH)
                .WithMessage($"Nome deve ter no máximo {Constants.Validation.NOME_MAX_LENGTH} caracteres");

            RuleFor(x => x.Descricao)
                .MaximumLength(Constants.Validation.DESCRICAO_MAX_LENGTH)
                .WithMessage($"Descrição deve ter no máximo {Constants.Validation.DESCRICAO_MAX_LENGTH} caracteres");

            RuleFor(x => x.DataEvento)
                .NotEmpty()
                .WithMessage("Data do evento é obrigatória")
                .Must(Helpers.IsFutureDate)
                .WithMessage("Data do evento deve ser futura");

            RuleFor(x => x.Local)
                .NotEmpty()
                .WithMessage("Local é obrigatório")
                .MaximumLength(Constants.Validation.LOCAL_MAX_LENGTH)
                .WithMessage($"Local deve ter no máximo {Constants.Validation.LOCAL_MAX_LENGTH} caracteres");

            RuleFor(x => x.PrecoIngresso)
                .GreaterThanOrEqualTo(Constants.Validation.PRECO_MINIMO)
                .WithMessage($"Preço deve ser maior ou igual a {Constants.Validation.PRECO_MINIMO:C}");

            RuleFor(x => x.CapacidadeMaxima)
                .GreaterThanOrEqualTo(Constants.Validation.CAPACIDADE_MINIMA)
                .WithMessage($"Capacidade deve ser maior ou igual a {Constants.Validation.CAPACIDADE_MINIMA}");
        }
    }
}