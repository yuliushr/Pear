﻿
using DevExpress.Web.Mvc;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Select;
using DSLNG.PEAR.Services.Requests.Weather;
using DSLNG.PEAR.Web.ViewModels.Weather;
using System.Web.Mvc;
using System.Linq;
using System;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Web.Controllers
{
    public class WeatherController : BaseController
    {
        private IWeatherService _weatherService;
        private ISelectService _selectService;

        public WeatherController(IWeatherService weatherService,ISelectService selectService) {
            _weatherService = weatherService;
            _selectService = selectService;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("gridArtifactIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return BindingCore(viewModel);
        }

        PartialViewResult BindingCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                GetDataRowCount,
                GetData
            );
            return PartialView("_IndexGridPartial", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("PeriodeType");
            viewModel.Columns.Add("Date");
            viewModel.Columns.Add("Value");
            viewModel.Columns.Add("Temperature");
           
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public ActionResult PagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridHighlightIndex");
            viewModel.ApplyPagingState(pager);
            return BindingCore(viewModel);
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            e.DataRowCount = _weatherService.GetWeathers(new GetWeathersRequest { OnlyCount = true }).Count;
        }

        public void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = _weatherService.GetWeathers(new GetWeathersRequest
            {
                Skip = e.StartDataRowIndex,
                Take = e.DataRowCount
            }).Weathers;
        }

        public ActionResult Create() {
            var viewModel = new WeatherViewModel();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(WeatherViewModel viewModel) {
            var request = viewModel.MapTo<SaveWeatherRequest>();
            _weatherService.SaveWeather(request);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            var viewModel = _weatherService.GetWeather(new GetWeatherRequest { Id = id }).MapTo<WeatherViewModel>();
            foreach (var name in Enum.GetNames(typeof(PeriodeType)))
            {
                if (!name.Equals("Hourly") && !name.Equals("Weekly"))
                {
                    viewModel.PeriodeTypes.Add(new SelectListItem { Text = name, Value = name });
                }
            }
            viewModel.Values = _selectService.GetSelect(new GetSelectRequest { Name = "weather-values" }).Options
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(WeatherViewModel viewModel)
        {
            var request = viewModel.MapTo<SaveWeatherRequest>();
            _weatherService.SaveWeather(request);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            _weatherService.Delete(new DeleteWeatherRequest { Id = id });
            return RedirectToAction("Index");
        }


    }
}
