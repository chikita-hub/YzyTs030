# ProPlaWeb (ASP.NET) ディレクトリ構成案

`ProPla` (WPF) から `ProPlaWeb` (ASP.NET) へ移行する前提で、最初に切っておく構成案です。

## ルート構成

```text
ProPlaWeb/
├─ README.md
├─ docs/                        # 移行方針・要件・画面設計
├─ src/
│  ├─ ProPla.Web/               # ASP.NET Core (UI: MVC/Razor/Blazor)
│  ├─ ProPla.Application/        # ユースケース・アプリケーションサービス
│  ├─ ProPla.Domain/             # ドメインモデル・ビジネスルール
│  ├─ ProPla.Infrastructure/     # DB, Mail, 外部I/O実装
│  └─ ProPla.Contracts/          # DTO, API契約, 共通IF
├─ tests/
│  ├─ ProPla.Web.Tests/          # UI/API層のテスト
│  ├─ ProPla.Application.Tests/  # アプリケーション層の単体テスト
│  └─ ProPla.IntegrationTests/   # DBや外部連携を含む統合テスト
└─ tools/
   └─ migration/                 # 移行補助スクリプト（データ変換等）
```

## 既存WPF資産との対応イメージ

- `Propla.App/Views` → `src/ProPla.Web` の画面 (Razor/Blazor)
- `Propla.App/ViewModels` → `src/ProPla.Application` のユースケース + `src/ProPla.Web` のViewModel
- `Propla.App/Models` → `src/ProPla.Domain` のエンティティ
- `Infrastructure/Data`, `Infrastructure/Mail`, `Infrastructure/Export` → `src/ProPla.Infrastructure`

## 次アクション

1. `src/` 配下に .NET solution (`ProPlaWeb.sln`) を作成
2. `ProPla.Domain` と `ProPla.Application` から先に作り、UI層依存を逆転
3. 既存 `Parameter.json` の設定項目を ASP.NET の `appsettings*.json` へ再配置
4. DB接続 (`PostgresDb`) とメール送信 (`ProPlaOutlookMail`) の移行方針を `docs/` に明文化

## 画面移行ガイド

- `StdLogin.xaml` / `ProPlaMenu.xaml` / `ProPlaSearch.xaml` / `ProPlaMain.xaml` の移行手順（Blazor Server + PostgreSQL + ASP.NET Core Identity）: `docs/wpf-to-aspnet-screen-migration.md`
