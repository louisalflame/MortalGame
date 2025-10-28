using UnityEngine;

public interface IDispositionManager
{
    int CurrentDisposition { get; }
    int MaxDisposition { get; }
}

public record DispositionInfo(int CurrentDisposition, int MaxDisposition);

public class DispositionManager : IDispositionManager
{
    public int CurrentDisposition => _disposition;
    public int MaxDisposition => _maxDisposition;

    private int _disposition;
    private readonly int _maxDisposition;

    public DispositionManager(int initialDisposition, int maxDisposition)
    {
        _disposition = initialDisposition;
        _maxDisposition = maxDisposition;
    }
}