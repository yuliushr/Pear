﻿

namespace DSLNG.PEAR.Services.Requests.NLS
{
    public class GetNLSListRequest
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string Term { get; set; }
        public bool OnlyCount { get; set; }
    }
}
