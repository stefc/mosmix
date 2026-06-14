/// Coordinate containing latitude, longitude, and optional altitude.
#[derive(Debug, Clone, PartialEq)]
pub struct Coordinate {
    pub lat: f32,
    pub lon: f32,
    pub alt: Option<i32>,
}

/// Station containing identifier, name, and location.
#[derive(Debug, Clone, PartialEq)]
pub struct Station {
    pub id: String,
    pub name: String,
    pub location: Coordinate,
}
