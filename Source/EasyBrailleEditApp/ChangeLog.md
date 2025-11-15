# 易點雙視變更歷史

## User features

- Upgrade .NET Framework from 7.0 to 10.0.
- Upgrade dependent NuGet packages to latest versions.

## Breaking changes
 
- 從現在起，最低支援的 Windows 版本是 Windows 10。
- 自動更新的檔案來源改為以下 GitHub 網址（此設定在應用程式目錄下的 AppConfig.ini 檔案中）：
  https://raw.githubusercontent.com/braillekit/text-to-braille-updates/refs/heads/main/Files/

## Developer features

- Use GitHub Actions for CI/CD.
- 使用 Central Package Management (CPM) 來統一管理相依套件的版本。

## Release instructions

Tag a specific commit with a version:

```bash
git tag v5.0.0
git push origin v5.0.0
```		
