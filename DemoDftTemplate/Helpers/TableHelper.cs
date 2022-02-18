using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DemoDftTemplate.Helpers
{
    //Helper class for displaying tables/lists of data on grid/table views (see RVSS Organisation Index view for example of use)
    public static class TableHelper
    {
        //Takes the IEnuerable list of any type and sorts by specific held and asc or desc
        public static IEnumerable<T> Sort<T>(IEnumerable<T> enumerable, string sortByField, string sortOrder)
        {
            if (String.IsNullOrWhiteSpace(sortByField))
                return enumerable;

            if (String.IsNullOrEmpty(sortOrder))
                return enumerable;

            var orderByPropertyInfo = typeof(T).GetProperty(sortByField);

            if (sortOrder == "asc")
                return enumerable.OrderBy(x => orderByPropertyInfo.GetValue(x, null));

            return enumerable.OrderByDescending(x => orderByPropertyInfo.GetValue(x, null));
        }

        //Builds the full set of ViewData needed by the grid
        public static Dictionary<string, string> BuildTableViewData<T>(IEnumerable<T> enumerable, string sortOrder, string searchString, string currentFilter)
        {
            Dictionary<string, string> vData = new Dictionary<string, string>();

            //cycles through properties of the object in question
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                //sets the sort value and the asc/desc marker for each column depending on whether it's been clicked last or not
                string ascOrDesc = sortOrder == property.Name + "_asc" ? property.Name + "_desc" : property.Name + "_asc";
                vData.Add(property.Name + "Sort", ascOrDesc);
            }

            //add the current sort to view data so it can be used by paging buttons
            vData.Add("CurrentSort", sortOrder);

            //add the current search term to viewdata so it can be used by paging and sort buttons
            searchString = string.IsNullOrEmpty(currentFilter) ? searchString : currentFilter;
            vData.Add("CurrentFilter", searchString);

            return vData;
        }

        public class PaginatedList<T> : List<T>
        {
            public int PageIndex { get; private set; }
            public int TotalPages { get; private set; }
            public int TotalItems { get; private set; }
            public string PageInfo { get; private set; }

            public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
            {
                PageIndex = pageIndex;
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                TotalItems = count;

                if (TotalItems == 0)
                {
                    PageInfo = "No Records Found";
                }
                else
                {
                    string topRecord = pageSize * PageIndex > TotalItems ? TotalItems.ToString() : (pageSize * PageIndex).ToString();
                    string bottomRecord = PageIndex > 1 ? ((pageSize * (PageIndex - 1)) + 1).ToString() : "1";

                    PageInfo = "Showing " + bottomRecord + " - " + topRecord + " of " + TotalItems.ToString() + " records - Page " + PageIndex.ToString() + " of " + TotalPages.ToString();
                }

                this.AddRange(items);
            }

            public bool HasPreviousPage
            {
                get
                {
                    return (PageIndex > 1);
                }
            }

            public bool HasNextPage
            {
                get
                {
                    return (PageIndex < TotalPages);
                }
            }

            public static PaginatedList<T> Create(
                IEnumerable<T> source, int pageIndex, int pageSize)
            {
                var count = source.Count();
                var items = source.Skip(
                    (pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();
                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
        }
    }
}
