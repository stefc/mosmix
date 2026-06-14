use crate::geo_coord::LatLon;
use crate::iso6709;
use std::io::Error;

pub fn list_stations(geo: &Option<String>, _radius: &Option<i16>) -> Result<(), Error> {
    match mosmix::get_stations() {
        Ok(stations) => {
            if let Some(geo) = geo {
                let origin = iso6709::parse_iso6709(geo.as_str()).expect("invalid geo");
                let radius = _radius.unwrap_or(50);

                println!("id;name;location;distance");
                let mut filtered_stations: Vec<_> = stations
                    .into_iter()
                    .filter_map(|station| {
                        let target = LatLon { lat: station.location.lat, lon: station.location.lon };
                        let distance = calculate_distance_km(&origin, &target);
                        (distance <= radius as f32).then_some((station, distance))
                    })
                    .collect();

                filtered_stations.sort_by(|a, b| a.1.total_cmp(&b.1));
                for (station, distance) in filtered_stations {
                    let location = iso6709::format_iso6709(station.location.lat, station.location.lon);
                    println!("{};'{}';{};{:.0}", station.id, station.name, location, distance);
                }
            }
            else {
                println!("id;name;location");
                for station in stations {
                    let location = iso6709::format_iso6709(station.location.lat, station.location.lon);
                    println!("{};'{}';{}", station.id, station.name, location);
                }
            }
        },
        Err(e) => {
            // Falls die daten.bin keine gültigen Protobuf-Daten für dieses Struct enthält
            eprintln!("Fehler beim Parsen der Protobuf-Daten: {}", e);
        }
    }
    Ok(())
}

fn calculate_distance_km(p1: &LatLon, p2: &LatLon) -> f32 {
    const EARTH_RADIUS_KM: f32 = 6371.0;

    let d_lat = (p2.lat - p1.lat).to_radians();
    let d_lon = (p2.lon - p1.lon).to_radians();

    let lat1_rad = p1.lat.to_radians();
    let lat2_rad = p2.lat.to_radians();

    let a = (d_lat / 2.0).sin().powi(2) + lat1_rad.cos() * lat2_rad.cos() * (d_lon / 2.0).sin().powi(2);
    let c = 2.0 * a.sqrt().atan2((1.0 - a).sqrt());

    EARTH_RADIUS_KM * c
}
