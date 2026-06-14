use anyhow::{Context, Error, Result};
use chrono::Utc;
use protobuf::well_known_types::timestamp::Timestamp;
use protobuf::{EnumOrUnknown, Message, MessageField};
use std::io::{BufRead, BufReader};
use std::path::{Path, PathBuf};

use crate::gen_protobuf::stations::{Area, Coordinate, Station, StationRegistry};
use crate::read_file::read_win_1252_file;

pub fn convert_stations_file(input: &Path, output: &PathBuf) -> Result<(), Error> {
    let stations = parse_st_cfg(input)?;
    let timestamp = get_time_stamp();
    let registry = StationRegistry {
        stations,
        last_updated: MessageField::some(timestamp),
        ..StationRegistry::new()
    };

    let bytes = registry
        .write_to_bytes()
        .context("Failed to serialize protobuf message")?;

    std::fs::write(&output, bytes).context("Failed to create output file")?;
    Ok(())
}

fn get_time_stamp() -> Timestamp {
    let now = Utc::now();
    Timestamp {
        seconds: now.timestamp(),
        nanos: now.timestamp_subsec_nanos() as i32,
        ..Timestamp::new()
    }
}

fn slice_chars(s: &str, start: usize, end: usize) -> &str {
    let start_byte = s
        .char_indices()
        .nth(start)
        .map(|(i, _)| i)
        .unwrap_or(s.len());
    let end_byte = s.char_indices().nth(end).map(|(i, _)| i).unwrap_or(s.len());
    &s[start_byte..end_byte].trim()
}

fn parse_st_cfg(path: &Path) -> Result<Vec<Station>> {
    let content = read_win_1252_file(path).context("Failed to open file")?;
    let reader = BufReader::new(content.as_bytes());
    let mut stations = Vec::new();
    let mut line_no = 0;

    for line_result in reader.lines() {
        line_no += 1;
        let line = line_result?;

        if line.starts_with("TABLE")
            || line.starts_with("clu")
            || line.starts_with("==")
            || line.trim().is_empty()
        {
            continue;
        }

        let char_count = line.chars().count();
        if char_count < 76 {
            continue;
        }

        let clu = slice_chars(&line, 0, 5)
            .parse::<i32>()
            .with_context(|| format!("Failed to parse 'clu' on line ({}): {}", line_no, line))?;
        let id = slice_chars(&line, 12, 17).to_string();
        let name = slice_chars(&line, 23, 43).to_string();
        let lat = slice_chars(&line, 44, 50)
            .parse::<f32>()
            .with_context(|| format!("Failed to parse 'lat' on line ({}): {}", line_no, line))?;
        let lon = slice_chars(&line, 51, 58).parse::<f32>().with_context(|| {
            format!(
                "Failed to parse 'lon' on line ({}): {} ['{}']",
                line_no,
                line,
                slice_chars(&line, 51, 58).trim()
            )
        })?;
        let elev = slice_chars(&line, 59, 64)
            .parse::<i32>()
            .with_context(|| format!("Failed to parse 'elev' on line({}): {}", line_no, line))?;

        let station_type = slice_chars(&line, 72, char_count).to_string();
        let area = match station_type.as_str() {
            "LAND" => EnumOrUnknown::from(Area::LAND),
            "KUES" => EnumOrUnknown::from(Area::COAST),
            "MEER" => EnumOrUnknown::from(Area::OCEAN),
            "BERG" => EnumOrUnknown::from(Area::MOUNTAIN),
            _ => EnumOrUnknown::default(),
        };

        let coord = Coordinate {
            lon,
            lat,
            alt: Some(elev),
            ..Coordinate::new()
        };

        let station = Station {
            clu,
            id,
            name,
            location: MessageField::some(coord),
            area,
            ..Station::new()
        };

        stations.push(station);
    }

    Ok(stations)
}
