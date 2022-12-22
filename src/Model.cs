﻿namespace stefc.mosmix;

public record Document(ProductDefinition Definition, PlaceMark PlaceMark)
{
    internal Document() : this(
        Definition: new ProductDefinition(),
        PlaceMark: new PlaceMark(string.Empty, string.Empty, null, new Dictionary<string, double?[]> { }))
    { }
}

public record ProductDefinition(string Issuer, string Id, DateTime? IssueTime, IEnumerable<DateTime> TimeSteps)
{
    internal ProductDefinition() : this(string.Empty, string.Empty, null, Enumerable.Empty<DateTime>()) { }
}

public record PlaceMark(string Name, string Description, Coordinate? Coordinate, IDictionary<string, double?[]> Forecasts) { }

public record Coordinate(decimal Latitude, decimal Longitude, decimal Altitude) { }

public enum Area {
    Land = 1, 
    Mountain = 2,
    Ocean = 3,
    Coast = 4
}

public record class Station (int Clu, string Id, string Name, Coordinate Location, Area Area) {}
