# QA Inventory

## Repository

- `README.md` exists.
- `README.ja.md` exists.
- `LICENSE`, `NOTICE.md`, `SECURITY.md`, and `CONTRIBUTING.md` exist.
- `src/PoliticsJaPatch.cs` contains the current patcher source.
- Helper scripts live under `tools/`.
- CI workflows live under `.github/workflows/`.
- Docs live under `docs/`.
- Header and social preview artwork live under `assets/`.
- Docs-visible header artwork is mirrored under `docs/public/`.

## Binary Policy

The repository must not include:

- `Cities-Skyline-Politics-Mod.dll`
- `Mono.Cecil.dll`
- `PoliticsJaPatch.exe`
- `.pdb` files
- `*.codex-ja*-backup-*`

`tools/validate-repo.ps1` checks for committed binaries outside build output.

## Manual Game QA

After applying the patch:

- Main window title should be Japanese.
- Election status should use Japanese labels.
- Overlay selector button should show Japanese text.
- Overlay panel header should show Japanese text.
- Default party names should display in Japanese where the patch can intercept them.
- Any remaining English text should be captured with a screenshot and added as a new patcher map entry or IL hook.

## 2026-05-09 Local Verification

- `tools\validate-repo.ps1`: passed.
- `tools\build.ps1`: passed and produced `bin\Release\PoliticsJaPatch.exe`.
- Binary scan outside `bin\`: passed.
- `npm run build` for VitePress docs: not run locally because `npm` is not available on this machine's PATH. The GitHub Actions workflow installs Node and runs the docs build in CI.
- Git initialization and commit: not run locally because `git` is not available on this machine's PATH.
