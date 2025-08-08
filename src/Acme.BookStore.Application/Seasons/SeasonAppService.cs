using System;
using System.Collections.Generic;
using System.Globalization;

namespace Acme.BookStore.Seasons
{
    public class SeasonAppService : BookStoreAppService, ISeasonAppService
    {
        public List<SeasonDto> GetSeasons(DateTime startDate, DateTime endDate)
        {
            var seasons = new List<SeasonDto>();
            var persianCalendar = new PersianCalendar();
            var startYear = persianCalendar.GetYear(startDate);
            var endYear = persianCalendar.GetYear(endDate);

            for (int year = startYear; year <= endYear; year++)
            {
                var spring = new SeasonDto { Name = "Bahar (Spring)", StartDate = persianCalendar.ToDateTime(year, 1, 1, 0, 0, 0, 0), EndDate = persianCalendar.ToDateTime(year, 3, 31, 0, 0, 0, 0) };
                var summer = new SeasonDto { Name = "Tabestan (Summer)", StartDate = persianCalendar.ToDateTime(year, 4, 1, 0, 0, 0, 0), EndDate = persianCalendar.ToDateTime(year, 6, 31, 0, 0, 0, 0) };
                var autumn = new SeasonDto { Name = "Paeez (Autumn)", StartDate = persianCalendar.ToDateTime(year, 7, 1, 0, 0, 0, 0), EndDate = persianCalendar.ToDateTime(year, 9, 30, 0, 0, 0, 0) };
                var winter = new SeasonDto { Name = "Zemestan (Winter)", StartDate = persianCalendar.ToDateTime(year, 10, 1, 0, 0, 0, 0), EndDate = persianCalendar.ToDateTime(year, 12, 29, 0, 0, 0, 0) };
                if(persianCalendar.IsLeapYear(year))
                {
                    winter.EndDate = persianCalendar.ToDateTime(year, 12, 30, 0, 0, 0, 0);
                }


                AddSeasonIfInRange(seasons, spring, startDate, endDate);
                AddSeasonIfInRange(seasons, summer, startDate, endDate);
                AddSeasonIfInRange(seasons, autumn, startDate, endDate);
                AddSeasonIfInRange(seasons, winter, startDate, endDate);
            }

            return seasons;
        }

        private void AddSeasonIfInRange(List<SeasonDto> seasons, SeasonDto season, DateTime startDate, DateTime endDate)
        {
            if (season.StartDate <= endDate && season.EndDate >= startDate)
            {
                seasons.Add(season);
            }
        }
    }
}
