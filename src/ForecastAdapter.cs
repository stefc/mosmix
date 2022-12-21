namespace stefc.mosmix;

using Pa = Nullable<System.UInt32>; 
using hPa = Nullable<System.Single>; 
using Angle = Nullable<System.UInt32>; 
using Temp = Nullable<System.Single>;
using Speed = Nullable<System.Single>;

public class ForecastAdapter 
{
    private readonly IDictionary<string, double?[]> forecasts;

    private readonly Lazy<Pa[]> surfacePressure;
    private readonly Lazy<hPa[]> absErrorSurfacePressure;
    private readonly Lazy<Temp[]> temperature;
    private readonly Lazy<Temp[]> absErrorTemperature;
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
        this.surfacePressure = this.CreateTransform("PPPP", ToPa);
        this.absErrorSurfacePressure = this.CreateTransform("E_PPP",TohPa);
        this.temperature = this.CreateTransform("TTT",ToTemp);
        this.absErrorTemperature = this.CreateTransform("E_TTT",ToTempAbs);
        this.minTemperature = this.CreateTransform("TN",ToTemp);
        this.maxTemperature = this.CreateTransform("TX",ToTemp);
        this.meanTemperature = this.CreateTransform("TM",ToTemp);
        this.dewPoint = this.CreateTransform("Td",ToTemp);
        this.windDirection = this.CreateTransform("DD",ToAngle);
        this.windSpeed = this.CreateTransform("FF",ToSpeed);
        this.windSpeed1h = this.CreateTransform("FX1",ToSpeed);
        this.windSpeed3h = this.CreateTransform("FX3",ToSpeed);
        this.windSpeed12h = this.CreateTransform("FXh",ToSpeed); 
    }

    private Lazy<T[]> CreateTransform<T>(string elementName, Func<double?, T> transform) 
    => new Lazy<T[]>(() => this.forecasts[elementName].Select(transform).ToArray());

    // Surface Pressure in Pa & Abs. Error in hPa
    public Pa[] SurfacePressure => this.surfacePressure.Value;
    public hPa[] AbsErrorSurfacePressure => this.absErrorSurfacePressure.Value;

    // Temperature's in Celcius °
    public Temp[] Temperature => this.temperature.Value;
    public Temp[] AbsErrorTemperature => this.absErrorTemperature.Value;
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
    private hPa TohPa(double? x) => x != null ? Convert.ToSingle(x/100.0) : null;

    // Degree ° 0..360
    private Angle ToAngle(double? x) => (Angle)x;

    // Temperature ° Celcius
    private Temp ToTemp(double? x) => x != null ? Convert.ToSingle(x-273.15) : null;
    private Temp ToTempAbs(double? x) => x != null ? Convert.ToSingle(x) : null;

    // Windspeed m/s 
    private Speed ToSpeed(double? x) => x != null ? Convert.ToSingle(x) : null;
}
