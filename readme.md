## Mosmix Library & CLI Tool 

## Command line tool `mosmix`

### Installation 

The following install the tool on your computer

`cargo install mosmix` 

###  Convert `st.cfg` into a binary protobuf file 

To embedd all ~6.000 mosmix station in the binary without consuming much memory
I transform it to a protobuf serialization format with a given scheme `stations.proto`
460Kb shrinks towards 250Kb.

This conversion you can do yourself in case a new `st.cfg` is available. 

```zsh
mosmix convert -i data/st.cfg data/stations.pb.bin
```

### List all stations 

You can list all mosmix stations of the library to the console by 
simple call : 

```zsh
mosmix list 
```
Sample output (`id;name;geo-location`): 
```csv
...
"E2834";"ROTES MEER";"+27.32+034.04/"
"E2835";"GOLF AKABA-S";"+28.25+034.35/"
"E6204";"SVINOY";"+62.20+004.23/"
"E6220";"BOTTENSEE";"+62.00+019.31/"
"E6300";"63NORD00OST";"+63.04+000.07/"
"E6319";"BOTTENSEE-NW";"+62.48+018.48/"
"E6407";"FROYABANKEN";"+64.00+007.00/"
"E6421";"QUARK";"+63.30+021.00/"
...
```

### Access the information from your app

`cargo add mosmix`

::further info to come

```rust
```

## Further information 
To see for a format description habe a look on (German)

(Mosmix)[https://www.dwd.de/DE/forschung/wettervorhersage/met_fachverfahren/nwv_anschlussverfahren/mosmix_verfahren_node.html]

To see a list of the five thousand weather stations worldwide see: 

(List of Mosmix stations)[https://www.dwd.de/EN/ourservices/met_application_mosmix/mosmix_stations.html]



