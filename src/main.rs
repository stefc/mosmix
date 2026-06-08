fn main() {
    println!("Hello, world!");
}

#[cfg(test)]
mod tests {
    use chrono::{Datelike, TimeZone, Timelike, Utc};

    #[test]
    fn truncate_year_works() {

        let dt = Utc.with_ymd_and_hms(2026, 4, 3, 12, 2, 34).unwrap();

        assert_eq!(dt.year(), 2026);
        assert_eq!(dt.month(), 4);
        assert_eq!(dt.day(), 3);
        assert_eq!(dt.hour(), 12);
        assert_eq!(dt.minute(), 2);
        assert_eq!(dt.second(), 34);
    }
}

