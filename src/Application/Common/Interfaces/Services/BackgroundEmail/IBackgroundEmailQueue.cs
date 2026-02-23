namespace Application.Common.Interfaces.Services.BackgroundEmail;

public interface IBackgroundEmailQueue
{
    /// <summary>
    /// Queues an email message to be sent asynchronously.
    /// </summary>
    /// <param name="message">The email message details.</param>
    Task QueueEmail(EmailMessage message);

    /// <summary>
    /// Dequeues the next email message to be processed.
    /// This method is designed for the background service consumer.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The next email message from the queue.</returns>
    ValueTask<EmailMessage> DequeueEmailAsync(CancellationToken cancellationToken);
}