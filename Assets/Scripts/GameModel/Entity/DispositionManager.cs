using UnityEngine;

public interface IDispositionManager
{
    int RecoverEnergyPoint { get; }
    int TurnStartDrawCardCount { get; }
    string Name { get; }
}

public class DispositionManager : IDispositionManager
{
    public readonly int[] DIPOSITION_RANGE = new int[] { 1, 3, 6, 8, 10 };
    public readonly string[] DISPODITION_NAMES = new string[]  { "Coward", "Cautious", "Moderate", "Brave", "Reckless", };
    public readonly int[] RECOVER_ENERGY_POINT = new int[] { 4, 4, 5, 6, 7, };
    public readonly int[] TURN_START_DRAW_CARD_COUNT = new int[] { 7, 6, 5, 4, 4, };

    public int CurrentDisposition => _disposition;
    private int _disposition;

    public DispositionManager(int initialDisposition)
    {
        _disposition = initialDisposition; 
    }

    public int RecoverEnergyPoint
    {
        get
        {
            return RECOVER_ENERGY_POINT[_GetDispositionIndex(_disposition)];
        }
    }

    public int TurnStartDrawCardCount
    {
        get
        {
            return TURN_START_DRAW_CARD_COUNT[_GetDispositionIndex(_disposition)];
        }
    }

    public string Name
    {
        get
        {
            return DISPODITION_NAMES[_GetDispositionIndex(_disposition)];
        }
    }

    private int _GetDispositionIndex(int disposition)
    {
        for (int i = 0; i < DIPOSITION_RANGE.Length; i++)
        {
            if (disposition <= DIPOSITION_RANGE[i])
                return i;
        }
        return DIPOSITION_RANGE.Length - 1;
    }
}