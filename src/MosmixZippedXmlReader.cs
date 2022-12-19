using System.IO.Compression;

namespace stefc.mosmix;

internal class MosmixZippedXmlReader : IMosmixReader
{
    private readonly IMosmixReader reader;

    internal MosmixZippedXmlReader() => this.reader = new MosmixXmlReader();

    public Document Read(Stream data)
    {
        using var archive = new ZipArchive(data, ZipArchiveMode.Read);
        var kmlFile = archive.Entries.First(e => e.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase));

        using var stream = kmlFile.Open();
        var document = this.reader.Read(stream);
        return document;
    }
}