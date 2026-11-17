{
  description = "difficalcy-performanceplus dev environment";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    { self, nixpkgs }:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs { inherit system; };
    in
    {
      devShells.${system}.default = pkgs.mkShell {
        packages = with pkgs; [
          nixfmt-tree
          just
          dotnet-sdk_10
          gh
        ];

        shellHook = ''
          export DIFFICALCY_PERFORMANCEPLUS_FLAKE_HASH="$(cat flake.nix flake.lock | sha256sum | cut -d' ' -f1)"
        '';
      };

      formatter.${system} = pkgs.nixfmt-tree;
    };
}
