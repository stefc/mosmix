#[cfg(test)]
mod test {
    use crate::date_time_ext::*;
    use chrono::{DateTime, Datelike, NaiveDate, NaiveDateTime, TimeZone, Timelike, Utc};

    #[test]
    fn trunc_year_naive_datetime_works() {
        let dt = create_naive_subject();

        let truncated_dt = dt.trunc_year();
        assert_eq!(truncated_dt.year(), 2026);
        assert_eq!(truncated_dt.month(), 1);
        assert_eq!(truncated_dt.day(), 1);
        assert_eq!(truncated_dt.hour(), 0);
        assert_eq!(truncated_dt.minute(), 0);
        assert_eq!(truncated_dt.second(), 0);

        let truncated = dt.trunc(Accuracy::Year).unwrap();
        assert_eq!(truncated, truncated_dt);
    }

    #[test]
    fn trunc_year_function_works() {
        let dt = create_subject();
        let truncated_dt = dt.trunc_year();

        assert_eq!(truncated_dt.year(), 2026);
        assert_eq!(truncated_dt.month(), 1);
        assert_eq!(truncated_dt.day(), 1);
        assert_eq!(truncated_dt.hour(), 0);
        assert_eq!(truncated_dt.minute(), 0);
        assert_eq!(truncated_dt.second(), 0);
        assert_eq!(truncated_dt.timezone(), dt.timezone());

        let truncated = dt.trunc(Accuracy::Year).unwrap();
        assert_eq!(truncated, truncated_dt);
    }

    #[test]
    fn trunc_month_function_works() {
        let dt = create_subject();
        let truncated_dt = dt.trunc_month();

        assert_eq!(truncated_dt.year(), 2026);
        assert_eq!(truncated_dt.month(), 4);
        assert_eq!(truncated_dt.day(), 1);
        assert_eq!(truncated_dt.hour(), 0);
        assert_eq!(truncated_dt.minute(), 0);
        assert_eq!(truncated_dt.second(), 0);
        assert_eq!(truncated_dt.timezone(), dt.timezone());

        let truncated = dt.trunc(Accuracy::Month).unwrap();
        assert_eq!(truncated, truncated_dt);
    }

    #[test]
    fn trunc_quarter_works() {
        let dt = create_subject(); // April (Month 4)
        let truncated_dt = dt.trunc_quarter();
        assert_eq!(truncated_dt.month(), 4); // Start of Q2

        let dt2 = Utc.with_ymd_and_hms(2026, 6, 3, 12, 2, 34).unwrap();
        assert_eq!(dt2.trunc_quarter().month(), 4);

        let dt3 = Utc.with_ymd_and_hms(2026, 11, 3, 12, 2, 34).unwrap();
        assert_eq!(dt3.trunc_quarter().month(), 10);
    }

    #[test]
    fn trunc_week_works() {
        let dt = Utc.with_ymd_and_hms(2023, 10, 26, 12, 0, 0).unwrap(); // Thursday
        let truncated_dt = dt.trunc_week();
        assert_eq!(truncated_dt.year(), 2023);
        assert_eq!(truncated_dt.month(), 10);
        assert_eq!(truncated_dt.day(), 23); // Monday
    }

    #[test]
    fn trunc_day_works() {
        let dt = create_subject();
        let truncated_dt = dt.trunc_day();
        assert_eq!(truncated_dt.hour(), 0);
        assert_eq!(truncated_dt.day(), 3);
    }

    #[test]
    fn trunc_hour_works() {
        let dt = create_subject();
        let truncated_dt = dt.trunc_hour();
        assert_eq!(truncated_dt.hour(), 12);
        assert_eq!(truncated_dt.minute(), 0);
    }

    fn create_subject() -> DateTime<Utc> {
        let dt = Utc.with_ymd_and_hms(2026, 4, 3, 12, 2, 34).unwrap();
        dt
    }

    fn create_naive_subject() -> NaiveDateTime {
        NaiveDate::from_ymd_opt(2026, 4, 3)
            .unwrap()
            .and_hms_opt(12, 2, 34)
            .unwrap()
    }

    #[test]
    fn test_get_stations_returns_stations() {
        let stations = crate::get_stations().unwrap();
        assert!(!stations.is_empty(), "Stations list should not be empty");
        
        // Assert on the first station to make sure properties are populated
        let first = &stations[0];
        assert!(!first.id.is_empty(), "Station ID should not be empty");
        assert!(!first.name.is_empty(), "Station Name should not be empty");
        // Verify lat/lon are within standard coordinate bounds
        assert!(first.location.lat >= -90.0 && first.location.lat <= 90.0);
        assert!(first.location.lon >= -180.0 && first.location.lon <= 180.0);
    }
}
