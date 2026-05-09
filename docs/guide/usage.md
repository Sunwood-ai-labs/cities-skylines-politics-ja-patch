# Usage

Close Cities: Skylines before patching.

```powershell
cd D:\Prj\cities-skylines-politics-ja-patch
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1
```

If the Steam library is not in a common location, pass explicit paths.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1 `
  -GameRoot "E:\SteamLibrary\steamapps\common\Cities_Skylines" `
  -WorkshopRoot "E:\SteamLibrary\steamapps\workshop\content\255710"
```

To build without applying:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\build.ps1
```

To apply a prebuilt patcher:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1 -NoBuild
```

## Restore

Find the newest backup beside the patched DLL and copy it back over
`Cities-Skyline-Politics-Mod.dll` while the game is closed.

## Troubleshooting

If the script cannot find `Mono.Cecil.dll`, pass either `-GameRoot` or
`-CecilPath`.

If the script says the game is running, close Cities: Skylines or rerun with
`-ForceStopGame`.
