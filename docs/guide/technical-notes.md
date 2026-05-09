# Technical Notes

The patcher uses the Mono.Cecil version bundled with Cities: Skylines because
the target mod is a .NET Framework assembly loaded by the game.

The patcher performs three kinds of changes:

- string literal replacement for visible UI text,
- enum field renaming where the mod renders enum names directly,
- IL call injection around party name load/store sites so default and saved
  party names can be translated consistently.

The repository intentionally excludes compiled executables, DLLs, and patched
Workshop files. Build artifacts are generated under `bin/Release`.

