using BlumeAPI.Services;
using PuppeteerSharp;
using PuppeteerSharp.Media;

public class PdfService : IPdfService
{
    public async Task<byte[]> convertHtmlToPdfAsync(string html)
    {
        var fetcher = new BrowserFetcher();
        var revisionInfo = await fetcher.DownloadAsync();

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = revisionInfo.GetExecutablePath()
        });

        var page = await browser.NewPageAsync();

        await page.SetContentAsync(html);

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true
        });

        await browser.CloseAsync();

        return pdfBytes;
    }
}

