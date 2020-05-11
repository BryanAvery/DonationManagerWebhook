using DonationManagerWebHooks.Models;
using DonationManagerWebHooks.WordPressService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DonationManagerWebHooks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class WebhookController : ControllerBase
    {
        private readonly IDonationManagerService donationManagerService;
        private readonly ILogger<WebhookController> logger;

        public WebhookController(IDonationManagerService donationManagerService, ILogger<WebhookController> logger)
        {
            this.donationManagerService = donationManagerService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<DonationHeader>> DonationManager([FromBody] List<DonationHeader> donations)
        {
            try
            {
                Log.Debug(JsonSerializer.Serialize(donations));

                foreach (var donation in donations)
                {
                    if(donation?.supporter != null)
                        await donationManagerService.SubscriptionAsync(donation.supporter);

                    if(donation?.appeal != null)
                        await donationManagerService.AppealAsync(donation.appeal);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
            return Ok(donations);
        }
    }
}