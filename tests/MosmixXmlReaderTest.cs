using Xunit;

namespace stefc.mosmix.tests;

public class MosmixXmlReaderTest
{
    

    [Fact]
    public void ImportKmlTest()
    {
        using (var stream = File.Open("MOSMIX_A762.kml", FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var reader = MosmixReaderFactory.CreateForKml(stream);

            var document = reader.Read(stream);

            Assert.Equal("Deutscher Wetterdienst", document.Definition.Issuer);
            Assert.Equal("MOSMIX", document.Definition.Id);
            Assert.Equal(new DateTime(2021,12,20,21,0,0), document.Definition.IssueTime);
            Assert.Equal(247, document.Definition.TimeSteps.Count());
            Assert.Equal(new DateTime(2021,12,20,22,0,0), document.Definition.TimeSteps.First());
            Assert.Equal(new DateTime(2021,12,31,04,0,0), document.Definition.TimeSteps.Last());
            
            Assert.Equal("A762", document.PlaceMark.Name);
            Assert.Equal("WITTENBORN", document.PlaceMark.Description);
            Assert.Equal(new Coordinate(10.23m,53.92m,38.0m), document.PlaceMark.Coordinate);
            Assert.Equal(114, document.PlaceMark.Forecasts.Count);
        }
    }

    [Fact]
    public void ImportKmzTest()
    {
        using (var stream = File.Open("MOSMIX_A762.kmz", FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var reader = MosmixReaderFactory.CreateForKmz(stream);

            var document = reader.Read(stream);

            Assert.Equal("Deutscher Wetterdienst", document.Definition.Issuer);
            Assert.Equal("MOSMIX", document.Definition.Id);
            Assert.Equal(new DateTime(2021,12,20,21,0,0), document.Definition.IssueTime);
            Assert.Equal(247, document.Definition.TimeSteps.Count());
            Assert.Equal(new DateTime(2021,12,20,22,0,0), document.Definition.TimeSteps.First());
            Assert.Equal(new DateTime(2021,12,31,04,0,0), document.Definition.TimeSteps.Last());
            
            Assert.Equal("A762", document.PlaceMark.Name);
            Assert.Equal("WITTENBORN", document.PlaceMark.Description);
            Assert.Equal(new Coordinate(10.23m,53.92m,38.0m), document.PlaceMark.Coordinate);
            Assert.Equal(114, document.PlaceMark.Forecasts.Count);
        }
    } 
}