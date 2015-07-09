﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Artifact;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Web.ViewModels.PmsConfigDetails;
using DSLNG.PEAR.Web.ViewModels.PmsSummary;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Web.ViewModels.Artifact;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PmsSummaryController : BaseController
    {
        private readonly IPmsSummaryService _pmsSummaryService;
        private readonly IDropdownService _dropdownService;
        private readonly IMeasurementService _measurementService;
        private readonly IArtifactService _artifactService;

        public PmsSummaryController(IPmsSummaryService pmsSummaryService, IDropdownService dropdownService, IMeasurementService measurementService, IArtifactService artifactService)
        {
            _pmsSummaryService = pmsSummaryService;
            _dropdownService = dropdownService;
            _measurementService = measurementService;
            _artifactService = artifactService;
        }

        public ActionResult Index(int? month, int? year)
        {
            var viewModel = new PmsSummaryIndexViewModel();
            var request = new GetPmsSummaryReportRequest
                {
                    Month = month.HasValue ? month.Value : DateTime.Now.Month,
                    Year = year.HasValue ? year.Value : DateTime.Now.Year
                };

            var response = _pmsSummaryService.GetPmsSummaryReport(request);
            if (response.IsSuccess)
            {
                viewModel.PmsSummaries = response.KpiDatas.MapTo<PmsSummaryViewModel>();
                viewModel.Year = request.Year;
                viewModel.Month = request.Month;
                viewModel.Title = response.Title;
                viewModel.YearList = _dropdownService.GetYearsForPmsSummary().MapTo<SelectListItem>();
                return View(viewModel);    
            }

            return base.ErrorPage(response.Message);

        }

        public ActionResult IndexGridPartial(int? month, int? year)
        {
            var viewModel = new PmsSummaryIndexViewModel();
            var request = new GetPmsSummaryReportRequest
            {
                Month = month.HasValue ? month.Value : DateTime.Now.Month,
                Year = year.HasValue ? year.Value : DateTime.Now.Year
            };

            var response = _pmsSummaryService.GetPmsSummaryReport(request);
            viewModel.PmsSummaries = response.KpiDatas.MapTo<PmsSummaryViewModel>();
            viewModel.Year = request.Year;
            viewModel.Month = request.Month;
            return PartialView("_IndexGridPartial", viewModel);
        }

        public ActionResult ReportDetails(int id, int month, int year)
        {
            var response = _pmsSummaryService.GetPmsDetails(new GetPmsDetailsRequest() { Id = id, Month = month, Year = year });
            var viewModel = response.MapTo<PmsReportDetailsViewModel>();
            var operationDate = new DateTime(response.Year, month, 1);
            viewModel.Title = response.Title;
            viewModel.Year = response.Year;
            viewModel.Month = operationDate.ToString("MMM");
            viewModel.MonthInt = month;
            viewModel.KpiAchievmentMonthly = response.KpiAchievmentMonthly.MapTo<PmsReportDetailsViewModel.KpiAchievment>();
            viewModel.KpiRelations = response.KpiRelations.MapTo<PmsReportDetailsViewModel.KpiRelation>();
            return PartialView("_ReportDetails", viewModel);
        }

        public ActionResult Configuration()
        {
            var response = _pmsSummaryService.GetPmsSummaryList(new GetPmsSummaryListRequest());
            if (response.IsSuccess)
            {
                var viewModel = new PmsSummaryConfigurationViewModel();
                viewModel.CorporatePortofolios =
                    response.PmsSummaryList.MapTo<PmsSummaryConfigurationViewModel.CorporatePortofolio>();
                return View(viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        public ActionResult Details(int id)
        {
            var response = _pmsSummaryService.GetPmsSummaryConfiguration(new GetPmsSummaryConfigurationRequest { Id = id });
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<PmsSummaryDetailsViewModel>();
                viewModel.PmsSummaryId = id;
                return View(viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        public ActionResult Create()
        {
            var viewModel = new CreatePmsSummaryViewModel();
            viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreatePmsSummaryViewModel viewModel)
        {
            var request = viewModel.MapTo<CreatePmsSummaryRequest>();
            var response = _pmsSummaryService.CreatePmsSummary(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Configuration");
        }

        public ActionResult Update(int id)
        {
            var response = _pmsSummaryService.GetPmsSummary(id);
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<UpdatePmsSummaryViewModel>();
                if (response.ScoreIndicators.Count() == 0)
                {
                    viewModel.ScoreIndicators.Add(new ViewModels.Common.PmsSummary.ScoreIndicatorViewModel
                    {
                        Id = 0,
                        Color = null,
                        Expression = null
                    });
                }
                viewModel.Years = _dropdownService.GetYears().MapTo<SelectListItem>();
                return PartialView("_Update", viewModel);
            }

            return base.ErrorPage(response.Message);
            
        }

        [HttpPost]
        public ActionResult Update(UpdatePmsSummaryViewModel viewModel)
        {
            var request = viewModel.MapTo<UpdatePmsSummaryRequest>();
            var response = _pmsSummaryService.UpdatePmsSummary(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Configuration");
        }

        public ActionResult ChartYearly(ViewModels.PmsSummary.ChartViewModel viewModel)
        {
            var artifactDesignerViewModel = new ArtifactDesignerViewModel();
            artifactDesignerViewModel.BarChart = new BarChartViewModel();
            var previewViewModel = new ArtifactPreviewViewModel();
            artifactDesignerViewModel.BarChart.Series.Add(new BarChartViewModel.SeriesViewModel
            {
                KpiId = viewModel.Id,
                Label = "Achievement",
                ValueAxis = ValueAxis.KpiActual.ToString()
            });

             var request = new GetCartesianChartDataRequest();
            request.GraphicName = "Yearly";
            request.GraphicType = "barachievement";
            request.HeaderTitle = "Yearly";
            request.MeasurementId = viewModel.MeasurementId;
            request.PeriodeType = PeriodeType.Yearly;
            request.RangeFilter = RangeFilter.CurrentYear;
            request.ValueAxis = ValueAxis.KpiActual;

            artifactDesignerViewModel.BarChart.MapPropertiesToInstance<GetCartesianChartDataRequest>(request);
            var chartData = _artifactService.GetChartData(request);
            previewViewModel.GraphicType = "barachievement";
            previewViewModel.BarChart = new BarChartDataViewModel();
            previewViewModel.BarChart.Title = "Yearly";
            previewViewModel.BarChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
            previewViewModel.BarChart.Series = chartData.Series.MapTo<BarChartDataViewModel.SeriesViewModel>();
            previewViewModel.BarChart.Periodes = chartData.Periodes;
            previewViewModel.BarChart.SeriesType = chartData.SeriesType;
            return Json(previewViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartMonthly(ViewModels.PmsSummary.ChartViewModel viewModel)
        {
            var artifactDesignerViewModel = new ArtifactDesignerViewModel();
            artifactDesignerViewModel.BarChart = new BarChartViewModel();
            var previewViewModel = new ArtifactPreviewViewModel();
            artifactDesignerViewModel.BarChart.Series.Add(new BarChartViewModel.SeriesViewModel
            {
                KpiId = viewModel.Id,
                Label = "Achievement",
                ValueAxis = ValueAxis.KpiActual.ToString()
            });

            var request = new GetCartesianChartDataRequest();
            request.GraphicName = "Monthly";
            request.GraphicType = "barachievement";
            request.HeaderTitle = "Monthly";
            request.MeasurementId = viewModel.MeasurementId;
            request.PeriodeType = PeriodeType.Monthly;
            request.RangeFilter = RangeFilter.Interval;
            request.Start = new DateTime(viewModel.Year, 1, 1);
            request.End = new DateTime(viewModel.Year, 12, 1);
            request.ValueAxis = ValueAxis.KpiActual;

            artifactDesignerViewModel.BarChart.MapPropertiesToInstance<GetCartesianChartDataRequest>(request);
            var chartData = _artifactService.GetChartData(request);
            previewViewModel.GraphicType = "barachievement";
            previewViewModel.BarChart = new BarChartDataViewModel();
            previewViewModel.BarChart.Title = "Monthly";
            previewViewModel.BarChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
            previewViewModel.BarChart.Series = chartData.Series.MapTo<BarChartDataViewModel.SeriesViewModel>();
            previewViewModel.BarChart.Periodes = chartData.Periodes;
            previewViewModel.BarChart.SeriesType = chartData.SeriesType;
            return Json(previewViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChartYtd(ViewModels.PmsSummary.ChartViewModel viewModel)
        {
            var artifactDesignerViewModel = new ArtifactDesignerViewModel();
            artifactDesignerViewModel.BarChart = new BarChartViewModel();
            var previewViewModel = new ArtifactPreviewViewModel();
            artifactDesignerViewModel.BarChart.Series.Add(new BarChartViewModel.SeriesViewModel
            {
                KpiId = viewModel.Id,
                Label = "Achievement",
                ValueAxis = ValueAxis.KpiActual.ToString()
            });

            var request = new GetCartesianChartDataRequest();
            request.GraphicName = "Year To Date";
            request.GraphicType = "barachievement";
            request.HeaderTitle = "Year To Date";
            request.MeasurementId = viewModel.MeasurementId;
            request.PeriodeType = PeriodeType.Monthly;
            request.RangeFilter = RangeFilter.YTD;
            request.Start = new DateTime(viewModel.Year, 1, 1);
            request.End = new DateTime(viewModel.Year, 12, 1);
            request.ValueAxis = ValueAxis.KpiActual;

            artifactDesignerViewModel.BarChart.MapPropertiesToInstance<GetCartesianChartDataRequest>(request);
            var chartData = _artifactService.GetChartData(request);
            previewViewModel.GraphicType = "barachievement";
            previewViewModel.BarChart = new BarChartDataViewModel();
            previewViewModel.BarChart.Title = "Year To Date";
            previewViewModel.BarChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
            previewViewModel.BarChart.Series = chartData.Series.MapTo<BarChartDataViewModel.SeriesViewModel>();
            previewViewModel.BarChart.Periodes = chartData.Periodes;
            previewViewModel.BarChart.SeriesType = chartData.SeriesType;
            return Json(previewViewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScoreIndicatorDetails(int id)
        {
            int pmsSummaryId = id;
            var response = _pmsSummaryService.GetScoreIndicators(new GetScoreIndicatorRequest{PmsSummaryId = pmsSummaryId});
            if (response.IsSuccess)
            {
                var viewModel = response.MapTo<ScoreIndicatorDetailsViewModel>();
                return PartialView("_ScoreIndicatorDetails", viewModel);
            }

            return base.ErrorPage(response.Message);
        }

        
        private IEnumerable<PmsSummaryViewModel> AddFakePmsSummaryData()
        {
            IList<PmsSummaryViewModel> list = new List<PmsSummaryViewModel>();
            var pmsSummary1 = new PmsSummaryViewModel
            {
                Id = 1,
                Unit = "Case",
                ActualMonthly = 10,
                ActualYearly = 20,
                ActualYtd = 30,
                Pillar = "Safety",
                Kpi = "Fatality/Strap Disability",
                Weight = 20,
                Score = 35.17,
                TargetMonthly = 10,
                TargetYearly = 29,
                TargetYtd = 20,
            };
            pmsSummary1.Pillar = "<span class='trafficlight grey'></span>Safety (" +
                              pmsSummary1.Weight.ToString() + ")";

            var pmsSummary2 = new PmsSummaryViewModel
            {
                Id = 2,
                Unit = "Case",
                Weight = 100,
                ActualMonthly = 210,
                ActualYearly = 320,
                ActualYtd = 340,
                Pillar = "Safety",
                //OspWeight = 20.00,
                Kpi = "RIF",
                Score = 202.20,
                TargetMonthly = 101,
                TargetYearly = 22,
                TargetYtd = 21
            };
            pmsSummary2.Pillar = "<span class='trafficlight grey'></span>Safety (" +
                              pmsSummary2.Weight.ToString() + ")";

            var pmsSummary3 = new PmsSummaryViewModel
            {
                Id = 3,
                Unit = "Case",
                Weight = 100,
                ActualMonthly = 210,
                ActualYearly = 320,
                ActualYtd = 340,
                Kpi = "RIF",
                Score = 202.31,
                TargetMonthly = 101,
                TargetYearly = 22,
                TargetYtd = 21
            };
            pmsSummary3.Pillar = "<span class='trafficlight grey'></span>Productivity and Efficiency (" +
                              pmsSummary3.Weight.ToString() + ")";

            list.Add(pmsSummary1);
            list.Add(pmsSummary2);
            list.Add(pmsSummary3);
            return list;

        }
    }
}