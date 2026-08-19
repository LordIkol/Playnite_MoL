# Mythos of Loki (Playnite Theme & MythosHelper Plugin)

A sleek, modern glassmorphic theme for [Playnite Desktop](https://playnite.link/), paired with the **MythosHelper** companion plugin for advanced interactive multi-select filtering, dynamic metadata chips, and responsive visual states.

---

## ✨ Features

### 🎮 1. Modern Glassmorphic UI & Layout
* **Glassmorphism & Depth:** Tailored acrylic backdrops, semi-transparent frosted glass layers (`GlassControlBrushDark`, `BackdropGlass1`), subtle glowing borders, and rounded control surfaces.
* **Redesigned Details View:** Pinned header layout with collapsible metadata sections, smooth gradient masks, and cohesive typography using Segoe Fluent Icons.

### 🏷️ 2. Interactive Metadata Chips Bar
* **Collapsible Attached Filter Bar:** Pinned directly beneath the game header with a toggle switch to show/hide at will.
* **Color-Coded Category Badges & Chips:**
  * 🟣 **Series** (Purple `#8A5CF6` / `#C084FC`)
  * 🔵 **Genres** (Sky Blue `#0284C7` / `#38BDF8`)
  * 🟢 **Features** (Emerald Green `#059669` / `#34D399`)
  * 🟡 **Categories** (Amber Gold `#D97706` / `#FBBF24`)
  * ⚪ **Tags** (Slate White `#FFFFFF` / `#94A3B8`)
* **Multi-Select Toggle Filtering (with MythosHelper):** Click any chip to add or remove it from the active filter in real time without wiping other filters or opening the sidebar filter panel.
* **Dynamic Glowing Highlights:** Active filter chips automatically illuminate with their category's glow color and border.
* **Auto-Select First Game:** Filtering automatically selects the first game in the filtered library view.
* **Centered & Balanced Layout:** All metadata chips and badges are horizontally and vertically centered with clean symmetrical spacing.

### ⚡ 3. Quick Filter Toolbars
* **Library Toolbar (Above Games List):** Convenient quick-action row between the search box and the game list in Details View:
  * 🧹 **Clear Filters:** Resets active metadata filters while preserving status flags (Installed / Favorites / Match All).
  * 📥 **Installed Filter:** One-click toggle for installed games with active state illumination.
  * ❤️ **Favorite Filter:** One-click toggle for favorite games.
  * 🔀 **Match All (AND/OR Mode):** Quickly toggle between matching *all* selected filters (AND) or *any* selected filter (OR).
* **Attached Chips Bar Toolbar:** Embedded Clear Filter and Match All quick buttons directly in the metadata row.

### 🛡️ 4. Standalone Fallback & Graceful Degradation
* **100% Functional Without Plugin:**
  * When **MythosHelper** is not installed, the theme gracefully degrades to Playnite's native single-select filter commands (`Set[Category]FilterCommand`).
  * The Clear Filter button falls back seamlessly to Playnite's native `ClearFiltersCommand`.
  * Category group visibility is bound to native Playnite element visibility, ensuring proper collapsing when games lack specific metadata.

---

## 🧩 MythosHelper Companion Plugin (`MythosHelper_Loki`)

The included C# plugin (`.NET Framework 4.6.2`) empowers the theme with deep Playnite integration:

* **`FilterService`:** Direct reflection-based interaction with Playnite's `DatabaseFilters` and `SelectableDbItemList`.
* **`ToggleFilterCommandConverter`:** Handles multi-select filter toggles per category.
* **`ClearMetadataFiltersCommandConverter`:** Clears metadata filters (genres, features, categories, tags, series, platforms, publishers, developers) without resetting status filters.
* **`IsFilterActiveConverter`:** Real-time multi-binding converter that tracks filter version changes and reflects active state onto WPF UI elements.

---

## 🚀 Installation & Deployment

### Quick Deploy Script
Run the automated deployment PowerShell script from the repository root:

```powershell
# Build plugin, kill running Playnite instances, and deploy theme + plugin
.\deploy.ps1 -KillPlaynite
```

### Manual Installation
1. **Theme:** Copy all repository folders (`CustomControls`, `DerivedStyles`, `Views`, `Common.xaml`, `theme.yaml`, etc.) to:
   ```
   %APPDATA%\Playnite\Themes\Desktop\Mythos_of_Loki\
   ```
2. **Plugin:** Build `Plugin/MythosHelper.csproj` in Release mode and copy `MythosHelper.dll` and `extension.yaml` to:
   ```
   %LOCALAPPDATA%\Playnite\Extensions\MythosHelper_Loki\
   ```

---

## 🛠️ Tech Stack & Requirements
* **Playnite:** Playnite Desktop 9+
* **Target Framework:** .NET Framework 4.6.2
* **Styling:** WPF XAML, Segoe Fluent Icons, Segoe MDL2 Assets
