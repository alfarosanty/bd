using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/pdf")]
public class PdfController : ControllerBase
{
    private readonly PdfService _pdfService;

    public PdfController(PdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost("convert")]
    public async Task<IActionResult> ConvertToPdf([FromBody] PdfRequest req)
    {
        var pdfBytes = await _pdfService.ConvertHtmlToPdfAsync(req.Html);
        return File(pdfBytes, "application/pdf", "factura.pdf");
    }

        [HttpPost("test")]
    public async Task<IActionResult> TestPdf()
    {
        string html = @"";


        var pdfBytes = await _pdfService.ConvertHtmlToPdfAsync(html);

        return File(pdfBytes, "application/pdf", "prueba.pdf");
    }
}

public class PdfRequest
{
    public string Html { get; set; }
}
