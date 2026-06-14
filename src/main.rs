use anyhow::Result;
use clap::{Parser, Subcommand};
use std::path::{Path, PathBuf};

mod convert_stations;
mod geo_coord;
mod iso6709;
mod list_stations;
mod read_file;

mod gen_protobuf {
    pub mod stations;
}

#[derive(Parser)]
#[command(version, about = "MOSMIX data utility", long_about = None)]
struct Cli {
    #[command(subcommand)]
    command: Commands,
}

#[derive(Subcommand)]
enum Commands {
    /// Converts a station configuration file to a binary format.
    Convert {
        #[arg(short, long)]
        input: String,

        #[arg(short, long)]
        output: Option<String>,
    },

    List {
        #[arg(long)]
        geo: Option<String>,
        #[arg(long)]
        radius: Option<i16>,
    },
}

fn main() -> Result<()> {
    let cli = Cli::parse();
    match &cli.command {
        Commands::Convert { input, output } => {
            let input = Path::new(input);
            let output = match output {
                None => input.with_extension("bin"),
                Some(x) => PathBuf::from(x),
            };

            convert_stations::convert_stations_file(input, &output)?;
            println!("Successfully converted {} to {:?}", input.display(), output);
        }

        Commands::List { geo, radius } => {
            list_stations::list_stations(geo, radius)?;
        }
    }

    Ok(())
}
