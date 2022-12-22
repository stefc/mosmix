using System.Text.RegularExpressions;
using LanguageExt;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

using stefc.mosmix;

using PbStation = stefc.mosmix.V1.Station;
using PbStationRegistry = stefc.mosmix.V1.StationRegistry;

using static LanguageExt.Prelude;

internal static class Program
{
    private static void Main(string[] args)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "../data/", "st.cfg");
        System.Console.WriteLine(path);
        
        var stations = File.ReadAllLines(path)
            .Where(line => !String.IsNullOrEmpty(line))
            .SelectMany(x => ToStation(x))
            .ToArray();

        // Serialze the stations with Protobuf format
        // https://developers.google.com/protocol-buffers/docs/csharptutorial
        var protobuf = ToProtobuf(stations);
        using (var output = File.Create( Path.Combine(Environment.CurrentDirectory, "../data/", "stations.dat")))
        {
            protobuf.WriteTo(output);
        }
    }
    
    public static Option<Station> ToStation(string line)
    {
        string pattern =
            // https://regex101.com/r/cR0wvF/1
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

    private static PbStation ToProtobuf(Station station)  {
        return new PbStation() {
            Clu = station.Clu,
            Id = station.Id, 
            Name = station.Name, 
            Location = new PbStation.Types.Coordinate() {
                Lat = Convert.ToSingle(station.Location.Latitude), 
                Lon = Convert.ToSingle(station.Location.Longitude),
                Alt = Convert.ToInt32(station.Location.Altitude)
            }, 
            Area = (PbStation.Types.Area)(int)station.Area};
    }

    private static PbStationRegistry ToProtobuf(IEnumerable<Station> stations) {
        var ret = new PbStationRegistry() { LastUpdated = DateTime.UtcNow.ToTimestamp()};
        ret.Stations.AddRange(stations.Select(ToProtobuf));
        return ret;
    }
}