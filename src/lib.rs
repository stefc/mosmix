use dto::Station;

pub(crate) mod date_time_ext;

mod gen_protobuf {
    pub mod stations;
}

mod dto;
mod geo_coord;
pub mod tests;

const STATIONS_DATA: &[u8] = include_bytes!("../data/stations.dat");

/// Retrieve all stations from the embedded stations data registry.
pub fn get_stations() -> Result<Vec<Station>, std::io::Error> {
    use crate::gen_protobuf::stations::StationRegistry;
    use dto::Coordinate;
    use protobuf::Message;

    let registry = StationRegistry::parse_from_bytes(STATIONS_DATA)
        .map_err(|e| std::io::Error::new(std::io::ErrorKind::Other, e))?;
    let stations = registry
        .stations
        .into_iter()
        .map(|station| {
            let location = Coordinate {
                lat: station.location.lat,
                lon: station.location.lon,
                alt: station.location.alt,
            };
            Station {
                id: station.id,
                name: station.name,
                location,
            }
        })
        .collect();
    Ok(stations)
}
