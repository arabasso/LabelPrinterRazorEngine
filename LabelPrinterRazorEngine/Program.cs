using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Windows.Forms;

namespace LabelPrinterRazorEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new TemplateServiceConfiguration
            {
                Debug = false,
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(_ => { }),
                TemplateManager = new EmbeddedResourceTemplateManager(typeof(Program))
            };

            using (var service = RazorEngineService.Create(config))
            {
                var now = DateTime.Now;

                var label = new Labels.LabelModel(10.0, 4.0)
                {
                    Title = "Label Printer",
                    Date = now.Date,
                    Time = now.TimeOfDay,
                    Description = "Label printer description."
                };

                using (var pd = new PrintDialog())
                {
                    if (pd.ShowDialog() == DialogResult.OK)
                    {
                        service.RunCompile("Labels.Label", label, pd.PrinterSettings);
                    }
                }
            }
        }
    }
}
