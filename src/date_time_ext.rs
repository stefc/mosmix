use chrono::{DateTime, Datelike, NaiveDate, NaiveDateTime, TimeZone, Timelike};

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum Accuracy {
    Year,
    Quarter,
    Month,
    Week,
    Day,
    Hour,
    Minute,
    Second,
}

/// Trait to extend `DateTime` with utility methods like `trunc_year`.
pub trait DateTimeExt {
    fn trunc_year(&self) -> Self;
    fn trunc_month(&self) -> Self;
    fn trunc_quarter(&self) -> Self;
    fn trunc_week(&self) -> Self;
    fn trunc_day(&self) -> Self;
    fn trunc_hour(&self) -> Self;
    fn trunc_minute(&self) -> Self;
    fn trunc_second(&self) -> Self;
    fn trunc(&self, accuracy: Accuracy) -> Result<Self, String>
    where
        Self: Sized;
}

impl<Tz: TimeZone> DateTimeExt for DateTime<Tz> {
    fn trunc_year(&self) -> Self {
        self.timezone()
            .with_ymd_and_hms(self.year(), 1, 1, 0, 0, 0)
            .unwrap()
    }

    fn trunc_month(&self) -> Self {
        self.timezone()
            .with_ymd_and_hms(self.year(), self.month(), 1, 0, 0, 0)
            .unwrap()
    }

    fn trunc_quarter(&self) -> Self {
        let month = self.month() - (self.month() - 1) % 3;
        self.timezone()
            .with_ymd_and_hms(self.year(), month, 1, 0, 0, 0)
            .unwrap()
    }

    fn trunc_week(&self) -> Self {
        let date = self.date_naive();
        let days_from_monday = date.weekday().num_days_from_monday();
        let truncated_date = date - chrono::Duration::days(days_from_monday as i64);
        self.timezone()
            .from_local_datetime(&truncated_date.and_hms_opt(0, 0, 0).unwrap())
            .unwrap()
    }

    fn trunc_day(&self) -> Self {
        self.timezone()
            .with_ymd_and_hms(self.year(), self.month(), self.day(), 0, 0, 0)
            .unwrap()
    }

    fn trunc_hour(&self) -> Self {
        self.timezone()
            .with_ymd_and_hms(self.year(), self.month(), self.day(), self.hour(), 0, 0)
            .unwrap()
    }

    fn trunc_minute(&self) -> Self {
        self.timezone()
            .with_ymd_and_hms(
                self.year(),
                self.month(),
                self.day(),
                self.hour(),
                self.minute(),
                0,
            )
            .unwrap()
    }

    fn trunc_second(&self) -> Self {
        self.timezone()
            .with_ymd_and_hms(
                self.year(),
                self.month(),
                self.day(),
                self.hour(),
                self.minute(),
                self.second(),
            )
            .unwrap()
    }

    fn trunc(&self, accuracy: Accuracy) -> Result<Self, String> {
        match accuracy {
            Accuracy::Year => Ok(self.trunc_year()),
            Accuracy::Month => Ok(self.trunc_month()),
            Accuracy::Quarter => Ok(self.trunc_quarter()),
            Accuracy::Week => Ok(self.trunc_week()),
            Accuracy::Day => Ok(self.trunc_day()),
            Accuracy::Hour => Ok(self.trunc_hour()),
            Accuracy::Minute => Ok(self.trunc_minute()),
            Accuracy::Second => Ok(self.trunc_second()),
        }
    }
}

impl DateTimeExt for NaiveDateTime {
    fn trunc_year(&self) -> Self {
        NaiveDate::from_ymd_opt(self.year(), 1, 1)
            .unwrap()
            .and_hms_opt(0, 0, 0)
            .unwrap()
    }

    fn trunc_month(&self) -> Self {
        NaiveDate::from_ymd_opt(self.year(), self.month(), 1)
            .unwrap()
            .and_hms_opt(0, 0, 0)
            .unwrap()
    }

    fn trunc_quarter(&self) -> Self {
        let month = self.month() - (self.month() - 1) % 3;
        NaiveDate::from_ymd_opt(self.year(), month, 1)
            .unwrap()
            .and_hms_opt(0, 0, 0)
            .unwrap()
    }

    fn trunc_week(&self) -> Self {
        let days_from_monday = self.weekday().num_days_from_monday();
        let truncated_date = self.date() - chrono::Duration::days(days_from_monday as i64);
        truncated_date.and_hms_opt(0, 0, 0).unwrap()
    }

    fn trunc_day(&self) -> Self {
        self.date().and_hms_opt(0, 0, 0).unwrap()
    }

    fn trunc_hour(&self) -> Self {
        self.date().and_hms_opt(self.hour(), 0, 0).unwrap()
    }

    fn trunc_minute(&self) -> Self {
        self.date()
            .and_hms_opt(self.hour(), self.minute(), 0)
            .unwrap()
    }

    fn trunc_second(&self) -> Self {
        self.date()
            .and_hms_opt(self.hour(), self.minute(), self.second())
            .unwrap()
    }

    fn trunc(&self, accuracy: Accuracy) -> Result<Self, String> {
        match accuracy {
            Accuracy::Year => Ok(self.trunc_year()),
            Accuracy::Month => Ok(self.trunc_month()),
            Accuracy::Quarter => Ok(self.trunc_quarter()),
            Accuracy::Week => Ok(self.trunc_week()),
            Accuracy::Day => Ok(self.trunc_day()),
            Accuracy::Hour => Ok(self.trunc_hour()),
            Accuracy::Minute => Ok(self.trunc_minute()),
            Accuracy::Second => Ok(self.trunc_second()),
        }
    }
}
