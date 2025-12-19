using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using System.IO;
using Org.BouncyCastle.X509; // para certificados, aunque no es estrictamente necesario aquí

public static class PdfUtils
{
    public static byte[] UnirPdfs(params byte[][] pdfs)
    {
        using var ms = new MemoryStream();
        using var writer = new PdfWriter(ms);
        using var pdfDestino = new PdfDocument(writer);

        var merger = new PdfMerger(pdfDestino);

        foreach (var pdfBytes in pdfs)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                continue;

            // Importante: PdfReader con permisos para PDFs firmados/cifrados
            var readerProperties = new ReaderProperties();
            using var reader = new PdfReader(new MemoryStream(pdfBytes), readerProperties);
            using var pdfOrigen = new PdfDocument(reader);

            merger.Merge(pdfOrigen, 1, pdfOrigen.GetNumberOfPages());
        }

        pdfDestino.Close();
        return ms.ToArray();
    }
}
