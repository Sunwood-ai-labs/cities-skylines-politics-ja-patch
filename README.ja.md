# Cities: Skylines Politics 日本語化パッチ

![Cities: Skylines Politics 日本語化パッチ](assets/repo-header.svg)

[English](README.md) | [ドキュメント](docs/ja/index.md) | [Notice](NOTICE.md)

Cities: Skylines 1 の Workshop MOD「Politics & Elections」を日本語化するための
ローカル DLL パッチャーです。

このリポジトリには、ゲーム内UI、選挙状態、オーバーレイ表示、政党名、政策文言、
既定政党名の読み込み部分を日本語化するパッチャーをまとめています。元のMODや
ゲームファイルは同梱せず、あなたのPCにインストール済みのDLLだけをローカルで変更します。

## 変更内容

- 画面に出る英語UIを日本語化します。
- 選挙フェーズやオーバーレイ種別など、UIに出る enum 名も日本語化します。
- 既定の政党名、略称、保存データから読み込まれる政党名を日本語化します。
- 元の Workshop MOD はこのリポジトリに含めません。

## 必要なもの

- Windows
- Steam版 Cities: Skylines 1
- Workshop MOD `3718153788`: Politics & Elections
- Windows 付属の .NET Framework C# コンパイラ (`csc.exe`)
- Cities: Skylines に同梱されている `Mono.Cecil.dll`

スクリプトは以下のような一般的な Steam パスを自動検出します。

- `D:\SteamLibrary\steamapps\common\Cities_Skylines`
- `D:\SteamLibrary\steamapps\workshop\content\255710`
- `C:\Program Files` 配下の Steam 標準パス

## すぐ使う

先に Cities: Skylines を閉じてください。

```powershell
cd D:\Prj\cities-skylines-politics-ja-patch
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1
```

Steam ライブラリが別の場所にある場合:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1 `
  -GameRoot "E:\SteamLibrary\steamapps\common\Cities_Skylines" `
  -WorkshopRoot "E:\SteamLibrary\steamapps\workshop\content\255710"
```

ゲームが起動中で、スクリプト側で終了してから当てたい場合:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\apply-patch.ps1 -ForceStopGame
```

## 手動ビルド

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\build.ps1
.\bin\Release\PoliticsJaPatch.exe "D:\SteamLibrary\steamapps\workshop\content\255710\3718153788\Cities-Skyline-Politics-Mod.dll"
```

## 復元

`apply-patch.ps1` は、変更前に対象DLLの隣へバックアップを作ります。

```text
Cities-Skyline-Politics-Mod.dll.codex-ja-backup-YYYYMMDD-HHMMSS
```

戻すときはゲームを閉じて、このバックアップを
`Cities-Skyline-Politics-Mod.dll` に上書きしてください。

## 構成

```text
src/PoliticsJaPatch.cs      パッチャー本体
tools/build.ps1             ビルド補助
tools/apply-patch.ps1       ビルド、バックアップ、パッチ適用
tools/validate-repo.ps1     リポジトリ検査
docs/                       利用・保守ドキュメント
```

## 検証

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\validate-repo.ps1
```

Cities: Skylines が入っているPCでは、あわせて次も実行できます。

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\tools\build.ps1
```
