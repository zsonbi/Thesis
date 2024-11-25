namespace DataTypes
{
    /// <summary>
    /// The possible task intervals
    /// </summary>
    public enum TaskIntervals
    {
        Hourly = 60,
        EveryTwoHours = 120,
        EveryFourHours = 240,
        Daily = 1440,
        EveryTwoDays = 2880,
        Weekly = 10080,
        EveryTwoWeeks = 20160,
        Monthly = 40320,
    }
}