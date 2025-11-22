# 易點雙視變更歷史

## User features

- Upgrade .NET Framework from 7.0 to 10.0.
- Upgrade dependent NuGet packages to latest versions.
- 新增 YAML 檔案格式 (.byml) 支援，並將其設為預設的存檔格式。此格式具有更好的人眼可讀性與版本控制友善性。**注意：** 只是方便診斷與除錯，若任意修改其內容，可能會導致應用程式無法正常運作。

## Breaking changes
 
- 從現在起，最低支援的 Windows 版本是 Windows 10。
- 自動更新的檔案來源改為以下 GitHub 網址（此設定在應用程式目錄下的 AppConfig.ini 檔案中）：
  https://raw.githubusercontent.com/braillekit/text-to-braille-updates/refs/heads/main/Files/

## Details for developers

- Use GitHub Actions for CI/CD.
- 使用 Central Package Management (CPM) 來統一管理相依套件的版本。
- 重構 C# 程式碼以符合最現代 C# 語法和最佳實踐。

## Release instructions

Tag a specific commit with a version:

```bash
git tag v5.0.0
git push origin v5.0.0 --force
```		
