using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpreadSheetLibrary.Data.Entities;
using Syncfusion.XlsIO;
using Color = System.Drawing.Color;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel Vm { get; set; }
        public MainWindow()
        {
            if (Vm == null)
            {
                Vm = new MainViewModel();
            }

            DataContext = Vm;
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                var xls = await ReadExcelFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        public async Task<bool> ReadExcelFile()
        {

            try
            {
                //using (ExcelEngine excelEngine = new ExcelEngine())
                //{
                //    IApplication application = excelEngine.Excel;
                //    application.DefaultVersion = ExcelVersion.Excel2013;

                //    //The new workbook will have 5 worksheets
                //    IWorkbook workbook = application.Workbooks.Create(5);
                //    //Creating a Sheet
                //    IWorksheet sheet = workbook.Worksheets.Create();
                //    //Creating a Sheet with name “Sample”
                //    IWorksheet namedSheet = workbook.Worksheets.Create("Sample");

                //    workbook.SaveAs("Output.xlsx");
                //}
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    //Instantiate the Excel application object
                    IApplication application = excelEngine.Excel;

                    //Set the default application version
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Load the existing Excel workbook into IWorkbook
                    IWorkbook workbook = application.Workbooks.Open("Copy of 6 monthly report.xlsx");

                    //Get the first worksheet in the workbook into IWorksheet
                    IWorksheet worksheet = workbook.Worksheets[0];
                    await ExtractDataFromWorksheet(worksheet);
                    IWorkbook newworkbook = application.Workbooks.Create(2);

                    IWorksheet worksheetnew = newworkbook.Worksheets[0];
                    workbook.SetPaletteColor(8, System.Drawing.Color.Aqua);

                    //Defining header style
                    IStyle headerStyle = workbook.Styles.Add("HeaderStyle");
                    WriteDataToWorksheet(worksheetnew, headerStyle);

                    //Save the Excel document
                    newworkbook.SaveAs($"Output_{DateTime.Now.ToString("dd_MM_yyyy")}.xlsx");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        private const string Youth = "Te Roopu Kimiora - DHB";
        public void WriteDataToWorksheet(IWorksheet worksheet, IStyle headerStyle)
        {

            try
            {
                //Assign some text in a cell
                worksheet.Range["A2"].Text = "NHI";
                worksheet.Range["B2"].Text = "Referral Source";
                worksheet.Range["C2"].Text = "Last Review Date";
                worksheet.Range["D2"].Text = "Entry Date to Support Hours";
                worksheet.Range["E2"].Text = "Weekly Allocated Hours";
                worksheet.Range["F2"].Text = "Weekly Allocated Travel Time";
                worksheet.Range["G2"].Text = "Exit Date";
                worksheet.Range["H2"].Text = "Child & Youth Client";
                worksheet.Range["I2"].Text = "Support hours narrative";

               
                headerStyle.BeginUpdate();
                headerStyle.Color = Color.Aqua;
                headerStyle.Font.Bold = true;
                headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                headerStyle.EndUpdate();

                worksheet.Rows[0].CellStyle = headerStyle;

                var count = 3;
                foreach (var service in Vm.Services)
                {
                    count++;
                    foreach (var cl in service.Clients)
                    {
                        worksheet.Range[$"A{count}"].Text =
                            string.IsNullOrEmpty(cl.NHINumber) ? cl.ClientName : cl.NHINumber;

                        //Referral Source
                        worksheet.Range[$"B{count}"].Text = cl.ReferralData.ReferrerOrganisation;

                        //Last Review Date
                        worksheet.Range[$"C{count}"].Text = cl.ReferralData.ReferralUpdated;

                        //Entry Date to Support Hours
                        worksheet.Range[$"D{count}"].Text = cl.ReferralData.ReferralDate;

                        //Weekly Allocated Hours
                        worksheet.Range[$"E{count}"].Text = cl.ServiceData.EngagementStatus;

                        //Weekly Allocated Time
                        //TODO Find out what this is??

                        //Exit Date
                        worksheet.Range[$"G{count}"].Text = string.IsNullOrEmpty(cl.ServiceData.ServiceEndDate) ? "Still Active" : cl.ServiceData.ServiceEndDate;

                        //Child & Youth
                        if (cl.ReferralData.ReferrerOrganisation == Youth)
                        {
                            worksheet.Range[$"H{count}"].Text = "Yes";
                        }
                        worksheet.Range[$"I{count}"].Text = service.ServiceName;
                        count++;

                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }


        bool isService = false;
        bool isWorker = false;
        bool isReferral = false;
        bool isGeneralStatus = false;

        private void SetIsServiceFlag()
        {
            isService = true;
            isWorker = false;
            isReferral = false;
            isGeneralStatus = false;
        }

        private void SetIsWorkerFlag()
        {

            isService = false;
            isWorker = true;
            isReferral = false;
            isGeneralStatus = false;
        }

        private void SetIsReferralFlag()
        {

            isService = false;
            isWorker = false;
            isReferral = true;
            isGeneralStatus = false;
        }

        private void SetIsGeneralStatusFlag()
        {

            isService = false;
            isWorker = false;
            isReferral = false;
            isGeneralStatus = true;
        }

        private int blankCells = 0;
        private const string ServiceConst = "Service Reason";
        private const string WorkerConst = "Worker Name";
        private const string ReferralConst = "Referral Reason";
        private const string GeneralStatusConst = "General Status Name";
        private const string ServiceFlagConst = "Service:";
        public async Task ExtractDataFromWorksheet(IWorksheet worksheet)
        {
            try
            {
                //Search for client name
                var count = 10;
                var acell = worksheet.Range[$"A{count}"];
                var bcell = worksheet.Range[$"B{count}"];
                var ccell = worksheet.Range[$"C{count}"];
                var dcell = worksheet.Range[$"D{count}"];
                var ecell = worksheet.Range[$"E{count}"];
                var fcell = worksheet.Range[$"F{count}"];
                var gcell = worksheet.Range[$"G{count}"];
                var hcell = worksheet.Range[$"H{count}"];
                var icell = worksheet.Range[$"I{count}"];




                while ((acell.HasString || bcell.HasString || ccell.HasString || dcell.HasString || ecell.HasString ||
                        fcell.HasString || gcell.HasString || hcell.HasString || icell.HasString))
                {
                    
                    if (acell.DisplayText == ServiceFlagConst)
                    {
                        if (!string.IsNullOrEmpty(Vm.CurrentService.ServiceName))
                        {
                            Vm.Services.Add(Vm.CurrentService);
                        }

                        Vm.CurrentService = new OrganisationServiceData();
                        Vm.CurrentService.ServiceName = bcell.DisplayText;
                        count++;
                        acell = worksheet.Range[$"A{count}"];
                        bcell = worksheet.Range[$"B{count}"];
                        ccell = worksheet.Range[$"C{count}"];
                        dcell = worksheet.Range[$"D{count}"];
                        ecell = worksheet.Range[$"E{count}"];
                        fcell = worksheet.Range[$"F{count}"];
                        gcell = worksheet.Range[$"G{count}"];
                        hcell = worksheet.Range[$"H{count}"];
                        icell = worksheet.Range[$"I{count}"];
                        continue;
                    }
                    //if we don't have a current service, keep going until we get one.
                    if (string.IsNullOrEmpty(Vm.CurrentService.ServiceName))
                    {

                        count++;
                        acell = worksheet.Range[$"A{count}"];
                        bcell = worksheet.Range[$"B{count}"];
                        ccell = worksheet.Range[$"C{count}"];
                        dcell = worksheet.Range[$"D{count}"];
                        ecell = worksheet.Range[$"E{count}"];
                        fcell = worksheet.Range[$"F{count}"];
                        gcell = worksheet.Range[$"G{count}"];
                        hcell = worksheet.Range[$"H{count}"];
                        icell = worksheet.Range[$"I{count}"];
                        continue;
                    }

                    var nextCellCheck = worksheet.Range[$"A{count + 1}"];
                    if (nextCellCheck.DisplayText == ServiceConst)
                    {
                        acell = worksheet.Range[$"A{count}"];
                        bcell = worksheet.Range[$"B{count}"];
                        ccell = worksheet.Range[$"C{count}"];
                        dcell = worksheet.Range[$"D{count}"];
                        ecell = worksheet.Range[$"E{count}"];
                        fcell = worksheet.Range[$"F{count}"];
                        gcell = worksheet.Range[$"G{count}"];
                        hcell = worksheet.Range[$"H{count}"];
                        icell = worksheet.Range[$"I{count}"];


                        //we have a name!

                        var nextServiceCell = worksheet.Range[$"A{count + 2}"];


                        Vm.CurrentClient.ClientName = acell.DisplayText;
                        Vm.CurrentClient.NHINumber = bcell.DisplayText;
                        Vm.CurrentClient.Gender = ccell.DisplayText;
                        Vm.CurrentClient.Age = dcell.DisplayText;
                        Vm.CurrentClient.DOB = ecell.DisplayText;

                        while (nextServiceCell.Value2.ToString() != ServiceConst)
                        {
                            // Debug.WriteLine(count);
                            count++;


                            if (string.IsNullOrEmpty(acell.Value2.ToString()))
                            {
                                blankCells += 1;
                            }
                            else
                            {
                                blankCells = 0;
                            }

                            // need to exit out if we're at the end of the spreadsheet
                            if (blankCells >= 5)
                            {
                                Vm.Services.Add(Vm.CurrentService);
                                break;
                            }

                            acell = worksheet.Range[$"A{count}"];
                            bcell = worksheet.Range[$"B{count}"];
                            ccell = worksheet.Range[$"C{count}"];
                            dcell = worksheet.Range[$"D{count}"];
                            ecell = worksheet.Range[$"E{count}"];
                            fcell = worksheet.Range[$"F{count}"];
                            gcell = worksheet.Range[$"G{count}"];
                            hcell = worksheet.Range[$"H{count}"];
                            icell = worksheet.Range[$"I{count}"];

                            if (acell.DisplayText == ServiceFlagConst)
                            {
                                break;
                            }

                            if (acell.Value2.ToString() == ServiceConst)
                            {
                                SetIsServiceFlag();
                                continue;
                            }

                            if (acell.Value2.ToString() == WorkerConst)
                            {
                                SetIsWorkerFlag();
                                continue;
                            }

                            if (acell.Value2.ToString() == ReferralConst)
                            {
                                SetIsReferralFlag();
                                continue;
                            }

                            if (acell.Value2.ToString() == GeneralStatusConst)
                            {
                                SetIsGeneralStatusFlag();
                                continue;
                            }

                            if (isService)
                            {

                                try
                                {
                                    //start adding the service stuff
                                    Vm.CurrentClient.ServiceData.ServiceReason = acell.Value2.ToString();

                                    Vm.CurrentClient.ServiceData.EngagementStatus = bcell.Value2.ToString();

                                    if (ccell.Value2 != null)
                                        Vm.CurrentClient.ServiceData.ServiceStartDate = ccell.DisplayText;

                                    Vm.CurrentClient.ServiceData.ServiceEndDate = dcell.DisplayText;
                                    Vm.CurrentClient.ServiceData.ContractNumber = ecell.Value2.ToString();
                                    Vm.CurrentClient.ServiceData.ContractName = fcell.Value2.ToString();

                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                isService = false;
                            }


                            if (isWorker)
                            {

                                try
                                {
                                    var worker = new WorkerInfo();
                                    //start adding the worker stuff
                                    worker.WorkerName = acell.DisplayText;
                                    worker.Start = bcell.DisplayText;
                                    worker.End = ccell.DisplayText;
                                    Vm.CurrentClient.Workers.Add(worker);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                isWorker = false;
                            }

                            if (isReferral)
                            {

                                try
                                {
                                    //start adding the service stuff
                                    Vm.CurrentClient.ReferralData.Reason = acell.Value2.ToString();
                                    Vm.CurrentClient.ReferralData.ReferralDate = bcell.DisplayText.ToString();
                                    Vm.CurrentClient.ReferralData.ReferralCreationDate = ccell.DisplayText;
                                    Vm.CurrentClient.ReferralData.ReferralUpdated = dcell.DisplayText;
                                    Vm.CurrentClient.ReferralData.ReferralEndDate = ecell.Value2.ToString();
                                    Vm.CurrentClient.ReferralData.ReferralAcceptedDeclined =
                                        fcell.DisplayText.ToString();
                                    Vm.CurrentClient.ReferralData.ReferralAcceptedDeclinedReason =
                                        gcell.DisplayText.ToString();
                                    Vm.CurrentClient.ReferralData.ReferrerOrganisation =
                                        hcell.DisplayText.ToString();
                                    Vm.CurrentClient.ReferralData.ReferrerIndividual = icell.DisplayText.ToString();

                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                isReferral = false;
                            }

                            if (isGeneralStatus)
                            {

                                try
                                {
                                    //start adding the general status stuff
                                    Vm.CurrentClient.GeneralStatus.StatusName = acell.DisplayText;
                                    Vm.CurrentClient.GeneralStatus.WhenStarted = bcell.DisplayText;
                                    Vm.CurrentClient.GeneralStatus.WhenEnded = ccell.DisplayText;

                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                isGeneralStatus = false;
                            }

                            nextServiceCell = worksheet.Range[$"A{count + 2}"];


                        }

                        Vm.CurrentService.Clients.Add(Vm.CurrentClient);
                        Vm.CurrentClient = new ClientData();

                        continue;
                    }

                    count++;

                }

                foreach (var item in Vm.Services)
                {
                    foreach (var cl in item.Clients)
                    {
                        Debug.WriteLine($"**ClientName: {cl.ClientName}");
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
