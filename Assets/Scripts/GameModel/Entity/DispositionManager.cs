using UnityEngine;

public class DispositionManager
{
    public enum Name 
    {
        Coward = -2,
        Cautious = -1,
        Moderate = 0,
        Brave = 1,
        Reckless = 2,
    }

    public int CurrentDisposition;
    public const int MAX_DISPOSITION = 10;

    public DispositionManager(int initialDisposition)
    {
        CurrentDisposition = initialDisposition; 
    }

    public string GetName() {
        if      ( CurrentDisposition <= 1 )
            return Name.Coward.ToString();
        else if ( CurrentDisposition <= 3 )
            return Name.Cautious.ToString();
        else if ( CurrentDisposition <= 6 )
            return Name.Moderate.ToString();
        else if ( CurrentDisposition <= 8 )
            return Name.Brave.ToString();
        else
            return Name.Reckless.ToString();
    }
}