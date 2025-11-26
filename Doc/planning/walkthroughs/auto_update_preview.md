# Auto-Update Braille Preview Walkthrough

## Changes Overview

We have implemented an automatic update mechanism for the Braille Preview using a **Debounce** strategy. This ensures that the preview updates only after the user pauses typing, preventing performance issues.

### Key Features

- **Debounce Timer**: Updates occur after **1.5 seconds** (default) of inactivity.
- **Configurable Delay**: The delay time can be adjusted in `AppConfig.ini` under `[Braille]` section with the key `AutoPreviewDelay`.
- **Performance Optimization**: Typing resets the timer, so no conversion happens while you are actively typing.

## Verification Steps

### 1. Enable Instant Preview

1. Open **EasyBrailleEdit**.
2. Click the **"啟用即時預覽"** (Enable Instant Preview) button on the toolbar.
3. Ensure the preview window appears.

### 2. Test Unsaved File (New!)

1. Click **"File"** -> **"New"** (or Ctrl+N) to create a new, empty document.
2. Ensure the title bar shows "未命名" (Untitled).
3. Type some text (e.g., "Testing unsaved file").
4. **Verify**: The preview updates automatically after the delay, even though the file is not saved.

### 3. Test Auto-Update

1. Type a sentence in the editor, e.g., "Hello World".
2. **Stop typing** and watch the preview window.
3. **Verify**: After approximately 1.5 seconds, the preview should automatically update to show the Braille for "Hello World".

### 4. Test Debounce (Typing Continuity)

1. Type a long sentence continuously for about 5 seconds without stopping.
2. **Verify**: The preview should **NOT** update while you are typing.
3. Stop typing.
4. **Verify**: The preview updates shortly after you stop.

### 5. Configuration (Optional)

1. Close EasyBrailleEdit.
2. Open `AppConfig.ini` in a text editor.
3. Find or add `AutoPreviewDelay=3000` under `[Braille]`.
4. Save and restart EasyBrailleEdit.
5. **Verify**: The auto-update now takes 3 seconds to trigger.

## Technical Details

- **File**: `MainForm.cs`
- **Timer**: `m_PreviewUpdateTimer`
- **Event**: `TextArea_TextChanged` resets the timer.
