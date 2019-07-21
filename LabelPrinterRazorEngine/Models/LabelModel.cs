using System;

namespace LabelPrinterRazorEngine.Labels
{
    public abstract class LabelPrinter
    {
        public double Dpi { get; }
        public double Ratio { get; }
        public double Width { get; }
        public double Height { get; }

        protected LabelPrinter(
            double width,
            double height,
            double dpi)
        {
            Width = width;
            Height = height;

            Dpi = dpi;
            Ratio = Math.Round(Dpi / 2.54);
        }
    }

    public class LabelModel
        : LabelPrinter
    {
        public LabelModel(
            double width,
            double height)
            : base(width, height, 300.0)
        {
        }

        public LabelModel(
            double width,
            double height,
            double dpi)
            : base(width, height, dpi)
        {
        }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Description { get; set; }
    }
}
