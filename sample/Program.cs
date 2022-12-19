
using System.Diagnostics;
using stefc.mosmix;

internal class Program
{
    private static void Main(string[] args)
    {
        var path = Path.GetDirectoryName(typeof(Program).Assembly?.Location ) ?? string.Empty;
        using (var stream = File.Open(Path.Combine(path, "MOSMIX_A762.kmz"), FileMode.Open))
        {
            var reader = MosmixReaderFactory.CreateForKmz(stream);
            var document = reader.Read(stream);
            System.Console.WriteLine($"Issuer:{document.Definition.Issuer}");
            System.Console.WriteLine($"Name:{document.PlaceMark.Name}");
            System.Console.WriteLine($"Description:{document.PlaceMark.Description}");
        }
    }
}