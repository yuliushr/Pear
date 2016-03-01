﻿using DSLNG.PEAR.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.PopDashboard
{
    public class PopDashboardViewModel
    {
        public PopDashboardViewModel()
        {
            PopDashboards = new List<PopDashboard>();
        }
        public IList<PopDashboard> PopDashboards { get; set; }

        public class PopDashboard
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Number { get; set; }
            public string Subtitle { get; set; }
            public bool IsActive { get; set; }
        }
    }


    public class SavePopDashboardViewModel
    {
        public SavePopDashboardViewModel()
        {
            IsActive = true;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Subtitle { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetPopDashboardViewModel
    {
        public GetPopDashboardViewModel()
        {
            PopInformations = new List<PopInformation>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Subtitle { get; set; }
        public bool IsActive { get; set; }
        public IList<PopInformation> PopInformations { get; set; }

        public class PopInformation
        {
            public int Id { get; set; }
            public PopInformationType Type { get; set; }
            public string Title { get; set; }
            public string Value { get; set; }
        }
    }
}