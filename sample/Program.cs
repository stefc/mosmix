using stefc.mosmix;

using Spectre.Console;
using Innovative.SolarCalculator;
using MoreLinq;
using System.Collections.Immutable;
using System.Numerics;

internal static class Program
{
    private static void Main(string[] args)
    {

        var stations = MosmixStationRegistry.GetAll();
        var wittenborn = stations.Single( s => s.Id == "A762");
        System.Console.WriteLine(wittenborn.Clu);

        var path = Path.GetDirectoryName(typeof(Program).Assembly?.Location) ?? string.Empty;
        using (var stream = File.Open(Path.Combine(path, "MOSMIX_A762.kmz"), FileMode.Open))
        {
            var reader = MosmixReaderFactory.CreateForKmz(stream);
            var document = reader.Read(stream);

            var startDate = document.Definition.IssueTime ?? DateTime.Today;
            var geolocation = document.PlaceMark.Coordinate;

            AnsiConsole.MarkupLine(startDate.ToShortDateString());
            AnsiConsole.MarkupLine($"Mosmix Station : {document.PlaceMark.Description} ([red]{document.PlaceMark.Name}[/])");
            AnsiConsole.MarkupLine($"Issuer: {document.Definition.Issuer}");

            var days = (document.Definition.TimeSteps.Last() - document.Definition.TimeSteps.First()).Days;

            var timeSteps = document.Definition.TimeSteps.ToArray();

            var timeSeries = new ForecastAdapter(document.PlaceMark.Forecasts);


            Enumerable.Range(1, days-1).Select(days => startDate.AddDays(days)).ForEach(date =>
            {

                var solarTimes = new SolarTimes(date, geolocation!.Latitude, geolocation!.Longitude);
                var sunrise = solarTimes.Sunrise.Truncate(Accuracy.Hour).ToLocalTime();
                var sunset = solarTimes.Sunset.AddHours(+1).Truncate(Accuracy.Hour).ToLocalTime();

                var caption = $"{date.ToString("dd.MM. dddd")} ({solarTimes.Sunrise.ToLocalTime().ToShortTimeString()}-{solarTimes.Sunset.ToLocalTime().ToShortTimeString()})";
                AnsiConsole.MarkupLine(caption);

                var indices = Enumerable.Range(0, (sunset - sunrise).Hours + 1)
                    .Select(x => sunrise.ToLocalTime().AddHours(x))
                    .Select(hour => KeyValuePair.Create(hour, Array.FindIndex(timeSteps, 0, ts => ts == hour)))
                    .Where(kvp => kvp.Value != -1)
                    .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value);
 
                var windVisualizer = new Visualizer<Single>() { Transform = x => x * 3.6f };
                var tempVisualizer = new Visualizer<Single>() { Colorize = x => x < 0.0 ? TrafficLight.Red : TrafficLight.None };
                var pressureVisualizer = new Visualizer<UInt32>() { Transform = x => x / 100 };
                var pressureErrorVisualizer = new Visualizer<Single>() { Transform = x => x / 100 };

                var table = FormatCaptions("Hour", "HH:mm", sunrise, sunset)
                    .AddValues("Pressure (hPa)", "#,##0", sunrise, sunset, indices, timeSeries.SurfacePressure,  pressureVisualizer)
                    .AddValues("Abs. Error Pressure (hPa)", "0.#", sunrise, sunset, indices, timeSeries.AbsErrorSurfacePressure,  pressureErrorVisualizer)
                    .AddValues("Temp (°C)", "0.#", sunrise, sunset, indices, timeSeries.Temperature, tempVisualizer)
                    .AddValues("Abs. Error Temp (°C)", "0.#", sunrise, sunset, indices, timeSeries.AbsErrorTemperature)
                    .AddValues("Wind Direction (°)", "0", sunrise, sunset, indices, timeSeries.WindDirection)
                    .AddValues("Wind Speed (km/h)", "0", sunrise, sunset, indices, timeSeries.WindSpeed, windVisualizer);

                AnsiConsole.Write(table);

            });

        }

    }

    private static Table FormatCaptions(string label, string format, DateTime sunrise, DateTime sunset)
    {
        var table = new Table();
        table.AddColumn(label);
        var hour = sunrise;
        while (hour <= sunset)
        {
            table.AddColumn(new TableColumn(hour.ToString(format)).RightAligned());
            hour = hour.AddHours(1);
        }
        return table;
    }

    public enum TrafficLight { None, Green, Yellow, Red }

    private static string FormatValue(string value, TrafficLight color) => color switch
    {
        TrafficLight.Green => $"[green]{value}[/]",
        TrafficLight.Red => $"[red]{value}[/]",
        TrafficLight.Yellow => $"[yellow]{value}[/]",
        _ => value
    };

    private static Table AddValues<T>(this Table table, string label, string format, DateTime sunrise, DateTime sunset, 
        ImmutableDictionary<DateTime, int> indices, Nullable<T>[] forecast, Visualizer<T>? visualizer = null) where T : struct, INumber<T>
        {
            var hour = sunrise;

            var vis = visualizer ?? new Visualizer<T>();

            var columns = ImmutableArray<string>.Empty.Add(label); 
            while (hour <= sunset)
            {
                if (indices.TryGetValue(hour, out var idx))
                {
                    var value = forecast[idx];
                    if (value.HasValue) {
                        var v = vis.Transform(value.Value);
                        columns = columns.Add(FormatValue(vis.Formatter(v, format), vis.Colorize(v)));
                    }
                    else
                        columns = columns.Add("-");
                }
                else
                {
                    columns = columns.Add("-");
                }
                hour = hour.AddHours(1);
            }
            table.AddRow(columns.ToArray());
            return table;
        }

    class Visualizer<T> where T: struct {
                
         public Func<T,T> Transform  { get; internal init; } = x => x;

         public Func<T,TrafficLight> Colorize { get; internal init; } = _ => TrafficLight.None;

         public Func<T,string,string> Formatter { get; internal init; } = (v,fmt) => v switch {
            Double doubleValue => doubleValue.ToString(fmt), 
            Single floatValue => floatValue.ToString(fmt), 
            UInt32 intValue => intValue.ToString(fmt), 
            _ => v.ToString()!
         }; 
    }

}