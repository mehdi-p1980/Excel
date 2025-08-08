using System;
using System.Collections.Generic;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.Seasons
{
    public interface ISeasonAppService : IApplicationService
    {
        List<SeasonDto> GetSeasons(DateTime startDate, DateTime endDate);
    }
}
