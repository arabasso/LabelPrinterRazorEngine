using LabelPrinterRazorEngine.Labels;
using RazorEngine.Templating;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms;

namespace LabelPrinterRazorEngine
{
    public static class Extensions
    {
        public static void RunCompile(
            this IRazorEngineService service,
            string templateKey,
            LabelPrinter label,
            PrinterSettings printerSettings)
        {
            using (var pd = new PrintDocument())
            {
                pd.PrinterSettings = printerSettings;

                pd.PrintPage += (sender, args) =>
                {
                    using (var bitmap = service.RunCompile(templateKey, label))
                    {
                        args.Graphics.DrawImage(bitmap, Point.Empty);
                    }
                };

                pd.Print();
            }
        }

        public static Bitmap RunCompile(
            this IRazorEngineService service,
            string templateKey,
            LabelPrinter label)
        {
            Bitmap bitmap = null;

            var thread = new Thread(() =>
            {
                using (var webBrowser = CreateWebBrowser(label))
                {
                    string html = service.RunCompile(templateKey, label.GetType(), label);

                    webBrowser.DocumentText = html;

                    WaitComplete(webBrowser);

                    bitmap = new Bitmap(webBrowser.Width, webBrowser.Height);

                    bitmap.SetResolution((float)label.Dpi, (float)label.Dpi);

                    webBrowser.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return bitmap;
        }

        private static WebBrowser CreateWebBrowser(
            LabelPrinter label)
        {
            return new WebBrowser
            {
                Width = (int)Math.Round(label.Width * label.Ratio),
                Height = (int)Math.Round(label.Height * label.Ratio),
                ScrollBarsEnabled = false
            };
        }

        private static void WaitComplete(
            WebBrowser webBrowser)
        {
            while (webBrowser.IsBusy)
            {
                Application.DoEvents();
            }

            for (var i = 0; i < 500; i++)
            {
                if (webBrowser.ReadyState == WebBrowserReadyState.Complete)
                    break;

                Application.DoEvents();
                Thread.Sleep(10);
            }
        }
    }
}
