using Xunit;

namespace stefc.mosmix.tests; 

public class ForecastAdapterTest {


    private readonly Document document; 
    
    public ForecastAdapterTest()
    {
        using var stream = File.Open("MOSMIX_A762.kml", FileMode.Open, FileAccess.Read, FileShare.Read);
        var reader = MosmixReaderFactory.CreateForKml(stream);
        this.document = reader.Read(stream);
    }

    [Fact]
    public void SurfacePressureTest() {

        var adapter = new ForecastAdapter(this.document.PlaceMark.Forecasts);

        Assert.Equal(102860u, adapter.SurfacePressure[0]);
        Assert.Equal(102840u, adapter.SurfacePressure[1]);
        Assert.Equal(-2.1f, adapter.Temperature.First());
        Assert.Equal(-3.6f, adapter.MinTemperature.Min());
        Assert.Equal(6.8f, adapter.MaxTemperature.Max());
    }


}