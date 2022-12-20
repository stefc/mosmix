
using System.Collections.Immutable;
using System.Globalization;

namespace stefc.mosmix;

public class ForecastAdapter 
{
    private readonly IDictionary<string, double?[]> forecasts;

    private readonly Lazy<uint?[]> surfacePressure;
    private readonly Lazy<float?[]> temperature;
    private readonly Lazy<float?[]> minTemperature;
    private readonly Lazy<float?[]> maxTemperature;
    private readonly Lazy<float?[]> meanTemperature;

    public ForecastAdapter(IDictionary<string, double?[]> forecasts)
    {
        this.forecasts = forecasts;
        this.surfacePressure = new Lazy<uint?[]>(() => this.forecasts["PPPP"].Select(ToUInt).ToArray());
        this.temperature = new Lazy<float?[]>(() => this.forecasts["TTT"].Select(ToCelcius).ToArray());
        this.minTemperature = new Lazy<float?[]>(() => this.forecasts["TN"].Select(ToCelcius).ToArray());
        this.maxTemperature = new Lazy<float?[]>(() => this.forecasts["TX"].Select(ToCelcius).ToArray());
        this.meanTemperature = new Lazy<float?[]>(() => this.forecasts["TM"].Select(ToCelcius).ToArray());
    }

    // Sureface Pressure in pA
    public uint?[] SurfacePressure => this.surfacePressure.Value;

    // Temperature's in Celcius °
    public float?[] Temperature => this.temperature.Value;
    public float?[] MinTemperature => this.minTemperature.Value;
    public float?[] MaxTemperature => this.maxTemperature.Value;
    public float?[] MeanTemperature => this.meanTemperature.Value;

    private uint? ToUInt(double? x) => (uint?)x;
    private float? ToCelcius(double? x) => x != null ? Convert.ToSingle(x-273.15) : null;
   
}
