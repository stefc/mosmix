using Xunit;

namespace stefc.mosmix.tests; 

public class MosmixStationsRegistryTest {

    
    [Fact]
    public void RegistryGetAllTest() {

        var stations = MosmixStationRegistry.GetAll();
        Assert.Equal(4925, stations.Count());

        var wittenborn = stations.Single( s => s.Id == "A762");
        Assert.Equal("WITTENBORN", wittenborn.Name);
        Assert.Equal(Area.Land, wittenborn.Area);
        Assert.Equal(new Coordinate(Latitude: 53.55m, Longitude: 10.14m, Altitude: 38.0m), 
            wittenborn.Location); 
    }
}