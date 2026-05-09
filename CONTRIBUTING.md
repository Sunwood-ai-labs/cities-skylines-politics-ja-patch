# Contributing

Thank you for improving this patcher.

## Rules

- Do not commit Cities: Skylines files, Workshop DLLs, patched DLLs, Mono.Cecil binaries, or backups.
- Keep new translations focused on strings that actually appear in the mod UI.
- Update both `README.md` and `README.ja.md` when user-facing behavior changes.
- Update docs when command usage changes.

## Local Checks

```powershell
.\tools\validate-repo.ps1
```

If Cities: Skylines is installed on the machine:

```powershell
.\tools\build.ps1
```

## Release Checklist

- Run repository validation.
- Build the patcher on Windows.
- Apply it to a fresh copy of the Workshop DLL.
- Launch Cities: Skylines and verify the Politics & Elections panel.
- Confirm no DLL, EXE, PDB, or backup file is staged.
