# 圖片燈箱 (Lightbox) 功能實作

## 簡介

為了解決大圖片在網頁上縮小後看不清楚的問題，我們實作了點擊圖片放大顯示的功能（Lightbox）。此功能允許使用者點擊文件中的任何圖片，將其以原始尺寸顯示在全螢幕的模態視窗中。

## 實作細節

此功能完全透過 Hugo 的自訂功能與原生 HTML/CSS/JS 實作，不依賴任何外部套件。

### 1. 圖片渲染 Hook (`render-image.html`)

我們覆寫了 Hugo 預設的 Markdown 圖片渲染行為，為所有圖片自動加上 `lightbox-trigger` 類別，使其可以被 JavaScript 選取。

- **檔案位置**: `layouts/_default/_markup/render-image.html`
- **功能**: 
  - 保留原始圖片的 `src`, `alt`, `title` 屬性。
  - 加入 `class="lightbox-trigger"`。
  - 加入 `style="cursor: pointer;"` 提示使用者可點擊。

### 2. 模態視窗注入 (`body.html`)

我們利用 `hugo-book` 主題提供的注入點，將 Lightbox 的 HTML 結構與控制邏輯 (JavaScript) 注入到頁面底部。

- **檔案位置**: `layouts/partials/docs/inject/body.html`
- **功能**:
  - **HTML**: 定義 `#lightbox-modal` 容器，包含關閉按鈕、圖片容器與標題容器。
  - **JavaScript**: 
    - 監聽所有 `.lightbox-trigger` 的點擊事件。
    - 點擊圖片時，顯示模態視窗並載入對應圖片。
    - 支援點擊關閉按鈕、點擊背景或按 `Escape` 鍵關閉視窗。

### 3. 樣式定義 (`_custom.scss`)

我們在自訂樣式檔中加入了 Lightbox 所需的 CSS。

- **檔案位置**: `assets/_custom.scss`
- **功能**:
  - 定義模態視窗的全螢幕遮罩 (半透明黑色背景)。
  - 設定圖片的最大寬度與高度 (RWD)，確保不超出螢幕。
  - 加入簡單的放大動畫效果。

## 如何驗證

1. 啟動 Hugo 本地伺服器：`hugo server`
2. 瀏覽任何包含圖片的頁面。
3. 點擊圖片，確認是否出現黑色背景的放大視窗。
