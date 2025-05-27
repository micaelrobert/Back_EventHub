using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using System;
using GestaoEventos.API.Models;

public class PdfService : IPdfService
{
    public byte[] GerarPdfDeIngresso(Ingresso ingresso)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        // Fonte
        var fontTitulo = new XFont("Verdana", 16, XFontStyle.Bold);
        var fontConteudo = new XFont("Verdana", 12, XFontStyle.Regular);

        double margem = 40;
        double larguraUtil = page.Width - 2 * margem;
        double alturaUtil = page.Height - 2 * margem;

        // Desenhar borda
        var bordaRect = new XRect(margem, margem, larguraUtil, alturaUtil);
        gfx.DrawRectangle(XPens.Black, bordaRect);

        // Desenhar título centralizado
        string titulo = $"Ingresso #{ingresso.Id}";
        var tituloRect = new XRect(margem, margem, larguraUtil, 40);
        gfx.DrawString(titulo, fontTitulo, XBrushes.Black, tituloRect, XStringFormats.Center);

        // Conteúdo
        string conteudo = $@"
Nome do Evento: {ingresso.Evento?.Nome}
Local: {ingresso.Evento?.Local}
Data do Evento: {ingresso.Evento?.DataEvento:dd/MM/yyyy HH:mm}

Nome do Comprador: {ingresso.NomeComprador}
Email: {ingresso.EmailComprador}
Telefone: {ingresso.TelefoneComprador}

Valor Pago: R$ {ingresso.ValorPago:F2}
Data da Compra: {ingresso.DataCompra:dd/MM/yyyy HH:mm}";

        var conteudoRect = new XRect(margem + 20, margem + 60, larguraUtil - 40, alturaUtil - 80);
        gfx.DrawString(conteudo.Trim(), fontConteudo, XBrushes.Black, conteudoRect, XStringFormats.TopLeft);

        // Salvar como byte[]
        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }
}
