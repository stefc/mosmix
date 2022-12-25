using Xunit;

namespace stefc.mosmix.tests; 

public class MosmixStationsRegistryTest {

    
    [Fact]
    public void RegistryGetAllTest() {

        var stations = MosmixStationRegistry.GetAll();
        Assert.Equal(5586, stations.Count());

        var wittenborn = stations.Single( s => s.Id == "A762");
        Assert.Equal("WITTENBORN", wittenborn.Name);
        Assert.Equal(Area.Land, wittenborn.Area);
        Assert.Equal(new Coordinate(Latitude: 53.91667m, Longitude: 10.23333m, Altitude: 38.0m), 
            wittenborn.Location); 

        Assert.Equal(69, (int)wittenborn.CountryId!);
        Assert.Equal(78, (int)wittenborn.StateId!);
    }

    [Fact]
    public void RegistryGetPatriciaTrie() {

        var trie = MosmixStationRegistry.GetCountryStatePatriciaTrieRaw();

        Assert.NotNull(trie);
        Assert.True(trie.Length > 2000 && trie.Length < 10_000);
    }
}