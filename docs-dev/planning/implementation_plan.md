# Embedded Instant Preview

## Goal Description

Embed the "Instant Braille Preview" directly into the main application window instead of using a separate popup window. This will provide a more integrated user experience.

## User Review Required
>
> [!NOTE]
> **Layout**: The editor will be on the left (35% width) and the preview on the right (65% width), separated by a movable splitter.
> **Technology**: We will use a `SplitContainer` control. The `PreviewConversionForm` will be replaced by a `PreviewPanel` UserControl.

## Proposed Changes

### EasyBrailleEdit

#### [NEW] [Controls/PreviewPanel.cs](../../src/EasyBrailleEditApp/EasyBrailleEdit/Controls/PreviewPanel.cs)

- Create a new `UserControl` named `PreviewPanel`.
- Add a `WebBrowser` control docked to Fill.
- Implement `UpdatePreview(List<BrailleLine> lines)` method (logic moved from `PreviewConversionForm`).

#### [MODIFY] [MainForm.cs](../../src/EasyBrailleEditApp/EasyBrailleEdit/MainForm.cs)

- **InitTextArea**:
  - Create `SplitContainer` (`m_SplitContainer`).
  - Dock `m_SplitContainer` to Fill in `panFill`.
  - Add `m_TextArea` to `m_SplitContainer.Panel1`.
  - Add `m_PreviewPanel` to `m_SplitContainer.Panel2`.
  - Set initial `SplitterDistance` to achieve 35%/65% ratio.
- **EnablePreviewConversion**:
  - Toggle `m_SplitContainer.Panel2Collapsed` instead of showing/hiding a form.
- **UpdatePreviewAsync**:
  - Call `m_PreviewPanel.UpdatePreview` instead of the form's method.
- **Remove** references to `PreviewConversionForm`.

#### [DELETE] [PreviewConversionForm.cs](../../src/EasyBrailleEditApp/EasyBrailleEdit/PreviewConversionForm.cs)

- Remove the obsolete form.

## Verification Plan

### Manual Verification

1. **Layout Test**:
   - Open EasyBrailleEdit.
   - Enable Instant Preview.
   - Verify the editor is on the left (approx 35%) and preview on the right (approx 65%).
   - Verify the splitter can be moved.
2. **Functionality Test**:
   - Type text and verify the preview updates (using the existing auto-update logic).
3. **Toggle Test**:
   - Disable Instant Preview. Verify the preview panel disappears and the editor takes up the full width (or returns to previous state).
   - Enable again and verify it restores.
