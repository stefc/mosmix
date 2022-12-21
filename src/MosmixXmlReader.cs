using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace stefc.mosmix;

internal class MosmixXmlReader : IMosmixReader
{

    private readonly XNamespace dwdNs;
    private readonly XNamespace kmlNs;

    public MosmixXmlReader()
    {
        this.dwdNs = "https://opendata.dwd.de/weather/lib/pointforecast_dwd_extension_V1_0.xsd";
        this.kmlNs = "http://www.opengis.net/kml/2.2";
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public Document Read(Stream data)
    {
        var xmlNameTable = new NameTable();
        var document = new Document();
        var productName = xmlNameTable.Add("dwd:ProductDefinition");
        var placeMark = xmlNameTable.Add("kml:Placemark");
        using (var xmlReader = XmlReader.Create(data, this.CreateReaderSettings(xmlNameTable)))
        {
            xmlReader.MoveToContent();
            while (!xmlReader.EOF)
            {
                if (xmlReader.NodeType == XmlNodeType.Element && (ReferenceEquals(xmlReader.Name, productName) ))
                    document = document with { Definition = ReadProductDefinition((XElement)XNode.ReadFrom(xmlReader)) };
                else if (xmlReader.NodeType == XmlNodeType.Element && (ReferenceEquals(xmlReader.Name, placeMark) ))
                    document = document with { PlaceMark = ReadPlaceMark((XElement)XNode.ReadFrom(xmlReader)) };
                else
                    xmlReader.Read();
            }
        }

        return document;
    }

    private ProductDefinition ReadProductDefinition(XElement element) {
        var issuer = element.Descendants(this.dwdNs.GetName("Issuer")).SingleOrDefault()?.Value ?? string.Empty;
        var id = element.Descendants(this.dwdNs.GetName("ProductID")).SingleOrDefault()?.Value ?? string.Empty;
        var issued = ValueConverter.ToDate(element.Descendants(this.dwdNs.GetName("IssueTime")).SingleOrDefault()?.Value ?? string.Empty);
        var timeSteps = ReadForecastTimeSteps(element.Descendants(this.dwdNs.GetName("ForecastTimeSteps")).SingleOrDefault());
        return new ProductDefinition(issuer, id, issued, timeSteps);
    }

    private PlaceMark ReadPlaceMark(XElement element) {
        var name = element.Descendants(this.kmlNs.GetName("name")).SingleOrDefault()?.Value ?? string.Empty;
        var description = element.Descendants(this.kmlNs.GetName("description")).SingleOrDefault()?.Value ?? string.Empty;
        var point = ReadPoint(element.Descendants(this.kmlNs.GetName("Point")).SingleOrDefault());
        var timeSeries = ReadTimeSeries(element.Descendants(this.dwdNs.GetName("Forecast")));
        return new PlaceMark(name, description, point, timeSeries);
    }

    private IEnumerable<DateTime> ReadForecastTimeSteps(XElement? element) {

        var steps = element?
            .Descendants(this.dwdNs.GetName("TimeStep"))
            .Select( xe => xe.Value)
            .Select(ValueConverter.ToDateExact)
            .ToImmutableArray();

        return steps ?? Enumerable.Empty<DateTime>();
    }

    private Coordinate? ReadPoint(XElement? element) {
        var coords = element?.Descendants(this.kmlNs.GetName("coordinates")).SingleOrDefault()?.Value ?? string.Empty;
        var parts = coords?
            .Split(',')
            .Select(ValueConverter.ToDecimal)
            .ToArray();

        return parts != null ? new Coordinate(parts[1], parts[0], parts[2]) : null;
    }

    private IDictionary<string,double?[]> ReadTimeSeries(IEnumerable<XElement> elements) {
        return elements.ToDictionary( 
            element => element.Attribute(this.dwdNs.GetName("elementName"))?.Value ?? string.Empty, 
            element => ToTimeSeries(element.Value)
        );
    }

    private XmlReaderSettings CreateReaderSettings(NameTable nameTable)
        => new XmlReaderSettings { IgnoreWhitespace = true, NameTable = nameTable };

    private double?[] ToTimeSeries(string line) 
    {
        string pattern =  // https://regex101.com/r/cR0wvF/1
            @"\s*(-|[\d.]{1,11})\s*";

		var matches = Regex.Matches(line, pattern);

        var x = matches.Select( m => ToDouble(m.Groups[1].Value)).ToArray();
        return x;
    }

    private static double? ToDouble(string s) {
        if (s=="-") return null;
        return Convert.ToDouble(s.Trim(), new CultureInfo("en-US"));
    }
}