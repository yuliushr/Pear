﻿using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Measurement;
using DSLNG.PEAR.Web.ViewModels.Artifact;
using System;
using System.Linq;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.Artifact;
using System.Collections.Generic;

namespace DSLNG.PEAR.Web.Controllers
{
    public class ArtifactController : BaseController
    {
        public IMeasurementService _measurementService;
        public IKpiService _kpiService;
        public IArtifactService _artifactServie;

        public ArtifactController(IMeasurementService measurementService,
            IKpiService kpiService,
            IArtifactService artifactServcie) {
            _measurementService = measurementService;
            _kpiService = kpiService;
            _artifactServie = artifactServcie;
        }

        public ActionResult Designer()
        {
            var viewModel = new ArtifactDesignerViewModel();
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "bar", Text = "Bar" });
            viewModel.GraphicTypes.Add(new SelectListItem { Value = "line", Text = "Line" });

            viewModel.Measurements = _measurementService.GetMeasurements(new GetMeasurementsRequest()).Measurements
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            return View(viewModel);
        }

        public ActionResult GraphSettings()
        {
            switch (Request.QueryString["type"])
            {
                case "bar":
                    {
                        var viewModel = new BarChartViewModel();
                        this.SetPeriodeTypes(viewModel.PeriodeTypes);
                        this.SetRangeFilters(viewModel.RangeFilters);
                        this.SetValueAxes(viewModel.ValueAxes);
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.SingleStack.ToString(), Text = "Single Stack" });
                        viewModel.SeriesTypes.Add(new SelectListItem { Value = SeriesType.MultiStacks.ToString(), Text = "Multi Stacks" });
                        this.SetKpiList(viewModel.KpiList);
                        var series = new BarChartViewModel.Series();
                        series.Stacks.Add(new BarChartViewModel.Stack());
                        viewModel.SeriesList.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.BarChart = viewModel;
                        return PartialView("~/Views/BarChart/_Create.cshtml", artifactViewModel);
                    }
                case "line":
                    {
                        var viewModel = new LineChartViewModel();
                        this.SetPeriodeTypes(viewModel.PeriodeTypes);
                        this.SetRangeFilters(viewModel.RangeFilters);
                        this.SetValueAxes(viewModel.ValueAxes);
                        this.SetKpiList(viewModel.KpiList);
                        var series = new LineChartViewModel.Series();
                        viewModel.SeriesList.Add(series);
                        var artifactViewModel = new ArtifactDesignerViewModel();
                        artifactViewModel.LineChart = viewModel;
                        return PartialView("~/Views/LineChart/_Create.cshtml", artifactViewModel);
                    }
                default:
                    return PartialView("NotImplementedChart.cshtml");
            }
        }

        public void SetKpiList(IList<SelectListItem> kpiList) {
            foreach(var kpi in _kpiService.GetKpiToSeries().KpiList){
                kpiList.Add(new SelectListItem { Value = kpi.Id.ToString(), Text = kpi.Name });
            }
        }

        public void SetValueAxes(IList<SelectListItem> valueAxes) {
            valueAxes.Add(new SelectListItem { Value = ValueAxis.KpiTarget.ToString(), Text = "Kpi Target" });
            valueAxes.Add(new SelectListItem { Value = ValueAxis.KpiActual.ToString(), Text = "Kpi Actual" });
            valueAxes.Add(new SelectListItem { Value = ValueAxis.KpiEconomic.ToString(), Text = "Kpi Economic" });
            valueAxes.Add(new SelectListItem { Value = ValueAxis.Custom.ToString(), Text = "Uniqe Each Series" });
        }

        public void SetPeriodeTypes(IList<SelectListItem> periodeTypes) {
            foreach (var name in Enum.GetNames(typeof(PeriodeType))) {
                periodeTypes.Add(new SelectListItem { Text = name, Value = name });
            }
        }

        public void SetRangeFilters(IList<SelectListItem> rangeFilters) {
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentHour.ToString(), Text = "CURRENT HOUR" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentDay.ToString(), Text = "CURRENT DAY" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentWeek.ToString(), Text = "CURRENT WEEK" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentMonth.ToString(), Text = "CURRENT MONTH" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.CurrentYear.ToString(), Text = "CURRENT YEAR" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.DTD.ToString(), Text = "DAY TO DATE" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.MTD.ToString(), Text = "MONTH TO DATE" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.YTD.ToString(), Text = "YEAR TO DATE" });
            rangeFilters.Add(new SelectListItem { Value = RangeFilter.Interval.ToString(), Text = "INTERVAL" });
        }

        [HttpPost]
        public ActionResult Preview(ArtifactDesignerViewModel viewModel) {
            var previewViewModel = new ArtifactPreviewViewModel();
            switch (viewModel.GraphicType) {
                case "line":
                    {
                        var chartData = _artifactServie.GetChartData(viewModel.LineChart.MapTo<GetChartDataRequest>());
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.LineChart = new LineChartDataViewModel();
                        previewViewModel.LineChart.Title = viewModel.HeaderTitle;
                        previewViewModel.LineChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.LineChart.Series = chartData.Series.MapTo<LineChartDataViewModel.SeriesViewModel>();
                        previewViewModel.LineChart.Periodes = chartData.Periodes;
                    }
                    break;
                default:
                    {
                        var chartData = _artifactServie.GetChartData(viewModel.BarChart.MapTo<GetChartDataRequest>());
                        previewViewModel.GraphicType = viewModel.GraphicType;
                        previewViewModel.BarChart = new BarChartDataViewModel();
                        previewViewModel.BarChart.Title = viewModel.HeaderTitle;
                        previewViewModel.BarChart.ValueAxisTitle = _measurementService.GetMeasurement(new GetMeasurementRequest { Id = viewModel.MeasurementId }).Name;
                        previewViewModel.BarChart.Series = chartData.Series.MapTo<BarChartDataViewModel.SeriesViewModel>();
                        previewViewModel.BarChart.Periodes = chartData.Periodes;
                        previewViewModel.BarChart.SeriesType = chartData.SeriesType;
                    }
                    break;
            }
            return Json(previewViewModel);
        }
    }
}
