# EasyBrailleEdit 使用手冊文件專案

這個目錄包含了 EasyBrailleEdit 使用手冊的原始碼與設定檔。本網站使用 [Hugo](https://gohugo.io/) 靜態網站產生器建置，並透過 GitHub Actions 自動發布。

## 目錄結構

-   **`content/`**: 存放 Markdown 格式的文件內容。
    -   `content/docs/`: 主要章節內容。
-   **`hugo.toml`**: Hugo 的主要設定檔。
-   **`go.mod` & `go.sum`**: Go 模組定義檔，用於管理 Hugo 模組 (如主題)。
-   **`package-lock.json`**: (如果有的話) npm 依賴鎖定檔。

## 如何在本地預覽網站

若要在您的電腦上預覽網站，請依照以下步驟操作：

1.  **安裝 Go**:
    -   由於本專案使用 Hugo Modules，您需要安裝 Go 語言環境。
    -   請至 [Go 官方網站](https://go.dev/dl/) 下載並安裝。

2.  **安裝 Hugo**:
    -   請參考 [Hugo 官方安裝指南](https://gohugo.io/installation/) 安裝 Hugo (建議安裝 `extended` 版本)。
    -   Windows 使用者可以使用 Chocolatey (`choco install hugo-extended`) 或 Scoop (`scoop install hugo-extended`)。

3.  **開啟終端機並切換到此目錄**:
    ```bash
    cd docs-user
    ```

4.  **啟動預覽伺服器**:
    ```bash
    hugo server
    ```
    > 第一次執行時，Hugo 會自動下載所需的模組 (如 `hugo-book` 主題)，請稍候。

5.  **瀏覽網站**:
    -   打開瀏覽器，輸入網址：`http://localhost:1313/text-to-braille/`
    -   當您修改 Markdown 檔案時，瀏覽器會自動重新整理顯示最新內容。

## 網站發布流程

本專案已設定 GitHub Actions 自動化發布流程，無需手動建置。

1.  **觸發條件**:
    -   只要將程式碼推送到 GitHub 的 `main` 或 `master` 分支，就會自動觸發發布流程。

2.  **發布機制**:
    -   GitHub Action 會讀取 `.github/workflows/publish-docs.yml` 設定。
    -   它會自動安裝 Hugo，建置靜態網頁。
    -   建置完成的網頁會被推送到 `gh-pages` 分支。

3.  **GitHub Pages 設定**:
    -   請確保 GitHub Repository 的 **Settings > Pages** 中，Build and deployment 的 Source 設定為 **Deploy from a branch**，並且 Branch 選擇 **gh-pages**。
