using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Xml.Xsl;
using TaxiTicketSystem.Models;

namespace TaxiTicketSystem.GenerationLogic
{
    public class ExportPdf
    {
        public string GeneratePdf(XElement element, string xsltFileName, PdfPageSettings setting)
        {           
            string file_name = GetTempFilePathWithExtension();

            try
            {
                XslCompiledTransform _transform = new XslCompiledTransform(false);
                _transform.Load(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"App_Data\PDF", xsltFileName));

                using (StringWriter sw = new StringWriter())
                {
                    _transform.Transform(new XDocument(element).CreateReader(), new XsltArgumentList(), sw);

                    using (PdfDocument pdf = new PdfDocument())
                    {
                        PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat() { IsWaiting = false };                        

                        Thread thread = new Thread(() => pdf.LoadFromHTML(sw.ToString(), false, setting, htmlLayoutFormat));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();

                        pdf.SaveToFile(file_name);
                    }
                }
            }
            catch
            {
                file_name = string.Empty;
            }

            return file_name;
        }

        private string GetTempFilePathWithExtension()
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + ".pdf";
            return Path.Combine(path, fileName);
        }
    }
}