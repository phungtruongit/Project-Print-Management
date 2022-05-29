using GroupDocs.Watermark;
using GroupDocs.Watermark.Common;
using GroupDocs.Watermark.Options.Pdf;
using GroupDocs.Watermark.Options;
using GroupDocs.Watermark.Watermarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.WatermarkHub
{
    public class WatermarkProvider
    {
        public static void AddTextWatermarkToFile(TextWatermark textWatermark, string fileInput, string fileOutput) {
            LoadOptions loadOptions = new LoadOptions();
            using (Watermarker watermarker = new Watermarker(fileInput, loadOptions)) {
                PdfArtifactWatermarkOptions textWatermarkOptions = new PdfArtifactWatermarkOptions();
                watermarker.Add(textWatermark, textWatermarkOptions);
                watermarker.Save(fileOutput);
            }
        }

        public static void AddImageWatermarkToFile(ImageWatermark imageWatermark, string fileInput, string fileOutput) {
            LoadOptions loadOptions = new LoadOptions();
            using (Watermarker watermarker = new Watermarker(fileInput, loadOptions)) {
                PdfArtifactWatermarkOptions imageWatermarkOptions = new PdfArtifactWatermarkOptions();
                watermarker.Add(imageWatermark, imageWatermarkOptions);
                watermarker.Save(fileOutput);
            }
        }
    }
}
