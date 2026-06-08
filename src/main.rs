use anyhow::Result;
use clap::{Parser, Subcommand};
use std::path::{Path, PathBuf};

mod read_file;
mod convert_stations;
mod list_stations;

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
    }
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
        },

        Commands::List { } => {
            list_stations::list_stations()?;
            println!("All mosmix stations currently available");
        }
    }

    Ok(())
}
