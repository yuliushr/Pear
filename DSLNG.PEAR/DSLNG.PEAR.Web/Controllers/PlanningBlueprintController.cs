﻿using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PlanningBlueprint;
using DSLNG.PEAR.Services.Responses.PlanningBlueprint;
using DSLNG.PEAR.Web.Grid;
using DSLNG.PEAR.Web.ViewModels.PlanningBlueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Services.Requests.BusinessPosture;
using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using DSLNG.PEAR.Web.ViewModels.EnvironmentScanning;

namespace DSLNG.PEAR.Web.Controllers
{
    public class PlanningBlueprintController : BaseController
    {
        private readonly IPlanningBlueprintService _planningBlueprintService;
        private readonly IEnvironmentScanningService _environmentScanningService;
        private readonly IBusinessPostureIdentificationService _businessPostureIdentification;
        private readonly IMidtermFormulationService _midtermFormulationService;
        private readonly IMidtermPlanningService _midtermPlanningService;
        private readonly IOutputCategoryService _outputCategoryService;

        public PlanningBlueprintController(IPlanningBlueprintService planningBlueprintService,
            IBusinessPostureIdentificationService businessPostureIdentification,
            IEnvironmentScanningService environmentScanningService,
            IMidtermFormulationService midtermFormulationService,
            IMidtermPlanningService midtermPlanningService,
            IOutputCategoryService outputCategoryService)
        {
            _planningBlueprintService = planningBlueprintService;
            _businessPostureIdentification = businessPostureIdentification;
            _environmentScanningService = environmentScanningService;
            _midtermFormulationService = midtermFormulationService;
            _midtermPlanningService = midtermPlanningService;
            _outputCategoryService = outputCategoryService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridParams gridParams)
        {
            var planningBlueprintData = _planningBlueprintService.GetPlanningBlueprints(new GetPlanningBlueprintsRequest
            {
                Skip = gridParams.DisplayStart,
                Take = gridParams.DisplayLength,
                Search = gridParams.Search,
                SortingDictionary = gridParams.SortingDictionary
            });
            IList<GetPlanningBlueprintsResponse.PlanningBlueprint> DatasResponse = planningBlueprintData.PlanningBlueprints;
            var data = new
            {
                sEcho = gridParams.Echo + 1,
                iTotalDisplayRecords = planningBlueprintData.TotalRecords,
                iTotalRecords = planningBlueprintData.PlanningBlueprints.Count,
                aaData = DatasResponse
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Create()
        {
            return View(new PlanningBlueprintViewModel());
        }

        [HttpPost]
        public ActionResult Create(PlanningBlueprintViewModel viewModel)
        {
            var request = viewModel.MapTo<SavePlanningBlueprintRequest>();
            var response = _planningBlueprintService.SavePlanningBlueprint(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id) {
            return View(_planningBlueprintService.GetPlanningBlueprint(id).MapTo<PlanningBlueprintViewModel>());
        }

        [HttpPost]
        public ActionResult Edit(PlanningBlueprintViewModel viewModel) {
            var request = viewModel.MapTo<SavePlanningBlueprintRequest>();
            var response = _planningBlueprintService.SavePlanningBlueprint(request);
            TempData["IsSuccess"] = response.IsSuccess;
            TempData["Message"] = response.Message;
            return RedirectToAction("Index");
        }

        public ActionResult EnvironmentsScanning(int id)
        {
            var viewModel = _environmentScanningService.GetEnvironmentsScanning(new GetEnvironmentsScanningRequest { Id = id }).MapTo<EnvironmentScanningViewModel>();
                //if (viewModel.IsLocked) {
                //    return RedirectToAction("Index");
                //}
            var ListType = new List<SelectListItem>();
            var type1 = new SelectListItem() { Text = "Internal", Value = "Internal" };
            ListType.Add(type1);
            var type2 = new SelectListItem() { Text = "External", Value = "External" };
            ListType.Add(type2);

            var listCategory = new List<SelectListItem>();
            var category1 = new SelectListItem() { Text = "Politic", Value = "Politic" };
            listCategory.Add(category1);
            var category2 = new SelectListItem() { Text = "Economic", Value = "Economic" };
            listCategory.Add(category2);

            viewModel.Types = ListType;
            viewModel.Categories = listCategory;

            return View(viewModel);
        }
        public ActionResult ESReview(int id)
        {
            var viewModel = _environmentScanningService.GetEnvironmentsScanning(new GetEnvironmentsScanningRequest { Id = id }).MapTo<EnvironmentScanningViewModel>();
            viewModel.IsReviewer = true;
            //if (viewModel.IsLocked) {
            //    return RedirectToAction("Index");
            //}
            var ListType = new List<SelectListItem>();
            var type1 = new SelectListItem() { Text = "Internal", Value = "Internal" };
            ListType.Add(type1);
            var type2 = new SelectListItem() { Text = "External", Value = "External" };
            ListType.Add(type2);

            var listCategory = new List<SelectListItem>();
            var category1 = new SelectListItem() { Text = "Politic", Value = "Politic" };
            listCategory.Add(category1);
            var category2 = new SelectListItem() { Text = "Economic", Value = "Economic" };
            listCategory.Add(category2);

            viewModel.Types = ListType;
            viewModel.Categories = listCategory;

            return View("EnvironmentsScanning",viewModel);
        }

        [HttpPost]
        public ActionResult SubmitEnvironmentsScanning(int id) {
            var resp = _environmentScanningService.SubmitEnvironmentsScanning(id);
            if (resp.IsSuccess) {
                return RedirectToAction("BusinessPostureIdentification", new { id = resp.BusinessPostureId});
            }
            return RedirectToAction("EnvironmentsScanning", new { id = id });
        }

        public ActionResult BusinessPostureIdentification(int id)
        {
            var viewModel = _businessPostureIdentification.Get(new GetBusinessPostureRequest { Id = id }).MapTo<BusinessPostureViewModel>();

            return View(viewModel);
        }

        public ActionResult BPReview(int id)
        {
            var viewModel = _businessPostureIdentification.Get(new GetBusinessPostureRequest { Id = id }).MapTo<BusinessPostureViewModel>();
            viewModel.IsReviewer = true;
            return View("BusinessPostureIdentification", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitBusinessPosture(int id)
        {
            var resp = _businessPostureIdentification.SubmitBusinessPosture(id);
            if (resp.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("BusinessPostureIdentification", new { id = id });
        }

        [HttpPost]
        public ActionResult ApproveVoyagePlan(int id) {
            var resp = _planningBlueprintService.ApproveVoyagePlan(id);
            if (resp.IsSuccess) {
                return RedirectToAction("Approval");
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult RejectVoyagePlan(RejectVoyagePlanViewModel viewModel) {
            var resp = _planningBlueprintService.RejectVoyagePlan(viewModel.MapTo<RejectVoyagePlanRequest>());
            if (resp.IsSuccess)
            {
                return RedirectToAction("Approval");
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }


        public ActionResult VoyagePlan()
        {
            var resp = _planningBlueprintService.GetVoyagePlan();
            if (resp != null)
                return View(resp.MapTo<VoyagePlanViewModel>());
            return View();
        }

        public ActionResult Approval() {
            return View();
        }

        public ActionResult MidtermPhaseFormulation(int id) {
            return View(_midtermFormulationService.Get(id).MapTo<MidtermFormulationViewModel>());
        }
        [HttpPost]
        public ActionResult SubmitMidtermFormulation(int id)
        {
            var resp = _midtermFormulationService.SubmitMidtermFormulation(id);
            if (resp.IsSuccess)
            {
                return RedirectToAction("MidtermStrategyPlanning", new { id = id });
            }
            return RedirectToAction("MidtermPhaseFormulation", new { id = id });
        }

        public ActionResult MPFReview(int id)
        {
            var viewModel = _midtermFormulationService.Get(id).MapTo<MidtermFormulationViewModel>();
            viewModel.IsReviewer = true;
            return View("MidtermPhaseFormulation", viewModel);
        }
        //this id is id of midther phase formulation
        public ActionResult MidtermStrategyPlanning(int id) {
            return View(_midtermFormulationService.Get(id).MapTo<MidtermFormulationViewModel>());
        }

        public ActionResult MSPReview(int id)
        {
            var viewModel = _midtermFormulationService.Get(id).MapTo<MidtermFormulationViewModel>();
            viewModel.IsReviewer = true;
            return View("MidtermStrategyPlanning", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitMidtermPlanning(int id)
        {
            var resp = _midtermPlanningService.SubmitMidtermPlanning(id);
            if (resp.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("MidtermStrategyPlanning", new { id = id });
        }

        [HttpPost]
        public ActionResult ApproveMidtermStrategy(int id)
        {
            var resp = _planningBlueprintService.ApproveMidtermStrategy(id);
            if (resp.IsSuccess)
            {
                return RedirectToAction("Approval");
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult RejectMidtermStrategy(RejectMidtermStrategyViewModel viewModel)
        {
            var resp = _planningBlueprintService.RejectMidtermStrategy(viewModel.MapTo<RejectMidtermStrategyRequest>());
            if (resp.IsSuccess)
            {
                return RedirectToAction("Approval");
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult MidtermStrategy()
        {
            var resp = _planningBlueprintService.GetMidtermStrategy();
            if (resp != null)
            {
                var viewModel = resp.MapTo<MidtermFormulationViewModel>();
                viewModel.IsDashboardExist = true;
                viewModel.IsDashboard = true;
                viewModel.IsReviewer = true;
                return View("MidtermPhaseFormulation", viewModel);
            }
            else
            {
                var viewModel = new MidtermFormulationViewModel
                {
                    IsDashboard = true,
                    IsDashboardExist = false,
                    IsReviewer = true
                };
                return View("MidtermPhaseFormulation", viewModel);
            }
        }

        public ActionResult GetEconomicIndicators() {
            var viewModel = _outputCategoryService.GetActiveOutputCategories(false).MapTo<EconomicIndicatorsViewModel>();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult KpiTargetInput(KpiTargetInputViewModel model) {
            return Json(_planningBlueprintService.KpiTargetInput(model.MapTo<KpiTargetInputRequest>()));
        }

        [HttpPost]
        public ActionResult KpiEconomicInput(KpiEconomicInputViewModel model)
        {
            return Json(_planningBlueprintService.KpiEconomicInput(model.MapTo<KpiEconomicInputRequest>()));
        }

        public ActionResult AnnualBusinessPlan() {
            var viewModel = new AnnualBusinessPlanViewModel();
            viewModel.PlanningBlueprints = _planningBlueprintService.GetPlanningBlueprints().PlanningBlueprints.Select(x => new SelectListItem{Value = x.Id.ToString(), Text = x.Title}).ToList();
            return View(viewModel);
            //throw new NotImplementedException();
        }

    }
}