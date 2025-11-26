# Refine Instant Preview: Support Unsaved Files

## Goal Description
Confirm and ensure that the "Instant Braille Preview" feature works for new, unsaved files. The user noted that previously they had to save to trigger the preview. With the new auto-update (debounce) mechanism, this requirement should theoretically be gone. We will verify this and update the documentation.

## User Review Required
> [!NOTE]
> **No Code Changes Expected**: Based on code analysis, the current implementation already supports converting unsaved content because it streams text directly from the editor to the converter and uses a temporary file for output. This plan focuses on verification and documentation updates.

## Proposed Changes

### Documentation
#### [MODIFY] [walkthroughs/auto_update_preview.md](file:///d:/Projects/BrailleKit/text-to-braille/Doc/planning/walkthroughs/auto_update_preview.md)
- Add a verification step to explicitly test with a "New File" (unsaved).

## Verification Plan

### Manual Verification
1. **Unsaved File Test**:
   - Open EasyBrailleEdit.
   - Do **NOT** save the file (title should be "未命名").
   - Enable "Instant Preview".
   - Type text.
   - Verify preview updates automatically.
