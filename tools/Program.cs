using System.Text.RegularExpressions;
using LanguageExt;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

using stefc.mosmix;

using PbStation = stefc.mosmix.V1.Station;
using PbStationRegistry = stefc.mosmix.V1.StationRegistry;

using static LanguageExt.Prelude;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using MoreLinq;

using System.Collections.Immutable;

public static class Program
{
    public static void Main(string[] args)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "../data/", "st.cfg");
        System.Console.WriteLine(path);



        var stations = File.ReadAllLines(path)
            .Where(line => !String.IsNullOrEmpty(line))
            .SelectMany(x => ToStation(x))
            .ToArray();

        // Dump out only the GPS Coordinates for enhance them with a Region Identifier
        WriteGeoCodingCsv(stations.Select( c => c.Location), Path.Combine(Environment.CurrentDirectory, "locations.csv"));

        var regionReader = new ReadRegionIdentifiersCsv(Path.Combine(Environment.CurrentDirectory, "locations-ext.csv"));

        var regionIdentifiers = regionReader.ReadIdsOnly();

        var gpsLookUp = regionReader.ReadGpsLookUp();

        var topLevel = regionIdentifiers.Select( word => word.Split('|').Take(1).SingleOrDefault(string.Empty))
            .Where( word => word.Length == 2)
            .Distinct()
            .ToArray();
        var secondLevel = regionIdentifiers.Select( word => word.Split('|').Take(2).ToArray())
            .Where( parts => parts.Length > 1 && parts[1].Length<=3 &&  parts[1].All(c => char.IsUpper(c)))
            .Select( parts => string.Join('|',parts))
            .Distinct()
            .ToArray();

        var regions = 
                topLevel.ToImmutableSortedSet().Union(
                secondLevel.ToImmutableSortedSet());

        
        var trie = new PatriciaTrie();
        regions.Select( (word, index) => KeyValuePair.Create(word,index)).ForEach( kvp => trie.Insert( kvp.Key, kvp.Value));

        // Serialze the stations with Protobuf format
        // https://developers.google.com/protocol-buffers/docs/csharptutorial
        var protobuf = ToProtobuf(stations, gpsLookUp, trie);
        using (var output = File.Create(Path.Combine(Environment.CurrentDirectory, "../data/", "stations.dat")))
        {
            protobuf.WriteTo(output);
        }
    }

    public static Option<Station> ToStation(string line)
    {
        string pattern =
            // https://regex101.com/r/cR0wvF/1
            // https://regex101.com/r/bmZH6u/1
            @"^(\d{5})(.{7})(.{5})\s(\-{4}|[A-Z]{4})\s(.{20})\s([\d.\s]{6})\s([\d.\-\s]{7})\s([\d\-\s]{5})\s(.{6})\s(.{4}).*$";

        Match m = Regex.Match(line, pattern);
        if (m.Success)
        {
            try
            {
                return new Station(
                    Convert.ToInt32(m.Groups[1].Value),
                    m.Groups[3].Value.Trim(),
                    m.Groups[5].Value.Trim(),
                    new Coordinate(
                        ValueConverter.ToDecimal(m.Groups[6].Value),
                        ValueConverter.ToDecimal(m.Groups[7].Value),
                        ValueConverter.ToDecimal(m.Groups[8].Value)),
                    ToArea(m.Groups[10].Value));
            }
            catch (FormatException)
            {
                return None;
            }
        }

        return None;
    }


    private static Area ToArea(string area) => area switch
    {
        "KUES" => Area.Coast,
        "BERG" => Area.Mountain,
        "MEER" => Area.Ocean,
        "LAND" => Area.Land,
        _ => Area.Land
    };

    private static PbStation ToProtobuf(Station station, IDictionary<Coordinate, string> gpsLookUp, PatriciaTrie trie)
    {
        var lookUpCoord = new Coordinate(station.Location.Latitude, station.Location.Longitude, 0m);

        var foundName = gpsLookUp.TryGetValue(lookUpCoord, out var name);

        var country = name?.Split('|').First();
        var parts = name?.Split('|').Where( parts => parts.Length >= 2 ).Take(2) ?? Enumerable.Empty<string>();
        var state = parts.Count() == 2 ? string.Join('|', parts) : null; 

        var countryId = !string.IsNullOrEmpty(country) ? trie.FindValue(country!) : null;
        var stateId = !string.IsNullOrEmpty(state) ? trie.FindValue(state!) : null;

        return new PbStation()
        {
            Clu = station.Clu,
            Id = station.Id,
            Name = station.Name,
            Location = new PbStation.Types.Coordinate()
            {
                Lat = ToDecimal(station.Location.Latitude),
                Lon = ToDecimal(station.Location.Longitude),
                Alt = Convert.ToInt32(station.Location.Altitude)
            },
            Area = (PbStation.Types.Area)(int)station.Area, 

            // Referencing position in the trie 
            CountryId = countryId, 
            StateId =  stateId
        };
    }

    public static float ToDecimal(decimal coord)
    {
        decimal hours = (int)coord; // Die Stunden sind der ganzzahlige Teil des Werts
        decimal minutes = (coord - hours) * 100; // Die Minuten sind der Rest des Werts, multipliziert mit 100
        return Convert.ToSingle(hours + minutes / 60);
    }


    private static PbStationRegistry ToProtobuf(IEnumerable<Station> stations, IDictionary<Coordinate, string> gpsLookUp,  PatriciaTrie trie)
    {
        var ret = new PbStationRegistry() { 
            LastUpdated = DateTime.UtcNow.ToTimestamp(),
            CountryStateTrie = trie.ToString() };
        ret.Stations.AddRange(stations
            .Select(x => ToProtobuf(x,gpsLookUp, trie))
            .OrderBy( x => x.CountryId)
            .ThenBy(x => x.StateId));
        return ret;
    }

    private static void WriteGeoCodingCsv(IEnumerable<Coordinate> locations, string fileName)
    {
        using (var writer = new StreamWriter(fileName))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<CoordinateMap>();
            csv.WriteRecords(locations);
        }

    }

    public class CoordinateMap : ClassMap<Coordinate>
    {
        public CoordinateMap()
        {
            Map(m => m.Latitude).Index(0).Name("lat");
            Map(m => m.Longitude).Index(1).Name("lon");
            Map(m => m.Altitude).Ignore();
        }
    }
}