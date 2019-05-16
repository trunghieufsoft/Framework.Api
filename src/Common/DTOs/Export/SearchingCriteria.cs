using System;
using System.Collections.Generic;

namespace Common.DTOs.Export
{
    public class SearchingCriteria
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int RowNumber { get; set; }
    }
}