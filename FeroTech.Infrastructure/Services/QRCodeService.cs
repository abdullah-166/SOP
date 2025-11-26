using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;

namespace FeroTech.Infrastructure.Services
{
    public class QRCodeService
    {
        private readonly ApplicationDbContext _context;

        public QRCodeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GenerateAndSaveQRCodesAsync(Asset asset)
        {
            var qrCodes = new List<Domain.Entities.QRCode>();
            for (int i = 1; i <= asset.Quantity; i++)
            {
                string qrValue = $"{asset.AssetId}-{Guid.NewGuid()}";

                qrCodes.Add(new Domain.Entities.QRCode
                {
                    QRCodeId = Guid.NewGuid(),
                    AssetId = asset.AssetId,
                    QRCodeValue = qrValue,
                    GeneratedAt = DateTime.UtcNow,
                    Notes = $"QR Code {i} for {asset.Brand} {asset.Modell}"
                });
            }

            _context.QRCodes.AddRange(qrCodes);
            await _context.SaveChangesAsync();

            var pdfBytes = await GenerateQRCodePdfAsync(qrCodes);

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, $"{asset.Brand}_{asset.Modell}_QRCodes.pdf");
            await File.WriteAllBytesAsync(filePath, pdfBytes);
        }

        private async Task<byte[]> GenerateQRCodePdfAsync(IEnumerable<Domain.Entities.QRCode> qrCodes)
        {
            using var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            int x = 50;
            int y = 50;
            int qrSize = 150;
            int verticalSpacing = 200;

            foreach (var qr in qrCodes)
            {
                using var generator = new QRCodeGenerator();
                var qrData = generator.CreateQrCode(qr.QRCodeValue, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrData);
                var qrBytes = qrCode.GetGraphic(40);

                using var ms = new MemoryStream(qrBytes);
                var img = XImage.FromStream(() => ms);

                gfx.DrawImage(img, x, y, qrSize, qrSize);

                var font = new XFont("Arial", 12, XFontStyle.Regular);
                gfx.DrawString(qr.QRCodeValue, font, XBrushes.Black, new XRect(x, y + qrSize + 10, 400, 40), XStringFormats.TopLeft);

                y += verticalSpacing;

                if (y + qrSize + 60 > page.Height)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 50;
                }
            }

            using var stream = new MemoryStream();
            doc.Save(stream, false);
            return await Task.FromResult(stream.ToArray());
        }
    }
}