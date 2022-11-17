using Bvs.Entities;
using Bvs_API.Data;
using Bvs_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bvs_API.Controllers
{
    [Authorize]
    public class OverviewController : BaseApiController
    {
        private readonly IOverviewRepository _overviewRebo;

        public OverviewController(IOverviewRepository overviewRepository)
        {
            _overviewRebo = overviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<NumberOverview>> Getoverview()
        {
            return await _overviewRebo.GetOverviewAsync();
        }
    }
}
