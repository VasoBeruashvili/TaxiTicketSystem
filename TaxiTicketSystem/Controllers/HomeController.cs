using OfficeOpenXml;
using OfficeOpenXml.Style;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TaxiTicketSystem.Accounts;
using TaxiTicketSystem.GenerationLogic;
using TaxiTicketSystem.Models;
using TaxiTicketSystem.Utils;

namespace TaxiTicketSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        FinaContext context = new FinaContext();

        public ActionResult Index()
        {
            string returnUrl = "/Home/Index";

            dynamic userType = Session["userType"];
            if (userType != null && userType.x)
            {
                if(User.Identity.Name == "sofo")
                {
                    returnUrl = "/Home/DriverSalaryReport";
                    return Redirect(returnUrl);
                }
            }
            else if(userType != null && !userType.x)
            {
                returnUrl = "/Home/Reports";
                return Redirect(returnUrl);
            }
            else if(userType == null)
            {
                returnUrl = "/Account/Index";
                return Redirect(returnUrl);
            }

            return View();
        }

        public ActionResult Reports()
        {
            return View();
        }

        public ActionResult Invoices()
        {
            return View();
        }

        public ActionResult Contragents()
        {
            string returnUrl = "/Home/Contragents";

            dynamic userType = Session["userType"];
            if (userType != null && userType.x)
            {
                if(User.Identity.Name == "user")
                {
                    returnUrl = "/Home/Index";
                    return Redirect(returnUrl);
                }
            }
            else if (userType != null && !userType.x)
            {
                returnUrl = "/Home/Reports";
                return Redirect(returnUrl);
            }
            else if (userType == null)
            {
                returnUrl = "/Account/Index";
                return Redirect(returnUrl);
            }

            return View();
        }

        public ActionResult DebitAccounts()
        {
            string returnUrl = "/Home/DebitAccounts";

            dynamic userType = Session["userType"];
            if (userType != null && userType.x)
            {
                if (User.Identity.Name == "user")
                {
                    returnUrl = "/Home/Index";
                    return Redirect(returnUrl);
                }
            }
            else if (userType != null && !userType.x)
            {
                returnUrl = "/Home/Reports";
                return Redirect(returnUrl);
            }
            else if (userType == null)
            {
                returnUrl = "/Account/Index";
                return Redirect(returnUrl);
            }

            return View();
        }

        public ActionResult DriverSalaryReport()
        {
            string returnUrl = "/Home/DriverSalaryReport";

            dynamic userType = Session["userType"];
            if (userType != null && userType.x)
            {                
                if(User.Identity.Name != "sofo")
                {
                    returnUrl = "/Home/Index";
                    return Redirect(returnUrl);
                }
            }
            else if (userType != null && !userType.x)
            {
                returnUrl = "/Home/Reports";
                return Redirect(returnUrl);
            }
            else if (userType == null)
            {
                returnUrl = "/Account/Index";
                return Redirect(returnUrl);
            }

            return View();
        }

        public FileResult GenerateDriverSalaryReportExcel(string fromDate, string toDate)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);

            if (!isContragent)
            {
                fromDate += " 00:00:00";
                toDate += " 23:59:59";

                DateTime dateFrom = Convert.ToDateTime(fromDate);
                DateTime dateTo = Convert.ToDateTime(toDate);

                var result = (from p in context.ProductsFlow
                              where p.GeneralDocs.DocType.Value == 29
                             && p.GeneralDocs.CreateDate >= dateFrom
                             && p.GeneralDocs.CreateDate <= dateTo
                              join po in context.ProductOut on p.GeneralId equals po.GeneralId
                             join stf in context.Staff on po.StaffId equals stf.id
                             where (po.StaffId != 0)
                             select new
                             {
                                 staffID = po.StaffId,
                                 staffName = stf.name,
                                 amountPrice = po.GeneralDocs.Amount,
                                 parkingCosts = po.ParkingCosts,
                                 customsFees = po.CustomsFees,
                                 additionalCosts = po.AdditionalCosts,
                                 contrVat = po.contrVat,
                                 withWeb = po.AddedWithWeb
                             }).ToList();

                List<dynamic> finalResult = new List<dynamic>();
                result.ForEach(r =>
                {
                    var fr = new
                    {
                        staffID = r.staffID,
                        staffName = r.staffName,
                        amountPrice = r.contrVat == "1" ? r.amountPrice.Value - ((r.parkingCosts == null || r.parkingCosts == string.Empty ? 0 : Convert.ToDouble(r.parkingCosts, System.Globalization.CultureInfo.InvariantCulture)) + (r.customsFees == null || r.customsFees == string.Empty ? 0 : Convert.ToDouble(r.customsFees, System.Globalization.CultureInfo.InvariantCulture)) + (r.additionalCosts == null || r.additionalCosts == string.Empty ? 0 : Convert.ToDouble(r.additionalCosts, System.Globalization.CultureInfo.InvariantCulture)))
                        : r.contrVat == "2" ? (r.amountPrice.Value / 1.18) - ((r.parkingCosts == null || r.parkingCosts == string.Empty ? 0 : Convert.ToDouble(r.parkingCosts, System.Globalization.CultureInfo.InvariantCulture)) + (r.customsFees == null || r.customsFees == string.Empty ? 0 : Convert.ToDouble(r.customsFees, System.Globalization.CultureInfo.InvariantCulture)) + (r.additionalCosts == null || r.additionalCosts == string.Empty ? 0 : Convert.ToDouble(r.additionalCosts, System.Globalization.CultureInfo.InvariantCulture)))
                        : r.contrVat == "3" ? (r.amountPrice.Value + (r.amountPrice.Value * 0.18)) - ((r.parkingCosts == null || r.parkingCosts == string.Empty ? 0 : Convert.ToDouble(r.parkingCosts, System.Globalization.CultureInfo.InvariantCulture)) + (r.customsFees == null || r.customsFees == string.Empty ? 0 : Convert.ToDouble(r.customsFees, System.Globalization.CultureInfo.InvariantCulture)) + (r.additionalCosts == null || r.additionalCosts == string.Empty ? 0 : Convert.ToDouble(r.additionalCosts, System.Globalization.CultureInfo.InvariantCulture)))
                        : r.amountPrice.Value,
                        contrVat = r.contrVat,
                        withWeb = r.withWeb
                    };

                    finalResult.Add(fr);
                });

                List<dynamic> finRes = new List<dynamic>();
                var groupedResult = finalResult.GroupBy(fr => fr.staffID);
                groupedResult.ToList().ForEach(gr =>
                {
                    var finR = new
                    {
                        staffID = gr.ToList()[0].staffID,
                        staffName = gr.ToList()[0].staffName,
                        amountPrice1 = Math.Round((gr.Where(g => g.withWeb == "true").Select(g => g.amountPrice).Cast<double>().Sum()), 1),
                        amountPrice2 = Math.Round((gr.Where(g => g.withWeb == string.Empty).Select(g => g.amountPrice).Cast<double>().Sum()), 1),
                        amountSum = Math.Round((gr.Select(g => g.amountPrice).Cast<double>().Sum()), 1)
                    };

                    finRes.Add(finR);
                });

                var totalAmountSum = finRes.Select(fr => fr.amountSum).Cast<double>().Sum();

                var fileName = "DriverSalaryReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                var outputDir = Path.GetTempPath();
                var file = new FileInfo(outputDir + fileName);

                byte[] excelFile = null;

                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("დეტალური რეპორტი-" + DateTime.Now.ToShortDateString());

                    worksheet.Cells[1, 1].Value = "მძღოლი";
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 2].Value = "თანხა Web";
                    worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 3].Value = "თანხა FINA";
                    worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 4].Value = "ჯამი";
                    worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    for (int i = 2, j = 0; i <= finRes.Count + 1 && j < finRes.Count; i++, j++)
                    {
                        worksheet.Cells[i, 1].Value = finRes[j].staffName;
                        worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 2].Value = Math.Round(finRes[j].amountPrice1, 2);
                        worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 3].Value = Math.Round(finRes[j].amountPrice2, 2);
                        worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 4].Value = Math.Round(finRes[j].amountSum, 2);
                        worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }

                    //worksheet.Cells[finRes.Count + 2, 4].Style.Font.Bold = true;
                    //worksheet.Cells[finRes.Count + 2, 4].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    //worksheet.Cells[finRes.Count + 2, 4].Value = Math.Round(totalAmountSum, 2);

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();

                    excelFile = package.GetAsByteArray();
                }

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DriverSalaryReport.xlsx");
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetDriverSalaryReports(string fromDate, string toDate)
        {
            fromDate += " 00:00:00";
            toDate += " 23:59:59";

            DateTime dateFrom = Convert.ToDateTime(fromDate);
            DateTime dateTo = Convert.ToDateTime(toDate);

            var result = (from p in context.ProductsFlow
                          where p.GeneralDocs.DocType.Value == 29
                         && p.GeneralDocs.CreateDate >= dateFrom
                         && p.GeneralDocs.CreateDate <= dateTo
                          join po in context.ProductOut on p.GeneralId equals po.GeneralId
                         join stf in context.Staff on po.StaffId equals stf.id
                         where (po.StaffId != 0)
                         select new
                         {
                             staffID = po.StaffId,
                             staffName = stf.name,
                             amountPrice = po.GeneralDocs.Amount,
                             parkingCosts = po.ParkingCosts,
                             customsFees = po.CustomsFees,
                             additionalCosts = po.AdditionalCosts,
                             contrVat = po.contrVat,
                             withWeb = po.AddedWithWeb
                         }).ToList();

            List<dynamic> finalResult = new List<dynamic>();
            result.ForEach(r =>
            {
                var fr = new
                {
                    staffID = r.staffID,
                    staffName = r.staffName,
                    amountPrice = r.contrVat == "1" ? r.amountPrice.Value - ((r.parkingCosts == null || r.parkingCosts == string.Empty ? 0 : Convert.ToDouble(r.parkingCosts, System.Globalization.CultureInfo.InvariantCulture)) + (r.customsFees == null || r.customsFees == string.Empty ? 0 : Convert.ToDouble(r.customsFees, System.Globalization.CultureInfo.InvariantCulture)) + (r.additionalCosts == null || r.additionalCosts == string.Empty ? 0 : Convert.ToDouble(r.additionalCosts, System.Globalization.CultureInfo.InvariantCulture)))
                    : r.contrVat == "2" ? (r.amountPrice.Value / 1.18) - ((r.parkingCosts == null || r.parkingCosts == string.Empty ? 0 : Convert.ToDouble(r.parkingCosts, System.Globalization.CultureInfo.InvariantCulture)) + (r.customsFees == null || r.customsFees == string.Empty ? 0 : Convert.ToDouble(r.customsFees, System.Globalization.CultureInfo.InvariantCulture)) + (r.additionalCosts == null || r.additionalCosts == string.Empty ? 0 : Convert.ToDouble(r.additionalCosts, System.Globalization.CultureInfo.InvariantCulture)))
                    : r.contrVat == "3" ? (r.amountPrice.Value + (r.amountPrice.Value * 0.18)) - ((r.parkingCosts == null || r.parkingCosts == string.Empty ? 0 : Convert.ToDouble(r.parkingCosts, System.Globalization.CultureInfo.InvariantCulture)) + (r.customsFees == null || r.customsFees == string.Empty ? 0 : Convert.ToDouble(r.customsFees, System.Globalization.CultureInfo.InvariantCulture)) + (r.additionalCosts == null || r.additionalCosts == string.Empty ? 0 : Convert.ToDouble(r.additionalCosts, System.Globalization.CultureInfo.InvariantCulture)))
                    : r.amountPrice.Value,
                    contrVat = r.contrVat,
                    withWeb = r.withWeb
                };

                finalResult.Add(fr);
            });

            List<dynamic> finRes = new List<dynamic>();
            var groupedResult = finalResult.GroupBy(fr => fr.staffID);
            groupedResult.ToList().ForEach(gr =>
            {
                var finR = new
                {
                    staffID = gr.ToList()[0].staffID,
                    staffName = gr.ToList()[0].staffName,
                    amountPrice1 = Math.Round((gr.Where(g => g.withWeb == "true").Select(g => g.amountPrice).Cast<double>().Sum()), 1),
                    amountPrice2 = Math.Round((gr.Where(g => g.withWeb == string.Empty).Select(g => g.amountPrice).Cast<double>().Sum()), 1),
                    amountSum = Math.Round((gr.Select(g => g.amountPrice).Cast<double>().Sum()), 1)
                };

                finRes.Add(finR);
            });

            var totalAmountSum = finRes.Select(fr => fr.amountSum).Cast<double>().Sum();           

            var jsonResult = Json(new { reports = finRes, totalAmountSum = Math.Round(totalAmountSum, 2) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;

            return jsonResult;
        }

        public JsonResult GetContragentByFakeID(string contragentFakeID)
        {
            var contragent = context.Contragents.Where(c => c.id > 0 && c.fakeID == contragentFakeID && c.path == "0#2#5").FirstOrDefault();

            var result = contragent == null ?
            new { id = 0, fakeID = "", name = "", vat = "" } :
            new { id = contragent.id, fakeID = contragent.fakeID, name = contragent.name, vat = contragent.customVat };

            return Json(new { contragent = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffByFakeID(string staffFakeID)
        {
            var staff = context.Staff.FirstOrDefault(s => s.fakeID != string.Empty && s.fakeID == staffFakeID);

            var result = staff == null ?
            new { id = 0, fakeID = "", name = "" } :
            new { id = staff.id, fakeID = staff.fakeID, name = staff.name };

            return Json(new { staff = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCarByFakeID(string carFakeID)
       {
            var car = context.Cars.FirstOrDefault(c => c.fakeID == carFakeID);

            var result = car == null ?
            new { id = 0, fakeID = "", num = "" } :
            new { id = car.id, fakeID = car.fakeID, num = car.num };

            return Json(new { car = result }, JsonRequestBehavior.AllowGet);
        }

        public bool IsCompanyVat(DateTime date)
        {
            Companies _res = context.Companies.Select(a => a).FirstOrDefault();
            if (!_res.vat.HasValue || !_res.vat.Value || date < _res.tdate)
                return false;
            return true;
        }

        public bool SaveEntriesFast(int general_id)
        {
            if (!DeleteEntries(general_id))
                return false;
            List<Entries> _entries = new List<Entries>();
            bool companyVat = IsCompanyVat(DateTime.Now);
            int contragent_id = context.GeneralDocs.FirstOrDefault(gd => gd.Id == general_id).ParamId1.Value;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragent_id);
            var generalDoc = context.GeneralDocs.FirstOrDefault(gd => gd.Id == general_id);
            string contragent_account = contragent.account;
            int contragent_vat_type = contragent.customVat != null && contragent.customVat != string.Empty ? Convert.ToInt32(contragent.customVat) : 2;
            int contragentGroupID = contragent.group_id.Value;
            double currency_rate = 1;
            int project_id = 1;
            int currency_id = 1;
            double vat = 0.0;
            double total_excise = 0;

            //end products
            double price_1s = 0.0;
            double price_2s = 0.0;
            double price_3s = 0.0;
            double price_4s = 0.0;
            // service out
            double vvt = 18;
            double service_vat = 0;
            double total_service_excise = 0;
            double product_quantity = 1;
            double price_amount = generalDoc.Amount.Value;
            double excise = 0;
            price_amount *= currency_rate;
            int product_vat_type = 0;
            double vatPercent = vvt;
            double pr = price_amount;
            double vat_val = (pr) - ((pr * 100) / (vatPercent + 100));
            if (vat_val > 0)
                price_amount -= vat_val;
            price_amount -= (product_quantity * excise);
            vat += vat_val;
            service_vat += vat;
            total_excise += (product_quantity * excise);
            total_service_excise += (product_quantity * excise);
            if (contragent_vat_type >= 2 && product_vat_type == 1)
                product_vat_type = contragent_vat_type;

            price_1s = 0;
            price_2s = 0;
            price_3s = 0;
            price_4s = 0;
            double original_price_1s = 0;
            double original_price_2s = 0;
            double original_price_3s = 0;
            double original_price_4s = 0;
            if (!companyVat)
            {
                price_1s += price_amount;
                original_price_1s += price_amount + vat_val;
            }
            else
            {
                if (product_vat_type == 1) //company vat and client vat
                {
                    price_2s += price_amount;
                    original_price_2s += price_amount + vat_val;
                }
                else if (product_vat_type == 2)//ჩათვლის უფლებით
                {
                    price_3s += price_amount;
                    original_price_3s += price_amount + vat_val;
                }
                else if (product_vat_type == 3)//ჩათვლის უფლების გარეშე
                {
                    price_4s += price_amount;
                    original_price_4s += price_amount + vat_val;
                }
            }

            if (vat > 0)
            {
                var generatedAmount = vat / currency_rate;            
                // შემოსავალი რეალიზაციიდან
                _entries.Add(new Entries { GeneralId = general_id, DebitAcc = contragent_account, CreditAcc = "6110", Amount = generalDoc.Amount - generatedAmount, Rate = currency_rate, N = product_quantity, N2 = product_quantity, A1 = contragent_id, A3 = contragentGroupID, B1 = 114, B3 = 2, Comment = "შემოსავალი რეალიზაციიდან", ProjectId = project_id, CurrencyId = currency_id });

                // გადასახდელი დღგ რეალიზაციიდან
                _entries.Add(new Entries { GeneralId = general_id, DebitAcc = contragent_account, CreditAcc = "3330", Amount = generatedAmount, Rate = currency_rate, A1 = contragent_id, A3 = contragentGroupID, Comment = "გადასახდელი დღგ რეალიზაციიდან", ProjectId = project_id, CurrencyId = currency_id });
            }

            _entries.ForEach(e =>
            {
                context.Entries.Add(e);
            });            

            return context.SaveChanges() >= 0;
        }

        public JsonResult SaveDocItem(DocItem docItem)
        {
            //take authenticated user user
            var currentUser = context.Users.FirstOrDefault(u => u.login == User.Identity.Name);

            //configure doc.GeneralDocs
            docItem.GeneralDocsItem.Tdate1 = docItem.GeneralDocsItem.Tdate; //For tdate and tdate1 balance reports
            docItem.GeneralDocsItem.Purpose = "მომსახურების გაწევა №"; //set purpose to GeneralDocs (as default)
            docItem.GeneralDocsItem.DocType = 29; //set doc_type to GeneralDocs (მომსახურების გაწევა)
            docItem.GeneralDocsItem.UserId = currentUser.id; //set user_id to GeneralDocs
            if (docItem.GeneralDocsItem.Id == 0) //set create_date to GeneralDocs
            {
                docItem.GeneralDocsItem.CreateDate = DateTime.Now;
            }
            docItem.GeneralDocsItem.Vat = 18; //set vat to GeneralDocs
            docItem.GeneralDocsItem.DocNum = context.GeneralDocs.Max(gd => gd.DocNum) + 1; //set doc_num + 1 to GeneralDocs
            docItem.GeneralDocsItem.ParamId2 = 1; //set param_id2 to GeneralDocs (საწყობი)
            docItem.GeneralDocsItem.StatusId = 8; //set status_id to GeneralDocs (added from web interface)
            docItem.GeneralDocsItem.MakeEntry = true; //set make_entry to GeneralDocs (გატარებები)
            docItem.GeneralDocsItem.Amount = docItem.GeneralDocsItem.TotalSumAll;
            if(docItem.GeneralDocsItem.customVat == "2")
            {
                docItem.GeneralDocsItem.Amount = Math.Round((docItem.GeneralDocsItem.Amount.Value + (docItem.GeneralDocsItem.Amount.Value * 0.18)), 2);
            }
            if(docItem.GeneralDocsItem.customVat == "3")
            {
                docItem.GeneralDocsItem.Amount = Math.Round((docItem.GeneralDocsItem.Amount.Value / 1.18), 2);
            }

            //configure doc.ProductOut

            docItem.ProductOutItem[0].TraveledDistance = docItem.ProductOutItem[0].TraveledDistance.Replace(',', '.');
            docItem.ProductOutItem[0].ParkingCosts = docItem.ProductOutItem[0].ParkingCosts == null ? "" : docItem.ProductOutItem[0].ParkingCosts.Replace(',', '.');
            docItem.ProductOutItem[0].CustomsFees = docItem.ProductOutItem[0].CustomsFees == null ? "" : docItem.ProductOutItem[0].CustomsFees.Replace(',', '.');
            docItem.ProductOutItem[0].AdditionalCosts = docItem.ProductOutItem[0].AdditionalCosts == null ? "" : docItem.ProductOutItem[0].AdditionalCosts.Replace(',', '.');

            docItem.ProductOutItem[0].ResponsablePersonDate = DateTime.Now;
            docItem.ProductOutItem[0].Other = true;
            docItem.ProductOutItem[0].TransportCostPayer = 2;
            docItem.ProductOutItem[0].InvoiceBankId = 1;
            docItem.ProductOutItem[0].PayType = "1"; // უნაღდო
            //docItem.ProductOutItem[0].CheckStatus = 0; //TODO add?
            docItem.ProductOutItem[0].AddedWithWeb = "true"; //This field is for records which are added from web interface (it's very Important!!!)!!!
            if (docItem.ProductOutItem[0].Id == 0)
            {
                docItem.ProductOutItem[0].Deleted = "false";
            } //Important!!!

            //configure doc.ProductsFlow
            docItem.ProductsFlowList[0].Amount = 1;//docItem.GeneralDocsItem.Amount; //the same Amount as doc.GeneralDocs
            docItem.ProductsFlowList[0].Price = docItem.GeneralDocsItem.Amount; //the same Amount as doc.GeneralDocs
            docItem.ProductsFlowList[0].ProductId = 114;
            docItem.ProductsFlowList[0].StoreId = 1;
            docItem.ProductsFlowList[0].VatPercent = 18;
            docItem.ProductsFlowList[0].IsOrder = null;
            docItem.ProductsFlowList[0].IsExpense = null;
            docItem.ProductsFlowList[0].IsMove = null;
            docItem.ProductsFlowList[0].UnitId = 17;
            docItem.ProductsFlowList[0].InId = null;
            docItem.ProductsFlowList[0].VendorId = null;
            docItem.ProductsFlowList[0].OutId = null;
            docItem.ProductsFlowList[0].ServiceStaffId = 0;

            //add doc.GeneralDocs, doc.ProductOut and doc.ProductsFlow to context
            context.GeneralDocs.Add(docItem.GeneralDocsItem);
            context.ProductOut.Add(docItem.ProductOutItem[0]);
            context.ProductsFlow.Add(docItem.ProductsFlowList[0]);            

            if (docItem.GeneralDocsItem.Id != 0)
            {
                context.Entry(docItem.GeneralDocsItem).State = EntityState.Modified;
                context.Entry(docItem.ProductOutItem[0]).State = EntityState.Modified;
                context.Entry(docItem.ProductsFlowList[0]).State = EntityState.Modified;
                //context.Entry(entry).State = EntityState.Modified;
            }

            var saveResult = context.SaveChanges() >= 0;

            SaveEntriesFast(docItem.GeneralDocsItem.Id);

            return Json(new { saveResult = saveResult }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDocItem(int GeneralDocID)
        {
            var generalDoc = context.GeneralDocs.FirstOrDefault(gd => gd.Id == GeneralDocID);
            var productOut = context.ProductOut.Where(po => po.GeneralId == GeneralDocID);
            var productsFlow = context.ProductsFlow.Where(pf => pf.GeneralId == GeneralDocID);
            var entries = context.Entries.Where(e => e.GeneralId == GeneralDocID);

            productOut.ToList().ForEach(po =>
            {
                context.ProductOut.Remove(po);
            });
            productsFlow.ToList().ForEach(pf =>
            {
                context.ProductsFlow.Remove(pf);
            });
            entries.ToList().ForEach(e =>
            {
                context.Entries.Remove(e);
            });
            context.GeneralDocs.Remove(generalDoc);

            var result = false;

            result = context.SaveChanges() >= 0;

            return Json(new { deleteResult = result }, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteEntries(int general_id)
        {
            var entries = context.Entries.Where(e => e.GeneralId == general_id).ToList();

            entries.ForEach(e =>
            {
                context.Entries.Remove(e);
            });

            return context.SaveChanges() >= 0;
        }

        public JsonResult GetDocItems(string fromDate, string toDate, bool Deleted)
        {
            fromDate += " 00:00:00";
            toDate += " 23:59:59";

            DateTime dateFrom = Convert.ToDateTime(fromDate);
            DateTime dateTo = Convert.ToDateTime(toDate);

            string isDeleted = Deleted.ToString();

            var result = context.ProductsFlow.
                Where(pf => pf.GeneralDocs.StatusId == 8 && pf.GeneralDocs.DocType.Value == 29 && /*pf.GeneralDocs.ParamId1 == 1 &&*/
                pf.GeneralDocs.Tdate.Value >= dateFrom &&
                pf.GeneralDocs.Tdate.Value <= dateTo).
                SelectMany(r => r.GeneralDocs.ProductOuts.Where(gdpo => gdpo.AddedWithWeb == "true" && gdpo.Deleted == isDeleted).
                Select(po => new
                {
                    POid = po.Id,
                    PFid = context.ProductsFlow.FirstOrDefault(pf => pf.GeneralId == po.GeneralId).Id,
                    GDid = po.GeneralDocs.Id,
                    companyName = context.Contragents.FirstOrDefault(c => c.id == po.GeneralDocs.ParamId1).name,
                    contragentId = context.Contragents.FirstOrDefault(c => c.id == po.GeneralDocs.ParamId1).id,
                    contragentFakeID = context.Contragents.FirstOrDefault(c => c.id == po.GeneralDocs.ParamId1).fakeID,
                    dateNow = po.GeneralDocs.Tdate.Value,
                    startTime = po.StartTime,
                    endTime = po.EndTime,
                    staffID = (int?)context.Staff.FirstOrDefault(s => s.id == po.StaffId).id,
                    staffFakeID = context.Staff.FirstOrDefault(s => s.id == po.StaffId).fakeID,
                    staffName = context.Staff.FirstOrDefault(s => s.id == po.StaffId).name,
                    carFakeID = context.Cars.FirstOrDefault(c => c.num == po.CarNumber).fakeID,
                    carNumber = po.CarNumber,
                    traveledDistance = po.TraveledDistance,
                    amountPrice = po.GeneralDocs.Amount,
                    parkingCosts = po.ParkingCosts,
                    customsFees = po.CustomsFees,
                    additionalCosts = po.AdditionalCosts,
                    withoutPrint = po.WithoutPrint,
                    commentOut = po.CommentOut,
                    Deleted = po.Deleted,
                    vat = context.Contragents.FirstOrDefault(c => c.id == po.GeneralDocs.ParamId1).customVat,
                    contrVat = po.contrVat
                }));

            result = result.OrderByDescending(r => r.POid);

            var jsonResult =  Json(new { docItems = result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;

            return jsonResult;
        }

        public JsonResult GetContragents()
        {
            return Json(new { contragents = context.Contragents.Where(c => c.fakeID != null && c.fakeID != string.Empty && c.id > 0 && c.path == "0#2#5").Select(c => new { c.id, c.name, c.fakeID }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDrivers()
        {
            return Json(new { drivers = context.Staff.Where(s => s.fakeID != string.Empty).Select(s => new { s.id, s.name, s.fakeID }) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetailedContragentReportsByContragentID(int? contragentID, string fromDate, string toDate)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);

            fromDate += " 00:00:00";
            toDate += " 23:59:59";

            DateTime dateFrom = Convert.ToDateTime(fromDate);
            DateTime dateTo = Convert.ToDateTime(toDate);

            if (contragentID != null)
            {
                if (isContragent)
                {
                    var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID.Value);

                    var now = DateTime.Now;
                    string[] contrMaxMonth = null;
                    if(contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                    {
                        contrMaxMonth = contragent.maxMonth.Split(';');

                        if(contrMaxMonth.Length == 2)
                        {
                            if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                            {
                                contragentID = null;
                            }
                        }
                    }                    
                }

                var result = (from p in context.ProductsFlow
                         where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29 && p.GeneralDocs.ParamId1 == contragentID
                         && p.GeneralDocs.Tdate.Value >= dateFrom
                         && p.GeneralDocs.Tdate.Value <= dateTo
                         join po in context.ProductOut on p.GeneralId equals po.GeneralId
                         join st in context.Staff on po.StaffId equals st.id
                         join c in context.Contragents on po.GeneralDocs.ParamId1 equals c.id
                         where (po.AddedWithWeb == "true" && po.Deleted == "false")
                         select new
                         {
                             POid = po.Id,
                             GDid = p.GeneralDocs.Id,
                             companyName = c.name,
                             dateNow = p.GeneralDocs.Tdate.Value,
                             startTime = po.StartTime,
                             endTime = po.EndTime,
                             staffName = st.name,
                             carNumber = po.CarNumber,
                             traveledDistance = po.TraveledDistance,
                             amountPrice = po.GeneralDocs.Amount,
                             parkingCosts = po.ParkingCosts,
                             customsFees = po.CustomsFees,
                             additionalCosts = po.AdditionalCosts,
                             withoutPrint = po.WithoutPrint,
                             commentOut = po.CommentOut,
                             vat = c.customVat,
                             contrVat = po.contrVat
                         }).OrderByDescending(r => r.POid).ToList();
                
                List<double> totalSumList = new List<double>();
                result.ForEach(r =>
                {
                    totalSumList.Add(r.amountPrice.Value);
                });

                var jsonResult = Json(new { reports = result, totalSum = Math.Round(totalSumList.Sum(), 2) }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;

                return jsonResult;
            }
            else
            {
                var result = (from p in context.ProductsFlow
                             where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                             && p.GeneralDocs.Tdate.Value >= dateFrom
                             && p.GeneralDocs.Tdate.Value <= dateTo
                             join po in context.ProductOut on p.GeneralId equals po.GeneralId
                             join st in context.Staff on po.StaffId equals st.id
                             join c in context.Contragents on po.GeneralDocs.ParamId1 equals c.id
                             where (po.AddedWithWeb == "true" && po.Deleted == "false")
                             select new
                             {
                                 POid = po.Id,
                                 GDid = p.GeneralDocs.Id,
                                 companyName = c.name,
                                 dateNow = p.GeneralDocs.Tdate.Value,
                                 startTime = po.StartTime,
                                 endTime = po.EndTime,
                                 staffName = st.name,
                                 carNumber = po.CarNumber,
                                 traveledDistance = po.TraveledDistance,
                                 amountPrice = po.GeneralDocs.Amount,
                                 parkingCosts = po.ParkingCosts,
                                 customsFees = po.CustomsFees,
                                 additionalCosts = po.AdditionalCosts,
                                 withoutPrint = po.WithoutPrint,
                                 commentOut = po.CommentOut,
                                 vat = c.customVat,
                                 contrVat = po.contrVat
                             }).OrderByDescending(r => r.POid).ToList();

                List<double> totalSumList = new List<double>();
                result.ForEach(r =>
                {
                    totalSumList.Add(r.amountPrice.Value);
                });

                var jsonResult = Json(new { reports = result, totalSum = Math.Round(totalSumList.Sum(), 2) }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;

                return jsonResult;
            }            
        }

        public JsonResult GetDetailedDriverReportsByContragentID(int? driverID, string fromDate, string toDate, bool withCreateDate)
        {
            fromDate += " 00:00:00";
            toDate += " 23:59:59";

            DateTime dateFrom = Convert.ToDateTime(fromDate);
            DateTime dateTo = Convert.ToDateTime(toDate);

            if (driverID != null)
            {
                var result = (from p in context.ProductsFlow
                    where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                    && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) >= dateFrom
                    && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) <= dateTo
                    join po in context.ProductOut on p.GeneralId equals po.GeneralId
                    join c in context.Contragents on p.GeneralDocs.ParamId1 equals c.id
                    join stf in context.Staff on po.StaffId equals stf.id
                    where (po.StaffId == driverID && po.AddedWithWeb == "true" && po.Deleted == "false")
                    select new
                    {
                        POid = po.Id,
                        GDid = p.GeneralDocs.Id,
                        staffName = stf.name,
                        dateNow = p.GeneralDocs.Tdate.Value,
                        startTime = po.StartTime,
                        endTime = po.EndTime,
                        contragentName = c.name,
                        carNumber = po.CarNumber,
                        traveledDistance = po.TraveledDistance,
                        amountPrice = po.GeneralDocs.Amount,
                        parkingCosts = po.ParkingCosts,
                        customsFees = po.CustomsFees,
                        additionalCosts = po.AdditionalCosts,
                        withoutPrint = po.WithoutPrint,
                        commentOut = po.CommentOut,
                        vat = c.customVat,
                        contrVat = po.contrVat,
                        contragentFakeID = c.fakeID
                    }).OrderByDescending(r => r.POid).ToList();
                
                List<double> totalSumList = new List<double>();
                result.ForEach(r =>
                {
                    totalSumList.Add(r.amountPrice.Value);
                });

                var jsonResult = Json(new { reports = result, totalSum = Math.Round(totalSumList.Sum(), 2) }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;

                return jsonResult;
            }
            else
            {
                var result = (from p in context.ProductsFlow
                             where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                             && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) >= dateFrom
                             && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) <= dateTo
                             join po in context.ProductOut on p.GeneralId equals po.GeneralId
                             join c in context.Contragents on p.GeneralDocs.ParamId1 equals c.id
                             join stf in context.Staff on po.StaffId equals stf.id
                             where (po.AddedWithWeb == "true" && po.Deleted == "false")
                             select new
                             {
                                 POid = po.Id,
                                 GDid = p.GeneralDocs.Id,
                                 staffName = stf.name,
                                 dateNow = p.GeneralDocs.Tdate.Value,
                                 startTime = po.StartTime,
                                 endTime = po.EndTime,
                                 contragentName = c.name,
                                 carNumber = po.CarNumber,
                                 traveledDistance = po.TraveledDistance,
                                 amountPrice = po.GeneralDocs.Amount,
                                 parkingCosts = po.ParkingCosts,
                                 customsFees = po.CustomsFees,
                                 additionalCosts = po.AdditionalCosts,
                                 withoutPrint = po.WithoutPrint,
                                 commentOut = po.CommentOut,
                                 vat = c.customVat,
                                 contrVat = po.contrVat,
                                 contragentFakeID = c.fakeID
                             }).OrderByDescending(r => r.POid).ToList();

                List<double> totalSumList = new List<double>();
                result.ForEach(r =>
                {
                    totalSumList.Add(r.amountPrice.Value);
                });

                var jsonResult = Json(new { reports = result, totalSum = Math.Round(totalSumList.Sum(), 2) }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;

                return jsonResult;
            }            
        }
        
        public JsonResult GetBalanceReportsByContragentID(int? contragentID, string year, string month)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);

            var yearNum = Convert.ToInt32(year);
            var monthNum = Convert.ToInt32(month);

            DateTime dateFrom = new DateTime(yearNum, monthNum, 1);
            DateTime dateTo = new DateTime(yearNum, monthNum, DateTime.DaysInMonth(yearNum, monthNum));
            List<BalanceModel> finalResult = new List<BalanceModel>();
            var prevSum = 0.0;
            var currentSum = 0.0;
            var totalSum = 0.0;
            var cur = 0.0;
            var jsonResult = Json(new { });

            if (contragentID != null)
            {
                var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);                                                   
                
                if(contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                {
                    var contrMonthPeriod = Convert.ToInt32(contragent.monthPeriod);
                    if(monthNum == 1)
                    {
                        dateFrom = new DateTime(yearNum - 1, 12, contrMonthPeriod);
                        dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                    }
                    else
                    {
                        dateFrom = new DateTime(yearNum, monthNum - 1, contrMonthPeriod);
                        dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                    }
                }

                if (isContragent)
                {
                    var now = DateTime.Now;
                    string[] contrMaxMonth = null;
                    if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                    {
                        contrMaxMonth = contragent.maxMonth.Split(';');

                        if(contrMaxMonth.Length == 2)
                        {
                            if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                            {
                                contragentID = null;
                                dateFrom = new DateTime(1000, 10, 10);
                                dateTo = new DateTime(1000, 10, 10);
                            }
                        }
                    }                    
                }

                string _path = contragent.path;
                string _account = GetContragentAccountByPath(_path, 0);
                string _account2 = GetContragentAccountByPath(_path, 1);

                //sawyisi
                var debts = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                var creds = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                          .Select(en => new
                                          {
                                              generalID = en.GeneralId,
                                              amountPrice = en.GeneralDocs.Amount.Value
                                          }).ToList();
                var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                       .Select(en => new
                                       {
                                           generalID = en.GeneralId,
                                           amountPrice = en.GeneralDocs.Amount.Value
                                       }).ToList();
                var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                //sawyisi nashti
                var startBalance = debetStartSum / 2;
                var stBalance = Math.Round(startBalance, 2);
                stBalance = stBalance - creditStartSum;

                if (!isContragent)
                {
                    var now = DateTime.Now;
                    string[] contrMaxMonth = null;
                    if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                    {
                        contrMaxMonth = contragent.maxMonth.Split(';');

                        if(contrMaxMonth.Length == 2)
                        {
                            if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                            {
                                contragentID = null;
                            }
                        }
                    }                    
                }

                //mimdinare                
                var debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                          .Select(en => new
                                          {
                                              generalID = en.GeneralId,
                                              amountPrice = en.GeneralDocs.Amount.Value
                                          }).ToList();
                var debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                var creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                       .Select(en => new
                                       {
                                           generalID = en.GeneralId,
                                           amountPrice = en.GeneralDocs.Amount.Value
                                       }).ToList();
                var creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();

                if (!string.IsNullOrEmpty(contragent.monthPeriod))
                {
                    debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                          .Select(en => new
                                          {
                                              generalID = en.GeneralId,
                                              amountPrice = en.GeneralDocs.Amount.Value
                                          }).ToList();
                    debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                    creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                           .Select(en => new
                                           {
                                               generalID = en.GeneralId,
                                               amountPrice = en.GeneralDocs.Amount.Value
                                           }).ToList();
                    creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();
                }

                //mimdinare nashti
                var currentBalance = debetCurrentSum / 2;
                var currBalance = Math.Round(currentBalance, 2);
                currBalance = currBalance - creditCurrentSum;



                var res = new BalanceModel
                {
                    fakeID = contragent.fakeID,
                    id = contragent.id,
                    companyName = contragent.name,
                    prevPeriod = Math.Round(stBalance, 2),
                    currentPeriod = Math.Round(currBalance, 2),
                    totalPeriod = Math.Round((stBalance + currBalance), 2),
                    dateFrom = dateFrom.ToString(),
                    dateTo = dateTo.ToString()
                };
                if(dateFrom != new DateTime(1000, 10, 10))
                {
                    finalResult.Add(res);
                }
                prevSum = finalResult.Where(fr => fr.prevPeriod > 0).Select(fr => fr.prevPeriod).Sum();
                currentSum = finalResult.Where(fr => fr.currentPeriod > 0).Select(fr=>fr.currentPeriod).Sum();
                totalSum = finalResult.Where(fr => fr.totalPeriod > 0).Select(fr=>fr.totalPeriod).Sum();
                cur = finalResult.Where(fr => fr.currentPeriod < 0).Select(fr => fr.currentPeriod).Sum();

                jsonResult = Json(new { balances = finalResult, prevSum = Math.Round(prevSum, 2), currentSum = Math.Round(currentSum, 2), totalSum = Math.Round(totalSum, 2), cur = Math.Round(cur, 2) }, JsonRequestBehavior.AllowGet);                                
            }
            else
            {
                dateFrom = dateFrom.Month == 1 ? new DateTime(dateFrom.Year - 1, 12, dateFrom.Day) : new DateTime(dateFrom.Year, dateFrom.Month - 1, dateFrom.Day);
                var contragents = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.DocType == 29 && gd.ProductOuts.Any(po => po.Deleted == "false"))
                    .GroupBy(gd => gd.ParamId1);

                contragents.ToList().ForEach(c =>
                {
                    dateFrom = new DateTime(yearNum, monthNum, 1);
                    dateTo = new DateTime(yearNum, monthNum, DateTime.DaysInMonth(yearNum, monthNum));

                    var contragent = context.Contragents.FirstOrDefault(cont => cont.id == c.Key && cont.path == "0#2#5" && cont.id > 0);
                    var cID = c.Key;

                    if(contragent != null)
                    {
                        if(contragent.id > 0)
                        {
                            if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                            {
                                var contrMonthPeriod = Convert.ToInt32(contragent.monthPeriod);
                                if (monthNum == 1)
                                {
                                    dateFrom = new DateTime(yearNum - 1, 12, contrMonthPeriod);
                                    dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                                }
                                else
                                {
                                    dateFrom = new DateTime(yearNum, monthNum - 1, contrMonthPeriod);
                                    dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                                }
                            }

                            string _path = contragent.path;
                            string _account = GetContragentAccountByPath(_path, 0);
                            string _account2 = GetContragentAccountByPath(_path, 1);

                            //sawyisi
                            var debts = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                            var creds = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                            var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                      .Select(en => new
                                                      {
                                                          generalID = en.GeneralId,
                                                          amountPrice = en.GeneralDocs.Amount.Value
                                                      }).ToList();
                            var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                            var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                   .Select(en => new
                                                   {
                                                       generalID = en.GeneralId,
                                                       amountPrice = en.GeneralDocs.Amount.Value
                                                   }).ToList();
                            var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                            //sawyisi nashti
                            var startBalance = debetStartSum / 2;
                            var stBalance = Math.Round(startBalance, 2);
                            stBalance = stBalance - creditStartSum;

                            if (!isContragent)
                            {
                                var now = DateTime.Now;
                                string[] contrMaxMonth = null;
                                if(contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                                {
                                    contrMaxMonth = contragent.maxMonth.Split(';');

                                    if(contrMaxMonth.Length == 2)
                                    {
                                        if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                        {
                                            cID = null;
                                        }
                                    }
                                }                                
                            }

                            //mimdinare                            
                            var debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                      .Select(en => new
                                                      {
                                                          generalID = en.GeneralId,
                                                          amountPrice = en.GeneralDocs.Amount.Value
                                                      }).ToList();
                            var debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                            var creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                   .Select(en => new
                                                   {
                                                       generalID = en.GeneralId,
                                                       amountPrice = en.GeneralDocs.Amount.Value
                                                   }).ToList();
                            var creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();

                            if (!string.IsNullOrEmpty(contragent.monthPeriod))
                            {
                                debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                      .Select(en => new
                                                      {
                                                          generalID = en.GeneralId,
                                                          amountPrice = en.GeneralDocs.Amount.Value
                                                      }).ToList();
                                debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                                creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                       .Select(en => new
                                                       {
                                                           generalID = en.GeneralId,
                                                           amountPrice = en.GeneralDocs.Amount.Value
                                                       }).ToList();
                                creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();
                            }

                            //mimdinare nashti
                            var currentBalance = debetCurrentSum / 2;
                            var currBalance = Math.Round(currentBalance, 2);
                            currBalance = currBalance - creditCurrentSum;



                            var res = new BalanceModel
                            {
                                fakeID = contragent.fakeID,
                                id = contragent.id,
                                companyName = contragent.name,
                                prevPeriod = Math.Round(stBalance, 2),
                                currentPeriod = Math.Round(currBalance, 2),
                                totalPeriod = Math.Round((stBalance + currBalance), 2),
                                dateFrom = dateFrom.ToString(),
                                dateTo = dateTo.ToString()
                            };
                            finalResult.Add(res);
                            prevSum = finalResult.Where(fr => fr.prevPeriod > 0).Select(fr => fr.prevPeriod).Sum();
                            currentSum = finalResult.Where(fr => fr.currentPeriod > 0).Select(fr => fr.currentPeriod).Sum();
                            totalSum = finalResult.Where(fr => fr.totalPeriod > 0).Select(fr => fr.totalPeriod).Sum();
                            cur = finalResult.Where(fr => fr.currentPeriod < 0).Select(fr => fr.currentPeriod).Sum();
                        }                        
                    }
                });

                finalResult = finalResult.OrderByDescending(fr => fr.totalPeriod).ToList();

                jsonResult = Json(new { balances = finalResult, prevSum = Math.Round(prevSum, 2), currentSum = Math.Round(currentSum, 2), totalSum = Math.Round(totalSum, 2), cur = Math.Round(cur, 2) }, JsonRequestBehavior.AllowGet);
            }

            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        public JsonResult GetSubReports(int contragentID)
        {
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            var debts = contragent.debts;

            var generalDocs29 = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ProductOuts.FirstOrDefault(po => po.GeneralId == gd.Id).Deleted == "false" && gd.ParamId1 == contragentID && gd.DocType == 29)
                .GroupBy(gd => DbFunctions.TruncateTime(gd.Tdate1.Value).Value.Month);
            var generalDocs38 = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId2 == contragentID && gd.DocType == 38)
                .GroupBy(gd => DbFunctions.TruncateTime(gd.Tdate1.Value).Value.Month);

            List<SubBalanceModel> result = new List<SubBalanceModel>();

            //Generate manually
            var rec = new SubBalanceModel
            {
                tDate = Convert.ToDateTime("2015-10-08"),
                tDate1 = "წინა პერიოდის დავალიანება",
                tDate2 = "",
                amount1 = debts == string.Empty || debts == null ? 0 : Convert.ToDouble(debts, System.Globalization.CultureInfo.InvariantCulture),
                amount2 = 0.0,
                balance = 0.0
            };

            result.Add(rec);
            //---

            generalDocs29.ToList().ForEach(gd29 =>
            {
                var gdc = gd29.OrderBy(gd => gd.Tdate1.Value);
                var r = new SubBalanceModel
                {
                    tDate = gdc.ToList()[0].Tdate1.Value,
                    tDate1 = string.Format("{0} - {1}", gdc.ToList().First().Tdate1.Value.ToString("yyyy-MM-dd"), gdc.ToList().Last().Tdate1.Value.ToString("yyyy-MM-dd")),
                    tDate2 = "",
                    amount1 = gdc.Select(g => new
                    {
                        amountPrice = g.Amount == null ? 0.0 : g.Amount.Value
                    }).ToList().Select(dc => dc.amountPrice).Sum(),
                    amount2 = 0.0,
                    balance = 0.0
                };

                var am1 = Math.Round(r.amount1.Value, 2);
                r.amount1 = am1;

                result.Add(r);
            });

            //generalDocs38.ToList().ForEach(gd38 =>
            //{
            //    var gdc = gd38.OrderBy(gd => gd.Tdate.Value);
            //    var r = new SubBalanceModel
            //    {
            //        tDate = gdc.ToList()[0].Tdate.Value,
            //        tDate1 = "",
            //        tDate2 = string.Format("{0} - {1}", gdc.ToList().First().Tdate.Value.ToString("yyyy-MM-dd"), gdc.ToList().Last().Tdate.Value.ToString("yyyy-MM-dd")),
            //        amount1 = 0.0,
            //        amount2 = gdc.Select(g => new
            //        {
            //            amountPrice = g.Amount == null ? 0.0 : g.Amount.Value
            //        }).ToList().Select(dc => dc.amountPrice).Sum(),
            //        balance = 0.0
            //    };

            //    result.Add(r);
            //});

            generalDocs38.ToList().ForEach(gd38 =>
            {
                var gdc = gd38.OrderBy(gd => gd.Tdate1.Value);
                gdc.ToList().ForEach(g =>
                {
                    var r = new SubBalanceModel
                    {
                        tDate = g.Tdate1.Value,
                        tDate1 = "",
                        tDate2 = g.Tdate1.Value.ToString("yyyy-MM-dd"),
                        amount1 = 0.0,
                        amount2 = g.Amount == null ? 0.0 : g.Amount.Value,
                        balance = 0.0
                    };

                    result.Add(r);
                });
            });

            result = result.OrderBy(r => r.tDate.Month).ToList();
            var rfrst = result.FirstOrDefault();
            var bl = 0.0;
            var curSum = 0.0;
            if (rfrst != null)
            {
                bl = Math.Round(rfrst.balance.Value, 2);
                curSum = rfrst.balance.Value;
            }
            result.ForEach(r =>
            {
                if (r.amount1 != 0.0)
                {
                    bl += r.amount1.Value;
                }
                else
                {
                    bl -= r.amount2.Value;
                }
                
                r.balance = Math.Round(bl, 2);
                curSum = bl;
            });

            var amount1Sum = result.Sum(r => r.amount1);
            var amount2Sum = result.Sum(r => r.amount2);

            return Json(new { result = result, amount1Sum = Math.Round(amount1Sum.Value, 2), amount2Sum = Math.Round(amount2Sum.Value, 2), curSum = Math.Round(curSum, 2) }, JsonRequestBehavior.AllowGet);
        }

        public FileResult GenerateSubReportsExcel(int contragentID)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);
            bool isOwner = false;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            var debts = contragent.debts;
            if (!isContragent)
            {
                isOwner = true;
            }
            else
            {
                try
                {
                    isOwner = User.Identity.Name == contragent.code;
                }
                catch (Exception)
                {
                    isOwner = false;
                }
            }

            if (isOwner)
            {
                var phones = contragent.phone.Split(';');
                var emails = contragent.email.Split(';');
                var contactPersons = contragent.contactPerson.Split(';');

                var generalDocs29 = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ProductOuts.FirstOrDefault(po => po.GeneralId == gd.Id).Deleted == "false" && gd.ParamId1 == contragentID && gd.DocType == 29)
                .GroupBy(gd => DbFunctions.TruncateTime(gd.Tdate1.Value).Value.Month);
                var generalDocs38 = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId2 == contragentID && gd.DocType == 38)
                    .GroupBy(gd => DbFunctions.TruncateTime(gd.Tdate1.Value).Value.Month);

                List<SubBalanceModel> result = new List<SubBalanceModel>();

                //Generate manually
                var rec = new SubBalanceModel
                {
                    tDate = Convert.ToDateTime("2015-10-08"),
                    tDate1 = "წინა პერიოდის დავალიანება",
                    tDate2 = "",
                    amount1 = debts == string.Empty || debts == null ? 0 : Convert.ToDouble(debts, System.Globalization.CultureInfo.InvariantCulture),
                    amount2 = 0.0,
                    balance = 0.0
                };

                result.Add(rec);
                //---

                generalDocs29.ToList().ForEach(gd29 =>
                {
                    var gdc = gd29.OrderBy(gd => gd.Tdate1.Value);
                    var r = new SubBalanceModel
                    {
                        tDate = gdc.ToList()[0].Tdate1.Value,
                        tDate1 = string.Format("{0} - {1}", gdc.ToList().First().Tdate1.Value.ToString("yyyy-MM-dd"), gdc.ToList().Last().Tdate1.Value.ToString("yyyy-MM-dd")),
                        tDate2 = "",
                        amount1 = gdc.Select(g => new
                        {
                            amountPrice = g.Amount == null ? 0.0 : g.Amount.Value
                        }).ToList().Select(dc => dc.amountPrice).Sum(),
                        amount2 = 0.0,
                        balance = 0.0
                    };

                    var am1 = Math.Round(r.amount1.Value, 2);
                    r.amount1 = am1;

                    result.Add(r);
                });

                //generalDocs38.ToList().ForEach(gd38 =>
                //{
                //    var gdc = gd38.OrderBy(gd => gd.Tdate.Value);
                //    var r = new SubBalanceModel
                //    {
                //        tDate = gdc.ToList()[0].Tdate.Value,
                //        tDate1 = "",
                //        tDate2 = string.Format("{0} - {1}", gdc.ToList().First().Tdate.Value.ToString("yyyy-MM-dd"), gdc.ToList().Last().Tdate.Value.ToString("yyyy-MM-dd")),
                //        amount1 = 0.0,
                //        amount2 = gdc.Select(g => new
                //        {
                //            amountPrice = g.Amount == null ? 0.0 : g.Amount.Value
                //        }).ToList().Select(dc => dc.amountPrice).Sum(),
                //        balance = 0.0
                //    };

                //    result.Add(r);
                //});

                generalDocs38.ToList().ForEach(gd38 =>
                {
                    var gdc = gd38.OrderBy(gd => gd.Tdate1.Value);
                    gdc.ToList().ForEach(g =>
                    {
                        var r = new SubBalanceModel
                        {
                            tDate = g.Tdate1.Value,
                            tDate1 = "",
                            tDate2 = g.Tdate1.Value.ToString("yyyy-MM-dd"),
                            amount1 = 0.0,
                            amount2 = g.Amount == null ? 0.0 : g.Amount.Value,
                            balance = 0.0
                        };

                        result.Add(r);
                    });
                });

                result = result.OrderBy(r => r.tDate.Month).ToList();
                var rfrst = result.FirstOrDefault();
                var bl = 0.0;
                var curSum = 0.0;
                if (rfrst != null)
                {
                    bl = Math.Round(rfrst.balance.Value, 2);
                    curSum = rfrst.balance.Value;
                }
                result.ForEach(r =>
                {
                    if (r.amount1 != 0.0)
                    {
                        bl += r.amount1.Value;
                    }
                    else
                    {
                        bl -= r.amount2.Value;
                    }
                    r.balance = Math.Round(bl, 2);
                    curSum = bl;
                });

                var amount1Sum = result.Sum(r => r.amount1);
                var amount2Sum = result.Sum(r => r.amount2);

                var fileName = "SubReportDetailedInfo-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                var outputDir = Path.GetTempPath();
                var file = new FileInfo(outputDir + fileName);

                byte[] excelFile = null;

                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("რეპორტის დეტალური ინფორმაცია-" + DateTime.Now.ToShortDateString());

                    worksheet.Cells[1, 1].Value = "მომსახურების დარიცხვის თარიღი";
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 2].Value = "მომსახურების დაფარვის თარიღი";
                    worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 3].Value = "დარიცხული თანხა";
                    worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 4].Value = "გადახდილი თანხა";
                    worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 5].Value = "მიმდინარე დავალიანება";
                    worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    for (int i = 2, j = 0; i <= result.Count + 1 && j < result.Count; i++, j++)
                    {
                        worksheet.Cells[i, 1].Value = result[j].tDate1;
                        worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 2].Value = result[j].tDate2;
                        worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 3].Value = result[j].amount1 == 0 ? null : result[j].amount1;
                        worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 4].Value = result[j].amount2 == 0 ? null : result[j].amount2;
                        worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 5].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        worksheet.Cells[i, 5].Value = result[j].balance == 0 ? null : result[j].balance;
                        worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }
                    
                    worksheet.Cells[result.Count + 2, 1].Value = "მიმდინარე დავალიანება:";
                    worksheet.Cells[result.Count + 2, 2].Style.Font.Bold = true;
                    worksheet.Cells[result.Count + 2, 2].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                    worksheet.Cells[result.Count + 2, 2].Value = Math.Round(curSum, 2);

                    worksheet.Cells[result.Count + 2, 3].Style.Font.Bold = true;
                    worksheet.Cells[result.Count + 2, 3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    worksheet.Cells[result.Count + 2, 3].Value = Math.Round(amount1Sum.Value, 2);

                    worksheet.Cells[result.Count + 2, 4].Style.Font.Bold = true;
                    worksheet.Cells[result.Count + 2, 4].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    worksheet.Cells[result.Count + 2, 4].Value = Math.Round(amount2Sum.Value, 2);

                    worksheet.Cells[result.Count + 5, 3].Value = "Tel:";
                    worksheet.Cells[result.Count + 5, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[result.Count + 5, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);

                    worksheet.Cells[result.Count + 5, 4].Value = "Email:";
                    worksheet.Cells[result.Count + 5, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[result.Count + 5, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);

                    worksheet.Cells[result.Count + 5, 5].Value = "საკ.პირი:";
                    worksheet.Cells[result.Count + 5, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[result.Count + 5, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGoldenrodYellow);

                    if (phones != null && emails != null && contactPersons != null)
                    {
                        if (phones.Count() == emails.Count() && phones.Count() == contactPersons.Count())
                        {
                            for (int k = 0, f = result.Count + 6; k < phones.Count() && f <= phones.Count() + result.Count() + 6; k++, f++)
                            {
                                worksheet.Cells[f, 3].Value = phones[k];
                                worksheet.Cells[f, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                                worksheet.Cells[f, 4].Value = emails[k];
                                worksheet.Cells[f, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                                worksheet.Cells[f, 5].Value = contactPersons[k];
                                worksheet.Cells[f, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                            }
                        }
                    }

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();

                    excelFile = package.GetAsByteArray();
                }

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SubReportDetailedInfo.xlsx");
            }
            else
            {
                return null;
            }
        }

        private string GetContragentAccountByPath(string path, int level)
        {
            string[] seps = path.Split('#');
            if (seps.Length > 4)
                path = seps[0] + "#" + seps[1] + "#" + seps[2] + "#" + seps[3];
            string ss = context.GroupContragents.Where(g => g.Account != "" && g.Path == path).Select(g => g.Account).FirstOrDefault();
            string account = "";
            if (!string.IsNullOrWhiteSpace(ss))
            {
                string[] _rr = ss.Split(';');
                if (_rr.Length > level)
                    account = _rr[level];
            }
            else
            {
                path = seps[0] + "#" + seps[1] + "#" + seps[2];
                ss = context.GroupContragents.Where(g => g.Account != "" && g.Path == path).Select(g => g.Account).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(ss))
                {
                    string[] _rr = ss.Split(';');
                    if (_rr.Length > level)
                        account = _rr[level];
                }
                else
                {
                    path = seps[0] + "#" + seps[1];
                    ss = context.GroupContragents.Where(g => g.Account != "" && g.Path == path).Select(g => g.Account).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(ss))
                    {
                        string[] _rr = ss.Split(';');
                        if (_rr.Length > level)
                            account = _rr[level];
                    }
                }
            }
            return account;
        }

        public JsonResult GeneratePage()
        {
            return Json(new { x = Session["userType"] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrentUser()
        {
            string fullName = "";
            string contragentID = null;
            var result = context.Users.Where(u => u.login == User.Identity.Name).FirstOrDefault();
            fullName = result == null ? "" : string.Format("{0} {1}", result.name, result.surname);

            if (result == null)
            {
                var res = context.Contragents.Where(c => c.path == "0#2#5" && c.code == User.Identity.Name).FirstOrDefault();
                fullName = res == null ? "" : res.name;
                contragentID = res == null ? null : res.id.ToString();
            }

            return Json(new { fullName = fullName, contragentID = contragentID }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContragentsForGrid()
        {           
            var contragents = context.Contragents.Where(c => c.fakeID != null && c.fakeID != string.Empty && c.id > 0 && c.path == "0#2#5").Select(c => new
            {
                id = c.id,
                fakeID = c.fakeID,
                name = c.name,
                code = c.code,
                address = c.address,
                email = c.email,
                phone = c.phone,
                contractStartDate = c.contractStartDate,
                contractExpirationDate = c.contractExpirationDate,
                servicePaymentDate = c.servicePaymentDate,
                serviceRates = c.serviceRates,
                monthPeriod = c.monthPeriod,
                customVat = c.customVat,
                contactPerson = c.contactPerson
            });

            List<object> expiredContragents = new List<object>();
            var yearNow = DateTime.Now.Year;
            var monthNow = DateTime.Now.Month;
            var dayNow = DateTime.Now.Day;
            contragents.ToList().ForEach(c =>
            {
                if(c.contractExpirationDate != null && c.contractExpirationDate != string.Empty)
                {
                    var contrDate = Convert.ToDateTime(c.contractExpirationDate.Substring(0, 10));
                    if (contrDate.Year == yearNow && contrDate.Month == monthNow)
                    {
                        var daysLeft = (contrDate.Date - DateTime.Now.Date).Days;
                        if (daysLeft <= 14)
                        {
                            expiredContragents.Add(new
                            {
                                daysLeft = daysLeft,
                                contrID = c.id
                            });
                        }
                    }
                }
            });

            contragents = contragents.OrderByDescending(c => c.id);            

            return Json(new { contragents = contragents, expiredContragents = expiredContragents }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveContragent(Contragents contragentModel)
        {           
            if (contragentModel.id != 0)
            {
                Contragents dbContragent = context.Contragents.FirstOrDefault(c => c.id == contragentModel.id);

                dbContragent.fakeID = contragentModel.fakeID;
                dbContragent.name = contragentModel.name;
                dbContragent.code = contragentModel.code;
                dbContragent.address = contragentModel.address;
                dbContragent.email = contragentModel.email;
                dbContragent.phone = contragentModel.phone;
                dbContragent.contractStartDate = contragentModel.contractStartDate;
                dbContragent.contractExpirationDate = contragentModel.contractExpirationDate;
                dbContragent.servicePaymentDate = contragentModel.servicePaymentDate;
                dbContragent.serviceRates = contragentModel.serviceRates;
                dbContragent.monthPeriod = contragentModel.monthPeriod;
                dbContragent.customVat = contragentModel.customVat;
                dbContragent.short_name = contragentModel.name;
                dbContragent.contactPerson = contragentModel.contactPerson;

                context.Entry(dbContragent).State = EntityState.Modified;
            }
            else
            {
                int lastContragentID = context.Contragents.ToList().Last(c => c.id > 0).id;
                contragentModel.id = lastContragentID + 1;

                contragentModel.group_id = 5; //მყიდველები
                contragentModel.path = "0#2#5"; //მყიდველები
                contragentModel.type = 1;
                contragentModel.vat_type = 1;
                contragentModel.short_name = contragentModel.name;
                contragentModel.acc_use = 6;
                contragentModel.account = "1410";
                contragentModel.account2 = "3120";
                contragentModel.tax = 20;
                contragentModel.min_tax = 0;
                contragentModel.country_id = 25;
                contragentModel.pwd = "12796d3d5f4e132385f8af8e281efa211cdb2a05b8450039f037a6573e0bcb3f";

                contragentModel.birth_date = DateTime.Now;
                contragentModel.client_id = 0;
                contragentModel.cons_period = 30;
                //contragentModel.create_user_id = 0;
                //contragentModel.create_date = DateTime.Now;
                //contragentModel.limit_type = 0;
                //contragentModel.limit_val = 0.00M;

                context.Contragents.Add(contragentModel);
            }            

            var saveResult = context.SaveChanges() >= 0;

            return Json(new { saveResult = saveResult }, JsonRequestBehavior.AllowGet);
        }       

        public JsonResult ReturnPwd(int contragentID)
        {
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            var result = false;
            if(contragent != null)
            {
                if(contragent.code != null && contragent.code != string.Empty)
                {
                    var changedPwd = HashHelper.Calc(contragent.code);
                    contragent.pwd = changedPwd;
                    context.Entry(contragent).State = EntityState.Modified;
                    result = context.SaveChanges() >= 0;
                }
            }

            return Json(new { saveResult = result }, JsonRequestBehavior.AllowGet);
        }

        public FileResult GenerateDetailedContragentPdf(int? contragentID, string fromDate, string toDate)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);
            bool isOwner = false;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            if (!isContragent)
            {
                isOwner = true;
            }
            else
            {
                try
                {
                    isOwner = User.Identity.Name == contragent.code;
                }
                catch (Exception)
                {
                    isOwner = false;
                }
            }

            if (isOwner)
            {
                fromDate += " 00:00:00";
                toDate += " 23:59:59";

                DateTime dateFrom = Convert.ToDateTime(fromDate);
                DateTime dateTo = Convert.ToDateTime(toDate);

                if (contragentID != null)
                {                   
                    //var contragents = context.Contragents.Where(c => c.code == User.Identity.Name);

                    if (isContragent)
                    {
                        var now = DateTime.Now;
                        string[] contrMaxMonth = null;
                        if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                        {
                            contrMaxMonth = contragent.maxMonth.Split(';');

                            if(contrMaxMonth.Length == 2)
                            {
                                if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                {
                                    contragentID = null;
                                }
                            }
                        }                        
                    }

                    var result = (from p in context.ProductsFlow
                                 where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29 && p.GeneralDocs.ParamId1 == contragentID
                                 && p.GeneralDocs.Tdate.Value >= dateFrom
                                 && p.GeneralDocs.Tdate.Value <= dateTo
                                 join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join st in context.Staff on po.StaffId equals st.id
                                 join c in context.Contragents on po.GeneralDocs.ParamId1 equals c.id
                                 where (po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     companyName = c.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     staffName = st.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat
                                 }).OrderByDescending(r => r.POid).ToList();

                    List<double> totalSumList = new List<double>();
                    result.ForEach(r =>
                    {
                        totalSumList.Add(r.amountPrice.Value);
                    });

                    var generalDocAmountSum = 0.0;
                    var vat = 0.0;
                    var withoutVat = 0.0;
                    if (contragent.customVat == "1")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "2")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "3")
                    {
                        vat = 0.0;
                        withoutVat = totalSumList.Sum();
                    }

                    if (result.Count() > 0)
                    {
                        XElement element = new XElement("root",
                            new XElement("items", result.ToList().Select(i => new XElement("item",
                                new XElement("companyName", i.companyName),
                                new XElement("dateNow", string.Format("{0} {1}-{2}", i.dateNow.ToString("yyyy-MM-dd"), i.startTime, i.endTime)),
                                new XElement("staffName", i.staffName),
                                new XElement("carNumber", i.carNumber),
                                new XElement("traveledDistance", i.traveledDistance),
                                new XElement("amountPrice",
                                i.contrVat == "1" ? Math.Round((i.amountPrice.Value - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "2" ? Math.Round(((i.amountPrice.Value / 1.18) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "3" ? Math.Round(((i.amountPrice.Value + (i.amountPrice.Value * 0.18)) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : i.amountPrice.Value),
                                new XElement("parkingCosts", i.parkingCosts),
                                new XElement("customsFees", i.customsFees),
                                new XElement("additionalCosts", i.additionalCosts),
                                new XElement("subTotalSum", i.contrVat == "1" ? Math.Round(i.amountPrice.Value, 1) : i.contrVat == "2" ? Math.Round((i.amountPrice.Value / 1.18), 1) : i.contrVat == "3" ? Math.Round((i.amountPrice.Value + (i.amountPrice.Value * 0.18)), 1) : 0),
                                new XElement("TSWV1", i.contrVat == "2" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("TSWV2", i.contrVat == "3" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("withoutPrint", i.withoutPrint),
                                new XElement("commentOut", i.commentOut)
                            ))),
                            new XElement("withoutVat", Math.Round(withoutVat, 2)),
                            new XElement("vat", Math.Round(vat, 2)),
                            new XElement("withVat", Math.Round(generalDocAmountSum, 2))
                        );

                        ExportPdf exportPdf = new ExportPdf();

                        string fileName = "DetailedContragentReport.pdf";
                        PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4, Orientation = PdfPageOrientation.Landscape };
                        string filePath = exportPdf.GeneratePdf(element, "DetailedContragentReport.xslt", setting);
                        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                        string contentType = MimeMapping.GetMimeMapping(filePath);

                        System.Net.Mime.ContentDisposition cd = new ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(fileData, contentType);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    var result = (from p in context.ProductsFlow
                                 where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                                 && p.GeneralDocs.Tdate.Value >= dateFrom
                                 && p.GeneralDocs.Tdate.Value <= dateTo
                                 join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join st in context.Staff on po.StaffId equals st.id
                                 join c in context.Contragents on po.GeneralDocs.ParamId1 equals c.id
                                 where (po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     companyName = c.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     staffName = st.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat
                                 }).OrderByDescending(r => r.POid).ToList();
                    
                    if (result.Count() > 0)
                    {
                        XElement element = new XElement("root",
                            new XElement("items", result.ToList().Select(i => new XElement("item",
                                new XElement("companyName", i.companyName),
                                new XElement("dateNow", string.Format("{0} {1}-{2}", i.dateNow.ToString("yyyy-MM-dd"), i.startTime, i.endTime)),
                                new XElement("staffName", i.staffName),
                                new XElement("carNumber", i.carNumber),
                                new XElement("traveledDistance", i.traveledDistance),
                                new XElement("amountPrice",
                                i.contrVat == "1" ? Math.Round((i.amountPrice.Value - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "2" ? Math.Round(((i.amountPrice.Value / 1.18) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "3" ? Math.Round(((i.amountPrice.Value + (i.amountPrice.Value * 0.18)) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : i.amountPrice.Value),
                                new XElement("parkingCosts", i.parkingCosts),
                                new XElement("customsFees", i.customsFees),
                                new XElement("additionalCosts", i.additionalCosts),
                                new XElement("subTotalSum", i.contrVat == "1" ? Math.Round(i.amountPrice.Value, 1) : i.contrVat == "2" ? Math.Round((i.amountPrice.Value / 1.18), 1) : i.contrVat == "3" ? Math.Round((i.amountPrice.Value + (i.amountPrice.Value * 0.18)), 1) : 0),
                                new XElement("TSWV1", i.contrVat == "2" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("TSWV2", i.contrVat == "3" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("withoutPrint", i.withoutPrint),
                                new XElement("commentOut", i.commentOut)
                            ))),
                            new XElement("withoutVat", 0),
                            new XElement("vat", 0),
                            new XElement("withVat", 0)
                        );

                        ExportPdf exportPdf = new ExportPdf();

                        string fileName = "DetailedContragentReport.pdf";
                        PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4, Orientation = PdfPageOrientation.Landscape };
                        string filePath = exportPdf.GeneratePdf(element, "DetailedContragentReport.xslt", setting);
                        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                        string contentType = MimeMapping.GetMimeMapping(filePath);

                        System.Net.Mime.ContentDisposition cd = new ContentDisposition
                        {
                            FileName = fileName,
                            Inline = true
                        };

                        Response.AppendHeader("Content-Disposition", cd.ToString());

                        return File(fileData, contentType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public FileResult GenerateDetailedContragentExcel(int? contragentID, string fromDate, string toDate)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);
            bool isOwner = false;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            if (!isContragent)
            {
                isOwner = true;
            }
            else
            {
                try
                {
                    isOwner = User.Identity.Name == contragent.code;
                }
                catch (Exception)
                {
                    isOwner = false;
                }
            }

            if (isOwner)
            {
                fromDate += " 00:00:00";
                toDate += " 23:59:59";

                DateTime dateFrom = Convert.ToDateTime(fromDate);
                DateTime dateTo = Convert.ToDateTime(toDate);

                if (contragentID != null)
                {                   
                    //var contragents = context.Contragents.Where(c => c.code == User.Identity.Name);

                    if (isContragent)
                    {
                        var now = DateTime.Now;
                        string[] contrMaxMonth = null;
                        if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                        {
                            contrMaxMonth = contragent.maxMonth.Split(';');

                            if(contrMaxMonth.Length == 2)
                            {
                                if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                {
                                    contragentID = null;
                                }
                            }
                        }                        
                    }

                    var result = (from p in context.ProductsFlow
                                 where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29 && p.GeneralDocs.ParamId1 == contragentID
                                 && p.GeneralDocs.Tdate.Value >= dateFrom
                                 && p.GeneralDocs.Tdate.Value <= dateTo
                                 join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join st in context.Staff on po.StaffId equals st.id
                                 join c in context.Contragents on po.GeneralDocs.ParamId1 equals c.id
                                 where (po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     companyName = c.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     staffName = st.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat
                                 }).OrderBy(r => r.dateNow).ToList();

                    List<double> totalSumList = new List<double>();
                    result.ForEach(r =>
                    {
                        totalSumList.Add(r.amountPrice.Value);
                    });

                    var generalDocAmountSum = 0.0;
                    var vat = 0.0;
                    var withoutVat = 0.0;
                    if (contragent.customVat == "1")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "2")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "3")
                    {
                        vat = 0.0;
                        withoutVat = totalSumList.Sum();
                    }

                    var fileName = "DetailedContragentReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                    var outputDir = Path.GetTempPath();
                    var file = new FileInfo(outputDir + fileName);

                    byte[] excelFile = null;

                    using (var package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("დეტალური რეპორტი-" + DateTime.Now.ToShortDateString());

                        worksheet.Cells[1, 1].Value = "კონტრაგენტი";
                        worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 2].Value = "მგზავრობის თარიღი";
                        worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 3].Value = "მგზავრობის დაწყების დრო";
                        worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 4].Value = "მგზავრობის დასრულების დრო";
                        worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 5].Value = "მძღოლი";
                        worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 6].Value = "ავტომობილის N";
                        worksheet.Cells[1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 7].Value = "გავლილი მანძილი";
                        worksheet.Cells[1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 8].Value = "ღირებულება";
                        worksheet.Cells[1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 9].Value = "სადგომის ხარჯი";
                        worksheet.Cells[1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 10].Value = "საბაჟოს მოსაკრებელი";
                        worksheet.Cells[1, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 11].Value = "დამატებითი ხარჯი";
                        worksheet.Cells[1, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 12].Value = "სულ ჯამი";
                        worksheet.Cells[1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 13].Value = "სულ ჯამი + დღგ";
                        worksheet.Cells[1, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 14].Value = "სულ ჯამი - დღგ";
                        worksheet.Cells[1, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 15].Value = "პრინტერის გარეშე";
                        worksheet.Cells[1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 15].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 16].Value = "კომენტარი";
                        worksheet.Cells[1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        for (int i = 2, j = 0; i <= result.Count + 1 && j < result.Count; i++, j++)
                        {
                            worksheet.Cells[i, 1].Value = result[j].companyName;
                            worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 2].Value = result[j].dateNow.ToString("yyyy-MM-dd");
                            worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 3].Value = result[j].startTime;
                            worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 4].Value = result[j].endTime;
                            worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 5].Value = result[j].staffName;
                            worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 6].Value = result[j].carNumber;
                            worksheet.Cells[i, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 7].Value = Convert.ToDouble(result[j].traveledDistance, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 8].Value =
                                result[j].contrVat == "1" ? Math.Round((result[j].amountPrice.Value - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "2" ? Math.Round(((result[j].amountPrice.Value / 1.18) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "3" ? Math.Round(((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : result[j].amountPrice.Value;
                            worksheet.Cells[i, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 9].Value = Convert.ToDouble(Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture));
                            worksheet.Cells[i, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 10].Value = Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 11].Value = Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 12].Value =
                                result[j].contrVat == "1" ? Math.Round(result[j].amountPrice.Value, 1) : result[j].contrVat == "2" ? Math.Round((result[j].amountPrice.Value / 1.18), 1) : result[j].contrVat == "3" ? Math.Round((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)), 1) : 0;
                            worksheet.Cells[i, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 13].Value =
                                result[j].contrVat == "2" ? Math.Round(result[j].amountPrice.Value, 2) : 0;
                            worksheet.Cells[i, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 14].Value =
                                result[j].contrVat == "3" ? Math.Round(result[j].amountPrice.Value, 2) : 0; ;
                            worksheet.Cells[i, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 15].Value = result[j].withoutPrint;
                            worksheet.Cells[i, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 16].Value = result[j].commentOut;
                            worksheet.Cells[i, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        //worksheet.Cells[res.Count + 2, 10].Style.Font.Bold = true;
                        //worksheet.Cells[res.Count + 2, 10].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[res.Count + 2, 10].Value = Math.Round(totalSumList.Sum(), 2);

                        worksheet.Cells[result.Count + 5, 1].Value = "თანხა დღგ-ს გარეშე";
                        worksheet.Cells[result.Count + 5, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 5, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[result.Count + 5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 1].Value = Math.Round(withoutVat, 2);
                        worksheet.Cells[result.Count + 6, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 6, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        worksheet.Cells[result.Count + 5, 2].Value = "დღგ";
                        worksheet.Cells[result.Count + 5, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 5, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[result.Count + 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 2].Value = Math.Round(vat, 2);
                        worksheet.Cells[result.Count + 6, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 6, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        worksheet.Cells[result.Count + 5, 3].Value = "თანხა დღგ-ს ჩათვლით";
                        worksheet.Cells[result.Count + 5, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 5, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[result.Count + 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 3].Value = Math.Round(generalDocAmountSum, 2);
                        worksheet.Cells[result.Count + 6, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 6, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        worksheet.Column(7).AutoFit();
                        worksheet.Column(8).AutoFit();
                        worksheet.Column(9).AutoFit();
                        worksheet.Column(10).AutoFit();
                        worksheet.Column(11).AutoFit();
                        worksheet.Column(12).AutoFit();
                        worksheet.Column(13).AutoFit();
                        worksheet.Column(14).AutoFit();
                        worksheet.Column(15).AutoFit();
                        worksheet.Column(16).AutoFit();

                        excelFile = package.GetAsByteArray();
                    }

                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailedContragentReport.xlsx");
                }
                else
                {
                    var result = (from p in context.ProductsFlow
                                 where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                                 && p.GeneralDocs.Tdate.Value >= dateFrom
                                 && p.GeneralDocs.Tdate.Value <= dateTo
                                 join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join st in context.Staff on po.StaffId equals st.id
                                 join c in context.Contragents on po.GeneralDocs.ParamId1 equals c.id
                                 where (po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     companyName = c.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     staffName = st.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat
                                 }).OrderBy(r => r.dateNow).ToList();

                    var fileName = "DetailedContragentReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                    var outputDir = Path.GetTempPath();
                    var file = new FileInfo(outputDir + fileName);

                    byte[] excelFile = null;

                    using (var package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("დეტალური რეპორტი-" + DateTime.Now.ToShortDateString());

                        worksheet.Cells[1, 1].Value = "კონტრაგენტი";
                        worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 2].Value = "მგზავრობის თარიღი";
                        worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 3].Value = "მგზავრობის დაწყების დრო";
                        worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 4].Value = "მგზავრობის დასრულების დრო";
                        worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 5].Value = "მძღოლი";
                        worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 6].Value = "ავტომობილის N";
                        worksheet.Cells[1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 7].Value = "გავლილი მანძილი";
                        worksheet.Cells[1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 8].Value = "ღირებულება";
                        worksheet.Cells[1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 9].Value = "სადგომის ხარჯი";
                        worksheet.Cells[1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 10].Value = "საბაჟოს მოსაკრებელი";
                        worksheet.Cells[1, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 11].Value = "დამატებითი ხარჯი";
                        worksheet.Cells[1, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 12].Value = "სულ ჯამი";
                        worksheet.Cells[1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 13].Value = "სულ ჯამი + დღგ";
                        worksheet.Cells[1, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 14].Value = "სულ ჯამი - დღგ";
                        worksheet.Cells[1, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 15].Value = "პრინტერის გარეშე";
                        worksheet.Cells[1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 15].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 16].Value = "კომენტარი";
                        worksheet.Cells[1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        for (int i = 2, j = 0; i <= result.Count + 1 && j < result.Count; i++, j++)
                        {
                            worksheet.Cells[i, 1].Value = result[j].companyName;
                            worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 2].Value = result[j].dateNow.ToString("yyyy-MM-dd");
                            worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 3].Value = result[j].startTime;
                            worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 4].Value = result[j].endTime;
                            worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 5].Value = result[j].staffName;
                            worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 6].Value = result[j].carNumber;
                            worksheet.Cells[i, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 7].Value = Convert.ToDouble(result[j].traveledDistance, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 8].Value =
                                result[j].contrVat == "1" ? Math.Round((result[j].amountPrice.Value - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "2" ? Math.Round(((result[j].amountPrice.Value / 1.18) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "3" ? Math.Round(((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : result[j].amountPrice.Value;
                            worksheet.Cells[i, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 9].Value = Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 10].Value = Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 11].Value = Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 12].Value =
                                result[j].contrVat == "1" ? Math.Round(result[j].amountPrice.Value, 1) : result[j].contrVat == "2" ? Math.Round((result[j].amountPrice.Value / 1.18), 1) : result[j].contrVat == "3" ? Math.Round((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)), 1) : 0;
                            worksheet.Cells[i, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 13].Value =
                                result[j].contrVat == "2" ? Math.Round(result[j].amountPrice.Value, 2) : 0;
                            worksheet.Cells[i, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 14].Value =
                                result[j].contrVat == "3" ? Math.Round(result[j].amountPrice.Value, 2) : 0; ;
                            worksheet.Cells[i, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 15].Value = result[j].withoutPrint;
                            worksheet.Cells[i, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 16].Value = result[j].commentOut;
                            worksheet.Cells[i, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        //worksheet.Cells[res.Count + 2, 10].Style.Font.Bold = true;
                        //worksheet.Cells[res.Count + 2, 10].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[res.Count + 2, 10].Value = 0;

                        worksheet.Cells[result.Count + 5, 1].Value = "თანხა დღგ-ს გარეშე";
                        worksheet.Cells[result.Count + 5, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 5, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[result.Count + 5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 1].Value = 0;
                        worksheet.Cells[result.Count + 6, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 6, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        worksheet.Cells[result.Count + 5, 2].Value = "დღგ";
                        worksheet.Cells[result.Count + 5, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 5, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[result.Count + 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 2].Value = 0;
                        worksheet.Cells[result.Count + 6, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 6, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        worksheet.Cells[result.Count + 5, 3].Value = "თანხა დღგ-ს ჩათვლით";
                        worksheet.Cells[result.Count + 5, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 5, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[result.Count + 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 3].Value = 0;
                        worksheet.Cells[result.Count + 6, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        worksheet.Cells[result.Count + 6, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[result.Count + 6, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        worksheet.Column(7).AutoFit();
                        worksheet.Column(8).AutoFit();
                        worksheet.Column(9).AutoFit();
                        worksheet.Column(10).AutoFit();
                        worksheet.Column(11).AutoFit();
                        worksheet.Column(12).AutoFit();
                        worksheet.Column(13).AutoFit();
                        worksheet.Column(14).AutoFit();
                        worksheet.Column(15).AutoFit();
                        worksheet.Column(16).AutoFit();

                        excelFile = package.GetAsByteArray();
                    }

                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailedContragentReport.xlsx");
                }
            }
            else
            {
                return null;
            }
        }

        public FileResult GenerateDetailedDriverPdf(int? driverID, string fromDate, string toDate, bool withCreateDate)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);

            if (!isContragent)
            {
                fromDate += " 00:00:00";
                toDate += " 23:59:59";

                DateTime dateFrom = Convert.ToDateTime(fromDate);
                DateTime dateTo = Convert.ToDateTime(toDate);

                if (driverID != null)
                {                   
                    var result = (from p in context.ProductsFlow
                                  where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) >= dateFrom
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) <= dateTo
                                  join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join c in context.Contragents on p.GeneralDocs.ParamId1 equals c.id
                                 join stf in context.Staff on po.StaffId equals stf.id
                                 where (po.StaffId == driverID && po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     staffName = stf.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     contragentName = c.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat,
                                     contragentFakeID = c.fakeID
                                 }).OrderByDescending(r => r.POid).ToList();

                    List<double> totalSumList = new List<double>();
                    result.ForEach(r =>
                    {
                        totalSumList.Add(r.amountPrice.Value);
                    });

                    XElement element = new XElement("root",
                        new XElement("items", result.ToList().Select(i => new XElement("item",
                                new XElement("contragentFakeID", i.contragentFakeID),
                                new XElement("staffName", i.staffName),
                                new XElement("dateNow", string.Format("{0} {1}-{2}", i.dateNow.ToString("yyyy-MM-dd"), i.startTime, i.endTime)),
                                new XElement("companyName", i.contragentName),
                                new XElement("carNumber", i.carNumber),
                                new XElement("traveledDistance", i.traveledDistance),
                                new XElement("amountPrice",
                                i.contrVat == "1" ? Math.Round((i.amountPrice.Value - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "2" ? Math.Round(((i.amountPrice.Value / 1.18) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "3" ? Math.Round(((i.amountPrice.Value + (i.amountPrice.Value * 0.18)) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : i.amountPrice.Value),
                                new XElement("parkingCosts", i.parkingCosts),
                                new XElement("customsFees", i.customsFees),
                                new XElement("additionalCosts", i.additionalCosts),
                                new XElement("subTotalSum", i.contrVat == "1" ? Math.Round(i.amountPrice.Value, 1) : i.contrVat == "2" ? Math.Round((i.amountPrice.Value / 1.18), 1) : i.contrVat == "3" ? Math.Round((i.amountPrice.Value + (i.amountPrice.Value * 0.18)), 1) : 0),
                                new XElement("TSWV1", i.contrVat == "2" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("TSWV2", i.contrVat == "3" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("withoutPrint", i.withoutPrint),
                                new XElement("commentOut", i.commentOut)
                        ))),
                        new XElement("totalSum", Math.Round(totalSumList.Sum(), 2))
                    );

                    ExportPdf exportPdf = new ExportPdf();

                    string fileName = "DetailedDriverReport.pdf";
                    PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4, Orientation = PdfPageOrientation.Landscape };
                    string filePath = exportPdf.GeneratePdf(element, "DetailedDriverReport.xslt", setting);
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    string contentType = MimeMapping.GetMimeMapping(filePath);

                    System.Net.Mime.ContentDisposition cd = new ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(fileData, contentType);
                }
                else
                {
                    var result = (from p in context.ProductsFlow
                                  where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) >= dateFrom
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) <= dateTo
                                  join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join c in context.Contragents on p.GeneralDocs.ParamId1 equals c.id
                                 join stf in context.Staff on po.StaffId equals stf.id
                                 where (po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     staffName = stf.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     contragentName = c.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat,
                                     contragentFakeID = c.fakeID
                                 }).OrderByDescending(r => r.POid).ToList();

                    List<double> totalSumList = new List<double>();
                    result.ForEach(r =>
                    {
                        totalSumList.Add(r.amountPrice.Value);
                    });

                    XElement element = new XElement("root",
                        new XElement("items", result.ToList().Select(i => new XElement("item",
                                new XElement("contragentFakeID", i.contragentFakeID),
                                new XElement("staffName", i.staffName),
                                new XElement("dateNow", string.Format("{0} {1}-{2}", i.dateNow.ToString("yyyy-MM-dd"), i.startTime, i.endTime)),
                                new XElement("companyName", i.contragentName),
                                new XElement("carNumber", i.carNumber),
                                new XElement("traveledDistance", i.traveledDistance),
                                new XElement("amountPrice",
                                i.contrVat == "1" ? Math.Round((i.amountPrice.Value - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "2" ? Math.Round(((i.amountPrice.Value / 1.18) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                i.contrVat == "3" ? Math.Round(((i.amountPrice.Value + (i.amountPrice.Value * 0.18)) - (Convert.ToDouble(i.parkingCosts == null || i.parkingCosts == string.Empty ? "0" : i.parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.customsFees == null || i.customsFees == string.Empty ? "0" : i.customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(i.additionalCosts == null || i.additionalCosts == string.Empty ? "0" : i.additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : i.amountPrice.Value),
                                new XElement("parkingCosts", i.parkingCosts),
                                new XElement("customsFees", i.customsFees),
                                new XElement("additionalCosts", i.additionalCosts),
                                new XElement("subTotalSum", i.contrVat == "1" ? Math.Round(i.amountPrice.Value, 1) : i.contrVat == "2" ? Math.Round((i.amountPrice.Value / 1.18), 1) : i.contrVat == "3" ? Math.Round((i.amountPrice.Value + (i.amountPrice.Value * 0.18)), 1) : 0),
                                new XElement("TSWV1", i.contrVat == "2" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("TSWV2", i.contrVat == "3" ? Math.Round(i.amountPrice.Value, 2) : 0),
                                new XElement("withoutPrint", i.withoutPrint),
                                new XElement("commentOut", i.commentOut)
                        ))),
                        new XElement("totalSum", Math.Round(totalSumList.Sum(), 2))
                    );

                    ExportPdf exportPdf = new ExportPdf();

                    string fileName = "DetailedDriverReport.pdf";
                    PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4, Orientation = PdfPageOrientation.Landscape };
                    string filePath = exportPdf.GeneratePdf(element, "DetailedDriverReport.xslt", setting);
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    string contentType = MimeMapping.GetMimeMapping(filePath);

                    System.Net.Mime.ContentDisposition cd = new ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(fileData, contentType);
                }
            }
            else
            {
                return null;
            }
        }

        public FileResult GenerateDetailedDriverExcel(int? driverID, string fromDate, string toDate, bool withCreateDate)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);

            if (!isContragent)
            {
                fromDate += " 00:00:00";
                toDate += " 23:59:59";

                DateTime dateFrom = Convert.ToDateTime(fromDate);
                DateTime dateTo = Convert.ToDateTime(toDate);

                if (driverID != null)
                {                   
                    var result = (from p in context.ProductsFlow
                                  where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) >= dateFrom
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) <= dateTo
                                  join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join c in context.Contragents on p.GeneralDocs.ParamId1 equals c.id
                                 join stf in context.Staff on po.StaffId equals stf.id
                                 where (po.StaffId == driverID && po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     staffName = stf.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     contragentName = c.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat,
                                     contragentFakeID = c.fakeID
                                 }).OrderByDescending(r => r.POid).ToList();


                    List<double> totalSumList = new List<double>();
                    result.ForEach(r =>
                    {
                        totalSumList.Add(r.amountPrice.Value);
                    });

                    var fileName = "DetailedDriverReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                    var outputDir = Path.GetTempPath();
                    var file = new FileInfo(outputDir + fileName);

                    byte[] excelFile = null;

                    using (var package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("დეტალური რეპორტი-" + DateTime.Now.ToShortDateString());

                        worksheet.Cells[1, 1].Value = "კომპანიის ID";
                        worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 2].Value = "მძღოლი";
                        worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 3].Value = "მგზავრობის თარიღი";
                        worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 4].Value = "კონტრაგენტი";
                        worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 5].Value = "ავტომობილის N";
                        worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 6].Value = "გავლილი მანძილი";
                        worksheet.Cells[1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 7].Value = "ღირებულება";
                        worksheet.Cells[1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 8].Value = "სადგომის ხარჯი";
                        worksheet.Cells[1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 9].Value = "საბაჟოს მოსაკრებელი";
                        worksheet.Cells[1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 10].Value = "დამატებითი ხარჯი";
                        worksheet.Cells[1, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 11].Value = "სულ ჯამი";
                        worksheet.Cells[1, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 12].Value = "სულ ჯამი + დღგ";
                        worksheet.Cells[1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 13].Value = "სულ ჯამი - დღგ";
                        worksheet.Cells[1, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 14].Value = "პრინტერის გარეშე";
                        worksheet.Cells[1, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 15].Value = "კომენტარი";
                        worksheet.Cells[1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 15].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        for (int i = 2, j = 0; i <= result.Count + 1 && j < result.Count; i++, j++)
                        {
                            worksheet.Cells[i, 1].Value = result[j].contragentFakeID;
                            worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 2].Value = result[j].staffName;
                            worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 3].Value = string.Format("{0} {1}-{2}", result[j].dateNow.ToString("yyyy-MM-dd"), result[j].startTime, result[j].endTime);
                            worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 4].Value = result[j].contragentName;
                            worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 5].Value = result[j].carNumber;
                            worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 6].Value = Convert.ToDouble(result[j].traveledDistance, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 7].Value =
                                result[j].contrVat == "1" ? Math.Round((result[j].amountPrice.Value - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "2" ? Math.Round(((result[j].amountPrice.Value / 1.18) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "3" ? Math.Round(((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : result[j].amountPrice.Value;
                            worksheet.Cells[i, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 8].Value = Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 9].Value = Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 10].Value = Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 11].Value =
                                result[j].contrVat == "1" ? Math.Round(result[j].amountPrice.Value, 1) : result[j].contrVat == "2" ? Math.Round((result[j].amountPrice.Value / 1.18), 1) : result[j].contrVat == "3" ? Math.Round((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)), 1) : 0;
                            worksheet.Cells[i, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 12].Value =
                                result[j].contrVat == "2" ? Math.Round(result[j].amountPrice.Value, 2) : 0;
                            worksheet.Cells[i, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 13].Value =
                                result[j].contrVat == "3" ? Math.Round(result[j].amountPrice.Value, 2) : 0;
                            worksheet.Cells[i, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 14].Value = result[j].withoutPrint;
                            worksheet.Cells[i, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 15].Value = result[j].commentOut;
                            worksheet.Cells[i, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        //worksheet.Cells[res.Count + 4, 11].Value = "ჯამი";
                        //worksheet.Cells[res.Count + 4, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //worksheet.Cells[res.Count + 4, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        //worksheet.Cells[res.Count + 5, 11].Value = Math.Round(totalSumList.Sum(), 2);
                        //worksheet.Cells[res.Count + 5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        worksheet.Column(7).AutoFit();
                        worksheet.Column(8).AutoFit();
                        worksheet.Column(9).AutoFit();
                        worksheet.Column(10).AutoFit();
                        worksheet.Column(11).AutoFit();
                        worksheet.Column(12).AutoFit();
                        worksheet.Column(13).AutoFit();
                        worksheet.Column(14).AutoFit();
                        worksheet.Column(15).AutoFit();

                        excelFile = package.GetAsByteArray();
                    }

                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailedDriverReport.xlsx");
                }
                else
                {
                    var result = (from p in context.ProductsFlow
                                  where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) >= dateFrom
                                 && (withCreateDate ? p.GeneralDocs.CreateDate : p.GeneralDocs.Tdate.Value) <= dateTo
                                  join po in context.ProductOut on p.GeneralId equals po.GeneralId
                                 join c in context.Contragents on p.GeneralDocs.ParamId1 equals c.id
                                 join stf in context.Staff on po.StaffId equals stf.id
                                 where (po.AddedWithWeb == "true" && po.Deleted == "false")
                                 select new
                                 {
                                     POid = po.Id,
                                     GDid = p.GeneralDocs.Id,
                                     staffName = stf.name,
                                     dateNow = p.GeneralDocs.Tdate.Value,
                                     startTime = po.StartTime,
                                     endTime = po.EndTime,
                                     contragentName = c.name,
                                     carNumber = po.CarNumber,
                                     traveledDistance = po.TraveledDistance,
                                     amountPrice = po.GeneralDocs.Amount,
                                     parkingCosts = po.ParkingCosts,
                                     customsFees = po.CustomsFees,
                                     additionalCosts = po.AdditionalCosts,
                                     withoutPrint = po.WithoutPrint,
                                     commentOut = po.CommentOut,
                                     vat = c.customVat,
                                     contrVat = po.contrVat,
                                     contragentFakeID = c.fakeID
                                 }).OrderByDescending(r => r.POid).ToList();

                    List<double> totalSumList = new List<double>();
                    result.ForEach(r =>
                    {
                        totalSumList.Add(r.amountPrice.Value);
                    });

                    var fileName = "DetailedDriverReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                    var outputDir = Path.GetTempPath();
                    var file = new FileInfo(outputDir + fileName);

                    byte[] excelFile = null;

                    using (var package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("დეტალური რეპორტი-" + DateTime.Now.ToShortDateString());

                        worksheet.Cells[1, 1].Value = "კომპანიის ID";
                        worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 2].Value = "მძღოლი";
                        worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 3].Value = "მგზავრობის თარიღი";
                        worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 4].Value = "კონტრაგენტი";
                        worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 5].Value = "ავტომობილის N";
                        worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 6].Value = "გავლილი მანძილი";
                        worksheet.Cells[1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 7].Value = "ღირებულება";
                        worksheet.Cells[1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 8].Value = "სადგომის ხარჯი";
                        worksheet.Cells[1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 9].Value = "საბაჟოს მოსაკრებელი";
                        worksheet.Cells[1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 10].Value = "დამატებითი ხარჯი";
                        worksheet.Cells[1, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 11].Value = "სულ ჯამი";
                        worksheet.Cells[1, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 12].Value = "სულ ჯამი + დღგ";
                        worksheet.Cells[1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 13].Value = "სულ ჯამი - დღგ";
                        worksheet.Cells[1, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 14].Value = "პრინტერის გარეშე";
                        worksheet.Cells[1, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 15].Value = "კომენტარი";
                        worksheet.Cells[1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 15].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        for (int i = 2, j = 0; i <= result.Count + 1 && j < result.Count; i++, j++)
                        {
                            worksheet.Cells[i, 1].Value = result[j].contragentFakeID;
                            worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 2].Value = result[j].staffName;
                            worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 3].Value = string.Format("{0} {1}-{2}", result[j].dateNow.ToString("yyyy-MM-dd"), result[j].startTime, result[j].endTime);
                            worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 4].Value = result[j].contragentName;
                            worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 5].Value = result[j].carNumber;
                            worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 6].Value = Convert.ToDouble(result[j].traveledDistance, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 7].Value =
                                result[j].contrVat == "1" ? Math.Round((result[j].amountPrice.Value - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "2" ? Math.Round(((result[j].amountPrice.Value / 1.18) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                                result[j].contrVat == "3" ? Math.Round(((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)) - (Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 1) : result[j].amountPrice.Value;
                            worksheet.Cells[i, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 8].Value = Convert.ToDouble(result[j].parkingCosts == null || result[j].parkingCosts == string.Empty ? "0" : result[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 9].Value = Convert.ToDouble(result[j].customsFees == null || result[j].customsFees == string.Empty ? "0" : result[j].customsFees, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 10].Value = Convert.ToDouble(result[j].additionalCosts == null || result[j].additionalCosts == string.Empty ? "0" : result[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture);
                            worksheet.Cells[i, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 11].Value =
                                result[j].contrVat == "1" ? Math.Round(result[j].amountPrice.Value, 1) : result[j].contrVat == "2" ? Math.Round((result[j].amountPrice.Value / 1.18), 1) : result[j].contrVat == "3" ? Math.Round((result[j].amountPrice.Value + (result[j].amountPrice.Value * 0.18)), 1) : 0;
                            worksheet.Cells[i, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 12].Value =
                                result[j].contrVat == "2" ? Math.Round(result[j].amountPrice.Value, 2) : 0;
                            worksheet.Cells[i, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 13].Value =
                                result[j].contrVat == "3" ? Math.Round(result[j].amountPrice.Value, 2) : 0;
                            worksheet.Cells[i, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 14].Value = result[j].withoutPrint;
                            worksheet.Cells[i, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 15].Value = result[j].commentOut;
                            worksheet.Cells[i, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        //worksheet.Cells[res.Count + 4, 11].Value = "ჯამი";
                        //worksheet.Cells[res.Count + 4, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //worksheet.Cells[res.Count + 4, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        //worksheet.Cells[res.Count + 5, 11].Value = Math.Round(totalSumList.Sum(), 2);
                        //worksheet.Cells[res.Count + 5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        worksheet.Column(7).AutoFit();
                        worksheet.Column(8).AutoFit();
                        worksheet.Column(9).AutoFit();
                        worksheet.Column(10).AutoFit();
                        worksheet.Column(11).AutoFit();
                        worksheet.Column(12).AutoFit();
                        worksheet.Column(13).AutoFit();
                        worksheet.Column(14).AutoFit();
                        worksheet.Column(15).AutoFit();

                        excelFile = package.GetAsByteArray();
                    }

                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailedDriverReport.xlsx");
                }
            }
            else
            {
                return null;
            }
        }

        public FileResult GenerateBalancePdf(int? contragentID, string year, string month)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);
            bool isOwner = false;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            if (!isContragent)
            {
                isOwner = true;
            }
            else
            {
                try
                {
                    isOwner = User.Identity.Name == contragent.code;
                }
                catch (Exception)
                {
                    isOwner = false;
                }
            }

            if (isOwner)
            {
                var yearNum = Convert.ToInt32(year);
                var monthNum = Convert.ToInt32(month);

                DateTime dateFrom = new DateTime(yearNum, monthNum, 1);
                DateTime dateTo = new DateTime(yearNum, monthNum, DateTime.DaysInMonth(yearNum, monthNum));
                List<BalanceModel> finalResult = new List<BalanceModel>();
                var prevSum = 0.0;
                var currentSum = 0.0;
                var totalSum = 0.0;
                var cur = 0.0;

                if (contragentID != null)
                {
                    if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                    {
                        var contrMonthPeriod = Convert.ToInt32(contragent.monthPeriod);
                        if (monthNum == 1)
                        {
                            dateFrom = new DateTime(yearNum - 1, 12, contrMonthPeriod);
                            dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                        }
                        else
                        {
                            dateFrom = new DateTime(yearNum, monthNum - 1, contrMonthPeriod);
                            dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                        }
                    }

                    if (isContragent)
                    {
                        var now = DateTime.Now;
                        string[] contrMaxMonth = null;
                        if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                        {
                            contrMaxMonth = contragent.maxMonth.Split(';');

                            if(contrMaxMonth.Length == 2)
                            {
                                if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                {
                                    contragentID = null;
                                    dateFrom = new DateTime(1000, 10, 10);
                                    dateTo = new DateTime(1000, 10, 10);
                                }
                            }
                        }                        
                    }

                    string _path = contragent.path;
                    string _account = GetContragentAccountByPath(_path, 0);
                    string _account2 = GetContragentAccountByPath(_path, 1);

                    //sawyisi
                    var debts = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var creds = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                    var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                    var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                           .Select(en => new
                                           {
                                               generalID = en.GeneralId,
                                               amountPrice = en.GeneralDocs.Amount.Value
                                           }).ToList();
                    var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                    //sawyisi nashti
                    var startBalance = debetStartSum / 2;
                    var stBalance = Math.Round(startBalance, 2);
                    stBalance = stBalance - creditStartSum;

                    if (!isContragent)
                    {
                        var now = DateTime.Now;
                        string[] contrMaxMonth = null;
                        if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                        {
                            contrMaxMonth = contragent.maxMonth.Split(';');

                            if(contrMaxMonth.Length == 2)
                            {
                                if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                {
                                    contragentID = null;
                                }
                            }
                        }                        
                    }

                    //mimdinare                    
                    var debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                    var debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                    var creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                           .Select(en => new
                                           {
                                               generalID = en.GeneralId,
                                               amountPrice = en.GeneralDocs.Amount.Value
                                           }).ToList();
                    var creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();

                    if (!string.IsNullOrEmpty(contragent.monthPeriod))
                    {
                        debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                        debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                        creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                               .Select(en => new
                                               {
                                                   generalID = en.GeneralId,
                                                   amountPrice = en.GeneralDocs.Amount.Value
                                               }).ToList();
                        creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();
                    }

                    //mimdinare nashti
                    var currentBalance = debetCurrentSum / 2;
                    var currBalance = Math.Round(currentBalance, 2);
                    currBalance = currBalance - creditCurrentSum;


                    var res = new BalanceModel
                    {
                        fakeID = contragent.fakeID,
                        id = contragent.id,
                        companyName = contragent.name,
                        prevPeriod = Math.Round(stBalance, 2),
                        currentPeriod = Math.Round(currBalance, 2),
                        totalPeriod = Math.Round((stBalance + currBalance), 2),
                        dateFrom = dateFrom.ToString(),
                        dateTo = dateTo.ToString()
                    };
                    if (dateFrom != new DateTime(1000, 10, 10))
                    {
                        finalResult.Add(res);
                    }
                    prevSum = finalResult.Where(fr => fr.prevPeriod > 0).Select(fr => fr.prevPeriod).Sum();
                    currentSum = finalResult.Where(fr => fr.currentPeriod > 0).Select(fr => fr.currentPeriod).Sum();
                    totalSum = finalResult.Where(fr => fr.totalPeriod > 0).Select(fr => fr.totalPeriod).Sum();
                    cur = finalResult.Where(fr => fr.currentPeriod < 0).Select(fr => fr.currentPeriod).Sum();

                    XElement element = new XElement("root",
                        new XElement("items", finalResult.Select(i => new XElement("item",
                            new XElement("fakeID", i.fakeID),
                            new XElement("companyName", i.companyName),
                            new XElement("prevPeriod", i.prevPeriod),
                            new XElement("currentPeriod", i.currentPeriod),
                            new XElement("totalPeriod", i.totalPeriod)
                        ))),
                        new XElement("prevSum", Math.Round(prevSum, 2)),
                        new XElement("currentSum", Math.Round(currentSum, 2)),
                        new XElement("totalSum", Math.Round(totalSum, 2)),
                        new XElement("cur", Math.Round(cur, 2))
                    );

                    ExportPdf exportPdf = new ExportPdf();

                    string fileName = "BalanceReport.pdf";
                    PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4, Orientation = PdfPageOrientation.Landscape };
                    string filePath = exportPdf.GeneratePdf(element, "BalanceReport.xslt", setting);
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    string contentType = MimeMapping.GetMimeMapping(filePath);

                    System.Net.Mime.ContentDisposition cd = new ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(fileData, contentType);
                }
                else
                {
                    dateFrom = dateFrom.Month == 1 ? new DateTime(dateFrom.Year - 1, 12, dateFrom.Day) : new DateTime(dateFrom.Year, dateFrom.Month - 1, dateFrom.Day);
                    var contragents = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.DocType == 29 && gd.ProductOuts.Any(po => po.Deleted == "false"))
                        .GroupBy(gd => gd.ParamId1);

                    contragents.ToList().ForEach(c =>
                    {
                        dateFrom = new DateTime(yearNum, monthNum, 1);
                        dateTo = new DateTime(yearNum, monthNum, DateTime.DaysInMonth(yearNum, monthNum));

                        var contragent1 = context.Contragents.FirstOrDefault(cont => cont.id == c.Key);
                        var cID = c.Key;

                        if (contragent1 != null)
                        {
                            if (contragent1.id > 0)
                            {
                                if (contragent1.monthPeriod != null && contragent1.monthPeriod != string.Empty)
                                {
                                    var contrMonthPeriod = Convert.ToInt32(contragent1.monthPeriod);
                                    if (monthNum == 1)
                                    {
                                        dateFrom = new DateTime(yearNum - 1, 12, contrMonthPeriod);
                                        dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                                    }
                                    else
                                    {
                                        dateFrom = new DateTime(yearNum, monthNum - 1, contrMonthPeriod);
                                        dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                                    }
                                }

                                string _path = contragent1.path;
                                string _account = GetContragentAccountByPath(_path, 0);
                                string _account2 = GetContragentAccountByPath(_path, 1);

                                //sawyisi
                                var debts = (contragent1.debts != string.Empty && contragent1.debts != null && Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                                var creds = (contragent1.debts != string.Empty && contragent1.debts != null && Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                                var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                          .Select(en => new
                                                          {
                                                              generalID = en.GeneralId,
                                                              amountPrice = en.GeneralDocs.Amount.Value
                                                          }).ToList();
                                var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                                var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                       .Select(en => new
                                                       {
                                                           generalID = en.GeneralId,
                                                           amountPrice = en.GeneralDocs.Amount.Value
                                                       }).ToList();
                                var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                                //sawyisi nashti
                                var startBalance = debetStartSum / 2;
                                var stBalance = Math.Round(startBalance, 2);
                                stBalance = stBalance - creditStartSum;

                                if (!isContragent)
                                {
                                    var now = DateTime.Now;
                                    string[] contrMaxMonth = null;
                                    if (contragent1.maxMonth != null && contragent1.maxMonth != string.Empty)
                                    {
                                        contrMaxMonth = contragent1.maxMonth.Split(';');

                                        if(contrMaxMonth.Length == 2)
                                        {
                                            if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                            {
                                                cID = null;
                                            }
                                        }
                                    }                                    
                                }

                                //mimdinare                                
                                var debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                          .Select(en => new
                                                          {
                                                              generalID = en.GeneralId,
                                                              amountPrice = en.GeneralDocs.Amount.Value
                                                          }).ToList();
                                var debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                                var creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                       .Select(en => new
                                                       {
                                                           generalID = en.GeneralId,
                                                           amountPrice = en.GeneralDocs.Amount.Value
                                                       }).ToList();
                                var creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();

                                if (!string.IsNullOrEmpty(contragent1.monthPeriod))
                                {
                                    debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                          .Select(en => new
                                                          {
                                                              generalID = en.GeneralId,
                                                              amountPrice = en.GeneralDocs.Amount.Value
                                                          }).ToList();
                                    debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                                    creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                           .Select(en => new
                                                           {
                                                               generalID = en.GeneralId,
                                                               amountPrice = en.GeneralDocs.Amount.Value
                                                           }).ToList();
                                    creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();
                                }

                                //mimdinare nashti
                                var currentBalance = debetCurrentSum / 2;
                                var currBalance = Math.Round(currentBalance, 2);
                                currBalance = currBalance - creditCurrentSum;



                                var res = new BalanceModel
                                {
                                    fakeID = contragent1.fakeID,
                                    id = contragent1.id,
                                    companyName = contragent1.name,
                                    prevPeriod = Math.Round(stBalance, 2),
                                    currentPeriod = Math.Round(currBalance, 2),
                                    totalPeriod = Math.Round((stBalance + currBalance), 2),
                                    dateFrom = dateFrom.ToString(),
                                    dateTo = dateTo.ToString()
                                };
                                finalResult.Add(res);
                                prevSum = finalResult.Where(fr => fr.prevPeriod > 0).Select(fr => fr.prevPeriod).Sum();
                                currentSum = finalResult.Where(fr => fr.currentPeriod > 0).Select(fr => fr.currentPeriod).Sum();
                                totalSum = finalResult.Where(fr => fr.totalPeriod > 0).Select(fr => fr.totalPeriod).Sum();
                                cur = finalResult.Where(fr => fr.currentPeriod < 0).Select(fr => fr.currentPeriod).Sum();
                            }
                        }
                    });

                    finalResult = finalResult.OrderByDescending(fr => fr.totalPeriod).ToList();

                    XElement element = new XElement("root",
                        new XElement("items", finalResult.Select(i => new XElement("item",
                            new XElement("fakeID", i.fakeID),
                            new XElement("companyName", i.companyName),
                            new XElement("prevPeriod", i.prevPeriod),
                            new XElement("currentPeriod", i.currentPeriod),
                            new XElement("totalPeriod", i.totalPeriod)
                        ))),
                        new XElement("prevSum", Math.Round(prevSum, 2)),
                        new XElement("currentSum", Math.Round(currentSum)),
                        new XElement("totalSum", Math.Round(totalSum)),
                        new XElement("cur", Math.Round(cur, 2))
                    );

                    ExportPdf exportPdf = new ExportPdf();

                    string fileName = "BalanceReport.pdf";
                    PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4, Orientation = PdfPageOrientation.Landscape };
                    string filePath = exportPdf.GeneratePdf(element, "BalanceReport.xslt", setting);
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    string contentType = MimeMapping.GetMimeMapping(filePath);

                    System.Net.Mime.ContentDisposition cd = new ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(fileData, contentType);
                }
            }
            else
            {
                return null;
            }
        }

        public FileResult GenerateBalanceExcel(int? contragentID, string year, string month)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);
            bool isOwner = false;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            if (!isContragent)
            {
                isOwner = true;
            }
            else
            {
                try
                {
                    isOwner = User.Identity.Name == contragent.code;
                }
                catch (Exception)
                {
                    isOwner = false;
                }
            }

            if (isOwner)
            {
                var yearNum = Convert.ToInt32(year);
                var monthNum = Convert.ToInt32(month);

                DateTime dateFrom = new DateTime(yearNum, monthNum, 1);
                DateTime dateTo = new DateTime(yearNum, monthNum, DateTime.DaysInMonth(yearNum, monthNum));
                List<BalanceModel> finalResult = new List<BalanceModel>();
                var prevSum = 0.0;
                var currentSum = 0.0;
                var totalSum = 0.0;
                var cur = 0.0;

                if (contragentID != null)
                {
                    if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                    {
                        var contrMonthPeriod = Convert.ToInt32(contragent.monthPeriod);
                        if (monthNum == 1)
                        {
                            dateFrom = new DateTime(yearNum - 1, 12, contrMonthPeriod);
                            dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                        }
                        else
                        {
                            dateFrom = new DateTime(yearNum, monthNum - 1, contrMonthPeriod);
                            dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                        }
                    }

                    if (isContragent)
                    {
                        var now = DateTime.Now;
                        string[] contrMaxMonth = null;
                        if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                        {
                            contrMaxMonth = contragent.maxMonth.Split(';');

                            if(contrMaxMonth.Length == 2)
                            {
                                if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                {
                                    contragentID = null;
                                    dateFrom = new DateTime(1000, 10, 10);
                                    dateTo = new DateTime(1000, 10, 10);
                                }
                            }
                        }                        
                    }

                    string _path = contragent.path;
                    string _account = GetContragentAccountByPath(_path, 0);
                    string _account2 = GetContragentAccountByPath(_path, 1);

                    //sawyisi
                    var debts = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var creds = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                    var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                    var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                           .Select(en => new
                                           {
                                               generalID = en.GeneralId,
                                               amountPrice = en.GeneralDocs.Amount.Value
                                           }).ToList();
                    var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                    //sawyisi nashti
                    var startBalance = debetStartSum / 2;
                    var stBalance = Math.Round(startBalance, 2);
                    stBalance = stBalance - creditStartSum;

                    if (!isContragent)
                    {
                        var now = DateTime.Now;
                        string[] contrMaxMonth = null;
                        if (contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                        {
                            contrMaxMonth = contragent.maxMonth.Split(';');

                            if(contrMaxMonth.Length == 2)
                            {
                                if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                {
                                    contragentID = null;
                                }
                            }
                        }                        
                    }

                    //mimdinare                    
                    var debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                    var debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                    var creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                           .Select(en => new
                                           {
                                               generalID = en.GeneralId,
                                               amountPrice = en.GeneralDocs.Amount.Value
                                           }).ToList();
                    var creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();

                    if (!string.IsNullOrEmpty(contragent.monthPeriod))
                    {
                        debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                        debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                        creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                               .Select(en => new
                                               {
                                                   generalID = en.GeneralId,
                                                   amountPrice = en.GeneralDocs.Amount.Value
                                               }).ToList();
                        creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();
                    }

                    //mimdinare nashti
                    var currentBalance = debetCurrentSum / 2;
                    var currBalance = Math.Round(currentBalance, 2);
                    currBalance = currBalance - creditCurrentSum;



                    var res = new BalanceModel
                    {
                        fakeID = contragent.fakeID,
                        id = contragent.id,
                        companyName = contragent.name,
                        prevPeriod = Math.Round(stBalance, 2),
                        currentPeriod = Math.Round(currBalance, 2),
                        totalPeriod = Math.Round((stBalance + currBalance), 2),
                        dateFrom = dateFrom.ToString(),
                        dateTo = dateTo.ToString()
                    };
                    if (dateFrom != new DateTime(1000, 10, 10))
                    {
                        finalResult.Add(res);
                    }
                    prevSum = finalResult.Where(fr => fr.prevPeriod > 0).Select(fr => fr.prevPeriod).Sum();
                    currentSum = finalResult.Where(fr => fr.currentPeriod > 0).Select(fr => fr.currentPeriod).Sum();
                    totalSum = finalResult.Where(fr => fr.totalPeriod > 0).Select(fr => fr.totalPeriod).Sum();
                    cur = finalResult.Where(fr => fr.currentPeriod < 0).Select(fr => fr.currentPeriod).Sum();

                    var fileName = "BalanceReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                    var outputDir = Path.GetTempPath();
                    var file = new FileInfo(outputDir + fileName);

                    byte[] excelFile = null;

                    using (var package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ბალანსის რეპორტი-" + DateTime.Now.ToShortDateString());

                        worksheet.Cells[1, 1].Value = "ID";
                        worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 2].Value = "კომპანია";
                        worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 3].Value = "წინა პერიოდი";
                        worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 4].Value = "მიმდინარე";
                        worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 5].Value = "ჯამი";
                        worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        for (int i = 2, j = 0; i <= finalResult.Count + 1 && j < finalResult.Count; i++, j++)
                        {
                            worksheet.Cells[i, 1].Value = finalResult[j].fakeID;
                            worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 2].Value = finalResult[j].companyName;
                            worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 3].Value = finalResult[j].prevPeriod;
                            worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 4].Value = finalResult[j].currentPeriod;
                            worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 5].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[i, 5].Value = finalResult[j].totalPeriod;
                            worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        //worksheet.Cells[finalResult.Count + 2, 1].Value = "მიმდინარე თვის მეტობა:";
                        //worksheet.Cells[finalResult.Count + 2, 2].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 2].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        //worksheet.Cells[finalResult.Count + 2, 2].Value = Math.Round(cur, 2);

                        //worksheet.Cells[finalResult.Count + 2, 3].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[finalResult.Count + 2, 3].Value = Math.Round(prevSum, 2);

                        //worksheet.Cells[finalResult.Count + 2, 4].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 4].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[finalResult.Count + 2, 4].Value = Math.Round(currentSum, 2);

                        //worksheet.Cells[finalResult.Count + 2, 5].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 5].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[finalResult.Count + 2, 5].Value = Math.Round(totalSum, 2);

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();

                        excelFile = package.GetAsByteArray();
                    }

                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceReport.xlsx");
                }
                else
                {
                    dateFrom = dateFrom.Month == 1 ? new DateTime(dateFrom.Year - 1, 12, dateFrom.Day) : new DateTime(dateFrom.Year, dateFrom.Month - 1, dateFrom.Day);
                    var contragents = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.DocType == 29 && gd.ProductOuts.Any(po => po.Deleted == "false"))
                        .GroupBy(gd => gd.ParamId1);

                    contragents.ToList().ForEach(c =>
                    {
                        dateFrom = new DateTime(yearNum, monthNum, 1);
                        dateTo = new DateTime(yearNum, monthNum, DateTime.DaysInMonth(yearNum, monthNum));

                        var contragent1 = context.Contragents.FirstOrDefault(cont => cont.id == c.Key);
                        var cID = c.Key;

                        if (contragent1 != null)
                        {
                            if (contragent1.id > 0)
                            {
                                if (contragent1.monthPeriod != null && contragent1.monthPeriod != string.Empty)
                                {
                                    var contrMonthPeriod = Convert.ToInt32(contragent1.monthPeriod);
                                    if (monthNum == 1)
                                    {
                                        dateFrom = new DateTime(yearNum - 1, 12, contrMonthPeriod);
                                        dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                                    }
                                    else
                                    {
                                        dateFrom = new DateTime(yearNum, monthNum - 1, contrMonthPeriod);
                                        dateTo = new DateTime(yearNum, monthNum, contrMonthPeriod);
                                    }
                                }

                                string _path = contragent1.path;
                                string _account = GetContragentAccountByPath(_path, 0);
                                string _account2 = GetContragentAccountByPath(_path, 1);

                                //sawyisi
                                var debts = (contragent1.debts != string.Empty && contragent1.debts != null && Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                                var creds = (contragent1.debts != string.Empty && contragent1.debts != null && Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent1.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                                var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                          .Select(en => new
                                                          {
                                                              generalID = en.GeneralId,
                                                              amountPrice = en.GeneralDocs.Amount.Value
                                                          }).ToList();
                                var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                                var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                       .Select(en => new
                                                       {
                                                           generalID = en.GeneralId,
                                                           amountPrice = en.GeneralDocs.Amount.Value
                                                       }).ToList();
                                var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                                //sawyisi nashti
                                var startBalance = debetStartSum / 2;
                                var stBalance = Math.Round(startBalance, 2);
                                stBalance = stBalance - creditStartSum;

                                if (!isContragent)
                                {
                                    var now = DateTime.Now;
                                    string[] contrMaxMonth = null;
                                    if (contragent1.maxMonth != null && contragent1.maxMonth != string.Empty)
                                    {
                                        contrMaxMonth = contragent1.maxMonth.Split(';');

                                        if(contrMaxMonth.Length == 2)
                                        {
                                            if (dateTo.Month > Convert.ToInt32(contrMaxMonth[0]) && dateTo.Year >= Convert.ToInt32(contrMaxMonth[1]))
                                            {
                                                cID = null;
                                            }
                                        }
                                    }                                    
                                }

                                //mimdinare                                
                                var debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                          .Select(en => new
                                                          {
                                                              generalID = en.GeneralId,
                                                              amountPrice = en.GeneralDocs.Amount.Value
                                                          }).ToList();
                                var debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                                var creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) <= dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                       .Select(en => new
                                                       {
                                                           generalID = en.GeneralId,
                                                           amountPrice = en.GeneralDocs.Amount.Value
                                                       }).ToList();
                                var creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();

                                if (!string.IsNullOrEmpty(contragent1.monthPeriod))
                                {
                                    debetCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && en.GeneralDocs.ProductOuts.FirstOrDefault(po => po.GeneralId == en.GeneralId).Deleted == "false" && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == cID)
                                                          .Select(en => new
                                                          {
                                                              generalID = en.GeneralId,
                                                              amountPrice = en.GeneralDocs.Amount.Value
                                                          }).ToList();
                                    debetCurrentSum = debetCurrent.Select(dc => dc.amountPrice).Sum();

                                    creditCurrent = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) >= dateFrom && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateTo && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == cID)
                                                           .Select(en => new
                                                           {
                                                               generalID = en.GeneralId,
                                                               amountPrice = en.GeneralDocs.Amount.Value
                                                           }).ToList();
                                    creditCurrentSum = creditCurrent.Select(dc => dc.amountPrice).Sum();
                                }

                                //mimdinare nashti
                                var currentBalance = debetCurrentSum / 2;
                                var currBalance = Math.Round(currentBalance, 2);
                                currBalance = currBalance - creditCurrentSum;



                                var res = new BalanceModel
                                {
                                    fakeID = contragent1.fakeID,
                                    id = contragent1.id,
                                    companyName = contragent1.name,
                                    prevPeriod = Math.Round(stBalance, 2),
                                    currentPeriod = Math.Round(currBalance, 2),
                                    totalPeriod = Math.Round((stBalance + currBalance), 2),
                                    dateFrom = dateFrom.ToString(),
                                    dateTo = dateTo.ToString()
                                };
                                finalResult.Add(res);
                                prevSum = finalResult.Where(fr => fr.prevPeriod > 0).Select(fr => fr.prevPeriod).Sum();
                                currentSum = finalResult.Where(fr => fr.currentPeriod > 0).Select(fr => fr.currentPeriod).Sum();
                                totalSum = finalResult.Where(fr => fr.totalPeriod > 0).Select(fr => fr.totalPeriod).Sum();
                                cur = finalResult.Where(fr => fr.currentPeriod < 0).Select(fr => fr.currentPeriod).Sum();
                            }
                        }
                    });

                    finalResult = finalResult.OrderByDescending(fr => fr.totalPeriod).ToList();

                    var fileName = "BalanceReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                    var outputDir = Path.GetTempPath();
                    var file = new FileInfo(outputDir + fileName);

                    byte[] excelFile = null;

                    using (var package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ბალანსის რეპორტი-" + DateTime.Now.ToShortDateString());

                        worksheet.Cells[1, 1].Value = "ID";
                        worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 2].Value = "კომპანია";
                        worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 3].Value = "წინა პერიოდი";
                        worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 4].Value = "მიმდინარე";
                        worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        worksheet.Cells[1, 5].Value = "ჯამი";
                        worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                        for (int i = 2, j = 0; i <= finalResult.Count + 1 && j < finalResult.Count; i++, j++)
                        {
                            worksheet.Cells[i, 1].Value = finalResult[j].fakeID;
                            worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 2].Value = finalResult[j].companyName;
                            worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 3].Value = finalResult[j].prevPeriod;
                            worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 4].Value = finalResult[j].currentPeriod;
                            worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                            worksheet.Cells[i, 5].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[i, 5].Value = finalResult[j].totalPeriod;
                            worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }

                        //worksheet.Cells[finalResult.Count + 2, 1].Value = "მიმდინარე თვის მეტობა:";
                        //worksheet.Cells[finalResult.Count + 2, 2].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 2].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        //worksheet.Cells[finalResult.Count + 2, 2].Value = Math.Round(cur, 2);

                        //worksheet.Cells[finalResult.Count + 2, 3].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[finalResult.Count + 2, 3].Value = Math.Round(prevSum, 2);

                        //worksheet.Cells[finalResult.Count + 2, 4].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 4].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[finalResult.Count + 2, 4].Value = Math.Round(currentSum, 2);

                        //worksheet.Cells[finalResult.Count + 2, 5].Style.Font.Bold = true;
                        //worksheet.Cells[finalResult.Count + 2, 5].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        //worksheet.Cells[finalResult.Count + 2, 5].Value = Math.Round(totalSum, 2);

                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();

                        excelFile = package.GetAsByteArray();
                    }

                    return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceReport.xlsx");
                }
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetInvoices(int? contragentID, string year)
        {           
            Contragents contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID.Value);
            var result = Json(new {  }, JsonRequestBehavior.AllowGet);
            if(contragent != null)
            {               
                List<object> generalDocResult = new List<object>();

                if(contragent.maxMonth != null && contragent.maxMonth != string.Empty)
                {
                    int mxM = Convert.ToInt32(contragent.maxMonth.Split(';')[0]);
                    int mxY = Convert.ToInt32(contragent.maxMonth.Split(';')[1]);
                    int y = Convert.ToInt32(year);

                    if(mxY > y)
                    {
                        mxM = 12;
                    }

                    for (int i = 1; i <= mxM; i++)
                    {
                        DateTime dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-1", year, i));
                        DateTime dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, i, DateTime.DaysInMonth(Convert.ToInt32(year), i)));

                        var generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId1 == contragent.id
                            && gd.DocType.Value == 29
                            && DbFunctions.TruncateTime(gd.Tdate.Value) >= dateFrom
                            && DbFunctions.TruncateTime(gd.Tdate.Value) <= dateTo
                            && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();

                        if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                        {
                            DateTime dateFrom1 = new DateTime();
                            if (i == 1)
                            {
                                var year1 = Convert.ToInt32(year);
                                dateFrom1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", (year1 - 1), 12, contragent.monthPeriod));
                            }
                            else
                            {
                                dateFrom1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, (i - 1), contragent.monthPeriod));
                            }
                            DateTime dateTo1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, i, contragent.monthPeriod));

                            generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ProductOuts.FirstOrDefault(po => po.GeneralId == gd.Id).Deleted == "false" && gd.ParamId1 == contragent.id
                            && gd.DocType.Value == 29
                            && DbFunctions.TruncateTime(gd.Tdate.Value) >= dateFrom1
                            && DbFunctions.TruncateTime(gd.Tdate.Value) < dateTo1
                            && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();
                        }

                        if (generalDocs.Count > 0)
                        {
                            List<double> totalSumList = new List<double>();
                            generalDocs.ForEach(r =>
                            {
                                totalSumList.Add(r.Amount.Value);
                            });

                            var generalDocAmountSum = 0.0;
                            var vat = 0.0;
                            var withoutVat = 0.0;
                            if (contragent.customVat == "1")
                            {
                                generalDocAmountSum = totalSumList.Sum();
                                vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                                withoutVat = generalDocAmountSum - vat;
                            }
                            else if(contragent.customVat == "2")
                            {
                                generalDocAmountSum = totalSumList.Sum();
                                vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                                withoutVat = generalDocAmountSum - vat;
                            }
                            else if(contragent.customVat == "3")
                            {
                                vat = 0.0;
                                withoutVat = totalSumList.Sum();
                            }

                            var generalDoc = new
                            {
                                tDate = Convert.ToInt32(contragent.monthPeriod) > generalDocs.Last().Tdate.Value.Day ? generalDocs.Last().Tdate.Value : contragent.monthPeriod == string.Empty || contragent.monthPeriod == null ? generalDocs.Last().Tdate.Value : new DateTime(Convert.ToInt32(year), (generalDocs.Last().Tdate.Value.Month + 1), 1),
                                //purpose = generalDocs[0].Purpose,
                                withoutVat = Math.Round(withoutVat, 2),
                                vat = Math.Round(vat, 2),
                                withVat = Math.Round(generalDocAmountSum, 2)
                            };

                            generalDocResult.Add(generalDoc);
                        }
                    }
                }

                result = Json(new { generalDocs = generalDocResult }, JsonRequestBehavior.AllowGet);
            }

            return result;
        }

        public JsonResult GetInvoicesAdvanced(string year, string month)
        {
            List<object> generalDocResult = new List<object>();

            DateTime dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-1", year, Convert.ToInt32(month)));
            DateTime dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month))));

            var generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.DocType.Value == 29
                && gd.ParamId1 > 0
                && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();

            var groupedGeneralDocs = generalDocs.GroupBy(gd => gd.ParamId1).Select(ggd => ggd).ToList();
            groupedGeneralDocs.ForEach(ggd =>
            {
                var contragent = context.Contragents.FirstOrDefault(c => c.id > 0 && c.id == ggd.Key.Value);                

                if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                {
                    DateTime dateFrom1 = new DateTime();
                    if(month == "1" || month == "01")
                    {
                        var year1 = Convert.ToInt32(year);
                        dateFrom1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", (year1 - 1), 12, contragent.monthPeriod));
                    }
                    else
                    {
                        dateFrom1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, (Convert.ToInt32(month) - 1), contragent.monthPeriod));
                    }
                    DateTime dateTo1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, month, contragent.monthPeriod));

                    var t = ggd.AsQueryable().Where(x => x.Tdate.Value.Date >= dateFrom1
                    && x.Tdate.Value.Date < dateTo1).ToList();

                    List<double> totalSumList = new List<double>();
                    t.ForEach(r =>
                    {
                        var po = context.ProductOut.Where(p => p.GeneralId == r.Id).ToList();
                        totalSumList.Add(r.Amount.Value);
                    });

                    var generalDocAmountSum = 0.0;
                    var vat = 0.0;
                    var withoutVat = 0.0;
                    if (contragent.customVat == "1")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "2")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "3")
                    {
                        vat = 0.0;
                        withoutVat = totalSumList.Sum();
                    }

                    var generalDoc = new
                    {
                        contragentID = ggd.Key.Value,
                        contragentName = contragent == null ? "" : contragent.name,
                        //tDate = gd.Tdate.Value,
                        //purpose = gd.Purpose,
                        withoutVat = Math.Round(withoutVat, 2),
                        vat = Math.Round(vat, 2),
                        withVat = Math.Round(generalDocAmountSum, 2)
                    };

                    generalDocResult.Add(generalDoc);
                }
                else
                {
                    List<double> totalSumList = new List<double>();
                    ggd.ToList().ForEach(r =>
                    {
                        if(r.Tdate.Value.Year == dateFrom.Year && r.Tdate.Value.Month == dateFrom.Month)
                        {
                            totalSumList.Add(r.Amount.Value);
                        }
                    });

                    var generalDocAmountSum = 0.0;
                    var vat = 0.0;
                    var withoutVat = 0.0;
                    if (contragent.customVat == "1")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "2")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "3")
                    {
                        vat = 0.0;
                        withoutVat = totalSumList.Sum();
                    }

                    var generalDoc = new
                    {
                        contragentID = ggd.Key.Value,
                        contragentName = contragent == null ? "" : contragent.name,
                        //tDate = gd.Tdate.Value,
                        //purpose = gd.Purpose,
                        withoutVat = Math.Round(withoutVat, 2),
                        vat = Math.Round(vat, 2),
                        withVat = Math.Round(generalDocAmountSum, 2)
                    };

                    generalDocResult.Add(generalDoc);
                }                              
            });

            return Json(new { generalDocs = generalDocResult }, JsonRequestBehavior.AllowGet);
        }

        public FileResult GenerateInvoicePdf(int? contragentID, string year, string month)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);
            bool isOwner = false;
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
            if (!isContragent)
            {
                isOwner = true;
            }
            else
            {
                try
                {
                    isOwner = User.Identity.Name == contragent.code;
                }
                catch (Exception)
                {
                    isOwner = false;
                }
            }

            if (isOwner)
            {
                var contragentAccount = context.ContragentAccounts.FirstOrDefault(ca => ca.contragent_id == contragent.id);
                var company = context.Companies.ToList()[0];

                DateTime dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-1", year, month));
                DateTime dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, month, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month))));

                List<GeneralDocs> generalDocs = new List<GeneralDocs>();

                if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                {
                    if (month == "1" || month == "01")
                    {
                        var year1 = Convert.ToInt32(year);
                        dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-{2}", (year1 - 1), 12, contragent.monthPeriod));
                    }
                    else
                    {
                        dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, (Convert.ToInt32(month) - 1), contragent.monthPeriod));
                    }
                    dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, month, contragent.monthPeriod));

                    generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId1 == contragent.id
                    && gd.DocType == 29
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) >= dateFrom
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) < dateTo
                    && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();
                }
                else
                {
                    generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId1 == contragent.id
                    && gd.DocType == 29
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) >= dateFrom
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) <= dateTo
                    && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();
                }                                

                if (generalDocs.Count > 0)
                {
                    List<double> totalSumList = new List<double>();
                    generalDocs.ForEach(r =>
                    {
                        var po = context.ProductOut.Where(p => p.GeneralId == r.Id).ToList();
                        totalSumList.Add(r.Amount.Value);
                    });

                    var generalDocAmountSum = 0.0;
                    var vat = 0.0;
                    var withoutVat = 0.0;
                    if (contragent.customVat == "1")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "2")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "3")
                    {
                        vat = 0.0;
                        withoutVat = totalSumList.Sum();
                    }

                    string _path = context.Contragents.Where(c => c.id == contragentID).Select(a => a.path).FirstOrDefault();
                    string _account = GetContragentAccountByPath(_path, 0);
                    string _account2 = GetContragentAccountByPath(_path, 1);

                    //sawyisi
                    var debts = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var creds = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == contragentID)
                                              .Select(en => new
                                              {
                                                  generalID = en.GeneralId,
                                                  amountPrice = en.GeneralDocs.Amount.Value
                                              }).ToList();
                    var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                    var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == contragentID)
                                           .Select(en => new
                                           {
                                               generalID = en.GeneralId,
                                               amountPrice = en.GeneralDocs.Amount.Value
                                           }).ToList();
                    var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);

                    //sawyisi nashti
                    var startBalance = debetStartSum / 2;
                    var stBalance = Math.Round(startBalance, 2);
                    stBalance = stBalance - creditStartSum;
                    var startBalanceVat = stBalance - (stBalance / 1.18);
                    var startBalanceWithoutVat = stBalance - startBalanceVat;

                    var totalSum = stBalance + (generalDocAmountSum == 0 ? withoutVat : generalDocAmountSum);
                    if (contragent.customVat == "3")
                    {
                        startBalanceWithoutVat = stBalance;
                        startBalanceVat = 0.0;
                        stBalance = 0.0;
                        totalSum = withoutVat + startBalanceWithoutVat;
                    }                    

                    var generalDoc = new
                    {
                        tDate = dateFrom,
                        purpose = string.Format("ხელშეკრულების საფუძველზე მგზავრობის ღირებულება: {0}", new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month))).ToString("MMMM")),

                        withoutVat = Math.Round(withoutVat, 2),
                        vat = Math.Round(vat, 2),
                        withVat = Math.Round(generalDocAmountSum, 2),

                        startBalanceWithoutVat = Math.Round(startBalanceWithoutVat, 2),
                        startBalanceVat = Math.Round(startBalanceVat, 2),
                        startBalanceWithVat = Math.Round(stBalance, 2),

                        totalSum = Math.Round(totalSum, 2)
                    };

                    XElement element = new XElement("root",
                        new XElement("companyName", contragent.name),
                        new XElement("companyCode", contragent.code),
                        new XElement("dateNow", DateTime.Now.ToString("dd.MM.yyyy")),
                        new XElement("servicePaymentDate", string.Format("{0} დღე", contragent.servicePaymentDate)),

                        new XElement("generalDocPurpose", generalDoc.purpose),
                        new XElement("generalDocWithoutVat", generalDoc.withoutVat),
                        new XElement("generalDocVat", generalDoc.vat),
                        new XElement("generalDocWithVat", generalDoc.withVat),

                        new XElement("startBalancePurpose", "წინა პერიოდის ნაშთი"),
                        new XElement("startBalanceWithoutVat", generalDoc.startBalanceWithoutVat),
                        new XElement("startBalanceVat", generalDoc.startBalanceVat),
                        new XElement("startBalanceWithVat", generalDoc.startBalanceWithVat),

                        new XElement("ownerCompanyName", company.name),
                        new XElement("ownerCompanyCode", company.code),
                        new XElement("ownerCompanyAddress", company.address),
                        new XElement("ownerCompanyChief", company.chief),

                        new XElement("totalSum", generalDoc.totalSum)
                    );

                    ExportPdf exportPdf = new ExportPdf();

                    string fileName = "Invoice.pdf";
                    PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4 };
                    string filePath = exportPdf.GeneratePdf(element, "Invoice.xslt", setting);
                    byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                    string contentType = MimeMapping.GetMimeMapping(filePath);

                    System.Net.Mime.ContentDisposition cd = new ContentDisposition
                    {
                        FileName = fileName,
                        Inline = true
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(fileData, contentType);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }        

        public bool SendEmail(List<string> file, string Subject, string SmtpHost, int SmtpPort, bool EnableSsl, string SenderAddress, string login, string password, List<string> BCC, string MessageText)
        {
            Attachment fileAttachment;
            using (MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(SenderAddress),
                Priority = MailPriority.High,
                Body = MessageText,
                Subject = Subject
            })
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient()
                    {
                        Host = SmtpHost,
                        Port = SmtpPort,
                        Credentials = new NetworkCredential(login, password),
                        EnableSsl = EnableSsl
                    };
                    file.ForEach(filePath =>
                    {
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            fileAttachment = new Attachment(filePath, MediaTypeNames.Application.Octet);
                            ContentDisposition disposition = fileAttachment.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(filePath);
                            disposition.FileName = filePath.Contains(".xlsx") ? "Report.xlsx" : "Invoice.pdf";
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(filePath);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(filePath);
                            mailMessage.Attachments.Add(fileAttachment);
                        }
                    });
                    //ReceiverAddress.ForEach(toEmail => { mailMessage.To.Add(toEmail); });
                    BCC.ForEach(bcc => { mailMessage.Bcc.Add(bcc); });
                    smtpClient.Send(mailMessage);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
        }

        private string GetTempFilePathWithExtensionExcel()
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + ".xlsx";
            return Path.Combine(path, fileName);
        }

        public JsonResult SendToEmail(List<int> contragentIDs, string year, string month, string emailText)
        {
            List<dynamic> sendResult = new List<dynamic>();

            contragentIDs.ForEach(ci =>
            {
                Contragents contragent = context.Contragents.FirstOrDefault(c => c.id == ci);
                contragent.maxMonth = string.Format("{0};{1}", month, year);
                context.Entry(contragent).State = EntityState.Modified;

                List<string> emailsList = new List<string>();
                if (contragent != null)
                {
                    if(contragent.email != null && contragent.email != string.Empty && contragent.email != "")
                    {
                        emailsList = contragent.email.Split(';').ToList();
                    }
                }
                var contragentAccount = context.ContragentAccounts.FirstOrDefault(ca => ca.contragent_id == contragent.id);
                var company = context.Companies.ToList()[0];

                DateTime dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-1", year, month));
                DateTime dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, month, DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month))));

                List<GeneralDocs> generalDocs = new List<GeneralDocs>();

                if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                {
                    if (month == "1" || month == "01")
                    {
                        var year1 = Convert.ToInt32(year);
                        dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-{2}", (year1 - 1), 12, contragent.monthPeriod));
                    }
                    else
                    {
                        dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, (Convert.ToInt32(month) - 1), contragent.monthPeriod));
                    }
                    dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, month, contragent.monthPeriod));

                    generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId1 == contragent.id
                    && gd.DocType == 29
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) >= dateFrom
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) < dateTo
                    && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();
                }
                else
                {
                    generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId1 == contragent.id
                    && gd.DocType == 29
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) >= dateFrom
                    && DbFunctions.TruncateTime(gd.Tdate1.Value) <= dateTo
                    && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();
                }                

                string filePathPdf = string.Empty;
                string filePathExcel = GetTempFilePathWithExtensionExcel();
                byte[] excelFile = null;

                var genDocs = generalDocs.ToList();

                if (genDocs.Count > 0)
                {
                    List<double> totalSumList = new List<double>();
                    genDocs.ForEach(r =>
                    {
                        totalSumList.Add(r.Amount.Value);
                    });

                    var generalDocAmountSum = 0.0;
                    var vat = 0.0;
                    var withoutVat = 0.0;
                    if (contragent.customVat == "1")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "2")
                    {
                        generalDocAmountSum = totalSumList.Sum();
                        vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                        withoutVat = generalDocAmountSum - vat;
                    }
                    else if (contragent.customVat == "3")
                    {
                        vat = 0.0;
                        withoutVat = totalSumList.Sum();
                    }

                    string _path = context.Contragents.Where(c => c.id == ci).Select(a => a.path).FirstOrDefault();
                    string _account = GetContragentAccountByPath(_path, 0);
                    string _account2 = GetContragentAccountByPath(_path, 1);

                    //sawyisi nashti
                    var debts = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) > 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var creds = (contragent.debts != string.Empty && contragent.debts != null && Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) < 0 ? Convert.ToDouble(contragent.debts, System.Globalization.CultureInfo.InvariantCulture) : 0.0);
                    var debetStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.DebitAcc == _account || (_account2 != null && _account2 != "" && en.DebitAcc == _account2)) && en.A1 == ci)
                        .Select(en => new
                        {
                            generalID = en.GeneralId,
                            amountPrice = en.GeneralDocs.Amount.Value
                        }).ToList();
                    var debetStartSum = debetStart.Select(dc => dc.amountPrice).Sum() + (debts * 2);

                    var creditStart = context.Entries.Where(en => en.GeneralDocs.StatusId == 8 && DbFunctions.TruncateTime(en.GeneralDocs.Tdate1.Value) < dateFrom && (en.CreditAcc == _account || (_account2 != null && _account2 != "" && en.CreditAcc == _account2)) && en.B1 == ci)
                        .Select(en => new
                        {
                            generalID = en.GeneralId,
                            amountPrice = en.GeneralDocs.Amount.Value
                        }).ToList();
                    var creditStartSum = creditStart.Select(dc => dc.amountPrice).Sum() + Math.Abs(creds);



                    //sawyisi nashti
                    var startBalance = debetStartSum / 2;
                    var stBalance = Math.Round(startBalance, 2);
                    stBalance = stBalance - creditStartSum;
                    var startBalanceVat = stBalance - (stBalance / 1.18);
                    var startBalanceWithoutVat = stBalance - startBalanceVat;

                    var totalSum = stBalance + (generalDocAmountSum == 0 ? withoutVat : generalDocAmountSum);
                    if (contragent.customVat == "3")
                    {
                        startBalanceWithoutVat = stBalance;
                        startBalanceVat = 0.0;
                        stBalance = 0.0;
                        totalSum = withoutVat + startBalanceWithoutVat;
                    }

                    var generalDoc = new
                    {
                        tDate = dateFrom,
                        purpose = string.Format("ხელშეკრულების საფუძველზე მგზავრობის ღირებულება: {0}", new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month))).ToString("MMMM")),

                        withoutVat = Math.Round(withoutVat, 2),
                        vat = Math.Round(vat, 2),
                        withVat = Math.Round(generalDocAmountSum, 2),

                        startBalanceWithoutVat = Math.Round(startBalanceWithoutVat, 2),
                        startBalanceVat = Math.Round(startBalanceVat, 2),
                        startBalanceWithVat = Math.Round(stBalance, 2),

                        totalSum = Math.Round(totalSum, 2)
                    };



                    XElement elementPdf = new XElement("root",
                        new XElement("companyName", contragent.name),
                        new XElement("companyCode", contragent.code),
                        new XElement("dateNow", DateTime.Now.ToString("dd.MM.yyyy")),
                        new XElement("servicePaymentDate", string.Format("{0} დღე", contragent.servicePaymentDate)),

                        new XElement("generalDocPurpose", generalDoc.purpose),
                        new XElement("generalDocWithoutVat", generalDoc.withoutVat),
                        new XElement("generalDocVat", generalDoc.vat),
                        new XElement("generalDocWithVat", generalDoc.withVat),

                        new XElement("startBalancePurpose", "წინა პერიოდის ნაშთი"),
                        new XElement("startBalanceWithoutVat", generalDoc.startBalanceWithoutVat),
                        new XElement("startBalanceVat", generalDoc.startBalanceVat),
                        new XElement("startBalanceWithVat", generalDoc.startBalanceWithVat),

                        new XElement("ownerCompanyName", company.name),
                        new XElement("ownerCompanyCode", company.code),
                        new XElement("ownerCompanyAddress", company.address),
                        new XElement("ownerCompanyChief", company.chief),

                        new XElement("totalSum", generalDoc.totalSum)
                    );

                    ExportPdf exportPdf = new ExportPdf();

                    PdfPageSettings setting = new PdfPageSettings() { Size = PdfPageSize.A4 };
                    filePathPdf = exportPdf.GeneratePdf(elementPdf, "Invoice.xslt", setting);
                    // End PDF
                    


                    // Start Excel
                    excelFile = GenerateExcelForEmail(ci, dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd"));
                    
                    System.IO.File.WriteAllBytes(filePathExcel, excelFile);
                    // End Excel
                }

                if (filePathPdf != string.Empty && filePathExcel != string.Empty)
                {
                    sendResult.Add(new
                    {
                        contragentName = contragent.name,
                        contragentCode = contragent.code,
                        sent = SendEmail(
                            new List<string> { filePathPdf, filePathExcel }, //file pathes for attachments PDF and Excel
                            string.Format("რეპორტი და ინვოისი ტაქსით მომსახურებაზე - {0}", new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1).ToString("MMMM")), //subject text
                            "smtp.gmail.com", //smtp host
                            587, //smtp port
                            true, //enable ssl
                            "wsidegroup@gmail.com", //sender address
                            "wsidegroup@gmail.com", //login
                            "taximinda101", //password
                            emailsList, //bcc list of email addresses to send
                            emailText //message text
                        ),
                        sendDateTime = DateTime.Now,
                        contragentEmails = emailsList
                    });
                    context.SaveChanges();
                }
            });

            StringBuilder sb = new StringBuilder();
            sendResult = sendResult.OrderBy(sr => sr.sent).ToList();
            sendResult.ForEach(sr =>
            {
                StringBuilder sbe = new StringBuilder();
                foreach(var ce in sr.contragentEmails)
                {
                    sbe.AppendLine(ce);
                }

                sb.AppendLine(string.Format("გაგზავნის სტატუსი: {0}", sr.sent ? "გაეგზავნა" : "ვერ გაეგზავნა"));
                sb.AppendLine(string.Format("თარიღი: {0}", sr.sendDateTime));
                sb.AppendLine(string.Format("კომპანია: {0}", sr.contragentName));
                sb.AppendLine(string.Format("ს/კ: {0}", sr.contragentCode));
                sb.AppendLine(string.Format("ელ-ფოსტა(ები): {0}", sbe.ToString()));                                
                sb.AppendLine("\n\n");                
            });

            bool sendToAdmin = SendEmail(
                new List<string> { },
                string.Format("კომპანიების ელ-ფოსტა(ებ)ზე გაგზავნის ინფორმაცია - {0}", DateTime.Now.ToString("dd MMMM yyyy")),
                "smtp.gmail.com",
                587,
                true,
                "wsidegroup@gmail.com",
                "wsidegroup@gmail.com",
                "taximinda101",
                new List<string> { "wsidegroup@gmail.com" },
                sb.ToString()
            );

            return Json(new { sendResult = sendResult, sendToAdmin = sendToAdmin }, JsonRequestBehavior.AllowGet);
        }

        public byte[] GenerateExcelForEmail(int contragentID, string fromDate, string toDate)
        {
            var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);

            DateTime dateFrom = Convert.ToDateTime(fromDate);
            DateTime dateTo = Convert.ToDateTime(toDate);

            var result = from p in context.ProductsFlow
                         where p.GeneralDocs.StatusId == 8 && p.GeneralDocs.DocType.Value == 29 && p.GeneralDocs.ParamId1 == contragentID
                         && DbFunctions.TruncateTime(p.GeneralDocs.Tdate.Value) >= dateFrom
                         && (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty ? DbFunctions.TruncateTime(p.GeneralDocs.Tdate.Value) < dateTo
                         : DbFunctions.TruncateTime(p.GeneralDocs.Tdate.Value) <= dateTo)
                         join po in context.ProductOut on p.GeneralId equals po.GeneralId
                         join st in context.Staff on po.StaffId equals st.id
                         where (po.AddedWithWeb == "true" && po.Deleted == "false")
                         select new
                         {
                             POid = po.Id,
                             GDid = p.GeneralDocs.Id,
                             companyName = context.Contragents.FirstOrDefault(c => c.id == po.GeneralDocs.ParamId1).name,
                             dateNow = p.GeneralDocs.Tdate.Value,
                             startTime = po.StartTime,
                             endTime = po.EndTime,
                             staffName = st.name,
                             carNumber = po.CarNumber,
                             traveledDistance = po.TraveledDistance,
                             amountPrice = po.GeneralDocs.Amount,
                             parkingCosts = po.ParkingCosts,
                             customsFees = po.CustomsFees,
                             additionalCosts = po.AdditionalCosts,
                             withoutPrint = po.WithoutPrint,
                             commentOut = po.CommentOut,
                             //vat = context.Contragents.FirstOrDefault(c => c.id == po.GeneralDocs.ParamId1).customVat,
                             contrVat = po.contrVat
                         };

            result = result.OrderBy(r => r.dateNow);
            List<double> totalSumList = new List<double>();
            var res = result.ToList();
            res.ForEach(r =>
            {
                totalSumList.Add(r.amountPrice.Value);
            });

            var generalDocAmountSum = 0.0;
            var vat = 0.0;
            var withoutVat = 0.0;
            if (contragent.customVat == "1")
            {
                generalDocAmountSum = totalSumList.Sum();
                vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                withoutVat = generalDocAmountSum - vat;
            }
            else if (contragent.customVat == "2")
            {
                generalDocAmountSum = totalSumList.Sum();
                vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                withoutVat = generalDocAmountSum - vat;
            }
            else if (contragent.customVat == "3")
            {
                vat = 0.0;
                withoutVat = totalSumList.Sum();
            }

            var fileName = "DetailedContragentReport-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
            var outputDir = Path.GetTempPath();
            var file = new FileInfo(outputDir + fileName);

            byte[] excelFile = null;

            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("დეტალური რეპორტი-" + DateTime.Now.ToShortDateString());

                worksheet.Cells[1, 1].Value = "კონტრაგენტი";
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 2].Value = "მგზავრობის თარიღი";
                worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 3].Value = "მგზავრობის დაწყების დრო";
                worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 4].Value = "მგზავრობის დასრულების დრო";
                worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 5].Value = "მძღოლი";
                worksheet.Cells[1, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 6].Value = "ავტომობილის N";
                worksheet.Cells[1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 7].Value = "გავლილი მანძილი";
                worksheet.Cells[1, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 7].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 8].Value = "ღირებულება";
                worksheet.Cells[1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 9].Value = "სადგომის ხარჯი";
                worksheet.Cells[1, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 10].Value = "საბაჟოს მოსაკრებელი";
                worksheet.Cells[1, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 10].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 11].Value = "დამატებითი ხარჯი";
                worksheet.Cells[1, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 12].Value = "სულ ჯამი";
                worksheet.Cells[1, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 12].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 13].Value = "სულ ჯამი + დღგ";
                worksheet.Cells[1, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 14].Value = "სულ ჯამი - დღგ";
                worksheet.Cells[1, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 15].Value = "პრინტერის გარეშე";
                worksheet.Cells[1, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 15].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                worksheet.Cells[1, 16].Value = "კომენტარი";
                worksheet.Cells[1, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                for (int i = 2, j = 0; i <= res.Count + 1 && j < res.Count; i++, j++)
                {
                    worksheet.Cells[i, 1].Value = res[j].companyName;
                    worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 2].Value = res[j].dateNow.ToString("yyyy-MM-dd");
                    worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 3].Value = res[j].startTime;
                    worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 4].Value = res[j].endTime;
                    worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 5].Value = res[j].staffName;
                    worksheet.Cells[i, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 6].Value = res[j].carNumber;
                    worksheet.Cells[i, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 7].Value = Convert.ToDouble(res[j].traveledDistance, System.Globalization.CultureInfo.InvariantCulture);
                    worksheet.Cells[i, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 8].Value =
                        res[j].contrVat == "1" ? Math.Round((res[j].amountPrice.Value - (Convert.ToDouble(res[j].parkingCosts == null || res[j].parkingCosts == string.Empty ? "0" : res[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(res[j].customsFees == null || res[j].customsFees == string.Empty ? "0" : res[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(res[j].additionalCosts == null || res[j].additionalCosts == string.Empty ? "0" : res[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                        res[j].contrVat == "2" ? Math.Round(((res[j].amountPrice.Value / 1.18) - (Convert.ToDouble(res[j].parkingCosts == null || res[j].parkingCosts == string.Empty ? "0" : res[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(res[j].customsFees == null || res[j].customsFees == string.Empty ? "0" : res[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(res[j].additionalCosts == null || res[j].additionalCosts == string.Empty ? "0" : res[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) :
                        res[j].contrVat == "3" ? Math.Round(((res[j].amountPrice.Value + (res[j].amountPrice.Value * 0.18)) - (Convert.ToDouble(res[j].parkingCosts == null || res[j].parkingCosts == string.Empty ? "0" : res[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(res[j].customsFees == null || res[j].customsFees == string.Empty ? "0" : res[j].customsFees, System.Globalization.CultureInfo.InvariantCulture) + Convert.ToDouble(res[j].additionalCosts == null || res[j].additionalCosts == string.Empty ? "0" : res[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture))), 2) : res[j].amountPrice.Value;
                    worksheet.Cells[i, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 9].Value = Convert.ToDouble(res[j].parkingCosts == null || res[j].parkingCosts == string.Empty ? "0" : res[j].parkingCosts, System.Globalization.CultureInfo.InvariantCulture);
                    worksheet.Cells[i, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 10].Value = Convert.ToDouble(res[j].customsFees == null || res[j].customsFees == string.Empty ? "0" : res[j].customsFees, System.Globalization.CultureInfo.InvariantCulture);
                    worksheet.Cells[i, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 11].Value = Convert.ToDouble(res[j].additionalCosts == null || res[j].additionalCosts == string.Empty ? "0" : res[j].additionalCosts, System.Globalization.CultureInfo.InvariantCulture);
                    worksheet.Cells[i, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 12].Value =
                        res[j].contrVat == "1" ? Math.Round(res[j].amountPrice.Value, 1) : res[j].contrVat == "2" ? Math.Round((res[j].amountPrice.Value / 1.18), 1) : res[j].contrVat == "3" ? Math.Round((res[j].amountPrice.Value + (res[j].amountPrice.Value * 0.18)), 2) : 0;
                    worksheet.Cells[i, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 13].Value =
                        res[j].contrVat == "2" ? Math.Round(res[j].amountPrice.Value, 2) : 0;
                    worksheet.Cells[i, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 14].Value =
                        res[j].contrVat == "3" ? Math.Round(res[j].amountPrice.Value, 2) : 0; ;
                    worksheet.Cells[i, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 15].Value = res[j].withoutPrint;
                    worksheet.Cells[i, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                    worksheet.Cells[i, 16].Value = res[j].commentOut;
                    worksheet.Cells[i, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                }

                //worksheet.Cells[res.Count + 2, 10].Style.Font.Bold = true;
                //worksheet.Cells[res.Count + 2, 10].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                //worksheet.Cells[res.Count + 2, 10].Value = Math.Round(totalSumList.Sum(), 2);

                worksheet.Cells[res.Count + 5, 1].Value = "თანხა დღგ-ს გარეშე";
                worksheet.Cells[res.Count + 5, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[res.Count + 5, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                worksheet.Cells[res.Count + 5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[res.Count + 6, 1].Value = Math.Round(withoutVat, 2);
                worksheet.Cells[res.Count + 6, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[res.Count + 6, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[res.Count + 6, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                worksheet.Cells[res.Count + 5, 2].Value = "დღგ";
                worksheet.Cells[res.Count + 5, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[res.Count + 5, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                worksheet.Cells[res.Count + 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[res.Count + 6, 2].Value = Math.Round(vat, 2);
                worksheet.Cells[res.Count + 6, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[res.Count + 6, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[res.Count + 6, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                worksheet.Cells[res.Count + 5, 3].Value = "თანხა დღგ-ს ჩათვლით";
                worksheet.Cells[res.Count + 5, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[res.Count + 5, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                worksheet.Cells[res.Count + 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[res.Count + 6, 3].Value = Math.Round(generalDocAmountSum, 2);
                worksheet.Cells[res.Count + 6, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                worksheet.Cells[res.Count + 6, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[res.Count + 6, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();
                worksheet.Column(15).AutoFit();
                worksheet.Column(16).AutoFit();

                excelFile = package.GetAsByteArray();
            }

            return excelFile;
        }

        public JsonResult ChangeContragentPwd(string oldPwd, string newPwd, string repeatNewPwd)
        {
            var result = false;
            string errorMsg = string.Empty;

            if(newPwd == repeatNewPwd)
            {
                if(newPwd.Length > 0 && newPwd.Length <= 12)
                {
                    var calculatedPwd = HashHelper.Calc(oldPwd);
                    var contragent = context.Contragents.FirstOrDefault(c => c.path == "0#2#5" && c.code == User.Identity.Name && c.pwd == calculatedPwd);

                    if (contragent == null)
                    {
                        errorMsg = "ძველი პაროლი არასწორია!";
                    }
                    else
                    {
                        string calculatedNewPwd = HashHelper.Calc(newPwd);
                        contragent.pwd = calculatedNewPwd;
                        context.Entry(contragent).State = EntityState.Modified;
                        result = context.SaveChanges() >= 0;
                    }
                }
            }

            return Json(new { result = result, errorMsg = errorMsg }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDebitAccounts(int? contragentID, string fromDate, string toDate)
        {
            fromDate += " 00:00:00";
            toDate += " 23:59:59";

            DateTime dateFrom = Convert.ToDateTime(fromDate);
            DateTime dateTo = Convert.ToDateTime(toDate);

            var generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.DocType.Value == 38
                    && gd.Tdate.Value >= dateFrom
                    && gd.Tdate.Value <= dateTo)
                    .ToList();

            if (contragentID != null)
            {
                generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.ParamId2 == contragentID.Value
                    && gd.DocType.Value == 38
                    && gd.Tdate.Value >= dateFrom
                    && gd.Tdate.Value <= dateTo)
                    .ToList();                
            }            

            var result = generalDocs
                .Select(gd => new
            {
                gdID = gd.Id,
                companyName = context.Contragents.FirstOrDefault(c => c.id == gd.ParamId2).name,
                tDate = gd.Tdate.Value,
                tDate1 = gd.Tdate1.Value,
                contragentID = gd.ParamId2.Value,
                purpose = gd.Purpose,
                userName = context.Users.FirstOrDefault(u => u.id == gd.UserId).name,
                amount = gd.Amount,
                currency_id = gd.CurrencyId,
                ccaID = gd.ParamId1.Value,
                currency = context.Currencies.FirstOrDefault(c => c.id == gd.CurrencyId).code,
                comment = context.Comments.FirstOrDefault(c => c.general_id == gd.Id) == null ? "" : gd.Comments.FirstOrDefault(c => c.general_id == gd.Id).comment
                });

            result = result.OrderByDescending(r => r.tDate.Date);

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCurrencies()
        {
            var result = context.Currencies;

            return Json(new { currencies = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashesAndCompanyAccounts()
        {
            var cashes = context.Cashes.ToList();
            var companyAccountsDB = context.CompanyAccounts.ToList();
            List<object> companyAccounts = new List<object>();
            companyAccountsDB.ForEach(ca =>
            {
                string caID = string.Format("-{0}", ca.id.ToString());
                companyAccounts.Add(new
                {
                    id = caID,
                    name = ca.name
                });
            });

            return Json(new { cashes = cashes, companyAccounts = companyAccounts }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDebitAccount(int gdID, double amount, int currencyID, int contragentID, int CCAID, string tDate, string tDate1, string comment)
        {
            if (gdID == 0)
            {
                //add
                var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
                var currentUser = context.Users.Where(u => u.login == User.Identity.Name).FirstOrDefault();                

                GeneralDocs generalDoc = new GeneralDocs
                {
                    Amount = amount,
                    CurrencyId = currencyID,
                    ParamId2 = contragentID,
                    ParamId1 = CCAID,
                    Tdate = Convert.ToDateTime(tDate),
                    Tdate1 = Convert.ToDateTime(tDate1),
                    Purpose = string.Format("თანხის მიღება მყიდველისგან - {0}", contragent == null ? "" : contragent.name),
                    UserId = currentUser.id,
                    DocType = 38,
                    Vat = 18,
                    MakeEntry = true,
                    DocNum = context.GeneralDocs.Where(gd => gd.DocType == 38).Select(gd => gd.DocNum).Max() + 1,
                    StoreId = CCAID,
                    StatusId = 8 //set statusid for added from web interface
                };
                context.GeneralDocs.Add(generalDoc);

                Comments comments = new Comments
                {
                    comment = comment
                };
                context.Comments.Add(comments);

                Entries entry = new Entries
                {
                    Amount = amount,
                    A1 = Math.Abs(CCAID),
                    B1 = contragentID,
                    DebitAcc = CCAID < 0 ? "1210" : "1110",
                    CreditAcc = "1410",
                    ProjectId = 1,
                    CurrencyId = 1
                };
                context.Entries.Add(entry);

                Operation operation = new Operation
                {
                    IsDocument = 1,
                    PersonId = 0,
                    Person = contragent == null ? "" : contragent.name
                };
                context.Operation.Add(operation);
            }
            else
            {
                //edit
                var contragent = context.Contragents.FirstOrDefault(c => c.id == contragentID);
                var currentUser = context.Users.Where(u => u.login == User.Identity.Name).FirstOrDefault();

                var generalDoc = context.GeneralDocs.FirstOrDefault(gd => gd.Id == gdID);
                generalDoc.Amount = amount;
                generalDoc.CurrencyId = currencyID;
                generalDoc.ParamId2 = contragentID;
                generalDoc.ParamId1 = CCAID;
                generalDoc.Tdate = Convert.ToDateTime(tDate);
                generalDoc.Tdate1 = Convert.ToDateTime(tDate1);
                generalDoc.Purpose = string.Format("თანხის მირება მყიდველისგან - {0}", contragent == null ? "" : contragent.name);
                generalDoc.UserId = currentUser.id;

                var comments = context.Comments.FirstOrDefault(c => c.general_id == gdID);
                comments.comment = comment;

                var entry = context.Entries.FirstOrDefault(en => en.GeneralId == gdID);
                entry.Amount = amount;
                entry.A1 = Math.Abs(CCAID);
                entry.B1 = contragentID;

                context.Entry(generalDoc).State = EntityState.Modified;
                context.Entry(comments).State = EntityState.Modified;
                context.Entry(entry).State = EntityState.Modified;
            }

            var saveResult = context.SaveChanges() >= 0;
            return Json(new { saveResult = saveResult }, JsonRequestBehavior.AllowGet);
        }

        public FileResult GenerateInvoicesExcel(string year, string month)
        {
            bool isContragent = !context.Users.Any(u => u.login == User.Identity.Name);

            if (!isContragent)
            {
                List<dynamic> generalDocResult = new List<dynamic>();

                DateTime dateFrom = Convert.ToDateTime(string.Format("{0}-{1}-1", year, Convert.ToInt32(month)));
                DateTime dateTo = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, Convert.ToInt32(month), DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month))));

                var generalDocs = context.GeneralDocs.Where(gd => gd.StatusId == 8 && gd.DocType.Value == 29
                    && gd.ParamId1 > 0
                    && gd.ProductOuts.Any(po => po.AddedWithWeb == "true" && po.Deleted == "false")).ToList();

                var groupedGeneralDocs = generalDocs.GroupBy(gd => gd.ParamId1).Select(ggd => ggd).ToList();
                groupedGeneralDocs.ForEach(ggd =>
                {
                    var contragent = context.Contragents.FirstOrDefault(c => c.id > 0 && c.id == ggd.Key.Value);

                    if (contragent.monthPeriod != null && contragent.monthPeriod != string.Empty)
                    {
                        DateTime dateFrom1 = new DateTime();
                        if (month == "1" || month == "01")
                        {
                            var year1 = Convert.ToInt32(year);
                            dateFrom1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", (year1 - 1), 12, contragent.monthPeriod));
                        }
                        else
                        {
                            dateFrom1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, (Convert.ToInt32(month) - 1), contragent.monthPeriod));
                        }
                        DateTime dateTo1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", year, month, contragent.monthPeriod));

                        var t = ggd.AsQueryable().Where(x => x.Tdate.Value.Date >= dateFrom1
                        && x.Tdate.Value.Date < dateTo1).ToList();

                        List<double> totalSumList = new List<double>();
                        t.ForEach(r =>
                        {
                            var po = context.ProductOut.Where(p => p.GeneralId == r.Id).ToList();
                            totalSumList.Add(r.Amount.Value);
                        });

                        var generalDocAmountSum = 0.0;
                        var vat = 0.0;
                        var withoutVat = 0.0;
                        if (contragent.customVat == "1")
                        {
                            generalDocAmountSum = totalSumList.Sum();
                            vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                            withoutVat = generalDocAmountSum - vat;
                        }
                        else if (contragent.customVat == "2")
                        {
                            generalDocAmountSum = totalSumList.Sum();
                            vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                            withoutVat = generalDocAmountSum - vat;
                        }
                        else if (contragent.customVat == "3")
                        {
                            vat = 0.0;
                            withoutVat = totalSumList.Sum();
                        }

                        var generalDoc = new
                        {
                            contragentID = ggd.Key.Value,
                            contragentName = contragent == null ? "" : contragent.name,
                            //tDate = gd.Tdate.Value,
                            //purpose = gd.Purpose,
                            withoutVat = Math.Round(withoutVat, 2),
                            vat = Math.Round(vat, 2),
                            withVat = Math.Round(generalDocAmountSum, 2)
                        };

                        generalDocResult.Add(generalDoc);
                    }
                    else
                    {
                        List<double> totalSumList = new List<double>();
                        ggd.ToList().ForEach(r =>
                        {
                            if (r.Tdate.Value.Year == dateFrom.Year && r.Tdate.Value.Month == dateFrom.Month)
                            {
                                totalSumList.Add(r.Amount.Value);
                            }
                        });

                        var generalDocAmountSum = 0.0;
                        var vat = 0.0;
                        var withoutVat = 0.0;
                        if (contragent.customVat == "1")
                        {
                            generalDocAmountSum = totalSumList.Sum();
                            vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                            withoutVat = generalDocAmountSum - vat;
                        }
                        else if (contragent.customVat == "2")
                        {
                            generalDocAmountSum = totalSumList.Sum();
                            vat = generalDocAmountSum - (generalDocAmountSum / 1.18);
                            withoutVat = generalDocAmountSum - vat;
                        }
                        else if (contragent.customVat == "3")
                        {
                            vat = 0.0;
                            withoutVat = totalSumList.Sum();
                        }

                        var generalDoc = new
                        {
                            contragentID = ggd.Key.Value,
                            contragentName = contragent == null ? "" : contragent.name,
                            //tDate = gd.Tdate.Value,
                            //purpose = gd.Purpose,
                            withoutVat = Math.Round(withoutVat, 2),
                            vat = Math.Round(vat, 2),
                            withVat = Math.Round(generalDocAmountSum, 2)
                        };

                        generalDocResult.Add(generalDoc);
                    }
                });

                var fileName = "Invoices-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + Guid.NewGuid().ToString() + ".xlsx";
                var outputDir = Path.GetTempPath();
                var file = new FileInfo(outputDir + fileName);

                byte[] excelFile = null;

                using (var package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ინვოისები-" + DateTime.Now.ToShortDateString());

                    worksheet.Cells[1, 1].Value = "კონტრაგენტი";
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 2].Value = "თანხა დღგ-ს გარეშე";
                    worksheet.Cells[1, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 3].Value = "დღგ";
                    worksheet.Cells[1, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    worksheet.Cells[1, 4].Value = "თანხა დღგ-ს ჩათვლით";
                    worksheet.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    for (int i = 2, j = 0; i <= generalDocResult.Count + 1 && j < generalDocResult.Count; i++, j++)
                    {
                        worksheet.Cells[i, 1].Value = generalDocResult[j].contragentName;
                        worksheet.Cells[i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 2].Value = generalDocResult[j].withoutVat;
                        worksheet.Cells[i, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 3].Value = generalDocResult[j].vat;
                        worksheet.Cells[i, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                        worksheet.Cells[i, 4].Value = generalDocResult[j].withVat;
                        worksheet.Cells[i, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }

                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();

                    excelFile = package.GetAsByteArray();
                }

                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Invoices.xlsx");
            }
            else
            {
                return null;
            }
        }
    }
}