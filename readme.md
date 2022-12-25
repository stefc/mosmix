## Mosmix Reading Library 

With this nuget package you can easy read MOSMIX data came from weather stations around the world. 
So if your service depends on weather data so why not read it out directly from your available weather stations

Have a look in the ```sample``` folder to see how to use it. 

Add a package reference to your ```csproj``` like the following 

````xml
<ItemGroup>
    <PackageReference Include="stefc.mosmix" Version="0.2.0" />
</ItemGroup>
````

After this you can create an ```IMosmixReader``` for ```kml``` or ```kmz``` files and read the 
complete weather station document out of it. 

````csharp
using (var stream = File.Open(Path.Combine(path, "MOSMIX_A762.kmz"), FileMode.Open))
{
    var reader = MosmixReaderFactory.CreateForKmz(stream);
    var document = reader.Read(stream);
    System.Console.WriteLine($"Issuer:{document.Definition.Issuer}");
    System.Console.WriteLine($"Name:{document.PlaceMark.Name}");
    System.Console.WriteLine($"Description:{document.PlaceMark.Description}");
}
````

For easy use the lib gives you also a class `ForecastAdapter` that gives you typed access to the raw
timeseries values. The following figures at the moment available [^1] : 

 Name              | Elementname   | Description                                       | Unit               
---|---|---|--- 
 SurfacePressure   | `PPPP`        | Surface pressure                                  | Pa                
 Temperature       | `TTT`         | Temperature 2m above surface                      | Celcius Degree    
 MaxTemperature    | `TX`          | Maximum temperature - within the last 12 hours    | Celcius Degree    
 MinTemperature    | `TN`          | Minimum temperature - within the last 12 hours    | Celcius Degree    
 DewPoint          | `Td`          | Dewpoint 2m above surface                         | Celcius Degree    
 WindDirection     | `DD`          | Wind Direction                                    | 0..360°           
 WindSpeed         | `FF`          | Wind Speed                                        | m/sec             
 WindSpeed1h       | `FX1`         | Maximum wind gust within the last hour            | m/sec             
 WindSpeed3h       | `FX3`         | Maximum wind gust within the last 3 hours         | m/sec             
 WindSpeed12h      | `FXh`         | Maximum wind gust within the last 12 hours        | m/sec             



[^1]: If you need more figures please entry an issuee or even better create a pull request.

To see for a format description habe a look on (German)

[Mosmix](https://www.dwd.de/DE/forschung/wettervorhersage/met_fachverfahren/nwv_anschlussverfahren/mosmix_verfahren_node.html)


To see a list of the five thousand weather stations worldwide see: 

[List of Mosmix stations](https://www.dwd.de/EN/ourservices/met_application_mosmix/mosmix_stations.html)

In the Library a list of all Mosmix stations is staticly embedded and can be accessed with the 
two methods above: 

````csharp
var stations = MosmixStationRegistry.GetAll();
var trie = MosmixStationRegistry.GetCountryStatePatriciaTrieRaw();
````

The first call retrieve a list of all Mosmix stations and the second a so called Patricia-Tree 
which contains all the referenced Countries and the belonging States of each Country. If available 
a station reference a index into this trie with the Properties `CountryId` and `StateId`. For the moment 
you must parse the Patricia Trie by yourself. In the future I'll implement it also here in the library. 
