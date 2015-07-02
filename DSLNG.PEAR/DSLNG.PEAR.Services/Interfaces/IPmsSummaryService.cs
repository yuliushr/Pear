﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.PmsSummary;
using DSLNG.PEAR.Services.Responses.PmsSummary;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPmsSummaryService
    {
        CreatePmsSummaryResponse CreatePmsSummary(CreatePmsSummaryRequest request);
        UpdatePmsSummaryResponse UpdatePmsSummary(UpdatePmsSummaryRequest request);
        GetPmsSummaryReportResponse GetPmsSummaryReport(GetPmsSummaryReportRequest request);
        GetPmsSummaryResponse GetPmsSummary(int id);
        GetPmsSummaryListResponse GetPmsSummaryList(GetPmsSummaryListRequest request);
        GetPmsSummaryConfigurationResponse GetPmsSummaryConfiguration(GetPmsSummaryConfigurationRequest request);
        GetScoreIndicatorsResponse GetScoreIndicators(int pmsConfigDetailId);
        GetPmsDetailsResponse GetPmsDetails(GetPmsDetailsRequest request);
        GetPmsConfigDetailsResponse GetPmsConfigDetails(int id);
        GetKpisByPillarIdResponse GetKpis(int pillarId);
        UpdatePmsConfigResponse UpdatePmsConfig(UpdatePmsConfigRequest request);
        CreatePmsConfigResponse CreatePmsConfig(CreatePmsConfigRequest request);
        
    }
}
