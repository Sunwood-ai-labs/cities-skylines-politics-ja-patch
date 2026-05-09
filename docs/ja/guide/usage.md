# 使い方

パッチを当てる前に Cities: Skylines を閉じます。

```powershell
cd D:\Prj\cities-skylines-politics-ja-patch
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1
```

Steam ライブラリが標準位置にない場合は、パスを指定します。

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1 `
  -GameRoot "E:\SteamLibrary\steamapps\common\Cities_Skylines" `
  -WorkshopRoot "E:\SteamLibrary\steamapps\workshop\content\255710"
```

ビルドだけ行う場合:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\build.ps1
```

すでにビルド済みのパッチャーを使う場合:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1 -NoBuild
```

## 復元

パッチ済みDLLの隣にできた最新のバックアップを、ゲームを閉じた状態で
`Cities-Skyline-Politics-Mod.dll` に戻してください。

## 困ったとき

`Mono.Cecil.dll` が見つからない場合は `-GameRoot` または `-CecilPath` を指定します。

ゲームが起動中と言われた場合は、Cities: Skylines を閉じるか `-ForceStopGame` を付けてください。
