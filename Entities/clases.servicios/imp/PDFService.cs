using PuppeteerSharp;
using PuppeteerSharp.Media;

public class PdfService
{
    public async Task<byte[]> ConvertHtmlToPdfAsync(string html)
    {
        // Descarga de Chromium (solo la primera vez lo guarda en caché)
        var fetcher = new BrowserFetcher();
        var revisionInfo = await fetcher.DownloadAsync();

        // Lanzar el navegador usando la ruta descargada
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = revisionInfo.GetExecutablePath()
        });

        var page = await browser.NewPageAsync();

        await page.SetContentAsync(html, new NavigationOptions
        {
            WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
        });

        // Crear PDF
        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PuppeteerSharp.Media.PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "10mm",
                Bottom = "10mm",
                Left = "10mm",
                Right = "10mm"
            }
        });

        await browser.CloseAsync();
        return pdfBytes;
    }
}
