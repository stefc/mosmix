namespace stefc.mosmix;

using System.Collections.Immutable;
using StationRegistry = stefc.mosmix.V1.StationRegistry;

public static class MosmixStationRegistry
{

    private static Lazy<StationRegistry> stationRegistry = new Lazy<StationRegistry>(() =>
    {
        var assembly = typeof(MosmixStationRegistry).Assembly!;
        Stream resource = assembly.GetManifestResourceStream("mosmix.stations.dat");
        StationRegistry registry = StationRegistry.Parser.ParseFrom(resource);
        return registry;
    });

    public static IEnumerable<Station> GetAll() => stationRegistry
           .Value
           .Stations
           .Select(station => new Station(
            station.Clu, station.Id, station.Name,
                new Coordinate(Convert.ToDecimal(station.Location.Lat),
                    Convert.ToDecimal(station.Location.Lon),
                    Convert.ToDecimal(station.Location.Alt)),
                    (Area)station.Area,
                    (ushort?)station.CountryId,  
                    (ushort?)station.StateId) 
                    ).ToImmutableArray();

    public static string GetCountryStatePatriciaTrieRaw() => stationRegistry
        .Value
        .CountryStateTrie;
}