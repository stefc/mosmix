## Mosmix Reading Library 

With this nuget package you can easy read MOSMIX data came from weather stations around the world. 

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

To see for a format description habe a look on (German)

(Mosmix)[https://www.dwd.de/DE/forschung/wettervorhersage/met_fachverfahren/nwv_anschlussverfahren/mosmix_verfahren_node.html]


To see a list of the five thousand weather stations worldwide see: 

(List of Mosmix stations)[https://www.dwd.de/EN/ourservices/met_application_mosmix/mosmix_stations.html]



