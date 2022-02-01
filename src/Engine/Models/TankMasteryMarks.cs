namespace Engine.Models;

public class TankMasteryMarks
{
    public int TankId { get; set; }
    public string TankName { get; set; }
    public int NumberOfBattles { get; set; }
    public int NumberOfMasteryMarks { get; set; }
    public int NumberOfMastery1Marks { get; set; }
    public int NumberOfMastery2Marks { get; set; }
    public int NumberOfMastery3Marks { get; set; }

    public int MarksEventPoints =>
        NumberOfMasteryMarks * 5 +
        NumberOfMastery1Marks * 3 +
        NumberOfMastery2Marks * 2 +
        NumberOfMastery3Marks * 1;

    public double MarksGettingsProbability => Math.Round(
        (NumberOfMasteryMarks +
        NumberOfMastery1Marks +
        NumberOfMastery2Marks +
        NumberOfMastery3Marks) / (double)NumberOfBattles, 2);
}
