using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using Webhoob.DbContext;
using Webhoob.Models;

namespace Webhoob.Service
{
    public class WebhookService
    {
        private readonly ObjectDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookService(ObjectDbContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// Retrieves all registered webhook subscriptions.
        /// </summary>
        /// <returns>Returns a list of all registered webhook subscriptions.</returns>
        public async Task<List<WebhookSubscription>> GetAllAsync()
        {
            return await _db.Webhooks.ToListAsync();
        }

        /// <summary>
        /// Registers a new webhook subscription.
        /// </summary>
        /// <param name="callbackUrl">This is the URL that will receive the webhook events.</param>
        /// <param name="eventTypes">This is the list of event types that this webhook is subscribed to.</param>
        /// <returns>Returns the unique identifier for the webhook subscription.</returns>
        public async Task<Guid> RegisterAsync(string callbackUrl, List<WebhookEventType> eventTypes)
        {
            var webhook = new WebhookSubscription
            {
                CallbackUrl = callbackUrl,
                EventTypes = eventTypes
            };

            _db.Webhooks.Add(webhook);
            await _db.SaveChangesAsync();
            return webhook.Guid;
        }
        /// <summary>
        /// Unregisters a webhook subscription.
        /// </summary>
        /// <param name="id">Given the unique identifier of the webhook subscription and unregisters it.</param>
        /// <returns>Returns true if the webhook was successfully unregistered, false otherwise.</returns>
        public async Task<bool> UnregisterAsync(Guid id)
        {
            var webhook = await _db.Webhooks.FindAsync(id);
            if (webhook == null) return false;
            _db.Webhooks.Remove(webhook);
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Pings all registered webhooks with a test message.
        /// </summary>
        /// <returns></returns>
        public async Task PingAllAsync()
        {
            var allWebhooks = await _db.Webhooks.ToListAsync();

            var client = new HttpClient();

            foreach (var webhook in allWebhooks)
            {
                var pingPayload = new
                {
                    message = "Ping - Webhook is working!",
                    timestamp = DateTime.UtcNow,
                    webhook.EventTypesCsv,
                    webhook.CallbackUrl
                };

                // simulate a ping with delay
                await Task.Delay(3000); // Simulate a delay of 3 second


                var content = new StringContent(JsonSerializer.Serialize(pingPayload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhook.CallbackUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to ping {webhook.CallbackUrl}. Status: {response.StatusCode}");
                    throw new Exception($"Failed to ping {webhook.CallbackUrl}. Status: {response.StatusCode}");
                }

            }
        }
    }

}