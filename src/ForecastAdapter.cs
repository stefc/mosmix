namespace stefc.mosmix;

using Pa = Nullable<System.UInt32>; 
using Angle = Nullable<System.UInt32>; 
using Temp = Nullable<System.Single>;
using Speed = Nullable<System.Single>;

public class ForecastAdapter 
{
    private readonly IDictionary<string, double?[]> forecasts;

    private readonly Lazy<Pa[]> surfacePressure;
    private readonly Lazy<Temp[]> temperature;
    private readonly Lazy<Temp[]> minTemperature;
    private readonly Lazy<Temp[]> maxTemperature;
    private readonly Lazy<Temp[]> meanTemperature;
    private readonly Lazy<Temp[]> dewPoint;
    private readonly Lazy<Angle[]> windDirection;
    private readonly Lazy<Speed[]> windSpeed;
    private readonly Lazy<Speed[]> windSpeed1h;
    private readonly Lazy<Speed[]> windSpeed3h;
    private readonly Lazy<Speed[]> windSpeed12h;

    public ForecastAdapter(IDictionary<string, double?[]> forecasts)
    {
        this.forecasts = forecasts;
        this.surfacePressure = new Lazy<Pa[]>(() => this.forecasts["PPPP"].Select(ToPa).ToArray());
        this.temperature = new Lazy<Temp[]>(() => this.forecasts["TTT"].Select(ToTemp).ToArray());
        this.minTemperature = new Lazy<Temp[]>(() => this.forecasts["TN"].Select(ToTemp).ToArray());
        this.maxTemperature = new Lazy<Temp[]>(() => this.forecasts["TX"].Select(ToTemp).ToArray());
        this.meanTemperature = new Lazy<Temp[]>(() => this.forecasts["TM"].Select(ToTemp).ToArray());
        this.dewPoint = new Lazy<Temp[]>(() => this.forecasts["Td"].Select(ToTemp).ToArray());
        this.windDirection = new Lazy<Angle[]>(() => this.forecasts["DD"].Select(ToAngle).ToArray());
        this.windSpeed = new Lazy<Speed[]>(() => this.forecasts["FF"].Select(ToSpeed).ToArray());
        this.windSpeed1h = new Lazy<Speed[]>(() => this.forecasts["FX1"].Select(ToSpeed).ToArray());
        this.windSpeed3h = new Lazy<Speed[]>(() => this.forecasts["FX3"].Select(ToSpeed).ToArray());
        this.windSpeed12h = new Lazy<Speed[]>(() => this.forecasts["FXh"].Select(ToSpeed).ToArray()); 

    }

    // Sureface Pressure in pA
    public Pa[] SurfacePressure => this.surfacePressure.Value;

    // Temperature's in Celcius °
    public Temp[] Temperature => this.temperature.Value;
    public Temp[] MinTemperature => this.minTemperature.Value;
    public Temp[] MaxTemperature => this.maxTemperature.Value;
    public Temp[] MeanTemperature => this.meanTemperature.Value;
    public Temp[] DewPoint => this.dewPoint.Value;

    // Wind Direction in 0..360°
    public Angle[] WindDirection => this.windDirection.Value;

    // Wind Speed m/s 
    public Speed[] WindSpeed => this.windSpeed.Value;
    public Speed[] WindSpeed1h => this.windSpeed1h.Value;
    public Speed[] WindSpeed3h => this.windSpeed3h.Value;
    public Speed[] WindSpeed12h => this.windSpeed12h.Value;

    // Pressure Pa
    private Pa ToPa(double? x) => (Pa)x;

    // Degree ° 0..360
    private Angle ToAngle(double? x) => (Angle)x;

    // Temperature ° Celcius
    private Temp ToTemp(double? x) => x != null ? Convert.ToSingle(x-273.15) : null;

    // Windspeed m/s 
    private Speed ToSpeed(double? x) => x != null ? Convert.ToSingle(x) : null;
}
