{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  buildInputs = [
    pkgs.dotnet-sdk
    pkgs.openjdk
    pkgs.fsautocomplete
  ];
}
