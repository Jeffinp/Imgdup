# Dark Theme Design — Imgdup

## Overview

Apply Catppuccin Mocha dark theme with Mauve accent to the WPF app. Organize all styles into separate ResourceDictionary files under `src/Imgdup.App/Themes/`.

---

## Color Palette

All colors defined as `SolidColorBrush` in `Themes/ColorPalette.xaml`.

| Key | Hex | Usage |
|-----|-----|-------|
| `BrushBackground` | `#1E1E2E` | Window bg, gallery bg |
| `BrushCrust` | `#11111B` | Status bar (sunken) |
| `BrushSurface0` | `#313244` | Toolbar, cards, combobox bg |
| `BrushSurface1` | `#45475A` | Borders, separators, slider track |
| `BrushOverlay` | `#6C7086` | Disabled controls |
| `BrushText` | `#CDD6F4` | Primary text |
| `BrushSubtext` | `#A6ADC8` | Secondary text (metadata, dates) |
| `BrushMauve` | `#CBA6F7` | Primary accent: buttons, selection, focus |
| `BrushMauveHover` | `#D4B0F7` | Mauve hover state |
| `BrushMauvePressed` | `#B899E0` | Mauve pressed state |
| `BrushMauveAlpha` | `#33CBA6F7` | Card selected bg (20% opacity) |
| `BrushRed` | `#F38BA8` | Danger: delete button |
| `BrushRedHover` | `#F5A0B8` | Red hover |
| `BrushGreen` | `#A6E3A1` | Success: "Manter" badge |
| `BrushYellow` | `#F9E2AF` | Card selected border |
| `BrushBlue` | `#89B4FA` | Progress bar fill |

Also define scalar `Color` resources (same names, prefix `Color`) for use in triggers that require `Color` not `Brush`.

---

## Typography

Defined in `Themes/Typography.xaml`.

| Key | Value |
|-----|-------|
| `FontFamilyDefault` | `Segoe UI` |
| `FontSizeBase` | `12` |
| `FontSizeSmall` | `10` |
| `FontSizeLarge` | `14` |
| `FontWeightNormal` | `Normal` |
| `FontWeightSemiBold` | `SemiBold` |

---

## Control Styles

Defined in `Themes/Controls.xaml`. All styles are implicit (no `x:Key`) so they apply globally.

### Button

Three named styles (via `x:Key`), no implicit Button style (too broad):

- `ButtonDefault` — bg `BrushSurface0`, fg `BrushText`, border `BrushSurface1`. Hover → `BrushSurface1`. Pressed → `BrushOverlay`. CornerRadius `4`.
- `ButtonPrimary` — bg `BrushMauve`, fg `BrushCrust` (dark text). Hover → `BrushMauveHover`. Pressed → `BrushMauvePressed`. FontWeight SemiBold.
- `ButtonDanger` — bg `BrushRed`, fg `BrushCrust`. Hover → `BrushRedHover`. CornerRadius `4`.

All buttons: no default Windows chrome (`BorderThickness="0"`, `Template` override for flat look).

### CheckBox

Implicit style. Tick color `BrushMauve`. Box border `BrushSurface1`, bg `BrushSurface0`. Checked bg `BrushMauve`.

### ComboBox

Implicit style. Bg `BrushSurface0`, fg `BrushText`, border `BrushSurface1`. Dropdown bg `BrushSurface0`. Selected item highlight `BrushMauveAlpha`.

### Slider

Implicit style. Track bg `BrushSurface1`. Thumb bg `BrushMauve`, border none. CornerRadius on track `2`.

### ProgressBar

Implicit style. Track bg `BrushSurface1`. Fill bg `BrushBlue`. CornerRadius `3`. Height `6` (thinner).

### ScrollBar

Implicit style. Track transparent. Thumb `BrushSurface1`, hover `BrushOverlay`. No arrow buttons.

### ListBox / ListBoxItem

Implicit styles. ListBox bg `BrushBackground`, border none. ListBoxItem: no highlight (selection handled by DataTemplate `Border` via `IsSelected` trigger).

---

## Layout — MainWindow.xaml

### Toolbar
- bg `BrushSurface0`, padding `10,8`, border-bottom `BrushSurface1` 1px
- "Verificar" button → `Style=ButtonPrimary`
- "Cancelar" button → `Style=ButtonDefault`
- "Enviar p/ Lixeira" button → `Style=ButtonDanger`
- All other buttons → `Style=ButtonDefault`

### Status Bar
- bg `BrushCrust`, padding `8,5`
- text fg `BrushSubtext`
- ProgressBar uses implicit style

### Gallery (ListBox)
- bg `BrushBackground`

### Group Header
- bg `BrushSurface0`, left-border `3px` `BrushMauve`, padding `8,4`
- text fg `BrushText`, FontWeight SemiBold

### Image Cards (DataTemplate Border)
- bg `BrushSurface0`, border `BrushSurface1` 1px, CornerRadius `6`, Margin `4`, Padding `6`
- Selected: bg `BrushMauveAlpha`, border `BrushYellow` 2px
- FileName text: fg `BrushText`
- Metadata (size, dimensions, date): fg `BrushSubtext`

### Folder Chips
- bg `BrushSurface1`, CornerRadius `4`, padding `6,3`
- text fg `BrushText`
- remove btn `✕` fg `BrushSubtext`, hover fg `BrushText`

### "Manter" Badge
- bg `BrushGreen`, fg `BrushCrust` (dark), CornerRadius `3`, padding `4,1`

---

## File Structure

```
src/Imgdup.App/
  Themes/
    ColorPalette.xaml     ← SolidColorBrush + Color tokens
    Typography.xaml       ← font keys
    Controls.xaml         ← all control styles (imports ColorPalette, Typography)
    DarkTheme.xaml        ← MergedDictionaries: ColorPalette + Typography + Controls
  App.xaml                ← merges only DarkTheme.xaml
  MainWindow.xaml         ← updated: WindowBackground, button styles, card template
```

`DarkTheme.xaml` is the single entry point — swapping themes later means only changing this import in `App.xaml`.

---

## Out of Scope

- Light theme toggle (future work)
- Custom window chrome / title bar
- Animations / transitions
