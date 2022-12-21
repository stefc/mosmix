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
    }

    [Fact]
    public void TemperatureTest() {

        var adapter = new ForecastAdapter(this.document.PlaceMark.Forecasts);

        Assert.Equal(-2.1f, adapter.Temperature.First());
        Assert.Equal(-3.6f, adapter.MinTemperature.Min());
        Assert.Equal(6.8f, adapter.MaxTemperature.Max());
        Assert.Equal(-1.5f, adapter.MeanTemperature.Min());
        Assert.Equal(4f, adapter.MeanTemperature.Max());
        Assert.Equal(-0.85d, (double)adapter.DewPoint.Average()!, 2);
    }

    [Fact]
    public void WindTest() {

        var adapter = new ForecastAdapter(this.document.PlaceMark.Forecasts);

        Assert.Equal(327u, adapter.WindDirection.First());
        Assert.Equal(1.03f, adapter.WindSpeed.First());
        Assert.Equal(2.06f, adapter.WindSpeed1h.First());
        Assert.Equal(2.57f, adapter.WindSpeed3h.First());
        Assert.Equal(3.09f, adapter.WindSpeed12h.OfType<Single>().First());
    }
}