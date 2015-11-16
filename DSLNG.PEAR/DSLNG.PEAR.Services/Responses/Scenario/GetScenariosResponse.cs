﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.Scenario
{
    public class GetScenariosResponse
    {
        public IList<Scenario> Scenarios { get; set; }
        public int TotalRecords { get; set; }
        public int Count { get; set; }
        public class Scenario
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
