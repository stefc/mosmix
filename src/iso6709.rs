use crate::geo_coord::LatLon;

pub fn format_iso6709(lat: f32, lon: f32) -> String {
    format!("{:+06.2}{:+07.2}/", lat, lon)
}

pub fn parse_iso6709(coord: &str) -> Result<LatLon, &'static str> {
    let coord = coord.trim_end_matches('/');
    if coord.len() < 2 {
        return Err("Coordinate string too short");
    }

    let lon_start = coord[1..]
        .find(|c| c == '+' || c == '-')
        .map(|idx| idx + 1) // Offset by 1 since we skipped the first character
        .ok_or("Missing longitude sign")?;

    let lon_end = coord[lon_start + 1..]
        .find(|c| c == '+' || c == '-')
        .map(|idx| lon_start + 1 + idx)
        .unwrap_or(coord.len());

    let lat = coord[..lon_start]
        .parse::<f32>()
        .map_err(|_| "Failed to parse latitude")?;
    let lon = coord[lon_start..lon_end]
        .parse::<f32>()
        .map_err(|_| "Failed to parse longitude")?;

    Ok(LatLon { lat, lon })
}
