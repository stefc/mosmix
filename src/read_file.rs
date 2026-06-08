use encoding_rs::WINDOWS_1252;
use std::fs::File;
use std::io::Read;
use std::path::Path;

pub fn read_win_1252_file(path: &Path) -> Result<String, std::io::Error> {
    let mut file = File::open(path)?;
    let mut buffer = Vec::new();
    file.read_to_end(&mut buffer)?;
    let (cow_str, _encoding_used, _had_errors) = WINDOWS_1252.decode(&buffer);
    Ok(cow_str.into_owned())
}
