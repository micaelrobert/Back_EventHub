using GestaoEventos.API.Models;

public interface IPdfService
{
    byte[] GerarPdfDeIngresso(Ingresso ingresso);
}
