# Dark Theme Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Apply Catppuccin Mocha dark theme with Mauve accent to the Imgdup WPF app, organized into separate ResourceDictionary files.

**Architecture:** Four theme files under `src/Imgdup.App/Themes/` — ColorPalette, Typography, Controls, DarkTheme (merge entry). App.xaml imports only DarkTheme.xaml. MainWindow.xaml references named button styles and uses theme brushes.

**Tech Stack:** WPF / XAML, .NET 10, no new NuGet packages.

---

## File Map

| Action | File | Responsibility |
|--------|------|----------------|
| Create | `src/Imgdup.App/Themes/ColorPalette.xaml` | All `SolidColorBrush` tokens |
| Create | `src/Imgdup.App/Themes/Typography.xaml` | Font family/size/weight keys |
| Create | `src/Imgdup.App/Themes/Controls.xaml` | Full ControlTemplate overrides for Button (3 styles), CheckBox, ComboBox, Slider, ProgressBar, ScrollBar, ListBox/ListBoxItem |
| Create | `src/Imgdup.App/Themes/DarkTheme.xaml` | MergedDictionaries entry point |
| Modify | `src/Imgdup.App/App.xaml` | Import DarkTheme.xaml, keep existing converters |
| Modify | `src/Imgdup.App/MainWindow.xaml` | Window bg, button Style references, card template colors, group header, status bar, folder chips |

---

## Task 1: ColorPalette.xaml

**Files:**
- Create: `src/Imgdup.App/Themes/ColorPalette.xaml`

- [ ] **Step 1: Create the file**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Catppuccin Mocha base -->
    <Color x:Key="ColorBackground">#1E1E2E</Color>
    <Color x:Key="ColorCrust">#11111B</Color>
    <Color x:Key="ColorSurface0">#313244</Color>
    <Color x:Key="ColorSurface1">#45475A</Color>
    <Color x:Key="ColorOverlay">#6C7086</Color>
    <Color x:Key="ColorText">#CDD6F4</Color>
    <Color x:Key="ColorSubtext">#A6ADC8</Color>

    <!-- Accent -->
    <Color x:Key="ColorMauve">#CBA6F7</Color>
    <Color x:Key="ColorMauveHover">#D4B0F7</Color>
    <Color x:Key="ColorMauvePressed">#B899E0</Color>
    <Color x:Key="ColorMauveAlpha">#33CBA6F7</Color>

    <!-- Semantic -->
    <Color x:Key="ColorRed">#F38BA8</Color>
    <Color x:Key="ColorRedHover">#F5A0B8</Color>
    <Color x:Key="ColorGreen">#A6E3A1</Color>
    <Color x:Key="ColorYellow">#F9E2AF</Color>
    <Color x:Key="ColorBlue">#89B4FA</Color>

    <!-- Brushes -->
    <SolidColorBrush x:Key="BrushBackground"  Color="{StaticResource ColorBackground}"/>
    <SolidColorBrush x:Key="BrushCrust"       Color="{StaticResource ColorCrust}"/>
    <SolidColorBrush x:Key="BrushSurface0"    Color="{StaticResource ColorSurface0}"/>
    <SolidColorBrush x:Key="BrushSurface1"    Color="{StaticResource ColorSurface1}"/>
    <SolidColorBrush x:Key="BrushOverlay"     Color="{StaticResource ColorOverlay}"/>
    <SolidColorBrush x:Key="BrushText"        Color="{StaticResource ColorText}"/>
    <SolidColorBrush x:Key="BrushSubtext"     Color="{StaticResource ColorSubtext}"/>
    <SolidColorBrush x:Key="BrushMauve"       Color="{StaticResource ColorMauve}"/>
    <SolidColorBrush x:Key="BrushMauveHover"  Color="{StaticResource ColorMauveHover}"/>
    <SolidColorBrush x:Key="BrushMauvePressed" Color="{StaticResource ColorMauvePressed}"/>
    <SolidColorBrush x:Key="BrushMauveAlpha"  Color="{StaticResource ColorMauveAlpha}"/>
    <SolidColorBrush x:Key="BrushRed"         Color="{StaticResource ColorRed}"/>
    <SolidColorBrush x:Key="BrushRedHover"    Color="{StaticResource ColorRedHover}"/>
    <SolidColorBrush x:Key="BrushGreen"       Color="{StaticResource ColorGreen}"/>
    <SolidColorBrush x:Key="BrushYellow"      Color="{StaticResource ColorYellow}"/>
    <SolidColorBrush x:Key="BrushBlue"        Color="{StaticResource ColorBlue}"/>

</ResourceDictionary>
```

- [ ] **Step 2: Commit**

```bash
git add src/Imgdup.App/Themes/ColorPalette.xaml
git commit -m "feat(theme): add ColorPalette.xaml — Catppuccin Mocha tokens"
```

---

## Task 2: Typography.xaml

**Files:**
- Create: `src/Imgdup.App/Themes/Typography.xaml`

- [ ] **Step 1: Create the file**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:sys="clr-namespace:System;assembly=mscorlib">

    <FontFamily x:Key="FontFamilyDefault">Segoe UI</FontFamily>
    <sys:Double x:Key="FontSizeBase">12</sys:Double>
    <sys:Double x:Key="FontSizeSmall">10</sys:Double>
    <sys:Double x:Key="FontSizeLarge">14</sys:Double>
    <FontWeight x:Key="FontWeightNormal">Normal</FontWeight>
    <FontWeight x:Key="FontWeightSemiBold">SemiBold</FontWeight>

</ResourceDictionary>
```

- [ ] **Step 2: Commit**

```bash
git add src/Imgdup.App/Themes/Typography.xaml
git commit -m "feat(theme): add Typography.xaml — font tokens"
```

---

## Task 3: Controls.xaml — Button styles

**Files:**
- Create: `src/Imgdup.App/Themes/Controls.xaml` (start with Button styles, expand in next tasks)

- [ ] **Step 1: Create Controls.xaml with Button styles**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="ColorPalette.xaml"/>
        <ResourceDictionary Source="Typography.xaml"/>
    </ResourceDictionary.MergedDictionaries>

    <!-- ===== BUTTON ===== -->
    <ControlTemplate x:Key="FlatButtonTemplate" TargetType="Button">
        <Border x:Name="border"
                Background="{TemplateBinding Background}"
                BorderBrush="{TemplateBinding BorderBrush}"
                BorderThickness="{TemplateBinding BorderThickness}"
                CornerRadius="4"
                Padding="{TemplateBinding Padding}"
                SnapsToDevicePixels="True">
            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"
                              RecognizesAccessKey="True"/>
        </Border>
        <ControlTemplate.Triggers>
            <Trigger Property="IsMouseOver" Value="True">
                <Setter TargetName="border" Property="Background" Value="{Binding Tag, RelativeSource={RelativeSource TemplatedParent}}"/>
            </Trigger>
            <Trigger Property="IsPressed" Value="True">
                <Setter TargetName="border" Property="Opacity" Value="0.85"/>
            </Trigger>
            <Trigger Property="IsEnabled" Value="False">
                <Setter TargetName="border" Property="Opacity" Value="0.4"/>
            </Trigger>
        </ControlTemplate.Triggers>
    </ControlTemplate>

    <Style x:Key="ButtonDefault" TargetType="Button">
        <Setter Property="Background"       Value="{StaticResource BrushSurface0}"/>
        <Setter Property="Foreground"       Value="{StaticResource BrushText}"/>
        <Setter Property="BorderBrush"      Value="{StaticResource BrushSurface1}"/>
        <Setter Property="BorderThickness"  Value="1"/>
        <Setter Property="Padding"          Value="10,5"/>
        <Setter Property="FontFamily"       Value="{StaticResource FontFamilyDefault}"/>
        <Setter Property="FontSize"         Value="{StaticResource FontSizeBase}"/>
        <Setter Property="Cursor"           Value="Hand"/>
        <Setter Property="Tag"              Value="{StaticResource BrushSurface1}"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border x:Name="border"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="4"
                            Padding="{TemplateBinding Padding}"
                            SnapsToDevicePixels="True">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"
                                          RecognizesAccessKey="True"/>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="border" Property="Background" Value="{StaticResource BrushSurface1}"/>
                        </Trigger>
                        <Trigger Property="IsPressed" Value="True">
                            <Setter TargetName="border" Property="Background" Value="{StaticResource BrushOverlay}"/>
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="False">
                            <Setter TargetName="border" Property="Opacity" Value="0.4"/>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style x:Key="ButtonPrimary" TargetType="Button" BasedOn="{StaticResource ButtonDefault}">
        <Setter Property="Background"      Value="{StaticResource BrushMauve}"/>
        <Setter Property="Foreground"      Value="{StaticResource BrushCrust}"/>
        <Setter Property="BorderBrush"     Value="{StaticResource BrushMauve}"/>
        <Setter Property="FontWeight"      Value="{StaticResource FontWeightSemiBold}"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border x:Name="border"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="4"
                            Padding="{TemplateBinding Padding}"
                            SnapsToDevicePixels="True">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"
                                          RecognizesAccessKey="True"/>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="border" Property="Background" Value="{StaticResource BrushMauveHover}"/>
                            <Setter TargetName="border" Property="BorderBrush" Value="{StaticResource BrushMauveHover}"/>
                        </Trigger>
                        <Trigger Property="IsPressed" Value="True">
                            <Setter TargetName="border" Property="Background" Value="{StaticResource BrushMauvePressed}"/>
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="False">
                            <Setter TargetName="border" Property="Opacity" Value="0.4"/>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style x:Key="ButtonDanger" TargetType="Button" BasedOn="{StaticResource ButtonDefault}">
        <Setter Property="Background"      Value="{StaticResource BrushRed}"/>
        <Setter Property="Foreground"      Value="{StaticResource BrushCrust}"/>
        <Setter Property="BorderBrush"     Value="{StaticResource BrushRed}"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border x:Name="border"
                            Background="{TemplateBinding Background}"
                            BorderBrush="{TemplateBinding BorderBrush}"
                            BorderThickness="{TemplateBinding BorderThickness}"
                            CornerRadius="4"
                            Padding="{TemplateBinding Padding}"
                            SnapsToDevicePixels="True">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"
                                          RecognizesAccessKey="True"/>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="border" Property="Background" Value="{StaticResource BrushRedHover}"/>
                        </Trigger>
                        <Trigger Property="IsPressed" Value="True">
                            <Setter TargetName="border" Property="Opacity" Value="0.85"/>
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="False">
                            <Setter TargetName="border" Property="Opacity" Value="0.4"/>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!-- transparent remove button for folder chips -->
    <Style x:Key="ButtonGhost" TargetType="Button" BasedOn="{StaticResource ButtonDefault}">
        <Setter Property="Background"     Value="Transparent"/>
        <Setter Property="BorderBrush"    Value="Transparent"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="Foreground"     Value="{StaticResource BrushSubtext}"/>
        <Setter Property="Padding"        Value="3,1"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border x:Name="border" Background="Transparent" Padding="{TemplateBinding Padding}">
                        <ContentPresenter x:Name="content" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="content" Property="TextElement.Foreground" Value="{StaticResource BrushText}"/>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

</ResourceDictionary>
```

- [ ] **Step 2: Verify build**

```
dotnet build src/Imgdup.App/Imgdup.App.csproj
```
Expected: Build succeeded (Controls.xaml not yet wired — no errors expected).

- [ ] **Step 3: Commit**

```bash
git add src/Imgdup.App/Themes/Controls.xaml
git commit -m "feat(theme): Controls.xaml — Button styles (default, primary, danger, ghost)"
```

---

## Task 4: Controls.xaml — CheckBox, ComboBox, Slider, ProgressBar, ScrollBar, ListBox

**Files:**
- Modify: `src/Imgdup.App/Themes/Controls.xaml` (append styles before `</ResourceDictionary>`)

- [ ] **Step 1: Append CheckBox style**

Add before `</ResourceDictionary>`:

```xml
    <!-- ===== CHECKBOX ===== -->
    <Style TargetType="CheckBox">
        <Setter Property="Foreground"  Value="{StaticResource BrushText}"/>
        <Setter Property="FontFamily"  Value="{StaticResource FontFamilyDefault}"/>
        <Setter Property="FontSize"    Value="{StaticResource FontSizeBase}"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="CheckBox">
                    <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
                        <Border x:Name="box" Width="14" Height="14" CornerRadius="3"
                                Background="{StaticResource BrushSurface0}"
                                BorderBrush="{StaticResource BrushSurface1}"
                                BorderThickness="1" SnapsToDevicePixels="True">
                            <Path x:Name="tick" Visibility="Collapsed"
                                  Data="M 2 6 L 5 10 L 12 2"
                                  Stroke="{StaticResource BrushCrust}"
                                  StrokeThickness="2" StrokeStartLineCap="Round" StrokeEndLineCap="Round"/>
                        </Border>
                        <ContentPresenter Margin="6,0,0,0" VerticalAlignment="Center"/>
                    </StackPanel>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsChecked" Value="True">
                            <Setter TargetName="box"  Property="Background"   Value="{StaticResource BrushMauve}"/>
                            <Setter TargetName="box"  Property="BorderBrush"  Value="{StaticResource BrushMauve}"/>
                            <Setter TargetName="tick" Property="Visibility"   Value="Visible"/>
                        </Trigger>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="box" Property="BorderBrush" Value="{StaticResource BrushMauve}"/>
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="False">
                            <Setter TargetName="box" Property="Opacity" Value="0.4"/>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```

- [ ] **Step 2: Append Slider style**

```xml
    <!-- ===== SLIDER ===== -->
    <Style TargetType="Slider">
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Slider">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto" MinHeight="{TemplateBinding MinHeight}"/>
                            <RowDefinition Height="Auto"/>
                        </Grid.RowDefinitions>
                        <Track x:Name="PART_Track" Grid.Row="1">
                            <Track.DecreaseRepeatButton>
                                <RepeatButton Command="Slider.DecreaseLarge">
                                    <RepeatButton.Template>
                                        <ControlTemplate TargetType="RepeatButton">
                                            <Border Height="4" CornerRadius="2"
                                                    Background="{StaticResource BrushMauve}"/>
                                        </ControlTemplate>
                                    </RepeatButton.Template>
                                </RepeatButton>
                            </Track.DecreaseRepeatButton>
                            <Track.IncreaseRepeatButton>
                                <RepeatButton Command="Slider.IncreaseLarge">
                                    <RepeatButton.Template>
                                        <ControlTemplate TargetType="RepeatButton">
                                            <Border Height="4" CornerRadius="2"
                                                    Background="{StaticResource BrushSurface1}"/>
                                        </ControlTemplate>
                                    </RepeatButton.Template>
                                </RepeatButton>
                            </Track.IncreaseRepeatButton>
                            <Track.Thumb>
                                <Thumb>
                                    <Thumb.Template>
                                        <ControlTemplate TargetType="Thumb">
                                            <Border Width="14" Height="14" CornerRadius="7"
                                                    Background="{StaticResource BrushMauve}"
                                                    BorderThickness="0"/>
                                        </ControlTemplate>
                                    </Thumb.Template>
                                </Thumb>
                            </Track.Thumb>
                        </Track>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```

- [ ] **Step 3: Append ProgressBar style**

```xml
    <!-- ===== PROGRESSBAR ===== -->
    <Style TargetType="ProgressBar">
        <Setter Property="Height"  Value="6"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="ProgressBar">
                    <Border CornerRadius="3" Background="{StaticResource BrushSurface1}"
                            SnapsToDevicePixels="True">
                        <Border x:Name="PART_Indicator" HorizontalAlignment="Left"
                                CornerRadius="3" Background="{StaticResource BrushBlue}"/>
                    </Border>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```

- [ ] **Step 4: Append ScrollBar style**

```xml
    <!-- ===== SCROLLBAR ===== -->
    <Style TargetType="ScrollBar">
        <Setter Property="Width"      Value="6"/>
        <Setter Property="MinWidth"   Value="6"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="ScrollBar">
                    <Grid>
                        <Border CornerRadius="3" Background="Transparent"/>
                        <Track x:Name="PART_Track" IsDirectionReversed="True">
                            <Track.DecreaseRepeatButton>
                                <RepeatButton Command="ScrollBar.PageUpCommand" Opacity="0">
                                    <RepeatButton.Template>
                                        <ControlTemplate TargetType="RepeatButton">
                                            <Border Background="Transparent"/>
                                        </ControlTemplate>
                                    </RepeatButton.Template>
                                </RepeatButton>
                            </Track.DecreaseRepeatButton>
                            <Track.IncreaseRepeatButton>
                                <RepeatButton Command="ScrollBar.PageDownCommand" Opacity="0">
                                    <RepeatButton.Template>
                                        <ControlTemplate TargetType="RepeatButton">
                                            <Border Background="Transparent"/>
                                        </ControlTemplate>
                                    </RepeatButton.Template>
                                </RepeatButton>
                            </Track.IncreaseRepeatButton>
                            <Track.Thumb>
                                <Thumb>
                                    <Thumb.Template>
                                        <ControlTemplate TargetType="Thumb">
                                            <Border x:Name="thumb" CornerRadius="3"
                                                    Background="{StaticResource BrushSurface1}"
                                                    Margin="1,0"/>
                                            <ControlTemplate.Triggers>
                                                <Trigger Property="IsMouseOver" Value="True">
                                                    <Setter TargetName="thumb" Property="Background"
                                                            Value="{StaticResource BrushOverlay}"/>
                                                </Trigger>
                                            </ControlTemplate.Triggers>
                                        </ControlTemplate>
                                    </Thumb.Template>
                                </Thumb>
                            </Track.Thumb>
                        </Track>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```

- [ ] **Step 5: Append ComboBox style**

```xml
    <!-- ===== COMBOBOX ===== -->
    <Style TargetType="ComboBox">
        <Setter Property="Background"      Value="{StaticResource BrushSurface0}"/>
        <Setter Property="Foreground"      Value="{StaticResource BrushText}"/>
        <Setter Property="BorderBrush"     Value="{StaticResource BrushSurface1}"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="Padding"         Value="6,4"/>
        <Setter Property="FontFamily"      Value="{StaticResource FontFamilyDefault}"/>
        <Setter Property="FontSize"        Value="{StaticResource FontSizeBase}"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="ComboBox">
                    <Grid>
                        <ToggleButton x:Name="toggleButton"
                                      IsChecked="{Binding IsDropDownOpen, RelativeSource={RelativeSource TemplatedParent}, Mode=TwoWay}"
                                      Focusable="False" ClickMode="Press">
                            <ToggleButton.Template>
                                <ControlTemplate TargetType="ToggleButton">
                                    <Border x:Name="border"
                                            Background="{StaticResource BrushSurface0}"
                                            BorderBrush="{StaticResource BrushSurface1}"
                                            BorderThickness="1" CornerRadius="4">
                                        <Grid>
                                            <Grid.ColumnDefinitions>
                                                <ColumnDefinition Width="*"/>
                                                <ColumnDefinition Width="20"/>
                                            </Grid.ColumnDefinitions>
                                            <ContentPresenter Grid.Column="0"/>
                                            <Path Grid.Column="1" Data="M 0 0 L 4 4 L 8 0" Stroke="{StaticResource BrushSubtext}"
                                                  StrokeThickness="1.5" HorizontalAlignment="Center" VerticalAlignment="Center"/>
                                        </Grid>
                                    </Border>
                                    <ControlTemplate.Triggers>
                                        <Trigger Property="IsMouseOver" Value="True">
                                            <Setter TargetName="border" Property="BorderBrush" Value="{StaticResource BrushMauve}"/>
                                        </Trigger>
                                    </ControlTemplate.Triggers>
                                </ControlTemplate>
                            </ToggleButton.Template>
                        </ToggleButton>
                        <ContentPresenter x:Name="contentSite"
                                          Content="{TemplateBinding SelectionBoxItem}"
                                          ContentTemplate="{TemplateBinding SelectionBoxItemTemplate}"
                                          Margin="8,4,24,4"
                                          VerticalAlignment="Center"
                                          IsHitTestVisible="False"/>
                        <Popup x:Name="PART_Popup"
                               IsOpen="{TemplateBinding IsDropDownOpen}"
                               Placement="Bottom" AllowsTransparency="True" Focusable="False"
                               PopupAnimation="Slide">
                            <Border Background="{StaticResource BrushSurface0}"
                                    BorderBrush="{StaticResource BrushSurface1}"
                                    BorderThickness="1" CornerRadius="4"
                                    MinWidth="{TemplateBinding ActualWidth}">
                                <ScrollViewer MaxHeight="200">
                                    <ItemsPresenter/>
                                </ScrollViewer>
                            </Border>
                        </Popup>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
        <Style.Resources>
            <Style TargetType="ComboBoxItem">
                <Setter Property="Padding"     Value="8,4"/>
                <Setter Property="Foreground"  Value="{StaticResource BrushText}"/>
                <Setter Property="Background"  Value="Transparent"/>
                <Setter Property="Template">
                    <Setter.Value>
                        <ControlTemplate TargetType="ComboBoxItem">
                            <Border x:Name="border" Background="{TemplateBinding Background}"
                                    Padding="{TemplateBinding Padding}">
                                <ContentPresenter/>
                            </Border>
                            <ControlTemplate.Triggers>
                                <Trigger Property="IsMouseOver" Value="True">
                                    <Setter TargetName="border" Property="Background" Value="{StaticResource BrushMauveAlpha}"/>
                                </Trigger>
                                <Trigger Property="IsSelected" Value="True">
                                    <Setter TargetName="border" Property="Background" Value="{StaticResource BrushMauveAlpha}"/>
                                </Trigger>
                            </ControlTemplate.Triggers>
                        </ControlTemplate>
                    </Setter.Value>
                </Setter>
            </Style>
        </Style.Resources>
    </Style>

    <!-- ===== LISTBOX ===== -->
    <Style TargetType="ListBox">
        <Setter Property="Background"      Value="{StaticResource BrushBackground}"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="Foreground"      Value="{StaticResource BrushText}"/>
        <Setter Property="Padding"         Value="4"/>
    </Style>

    <Style TargetType="ListBoxItem">
        <Setter Property="Padding"     Value="0"/>
        <Setter Property="Background"  Value="Transparent"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="ListBoxItem">
                    <ContentPresenter/>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
```

- [ ] **Step 6: Commit**

```bash
git add src/Imgdup.App/Themes/Controls.xaml
git commit -m "feat(theme): Controls.xaml — CheckBox, Slider, ProgressBar, ScrollBar, ComboBox, ListBox"
```

---

## Task 5: DarkTheme.xaml + App.xaml

**Files:**
- Create: `src/Imgdup.App/Themes/DarkTheme.xaml`
- Modify: `src/Imgdup.App/App.xaml`

- [ ] **Step 1: Create DarkTheme.xaml**

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="ColorPalette.xaml"/>
        <ResourceDictionary Source="Typography.xaml"/>
        <ResourceDictionary Source="Controls.xaml"/>
    </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>
```

- [ ] **Step 2: Update App.xaml**

Replace entire `App.xaml` with:

```xml
<Application x:Class="Imgdup.App.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:Imgdup.App"
             xmlns:conv="clr-namespace:Imgdup.App.Converters">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Themes/DarkTheme.xaml"/>
            </ResourceDictionary.MergedDictionaries>
            <BooleanToVisibilityConverter x:Key="BoolToVisibility"/>
            <conv:EnumEqualsConverter     x:Key="EnumEquals"/>
            <conv:MatchKindToBrushConverter x:Key="MatchKindToBrush"/>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

- [ ] **Step 3: Build to verify theme wires up**

```
dotnet build src/Imgdup.App/Imgdup.App.csproj
```
Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add src/Imgdup.App/Themes/DarkTheme.xaml src/Imgdup.App/App.xaml
git commit -m "feat(theme): wire DarkTheme into App.xaml"
```

---

## Task 6: MainWindow.xaml — apply theme

**Files:**
- Modify: `src/Imgdup.App/MainWindow.xaml`

Replace the entire `MainWindow.xaml` content with the updated version below. Key changes vs current:
- `Window.Background` → `BrushBackground`
- `Window.Foreground` → `BrushText`
- `Window.FontFamily` → `FontFamilyDefault`
- Toolbar `Border.Background` → `BrushSurface0`, border → `BrushSurface1`
- "Verificar" button → `Style={StaticResource ButtonPrimary}`
- "Cancelar", "Adicionar pastas", "Selecionar extras", "Limpar seleção" → `Style={StaticResource ButtonDefault}`
- "Enviar p/ Lixeira" → `Style={StaticResource ButtonDanger}`, remove inline `Background`/`Foreground`
- Folder chip `Border.Background` → `BrushSurface1`, remove button `BorderThickness="0" Background="Transparent"` → use `Style={StaticResource ButtonGhost}`
- Status bar `Border.Background` → `BrushCrust`, `TextBlock.Foreground` → `BrushSubtext`
- `ListBox` bg handled by implicit style
- Group header `Border.Background` → `BrushSurface0`, add `BorderBrush="{StaticResource BrushMauve}" BorderThickness="3,0,0,0"`
- Card: `Border.Background` normal → `BrushSurface0`, selected → `BrushMauveAlpha`, selected border → `BrushYellow`
- Card metadata `TextBlock.Foreground` → `BrushSubtext` (size, dims, date)
- "Manter" badge `Background` → `BrushGreen`, `Foreground` → `BrushCrust`

- [ ] **Step 1: Replace MainWindow.xaml**

```xml
<Window x:Class="Imgdup.App.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:vm="clr-namespace:Imgdup.App.ViewModels"
        xmlns:vwp="clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanel"
        mc:Ignorable="d"
        d:DataContext="{d:DesignInstance vm:MainViewModel}"
        Title="Imgdup — Verificador de Imagens Duplicadas"
        Height="720" Width="1100" WindowStartupLocation="CenterScreen"
        Background="{StaticResource BrushBackground}"
        Foreground="{StaticResource BrushText}"
        FontFamily="{StaticResource FontFamilyDefault}"
        FontSize="{StaticResource FontSizeBase}">

    <DockPanel>
        <!-- ===== Toolbar ===== -->
        <Border DockPanel.Dock="Top" Padding="10,8"
                Background="{StaticResource BrushSurface0}"
                BorderBrush="{StaticResource BrushSurface1}"
                BorderThickness="0,0,0,1">
            <StackPanel>
                <StackPanel Orientation="Horizontal">
                    <Button Content="Adicionar pastas…" Command="{Binding PickFoldersCommand}"
                            Style="{StaticResource ButtonDefault}" Margin="0,0,8,0"/>
                    <Button Content="Verificar" Command="{Binding ScanCommand}"
                            Style="{StaticResource ButtonPrimary}" Margin="0,0,4,0"/>
                    <Button Content="Cancelar" Command="{Binding CancelCommand}"
                            Style="{StaticResource ButtonDefault}" Margin="0,0,16,0"/>

                    <CheckBox Content="Subpastas" IsChecked="{Binding Recursive}"
                              VerticalAlignment="Center" Margin="0,0,12,0"/>
                    <CheckBox Content="Similares (perceptual)" IsChecked="{Binding DetectNearDuplicates}"
                              VerticalAlignment="Center" Margin="0,0,8,0"/>
                    <TextBlock Text="Tolerância:" VerticalAlignment="Center" Margin="0,0,4,0"
                               Foreground="{StaticResource BrushSubtext}"/>
                    <Slider Width="100" Minimum="0" Maximum="16" TickFrequency="1"
                            IsSnapToTickEnabled="True" Value="{Binding NearThreshold}"
                            VerticalAlignment="Center"
                            IsEnabled="{Binding DetectNearDuplicates}"/>
                    <TextBlock Text="{Binding NearThreshold}" VerticalAlignment="Center"
                               Width="20" Margin="4,0,0,0"
                               Foreground="{StaticResource BrushText}"/>
                </StackPanel>

                <StackPanel Orientation="Horizontal" Margin="0,8,0,0">
                    <TextBlock Text="Visualização:" VerticalAlignment="Center" Margin="0,0,4,0"
                               Foreground="{StaticResource BrushSubtext}"/>
                    <ComboBox ItemsSource="{Binding ThumbnailSizes}" SelectedItem="{Binding SelectedThumbnailSize}"
                              Width="100" Margin="0,0,16,0"/>
                    <TextBlock Text="Ordenar:" VerticalAlignment="Center" Margin="0,0,4,0"
                               Foreground="{StaticResource BrushSubtext}"/>
                    <ComboBox ItemsSource="{Binding SortModes}" SelectedItem="{Binding SelectedSort}"
                              Width="150" Margin="0,0,16,0"/>

                    <Button Content="Selecionar extras" Command="{Binding SelectExtrasCommand}"
                            Style="{StaticResource ButtonDefault}" Margin="0,0,4,0"/>
                    <Button Content="Limpar seleção" Command="{Binding ClearSelectionCommand}"
                            Style="{StaticResource ButtonDefault}" Margin="0,0,16,0"/>
                    <Button Command="{Binding DeleteSelectedCommand}" Style="{StaticResource ButtonDanger}">
                        <TextBlock>
                            <Run Text="Enviar p/ Lixeira ("/><Run Text="{Binding SelectedCount, Mode=OneWay}"/><Run Text=")"/>
                        </TextBlock>
                    </Button>
                </StackPanel>

                <!-- Selected folders -->
                <ItemsControl ItemsSource="{Binding Folders}" Margin="0,8,0,0">
                    <ItemsControl.ItemsPanel>
                        <ItemsPanelTemplate><WrapPanel/></ItemsPanelTemplate>
                    </ItemsControl.ItemsPanel>
                    <ItemsControl.ItemTemplate>
                        <DataTemplate>
                            <Border Background="{StaticResource BrushSurface1}"
                                    CornerRadius="4" Padding="6,3" Margin="0,0,6,4">
                                <StackPanel Orientation="Horizontal">
                                    <TextBlock Text="{Binding}" VerticalAlignment="Center" ToolTip="{Binding}"
                                               MaxWidth="320" TextTrimming="CharacterEllipsis"
                                               Foreground="{StaticResource BrushText}"/>
                                    <Button Content="✕" Margin="6,0,0,0"
                                            Style="{StaticResource ButtonGhost}"
                                            Command="{Binding DataContext.RemoveFolderCommand, RelativeSource={RelativeSource AncestorType=ItemsControl}}"
                                            CommandParameter="{Binding}"/>
                                </StackPanel>
                            </Border>
                        </DataTemplate>
                    </ItemsControl.ItemTemplate>
                </ItemsControl>
            </StackPanel>
        </Border>

        <!-- ===== Status bar ===== -->
        <Border DockPanel.Dock="Bottom" Padding="10,5"
                Background="{StaticResource BrushCrust}"
                BorderBrush="{StaticResource BrushSurface1}"
                BorderThickness="0,1,0,0">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="200"/>
                </Grid.ColumnDefinitions>
                <TextBlock Text="{Binding StatusText}" VerticalAlignment="Center"
                           Foreground="{StaticResource BrushSubtext}"/>
                <ProgressBar Grid.Column="1" Minimum="0" Maximum="1"
                             Value="{Binding ProgressFraction}"
                             Visibility="{Binding IsScanning, Converter={StaticResource BoolToVisibility}}"/>
            </Grid>
        </Border>

        <!-- ===== Gallery ===== -->
        <ListBox x:Name="GalleryList"
                 ItemsSource="{Binding ItemsView}"
                 ScrollViewer.HorizontalScrollBarVisibility="Disabled"
                 ScrollViewer.CanContentScroll="True"
                 VirtualizingPanel.IsVirtualizing="True"
                 VirtualizingPanel.VirtualizationMode="Recycling"
                 VirtualizingPanel.IsVirtualizingWhenGrouping="True"
                 VirtualizingPanel.ScrollUnit="Pixel"
                 SelectionMode="Extended"
                 HorizontalContentAlignment="Stretch">

            <ListBox.ItemsPanel>
                <ItemsPanelTemplate>
                    <vwp:VirtualizingWrapPanel Orientation="Horizontal" SpacingMode="StartAndEndOnly"/>
                </ItemsPanelTemplate>
            </ListBox.ItemsPanel>

            <ListBox.GroupStyle>
                <GroupStyle>
                    <GroupStyle.HeaderTemplate>
                        <DataTemplate>
                            <Border Margin="4,12,4,4" Padding="8,6"
                                    Background="{StaticResource BrushSurface0}"
                                    BorderBrush="{StaticResource BrushMauve}"
                                    BorderThickness="3,0,0,0">
                                <TextBlock Text="{Binding Name.Header}"
                                           FontWeight="SemiBold"
                                           Foreground="{StaticResource BrushText}"/>
                            </Border>
                        </DataTemplate>
                    </GroupStyle.HeaderTemplate>
                </GroupStyle>
            </ListBox.GroupStyle>

            <ListBox.ItemTemplate>
                <DataTemplate DataType="{x:Type vm:ImageItemViewModel}">
                    <Border Padding="6" Margin="4" CornerRadius="6"
                            BorderThickness="1" BorderBrush="{StaticResource BrushSurface1}"
                            Loaded="OnItemLoaded">
                        <Border.Style>
                            <Style TargetType="Border">
                                <Setter Property="Background" Value="{StaticResource BrushSurface0}"/>
                                <Style.Triggers>
                                    <DataTrigger Binding="{Binding IsSelected}" Value="True">
                                        <Setter Property="Background"   Value="{StaticResource BrushMauveAlpha}"/>
                                        <Setter Property="BorderBrush"  Value="{StaticResource BrushYellow}"/>
                                        <Setter Property="BorderThickness" Value="2"/>
                                    </DataTrigger>
                                </Style.Triggers>
                            </Style>
                        </Border.Style>
                        <StackPanel Width="{Binding DataContext.ThumbnailPixels, RelativeSource={RelativeSource AncestorType=ListBox}}">
                            <Grid Height="{Binding DataContext.ThumbnailPixels, RelativeSource={RelativeSource AncestorType=ListBox}}">
                                <Image Source="{Binding Thumbnail}" Stretch="Uniform"
                                       RenderOptions.BitmapScalingMode="HighQuality"/>
                                <CheckBox IsChecked="{Binding IsSelected}"
                                          VerticalAlignment="Top" HorizontalAlignment="Left" Margin="2"/>
                                <Border VerticalAlignment="Top" HorizontalAlignment="Right"
                                        Margin="2" Padding="4,1"
                                        Background="{StaticResource BrushGreen}"
                                        CornerRadius="3"
                                        Visibility="{Binding IsKeepCandidate, Converter={StaticResource BoolToVisibility}}">
                                    <TextBlock Text="Manter"
                                               Foreground="{StaticResource BrushCrust}"
                                               FontSize="{StaticResource FontSizeSmall}"
                                               FontWeight="SemiBold"/>
                                </Border>
                            </Grid>
                            <TextBlock Text="{Binding FileName}"
                                       TextTrimming="CharacterEllipsis" ToolTip="{Binding Path}"
                                       FontSize="{StaticResource FontSizeBase}"
                                       Foreground="{StaticResource BrushText}"
                                       Margin="0,4,0,0"/>
                            <TextBlock Text="{Binding SizeDisplay}"
                                       Foreground="{StaticResource BrushSubtext}"
                                       FontSize="{StaticResource FontSizeSmall}"/>
                            <TextBlock Text="{Binding Dimensions}"
                                       Foreground="{StaticResource BrushSubtext}"
                                       FontSize="{StaticResource FontSizeSmall}"/>
                            <TextBlock Text="{Binding LastModified, StringFormat=g}"
                                       Foreground="{StaticResource BrushSubtext}"
                                       FontSize="{StaticResource FontSizeSmall}"/>
                        </StackPanel>
                    </Border>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </DockPanel>
</Window>
```

- [ ] **Step 2: Build**

```
dotnet build src/Imgdup.App/Imgdup.App.csproj
```
Expected: Build succeeded, 0 errors.

- [ ] **Step 3: Commit**

```bash
git add src/Imgdup.App/MainWindow.xaml
git commit -m "feat(theme): apply dark theme to MainWindow — Catppuccin Mocha + Mauve"
```

---

## Self-Review Checklist

- [x] ColorPalette: all brushes from spec present
- [x] Typography: all font keys present
- [x] Controls: Button (default/primary/danger/ghost), CheckBox, Slider, ProgressBar, ScrollBar, ComboBox, ListBox/ListBoxItem
- [x] DarkTheme merges all 3 files
- [x] App.xaml uses DarkTheme + retains existing converters
- [x] MainWindow: all 6 buttons use named styles, card selected state uses BrushMauveAlpha + BrushYellow, group header has left Mauve border, status bar uses BrushCrust, "Manter" badge uses BrushGreen/BrushCrust
- [x] No TBDs or placeholders
- [x] All brush key names consistent across files
