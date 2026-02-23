using System.Threading.Channels;
using Application.Common.Interfaces.Services.BackgroundEmail;

namespace Infrastructure.Services;

public class BackgroundEmailQueue : IBackgroundEmailQueue
{
    private readonly Channel<EmailMessage> _channel;

    public BackgroundEmailQueue()
    {
        var options = new BoundedChannelOptions(capacity: 100)
        {
            FullMode = BoundedChannelFullMode.Wait, 
            SingleReader = true
        };
        
        _channel = Channel.CreateBounded<EmailMessage>(options);
    }

    /// <inheritdoc/>
    public async Task QueueEmail(EmailMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }
        await _channel.Writer.WriteAsync(message);
    }

    /// <inheritdoc/>
    public async ValueTask<EmailMessage> DequeueEmailAsync(CancellationToken cancellationToken)
    {
        // Wait until an item is available in the channel.
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}