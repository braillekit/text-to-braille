#How The Auto Updater Works

目前使用 GitHub 開放空間來存放更新的檔案。網址如下：

https://github.com/huanlin/EasyBrailleEdit/tree/master/UpdateFiles

未來如果有更換檔案所在的位址，只要修改 AppConfig.ini 裡面的 [Internet] 區段的 AppUpdateFilesUri 參數即可。參考以下範例：

    [Internet]
    AppServerName=
    AppUpdateFilesUri=https://raw.githubusercontent.com/huanlin/EasyBrailleEdit/master/UpdateFiles/

其中的 AppServerName 已經沒有實質作用（儘管有些程式碼裡面可能還有用到）。

**注意**: 伺服器上面至少要有兩個檔案，此更新機制才能運作：Update.txt 和 ChangeLog.txt。

每當 EBE 啟動時，便會到上述網址下載 Update.txt，然後跟本機的檔案版本比對，找出需要更新的檔案清單。接著便開始按照清單裡面的內容逐一下載檔案至本機。
若其中有一個檔案下載失敗，就會整批 rollback（把先前下載的檔案刪除）。
當整個更新程序順利完成，EBE 會寄一封電子郵件給 innoobject@gmail.com，內文是使用者的註冊資訊，例如：

    客戶名稱: XX大學
    授權序號: 1DE00A0A18A81F35
    程式版本: PRO
    更新前的版本號碼: 2.7.0


EasyBrailleEdit 的自動更新功能是由 Huanlin.AppBlock 組件的 HttpUpdater 類別提供。
使用方法可參考 EasyBrailleEdit 專案的 MainForm.cs 中的 DoUpdate() 方法。

## Update.txt 範例

Save the following text as a file named "Update.txt" and place it in the folder of the application files.

    EasyBrailleEdit.exe       ; 2.7.2 '若 client 端版本比這裡指定的版本還要舊，則下載
    Txt2Brl.exe               ; 2.7.2 'Txt2Brl.exe 版本應該跟 EasyBrailleEdit 一致
    Huanlin.DLL               ; 4.0.0
    Huanlin.AppBlock.dll      ; 4.0.0
    Huanlin.Braille.dll       ; 4.0.0
    Huanlin.WinForms.dll      ; 4.0.0
    Huanlin.TextServices.dll  ; 4.0.0
    Huanlin.WinApi.dll        ; 4.0.0
    AppConfig.Default.ini     ; ?     '若 client 端無此檔案才更新
    phrase.phf                ; ?     '若 client 端無此檔案才更新
    SourceGrid.DLL            ; 4.11
    Nini.dll                  ; 1.1
