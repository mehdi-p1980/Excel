using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Acme.BookStore.Seasons
{
    [RemoteService]
    [Route("api/seasons")]
    public class SeasonController : BookStoreController, ISeasonAppService
    {
        private readonly ISeasonAppService _seasonAppService;

        public SeasonController(ISeasonAppService seasonAppService)
        {
            _seasonAppService = seasonAppService;
        }

        [HttpGet]
        public List<SeasonDto> GetSeasons(DateTime startDate, DateTime endDate)
        {
            return _seasonAppService.GetSeasons(startDate, endDate);
        }
    }
}
