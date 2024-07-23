using System.Collections.Generic;

namespace Server.Nelderim;

public class NelderimRegionGuard
{
    public string Name { get; set; }
    public double Female { get; set; }
    public Dictionary<Race, double> Population { get; set; } = new();
}