using System.Collections.Immutable;
using System.Globalization;
using CsvHelper;
using stefc.mosmix;

internal  class ReadRegionIdentifiersCsv {


    public ReadRegionIdentifiersCsv(string fileName) => this.FileName = fileName;

    public string FileName { get; }

    public IDictionary<Coordinate, string> ReadGpsLookUp() {

        var coordType = new
        {
            lat = default(decimal),
            lon = default(decimal),
            name = default(string)
        };

        var source = ImmutableDictionary<Coordinate, string>.Empty;

        using (var reader = new StreamReader(this.FileName))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords(coordType);
            source = records.Aggregate(source, (acc, cur) => acc.Add(new Coordinate(cur.lat,cur.lon,0m), cur.name));
        }
        return source;
    }

    public IEnumerable<string> ReadIdsOnly() {
        
        var coordType = new
        {
            lat = default(double),
            lon = default(double),
            name = default(string)
        };

        var source = ImmutableHashSet<string>.Empty;

        using (var reader = new StreamReader(this.FileName))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords(coordType);
            source = records.Aggregate(source, (acc, cur) => acc.Add(cur.name ?? string.Empty));
        }

        return source; 
    }
}