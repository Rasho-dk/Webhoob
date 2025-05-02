using Microsoft.AspNetCore.Mvc;
using Webhoob.Models;
using Webhoob.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webhoob.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {

        private readonly WebhookService _webhookService;

        /// <summary>
        /// Constructor for WebhooksController
        /// </summary>
        /// <param name="webhookService">Instance of WebhookService to handle webhook operations.</param>
        public WebhooksController(WebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        /// <summary>
        /// Get all registered webhooks
        /// </summary>
        /// <returns>Returns a list of all registered webhooks.</returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<WebhookSubscription>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var webhooks = await _webhookService.GetAllAsync();
            if (webhooks == null || !webhooks.Any())
            {
                return NotFound("No webhooks found.");
            }
            return Ok(webhooks);
        }

        /// <summary>
        /// This method registers a new webhook subscription i.e. https://localtunnel.me/enpoint
        /// </summary>
        /// <param name="subscription">Object containing the webhook subscription details.</param>
        /// <returns>Returns the unique identifier for the webhook subscription.</returns>
        // POST api/<WebhooksController>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WebhookSubscription))]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] WebhookSubscription subscription)
        {
            if (subscription.CallbackUrl == null || !Uri.IsWellFormedUriString(subscription.CallbackUrl, UriKind.Absolute))
            {
                return BadRequest("Invalid Callback URL.");
            }
            if (subscription.EventTypes == null || !subscription.EventTypes.Any())
            {
                return BadRequest("At least one event type is required.");
            }

            await _webhookService.RegisterAsync(subscription.CallbackUrl, subscription.EventTypes);
            return Ok(new { WebhookId = subscription.Guid });

        }

        /// <summary>
        /// Unregisters a webhook subscription by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the webhook subscription to unregister.</param>
        /// <returns>Returns a 204 No Content status if successful, or a 404 Not Found status if the webhook was not found.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // DELETE api/<WebhooksController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Unregister(Guid id)
        {
            var result = await _webhookService.UnregisterAsync(id);
            return result ? NoContent() : NotFound();
        }

        /// <summary>
        /// Pinging all registered webhooks.
        /// This is a test method to check if the webhooks are reachable.
        /// </summary>
        /// <returns>Returns a 200 OK status if successful, or a 500 Internal Server Error status if there was an error with the ping.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("ping")]
        public async Task<IActionResult> PingAll()
        {
            try
            {
                await _webhookService.PingAllAsync();
                return Ok("Pinged all webhooks successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error pinging webhooks: {ex.Message}");
            }

        }
    }
}
