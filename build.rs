use std::env;
use std::error::Error;
use std::path::PathBuf;

fn main() -> Result<(), Box<dyn Error>> {
    let protobuf_gen_dir = PathBuf::from(env::var("CARGO_MANIFEST_DIR")?)
        .join("src")
        .join("gen_protobuf");
    std::fs::create_dir_all(&protobuf_gen_dir)?;

    println!(
        "cargo:warning=Generating rust-protobuf code into {:?}",
        protobuf_gen_dir
    );

    protobuf_codegen::Codegen::new()
        .pure()
        .includes(&["src/protos/"])
        .input("src/protos/stations.proto")
        .out_dir(&protobuf_gen_dir)
        .customize(protobuf_codegen::Customize::default().gen_mod_rs(false))
        .run_from_script();

    println!("cargo:warning=rust-protobuf code generation complete");

    Ok(())
}
