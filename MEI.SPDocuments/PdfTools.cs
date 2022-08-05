using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;

using MEI.SPDocuments.PdfSharp.Pdf.IO;

using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;

using IO_PdfReader = PdfSharp.Pdf.IO.PdfReader;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using PdfPage = PdfSharp.Pdf.PdfPage;
using PdfReader = iTextSharp.text.pdf.PdfReader;
using Rectangle = iTextSharp.text.Rectangle;

namespace MEI.SPDocuments
{
    public interface IPdfTools
    {
        /// <summary>
        ///     Watermarks a pdf file with the text 'VOID'.
        /// </summary>
        /// <param name="contents">The contents of the file to watermark.</param>
        /// <param name="repeatX">The number of horizontal tiles.</param>
        /// <param name="repeatY">The number of vertical tiles.</param>
        /// <param name="waterMarkText">The text to use for the watermark.</param>
        /// <returns>A <see cref="byte" /> array containing the contents of the watermarked pdf file.</returns>
        byte[] WatermarkDocument(byte[] contents, int repeatX, int repeatY, string waterMarkText);

        /// <summary>
        ///     Watermarks the center of a pdf file.
        /// </summary>
        /// <param name="contents">The contents of the file to watermark.</param>
        /// <param name="watermarkText">The watermark text.</param>
        /// <param name="watermarkStyle">The watermark style.</param>
        /// <returns>A <see cref="byte" /> array containing the contents of the watermarked pdf file.</returns>
        byte[] WatermarkDocumentCenter(byte[] contents, string watermarkText, WatermarkTextDrawStyle watermarkStyle);

        /// <summary>
        ///     Watermarks a pdf file with a tiled watermark.
        /// </summary>
        /// <param name="contents">The contents of the file to watermark.</param>
        /// <param name="repeatX">The number of horizontal tiles.</param>
        /// <param name="repeatY">The number of vertical tiles.</param>
        /// <param name="watermarkText">The watermark text.</param>
        /// <param name="size">Size of the watermark text.</param>
        /// <param name="watermarkStyle">The watermark style.</param>
        /// <returns>A <see cref="byte" /> array containing the contents of the watermarked pdf file.</returns>
        byte[] WatermarkDocumentTiled(byte[] contents,
                                                      int repeatX,
                                                      int repeatY,
                                                      string watermarkText,
                                                      int size,
                                                      WatermarkTextDrawStyle watermarkStyle);

        byte[] WatermarkDocument(byte[] contents, WatermarkProfile profile);

        int GetPageCount(byte[] contents);

        byte[] AddWatermark(byte[] bytes, string waterMarkText, BaseFont bf, int fontSize);
    }

    /// <summary>
    ///     Provides watermarking capabilities to PDF files.
    /// </summary>
    internal class PdfTools
        : IPdfTools
    {
        /// <summary>
        ///     Watermarks a pdf file with the text 'VOID'.
        /// </summary>
        /// <param name="contents">The contents of the file to watermark.</param>
        /// <param name="repeatX">The number of horizontal tiles.</param>
        /// <param name="repeatY">The number of vertical tiles.</param>
        /// <param name="waterMarkText">The text to use for the watermark.</param>
        /// <returns>A <see cref="byte" /> array containing the contents of the watermarked pdf file.</returns>
        public byte[] WatermarkDocument(byte[] contents, int repeatX, int repeatY, string waterMarkText)
        {
            Preconditions.CheckNotNull("contents", contents);

            return AddWatermark(contents, waterMarkText, BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false), 100);

            //        Dim stream As Stream = Nothing
            //        Dim newStream As Stream
            //        Dim pdfDoc As New Pdf.PdfDocument
            //        Dim newBytes() As Byte
            //        Dim xSize As XSize
            //        Dim gfx As XGraphics = Nothing
            //        Dim pen As XPen
            //        Dim path As XGraphicsPath
            //
            //        Dim yTimes As Integer = repeatY
            //        Dim xTimes As Integer = repeatX
            //
            //        Dim XTotalImgSpace As Double
            //        Dim XTotalGapSpace As Double
            //        Dim XGapSpace As Double
            //
            //        Dim YTotalImgSpace As Double
            //        Dim YTotalGapSpace As Double
            //        Dim YGapSpace As Double
            //
            //        Try
            //            pdfDoc = CompatiblePdfReader.Open(contents, PdfDocumentOpenMode.Modify)
            //
            //            For pgCount As Integer = 0 To pdfDoc.Pages.Count - 1
            //                gfx = XGraphics.FromPdfPage(pdfDoc.Pages(pgCount))
            //                xSize = gfx.MeasureString(waterMarkText, New XFont("BrandonLight", 40))
            //
            //                XTotalImgSpace = xSize.Width * xTimes
            //                XTotalGapSpace = pdfDoc.Pages(pgCount).Width.Value - XTotalImgSpace
            //                XGapSpace = XTotalGapSpace / xTimes
            //
            //                YTotalImgSpace = xSize.Height * yTimes
            //                YTotalGapSpace = pdfDoc.Pages(pgCount).Height.Value - YTotalImgSpace
            //                YGapSpace = YTotalGapSpace / yTimes
            //
            //                path = New XGraphicsPath()
            //                path.AddString(waterMarkText, New XFont("BrandonLight", 40).FontFamily, XFontStyle.Bold, 40, New XPoint((pdfDoc.Pages(pgCount).Width.Value - xSize.Width) / 2, (pdfDoc.Pages(pgCount).Height.Value - xSize.Height) / 2), XStringFormats.Default)
            //
            //                pen = New XPen(XColor.FromArgb(50, 0, 0, 0), 1)
            //
            //                If yTimes > xTimes Then
            //                    For y As Integer = 0 To yTimes - 1
            //                        For x As Integer = 0 To xTimes - 1
            //                            gfx.Save()
            //                            gfx.TranslateTransform(((-(pdfDoc.Pages(pgCount).Width.Value / 2) + (xSize.Width / 2)) + (XGapSpace / 2) + ((xSize.Width + XGapSpace) * x)), (-(pdfDoc.Pages(pgCount).Height.Value / 2) + (xSize.Height / 2)) + (YGapSpace / 2) + ((xSize.Height + YGapSpace) * y))
            //                            gfx.DrawPath(pen, path)
            //                            gfx.Restore()
            //                        Next
            //                    Next
            //                Else
            //                    For x As Integer = 0 To xTimes - 1
            //                        For y As Integer = 0 To yTimes - 1
            //                            gfx.Save()
            //                            gfx.TranslateTransform(((-(pdfDoc.Pages(pgCount).Width.Value / 2) + (xSize.Width / 2)) + (XGapSpace / 2) + ((xSize.Width + XGapSpace) * x)), (-(pdfDoc.Pages(pgCount).Height.Value / 2) + (xSize.Height / 2)) + (YGapSpace / 2) + ((xSize.Height + YGapSpace) * y))
            //                            gfx.DrawPath(pen, path)
            //                            gfx.Restore()
            //                        Next
            //                    Next
            //                End If
            //            Next
            //
            //            newStream = New MemoryStream(Convert.ToInt32(pdfDoc.FileSize))
            //            pdfDoc.Save(newStream, False)
            //            newStream.Flush()
            //        Finally
            //            If gfx IsNot Nothing Then
            //                gfx.Dispose()
            //            End If
            //            If pdfDoc IsNot Nothing Then
            //                pdfDoc.Close()
            //                pdfDoc.Dispose()
            //            End If
            //            If stream IsNot Nothing Then
            //                stream.Close()
            //            End If
            //        End Try
            //
            //        If newStream IsNot Nothing AndAlso newStream.Length > 0 Then
            //            newStream.Seek(0, SeekOrigin.Begin)
            //            newStream.Flush()
            //            ReDim newBytes(Convert.ToInt32(newStream.Length))
            //            newStream.Read(newBytes, 0, Convert.ToInt32(newStream.Length))
            //            newStream.Close()
            //        Else
            //            ReDim newBytes(1)
            //            newBytes(0) = New Byte
            //        End If
            //
            //        Return newBytes
        }

        /// <summary>
        ///     Watermarks the center of a pdf file.
        /// </summary>
        /// <param name="contents">The contents of the file to watermark.</param>
        /// <param name="watermarkText">The watermark text.</param>
        /// <param name="watermarkStyle">The watermark style.</param>
        /// <returns>A <see cref="byte" /> array containing the contents of the watermarked pdf file.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Will be used in the future.")]
        public byte[] WatermarkDocumentCenter(byte[] contents, string watermarkText, WatermarkTextDrawStyle watermarkStyle)
        {
            Preconditions.CheckNotNull("contents", contents);
            Preconditions.CheckNotNullOrEmpty("watermarkText", watermarkText);
            Preconditions.CheckEnum("watermarkStyle", watermarkStyle, WatermarkTextDrawStyle.None);

            return AddWatermark(contents, watermarkText, BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false), 100);
        }

        /// <summary>
        ///     Watermarks a pdf file with a tiled watermark.
        /// </summary>
        /// <param name="contents">The contents of the file to watermark.</param>
        /// <param name="repeatX">The number of horizontal tiles.</param>
        /// <param name="repeatY">The number of vertical tiles.</param>
        /// <param name="watermarkText">The watermark text.</param>
        /// <param name="size">Size of the watermark text.</param>
        /// <param name="watermarkStyle">The watermark style.</param>
        /// <returns>A <see cref="byte" /> array containing the contents of the watermarked pdf file.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Will be used in the future.")]
        public byte[] WatermarkDocumentTiled(byte[] contents,
                                             int repeatX,
                                             int repeatY,
                                             string watermarkText,
                                             int size,
                                             WatermarkTextDrawStyle watermarkStyle)
        {
            Preconditions.CheckNotNull("contents", contents);
            Preconditions.CheckNotNullOrEmpty("watermarkText", watermarkText);
            Preconditions.CheckEnum("watermarkStyle", watermarkStyle, WatermarkTextDrawStyle.None);

            //Return AddWatermark(contents, watermarkText, BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, False), 25)

            Stream newStream;

            using (PdfDocument pdfDoc = CompatiblePdfReader.Open(contents, PdfDocumentOpenMode.Modify))
            {
                var font = new XFont("BrandonLight", size, XFontStyle.Bold);
                XColor color = XColor.FromArgb(55, 255, 0, 0);
                XBrush brush = new XSolidBrush(color);
                var pen = new XPen(color, 1);

                foreach (PdfPage page in pdfDoc.Pages)
                {
                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        XSize xSize = gfx.MeasureString(watermarkText, font);

                        double combinedTextLengthHorizontal = xSize.Width * repeatX;
                        double combinedGapLengthHorizontal = page.Width.Value - combinedTextLengthHorizontal;
                        double gapLengthHorizontal = combinedGapLengthHorizontal / repeatX;

                        double combinedTextLengthVertical = xSize.Height * repeatY;
                        double combinedGapLengthVertical = page.Height.Value - combinedTextLengthVertical;
                        double gapLengthVertical = combinedGapLengthVertical / repeatY;

                        double xTotalItemSize = xSize.Width + gapLengthHorizontal;
                        double yTotalItemSize = xSize.Height + gapLengthVertical;
                        double xTransformBase = (xTotalItemSize - page.Width.Value) / 2;
                        double yTransformBase = (yTotalItemSize - page.Height.Value) / 2;

                        double xPointBase = (page.Width.Value - xSize.Width) / 2;
                        double yPointBase = (page.Height.Value - xSize.Height) / 2;

                        var basePoint = new XPoint(xPointBase, yPointBase);

                        var format = new XStringFormat
                                     {
                                         Alignment = XStringAlignment.Near,
                                         LineAlignment = XLineAlignment.Near
                                     };

                        XGraphicsPath path = null;

                        if (watermarkStyle == WatermarkTextDrawStyle.Solid)
                        { }
                        else if (watermarkStyle == WatermarkTextDrawStyle.Outline)
                        {
                            path = new XGraphicsPath();
                            path.AddString(watermarkText, font.FontFamily, font.Style, font.Size, basePoint, format);
                        }

                        for (var x = 0; x <= repeatX - 1; x++)
                        {
                            for (var y = 0; y <= repeatY - 1; y++)
                            {
                                gfx.Save();
                                gfx.TranslateTransform(xTransformBase + (xTotalItemSize * x), yTransformBase + (yTotalItemSize * y));

                                if (watermarkStyle == WatermarkTextDrawStyle.Solid)
                                {
                                    gfx.DrawString(watermarkText, font, brush, basePoint, format);
                                }
                                else if (watermarkStyle == WatermarkTextDrawStyle.Outline)
                                {
                                    gfx.DrawPath(pen, path);
                                }

                                gfx.Restore();
                            }
                        }
                    }
                }

                newStream = new MemoryStream(Convert.ToInt32(pdfDoc.FileSize));
                pdfDoc.Save(newStream, false);
                newStream.Flush();
            }

            byte[] newBytes;

            if (newStream.Length > 0)
            {
                newStream.Seek(0, SeekOrigin.Begin);
                newStream.Flush();
                newBytes = new byte[Convert.ToInt32(newStream.Length) + 1];
                newStream.Read(newBytes, 0, Convert.ToInt32(newStream.Length));
                newStream.Close();
            }
            else
            {
                newBytes = new byte[2];
                newBytes[0] = new byte();
            }

            return newBytes;

            //Throw New NotImplementedException()
        }

        public byte[] WatermarkDocument(byte[] contents, WatermarkProfile profile)
        {
            Preconditions.CheckNotNull(nameof(profile), profile);

            switch (profile.WaterMarkMethod)
            {
                case "WatermarkDocumentCentered":
                    return WatermarkDocumentCenter(contents, profile.WaterMarkText, profile.WaterMarkStyle);
                case "WatermarkDocumentTiled":
                    if (profile.RepeatX == null)
                    {
                        throw new ArgumentException("RepeatX must not be null.");
                    }

                    if (profile.RepeatY == null)
                    {
                        throw new ArgumentException("RepeatY must not be null.");
                    }

                    return WatermarkDocumentTiled(contents,
                        profile.RepeatX.Value,
                        profile.RepeatY.Value,
                        profile.WaterMarkText,
                        profile.Size ?? 50,
                        profile.WaterMarkStyle);
                default:
                    return contents;
            }
        }

        public int GetPageCount(byte[] contents)
        {
            Preconditions.CheckNotNull("contents", contents);

            using (var pdfReader = new PdfReader(contents))
            {
                return pdfReader.NumberOfPages;
            }
        }

        public byte[] AddWatermark(byte[] bytes, string waterMarkText, BaseFont bf, int fontSize)
        {
            using (var ms = new MemoryStream(10 * 1024))
            {
                using (var reader = new PdfReader(bytes))
                {
                    using (var stamper = new PdfStamper(reader, ms))
                    {
                        int times = reader.NumberOfPages;
                        for (var i = 1; i <= times; i++)
                        {
                            PdfContentByte dc = stamper.GetOverContent(i);
                            AddWatermark(dc,
                                waterMarkText.ToUpper(),
                                bf,
                                fontSize,
                                35,
                                new BaseColor(Color.Red),
                                reader.GetPageSizeWithRotation(i));
                        }

                        stamper.Close();
                    }
                }

                return ms.ToArray();
            }
        }

        private void AddWatermark(PdfContentByte dc,
                                  string text,
                                  BaseFont font,
                                  float fontSize,
                                  float angle,
                                  BaseColor color,
                                  Rectangle realPageSize,
                                  Rectangle rect = null)
        {
            var gState = new PdfGState
                         {
                             FillOpacity = 0.2f,
                             StrokeOpacity = 0.3f
                         };
            dc.SaveState();
            dc.SetGState(gState);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);

            Rectangle ps = rect ?? realPageSize;

            float x = (ps.Right + ps.Left) / 2;
            float y = (ps.Bottom + ps.Top) / 2;
            dc.ShowTextAligned(Element.ALIGN_CENTER, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }

        //Public Function WatermarkDocument1(contents() As Byte, repeatX As Integer, repeatY As Integer, Text As String) As Byte() Implements IWatermarker.WatermarkDocument

        //End Function
    }
}

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Watermark text draw styles
    /// </summary>
    public enum WatermarkTextDrawStyle
    {
        /// <summary>
        ///     No text draw style
        /// </summary>
        None,
        /// <summary>
        ///     A solid text draw style
        /// </summary>
        Solid,
        /// <summary>
        ///     An outline text draw style
        /// </summary>
        Outline
    }
}

namespace MEI.SPDocuments.PdfSharp.Pdf.IO
{
    public class CompatiblePdfReader
    {
        /// <summary>
        ///     Uses itextsharp to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.Open
        /// </summary>
        public static PdfDocument Open(string pdfPath, PdfDocumentOpenMode openMode)
        {
            using (var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                int len = Convert.ToInt32(fileStream.Length);
                var fileArray = new byte[len];
                fileStream.Read(fileArray, 0, len);
                fileStream.Close();

                return CompatiblePdfReader.Open(fileArray, openMode);
            }
        }

        /// <summary>
        ///     Uses itextsharp to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.Open
        /// </summary>
        /// <param name="fileArray"></param>
        /// <param name="openMode">The mode to use when opening the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static PdfDocument Open(byte[] fileArray, PdfDocumentOpenMode openMode)
        {
            return CompatiblePdfReader.Open(new MemoryStream(fileArray), openMode);
        }

        /// <summary>
        ///     Convert any pdf to 1.4 compatible pdf, called instead of PdfReader.Open
        /// </summary>
        public static PdfDocument Open(MemoryStream sourceStream, PdfDocumentOpenMode openMode)
        {
            PdfDocument outDoc;
            sourceStream.Position = 0;

            try
            {
                outDoc = IO_PdfReader.Open(sourceStream, openMode);

                // pdf version < 4 do not allow transparency
                if ((outDoc.Version == 10) || (outDoc.Version == 11) || (outDoc.Version == 12) || (outDoc.Version == 13))
                {
                    outDoc = CompatiblePdfReader.ChangeVersionTo4b(sourceStream);
                }
            }
            catch (PdfReaderException)
            {
                // pdfsharp cannot open pdf versions with iref
                outDoc = CompatiblePdfReader.ChangeVersionTo4b(sourceStream);
            }

            outDoc.Info.Author = "MEI_SP_Documents";
            return outDoc;
        }

        private static PdfDocument ChangeVersionTo4(MemoryStream sourceStream, PdfDocumentOpenMode openMode)
        {
            // workaround if pdfsharp doesn't support this pdf
            sourceStream.Position = 0;
            var outputStream = new MemoryStream();
            var reader = new PdfReader(sourceStream);
            var pdfStamper = new PdfStamper(reader, outputStream, '4');
            pdfStamper.Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_4);
            pdfStamper.Writer.PdfVersion = '4';

            // pdfStamper.FormFlattening = True
            pdfStamper.Writer.CloseStream = false;
            pdfStamper.Close();

            PdfDocument pdf = IO_PdfReader.Open(outputStream, openMode);

            return pdf;
        }

        private static PdfDocument ChangeVersionTo4b(MemoryStream sourceStream)
        {
            string sNewPdf = Path.GetTempPath() + Guid.NewGuid() + ".pdf";

            sourceStream.Position = 0;

            var reader = new PdfReader(sourceStream);

            int n = reader.NumberOfPages;

            var document = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1));

            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(sNewPdf, FileMode.Create));

            writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_4);

            document.Open();

            PdfContentByte cb = writer.DirectContent;

            var i = 0;
            while (i < n)
            {
                i = i + 1;
                Rectangle pageSize = reader.GetPageSizeWithRotation(i);
                document.SetPageSize(pageSize);
                document.NewPage();

                PdfImportedPage page = writer.GetImportedPage(reader, i);
                int rotation = reader.GetPageRotation(i);

                if ((rotation == 90) || (rotation == 270))
                {
                    cb.AddTemplate(page, 0, -1f, 1f, 0, 0, pageSize.Height);
                }
                else
                {
                    cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                }
            }

            document.Close();

            return IO_PdfReader.Open(sNewPdf);
        }
    }
}
