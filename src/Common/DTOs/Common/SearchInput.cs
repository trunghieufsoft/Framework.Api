using System.Collections.Generic;

namespace Common.DTOs.Common
{

    public class SearchInput
    {
        public Dictionary<string, string> KeySearch { get; set; } = null;

        public List<string> SearchEqual { get; set; } = null;

        public int? PageIndex { get; set; } = 0;

        public int? Limit { get; set; } = 0;

        public string OrderBy { get; set; } = "Id";

        public bool IsSortDescending { get; set; } = true;
    }

    public class SearchInput<TProperty>
    {
        public SearchInput() { }

        public SearchInput(SearchInput _search, TProperty _property)
        {
            DataSearch = _search;
            Property = _property;
        }

        public TProperty Property { get; set; }

        public SearchInput DataSearch { get; set; }
    }

    public class SearchExport
    {
        public Dictionary<string, string> KeySearch { get; set; } = null;

        public List<string> SearchEqual { get; set; } = null;

        public string OrderBy { get; set; } = "Id";

        public bool IsSortDescending { get; set; } = true;

        public string SheetName { get; set; } = null;
    }
}