use std::io::Error;

pub fn list_stations() -> Result<(), Error> {
    match mosmix::get_stations() {
        Ok(stations) => {
            println!("Erfolgreich geparst!");
            for station in stations {
                let location = format_iso6709(station.location.lat, station.location.lon);
                println!("{};'{}';{}", station.id, station.name, location);
            }
        },
        Err(e) => {
            // Falls die daten.bin keine gültigen Protobuf-Daten für dieses Struct enthält
            eprintln!("Fehler beim Parsen der Protobuf-Daten: {}", e);
        }
    }
    Ok(())
}
fn format_iso6709(lat: f32, lon: f32) -> String {
    format!("{:+06.2}{:+07.2}/", lat, lon)
}
