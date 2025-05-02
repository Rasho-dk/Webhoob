using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Webhoob.Models
{
   /// <summary>
   /// 
   /// </summary>
    public class WebhookSubscription
    {
        /// <summary>
        /// Unique identifier for the webhook subscription.
        /// </summary>
        [Key]
        //[JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The URL to which the webhook will send notifications.
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// Comma-separated list of event types that this webhook is subscribed to.
        /// </summary>
        // Saving as CSV string for simplicity in database, but in C# we will use Enum.
        [JsonIgnore]
        public string EventTypesCsv { get; set; }

        [NotMapped]
        public List<WebhookEventType> EventTypes
        {
            get => EventTypesCsv?.Split(',').Select(e => Enum.Parse<WebhookEventType>(e)).ToList() ?? new();
            set => EventTypesCsv = string.Join(",", value.Select(v => v.ToString()));
        }

        //public string? Secret { get; set; } // Used for HMAC signing
        public WebhookSubscription()
        {
            
        }
        public WebhookSubscription(string callbackUrl, List<WebhookEventType> eventTypes)
        {
            CallbackUrl = callbackUrl;
            EventTypes = eventTypes;
        }
    }
    
}

/// <summary>
/// This the event types that the webhook can subscribe to.
/// </summary>
public enum WebhookEventType
{
    PaymentInitiated,
    PaymentCompleted,
    PaymentFailed
}