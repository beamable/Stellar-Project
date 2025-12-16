using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Beamable.StellarFederation.BackgroundService;

public class BackoffDelayManager
{
    private readonly int _initialDelay;
    private readonly int _maxDelay;
    private int _currentDelay;
    private CancellationTokenSource _delayCts = new();
    private readonly ILogger<BackoffDelayManager> _logger;

    public BackoffDelayManager(ILogger<BackoffDelayManager> logger, int initialDelay = 100, int maxDelay = 1000000)
    {
        _logger = logger;
        _initialDelay = initialDelay;
        _maxDelay = maxDelay;
        _currentDelay = _initialDelay;
    }

    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _delayCts.Token);
        try
        {
            _logger.LogInformation("Backoff delay {CurrentDelay} ms", _currentDelay);
            await Task.Delay(_currentDelay, linkedCts.Token);
            IncreaseDelay();
        }
        catch (OperationCanceledException)
        {
            Reset();
        }
    }

    public void Reset()
    {
        _currentDelay = _initialDelay;
        _logger.LogInformation("DelayManager reset to {Delay}ms", _currentDelay);
        _delayCts.Cancel();
        _delayCts.Dispose();
        _delayCts = new CancellationTokenSource();
    }

    private void IncreaseDelay()
    {
        _currentDelay = _currentDelay >= int.MaxValue - 1000 ? _maxDelay : Math.Min(_currentDelay + 1000, _maxDelay);
    }
}