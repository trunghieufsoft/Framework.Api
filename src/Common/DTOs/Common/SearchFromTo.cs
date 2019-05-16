using Common.Core.Timing;
using System;

namespace Common.DTOs.Common
{
    public class SearchFromTo<TProperty>
    {
        public SearchFromTo() { }

        public SearchFromTo(SearchInput _search, TProperty _property)
        {
            DataSearch = _search;
            Property = _property;
        }

        public SearchFromTo(SearchInput _search, TProperty _property, DateTime? from = null, DateTime? to = null)
        {
            DataSearch = _search;
            Property = _property;
            From = from != null ? from : default(DateTime);
            To = from != null && to == null ? Clock.Now : to != null ? to : Clock.Now;
        }

        public TProperty Property { get; set; }

        public SearchInput DataSearch { get; set; }

        public DateTime? From { get; set; } = null;

        public DateTime? To { get; set; } = null;
    }

    public class SearchFromTo
    {
        public SearchFromTo() { }

        public SearchFromTo(SearchInput _search)
        {
            Search = _search;
        }

        public SearchFromTo(SearchInput _search, DateTime? from = null, DateTime? to = null)
        {
            Search = _search;
            From = from != null ? from : default(DateTime);
            To = from != null && to == null ? Clock.Now : to != null ? to : Clock.Now;
        }

        public SearchInput Search { get; set; }

        public DateTime? From { get; set; } = null;

        public DateTime? To { get; set; } = null;
    }
}