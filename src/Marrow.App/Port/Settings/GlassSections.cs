// PORT-ONLY, owner-requested (owner 2026-10-11, relayed by the lead: "add glass UI settings inside Customization, so the user can set it as he wishes";
// OD-0107; ED-NEW-glasslook-2) and owner-approved (OD-0147: "Glass UI Approved, ship it with option for user to be able to set all those settings ...
// but ship with its current default values"): the Customization page's Glass section, the design canvas's Glass settings panel
// (E:/Droppy/work/design/glass-ui/index.html, "Settings > Customization > Glass, as the app will ship it") with its words, ranges, steps and units:
// the live preview tile, the Look (Clear, Regular, Tinted) and the Transparency slider between them, the eight fine-tuning sliders with the Custom
// mark and Reset, the behaviour (tint colour, motion, Increase contrast, Reduce transparency, Translucent sidebar) and Reset to defaults. Every control
// writes the app's glass setting, which the settings mirror into the material: every glass piece of every window redraws with new uniforms on the
// next frame, no new program. A Look, a switch or a reset glides every surface to it (GlassLooks.Blend) while its sliders glide on the page; a drag
// is live; the store is written when a drag or a glide ends.
using Avalonia.Input;
using Avalonia.Interactivity;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;
using Marrow.UI.Port.Settings;

namespace Marrow.App.Port.Settings;

internal sealed class GlassSections
{
    internal const string Heading = "Glass";

    private readonly Func<GlassSettings> read;
    private readonly Action<GlassSettings> write;
    private readonly Action commit;
    private readonly CzNumber glide;
    private GlassSettings glideFrom = GlassSettings.Default, glideTo = GlassSettings.Default;
    private bool showing;

    internal CzSection Section { get; }
    internal GlassPreviewTile Preview { get; }
    internal CzSegmentRow Look { get; }
    internal CzSlider Transparency { get; }
    internal CzRow FineTuning { get; }
    internal CzCapsuleButton ResetFineTuning { get; }
    internal IReadOnlyDictionary<string, CzSlider> Sliders { get; }
    internal CzMenuButton TintColour { get; }
    internal CzSegmentRow Motion { get; }
    internal CzToggleRow IncreaseContrast { get; }
    internal CzToggleRow ReduceTransparency { get; }
    internal CzToggleRow TranslucentSidebar { get; }
    internal CzSegmentRow Desktop { get; }
    internal CzCapsuleButton Reset { get; }
    internal CzSegmentRow Engine { get; }
    internal CzSegmentRow RizzyPreset { get; }
    internal IReadOnlyDictionary<string, CzSlider> RizzySliders { get; }
    internal CzToggleRow LiquidGlass { get; }
    internal CzSegmentRow DasherPreset { get; }
    internal IReadOnlyDictionary<string, CzSlider> DasherSliders { get; }
    internal CzToggleRow DasherWarp { get; }
    internal CzSegmentRow HqPreset { get; }
    internal CzSegmentRow HqMaterial { get; }
    internal CzSegmentRow HqAppearance { get; }
    internal CzMenuButton HqTint { get; }
    internal CzToggleRow HqConcentric { get; }
    internal CzToggleRow HqInteractive { get; }
    internal CzSegmentRow HqMotion { get; }
    internal IReadOnlyDictionary<string, CzSlider> HqSliders { get; }
    internal CzSlider Intensity { get; }
    internal CzSlider SeeThrough { get; }
    private readonly Func<double> readSeeThrough;

    // ED-NEW-glass-alive-8: each engine's rows, shown only under its engine (the picker's choice), folding in and out.
    private readonly List<(CzRowSlot Slot, GlassEngine Engine)> engineRows = [];

    /// <param name="read">The app's glass setting; read inside a tracked read by the page.</param>
    /// <param name="write">Puts a new glass setting in the app's settings (live: the material follows at once).</param>
    /// <param name="commit">The settings' coalesced write goes out now (a drag, a key or a glide has ended).</param>
    /// <param name="seeThrough">ED-NEW-glass-layers-1: how much of the desktop shows behind the whole app, 0 solid to 1 fully see-through (the
    /// window's Transparency, read the other way round), and where a new value goes; null in a page that has none.</param>
    internal GlassSections(Func<GlassSettings> read, Action<GlassSettings> write, Action commit, (Func<double> Read, Action<double> Write)? seeThrough = null)
    {
        this.read = read;
        this.write = write;
        this.commit = commit;
        var settings = read().Held();
        var rows = new CzRowStack();

        Preview = new GlassPreviewTile();
        rows.Add(Preview);
        glide = new CzNumber(new CzMotion(Preview), 0, Glided); // the tile lives and leaves with the section

        // ED-NEW-glass-alive-5: the engine that draws every glass surface, and liquid-glass's own options below.
        Engine = new CzSegmentRow(GlassText.Engine, GlassText.EngineDetail, GlassSettings.Engines.Select(e => (e.Id, e.Name)).ToArray(),
            GlassSettings.IdOf(settings.Engine), id =>
            {
                if (GlassSettings.Engines.FirstOrDefault(e => e.Id == id) is not { Id: not null } engine) return;
                if (glide.IsMoving) glide.Jump(1);
                write(read() with { Engine = engine.Value });
                commit();
            });
        rows.Add(Engine.Row);
        // ED-NEW-glass-alive-8: the Liquid glass switch, for the whole UI (off: the classic canvas material whatever the engine)
        LiquidGlass = new CzToggleRow(GlassText.LiquidGlass, GlassText.LiquidGlassDetail, () => read().Liquid, on => Switch(s => s with { Liquid = on }));
        rows.Add(LiquidGlass.Row);
        // ED-NEW-glass-layers-1: Glass intensity (the liquid engines' bend, fringe and rim, as a multiple of their own) and See-through (the desktop
        // behind the whole app), for every engine
        var ik = GlassSettings.IntensityKnob;
        Intensity = Slider(ik, settings.Intensity, value => write(read() with { Intensity = value }));
        Avalonia.Automation.AutomationProperties.SetName(Intensity, ik.Name);
        rows.Add(new CzRow(ik.Name, ik.Description, Intensity, dimmed => Intensity.Dimmed = dimmed));
        var (readSee, writeSee) = seeThrough ?? (() => 1 - GlassSeeThrough.DefaultOpacity, static _ => { });
        readSeeThrough = readSee;
        var sk = GlassSeeThrough.Knob;
        SeeThrough = Slider(sk, readSee(), value => writeSee(sk.Hold(value)));
        Avalonia.Automation.AutomationProperties.SetName(SeeThrough, sk.Name);
        rows.Add(new CzRow(sk.Name, sk.Description, SeeThrough, dimmed => SeeThrough.Dimmed = dimmed));
        CzRowSlot For(GlassEngine engine, Avalonia.Controls.Control row, bool caption = false)
        {
            var slot = rows.Add(row, caption, open: settings.Drawn == engine);
            engineRows.Add((slot, engine));
            return slot;
        }

        Look = new CzSegmentRow(GlassText.Look, GlassText.LookDetail(settings), GlassSettings.Looks.Select(l => (l.Id, l.Name)).ToArray(),
            settings.Look is { } look ? GlassSettings.IdOf(look) : "regular", Pick);
        if (settings.Look is null) Look.Control.ShowNone();
        For(GlassEngine.Canvas, Look.Row);

        var t = GlassSettings.TransparencyKnob;
        Transparency = Slider(t, settings.Transparency, value => write(read().At(value)), GlassText.TransparencyValue);
        Avalonia.Automation.AutomationProperties.SetName(Transparency, GlassText.TransparencyName);
        For(GlassEngine.Canvas, new CzRow(t.Name, t.Description, Transparency, dimmed => Transparency.Dimmed = dimmed));

        For(GlassEngine.Canvas, new CzCaptionRow(GlassText.FineTuningCaption), caption: true);
        ResetFineTuning = new CzCapsuleButton(GlassText.ResetFineTuning, detail: GlassText.ResetFineTuningDetail);
        ResetFineTuning.Invoked += () => Glide((glide.IsMoving ? glideTo : read()).ResetFineTuning());
        ResetFineTuning.Dimmed = !settings.IsCustom;
        FineTuning = new CzRow(GlassText.FineTuning, GlassText.FineTuningDetail(settings.IsCustom), ResetFineTuning, dimmed => ResetFineTuning.Dimmed = dimmed || !read().IsCustom);
        For(GlassEngine.Canvas, FineTuning);
        var sliders = new Dictionary<string, CzSlider>();
        foreach (var knob in GlassSettings.Knobs)
        {
            var id = knob.Id;
            var slider = Slider(knob, settings.Get(id), value => write(read().With(id, value)));
            sliders[id] = slider;
            For(GlassEngine.Canvas, new CzRow(knob.Name, knob.Description, slider, dimmed => slider.Dimmed = dimmed));
        }
        Sliders = sliders;

        For(GlassEngine.Rizzy, new CzCaptionRow(GlassText.RizzyCaption), caption: true);
        RizzyPreset = new CzSegmentRow(GlassText.RizzyPreset, GlassText.RizzyPresetDetail(settings.Rizzy), RizzyGlass.Presets.Select(p => (p.Id, p.Name)).ToArray(),
            PresetId(settings.Rizzy) ?? "balanced", id =>
            {
                if (RizzyGlass.Presets.FirstOrDefault(p => p.Id == id) is not { Id: not null } preset) return;
                var now = glide.IsMoving ? glideTo : read();
                if (now.Rizzy != RizzyGlass.Of(preset.Value)) Glide(now with { Rizzy = RizzyGlass.Of(preset.Value) });
            });
        if (PresetId(settings.Rizzy) is null) RizzyPreset.Control.ShowNone();
        For(GlassEngine.Rizzy, RizzyPreset.Row);
        var rizzySliders = new Dictionary<string, CzSlider>();
        foreach (var knob in RizzyGlass.Knobs)
        {
            var id = knob.Id;
            var slider = Slider(knob, settings.Rizzy.Get(id), value => write(read() with { Rizzy = read().Rizzy.With(id, value) }));
            Avalonia.Automation.AutomationProperties.SetName(slider, GlassText.RizzyCaption + ", " + knob.Name);
            rizzySliders[id] = slider;
            For(GlassEngine.Rizzy, new CzRow(knob.Name, knob.Description, slider, dimmed => slider.Dimmed = dimmed));
        }
        RizzySliders = rizzySliders;

        // ED-NEW-glass-alive-6: dashersw/liquid-glass-js's own options
        For(GlassEngine.Dasher, new CzCaptionRow(GlassText.DasherCaption), caption: true);
        DasherPreset = new CzSegmentRow(GlassText.RizzyPreset, GlassText.EnginePresetDetail(settings.Dasher.Preset is null), DasherGlass.Presets.Select(p => (p.Id, p.Name)).ToArray(),
            DasherPresetId(settings.Dasher) ?? "balanced", id =>
            {
                if (DasherGlass.Presets.FirstOrDefault(p => p.Id == id) is not { Id: not null } preset) return;
                var now = glide.IsMoving ? glideTo : read();
                if (now.Dasher != DasherGlass.Of(preset.Value)) Glide(now with { Dasher = DasherGlass.Of(preset.Value) });
            });
        if (DasherPresetId(settings.Dasher) is null) DasherPreset.Control.ShowNone();
        For(GlassEngine.Dasher, DasherPreset.Row);
        var dasherSliders = new Dictionary<string, CzSlider>();
        foreach (var knob in DasherGlass.Knobs)
        {
            var id = knob.Id;
            var slider = Slider(knob, settings.Dasher.Get(id), value => write(read() with { Dasher = read().Dasher.With(id, value) }));
            Avalonia.Automation.AutomationProperties.SetName(slider, GlassText.DasherCaption + ", " + knob.Name);
            dasherSliders[id] = slider;
            For(GlassEngine.Dasher, new CzRow(knob.Name, knob.Description, slider, dimmed => slider.Dimmed = dimmed));
        }
        DasherSliders = dasherSliders;
        DasherWarp = new CzToggleRow(GlassText.Warp, GlassText.WarpDetail, () => read().Dasher.Warp, on => Switch(s => s with { Dasher = s.Dasher with { Warp = on } }));
        For(GlassEngine.Dasher, DasherWarp.Row);

        // ED-NEW-glass-alive-7: Glass-HQ/liquid-glass's own options
        For(GlassEngine.GlassHq, new CzCaptionRow(GlassText.HqCaption), caption: true);
        HqPreset = new CzSegmentRow(GlassText.RizzyPreset, GlassText.EnginePresetDetail(settings.GlassHq.Preset is null), GlassHqGlass.Presets.Select(p => (p.Id, p.Name)).ToArray(),
            settings.GlassHq.Preset ?? "regular", id =>
            {
                if (GlassHqGlass.Presets.FirstOrDefault(p => p.Id == id) is not { Id: not null } preset) return;
                var now = glide.IsMoving ? glideTo : read();
                if (now.GlassHq != preset.Value) Glide(now with { GlassHq = preset.Value });
            });
        if (settings.GlassHq.Preset is null) HqPreset.Control.ShowNone();
        For(GlassEngine.GlassHq, HqPreset.Row);
        HqMaterial = new CzSegmentRow(GlassText.HqMaterial, GlassText.HqMaterialDetail, GlassHqGlass.Materials.Select(m => (m.Id, m.Name)).ToArray(),
            GlassHqGlass.Materials.First(m => m.Value == settings.GlassHq.Material).Id, id =>
            {
                if (GlassHqGlass.Materials.FirstOrDefault(m => m.Id == id) is not { Id: not null } m) return;
                Switch(s => s with { GlassHq = s.GlassHq with { Material = m.Value } });
            });
        For(GlassEngine.GlassHq, HqMaterial.Row);
        HqAppearance = new CzSegmentRow(GlassText.HqAppearance, GlassText.HqAppearanceDetail, GlassHqGlass.Appearances.Select(m => (m.Id, m.Name)).ToArray(),
            GlassHqGlass.Appearances.First(m => m.Value == settings.GlassHq.Appearance).Id, id =>
            {
                if (GlassHqGlass.Appearances.FirstOrDefault(m => m.Id == id) is not { Id: not null } m) return;
                Switch(s => s with { GlassHq = s.GlassHq with { Appearance = m.Value } });
            });
        For(GlassEngine.GlassHq, HqAppearance.Row);
        var hqSliders = new Dictionary<string, CzSlider>();
        foreach (var knob in GlassHqGlass.Knobs)
        {
            var id = knob.Id;
            var slider = Slider(knob, settings.GlassHq.Get(id), value => write(read() with { GlassHq = read().GlassHq.With(id, value) }));
            Avalonia.Automation.AutomationProperties.SetName(slider, GlassText.HqCaption + ", " + knob.Name);
            hqSliders[id] = slider;
            For(GlassEngine.GlassHq, new CzRow(knob.Name, knob.Description, slider, dimmed => slider.Dimmed = dimmed));
        }
        HqSliders = hqSliders;
        HqTint = new CzMenuButton(GlassText.HqTint, GlassSettings.TintColours.Select(c => (c.Id, c.Name)).ToArray(), IdOf(settings.GlassHq.Tint), GlassText.HqTintDetail);
        HqTint.SelectionChanged += id =>
        {
            if (showing || GlassSettings.TintColours.FirstOrDefault(c => c.Id == id) is not { Id: not null } colour) return;
            Switch(s => s with { GlassHq = s.GlassHq with { Tint = colour.Value } });
        };
        For(GlassEngine.GlassHq, new CzRow(GlassText.HqTint, GlassText.HqTintDetail, HqTint, dimmed => HqTint.Dimmed = dimmed));
        HqConcentric = new CzToggleRow(GlassText.HqConcentric, GlassText.HqConcentricDetail, () => read().GlassHq.Concentric,
            on => Switch(s => s with { GlassHq = s.GlassHq with { Concentric = on } }));
        For(GlassEngine.GlassHq, HqConcentric.Row);
        HqInteractive = new CzToggleRow(GlassText.HqInteractive, GlassText.HqInteractiveDetail, () => read().GlassHq.Interactive,
            on => Switch(s => s with { GlassHq = s.GlassHq with { Interactive = on } }));
        For(GlassEngine.GlassHq, HqInteractive.Row);
        HqMotion = new CzSegmentRow(GlassText.HqMotion, GlassText.HqMotionDetail, GlassHqGlass.Motions.Select(m => (m.Id, m.Name)).ToArray(),
            GlassHqGlass.Motions.First(m => m.Value == settings.GlassHq.Motion).Id, id =>
            {
                if (GlassHqGlass.Motions.FirstOrDefault(m => m.Id == id) is not { Id: not null } m) return;
                Switch(s => s with { GlassHq = s.GlassHq with { Motion = m.Value } });
            });
        For(GlassEngine.GlassHq, HqMotion.Row);

        rows.Add(new CzCaptionRow(GlassText.BehaviourCaption));
        TintColour = new CzMenuButton(GlassText.TintColour, GlassSettings.TintColours.Select(c => (c.Id, c.Name)).ToArray(), IdOf(settings.TintColour), GlassText.TintColourDetail);
        TintColour.SelectionChanged += id =>
        {
            if (showing || GlassSettings.TintColours.FirstOrDefault(c => c.Id == id) is not { Id: not null } colour) return;
            var now = glide.IsMoving ? glideTo : read();
            if (now.TintColour != colour.Value) Glide(now with { TintColour = colour.Value });
        };
        rows.Add(new CzRow(GlassText.TintColour, GlassText.TintColourDetail, TintColour, dimmed => TintColour.Dimmed = dimmed));
        Motion = new CzSegmentRow(GlassText.Motion, GlassText.MotionDetail, [("full", GlassText.MotionFull), ("reduced", GlassText.MotionReduced)],
            ReducedMotion(settings) ? "reduced" : "full", id =>
            {
                if (glide.IsMoving) glide.Jump(1);
                write(read() with { ReducedMotion = id == "reduced" });
                commit();
            });
        rows.Add(Motion.Row);
        IncreaseContrast = new CzToggleRow(GlassText.IncreaseContrast, GlassText.IncreaseContrastDetail,
            () => read().IncreaseContrast ?? GlassLooks.System.IncreaseContrast, on => Switch(s => s with { IncreaseContrast = on }));
        rows.Add(IncreaseContrast.Row);
        ReduceTransparency = new CzToggleRow(GlassText.ReduceTransparency, GlassText.ReduceTransparencyDetail,
            () => read().ReduceTransparency ?? GlassLooks.System.ReduceTransparency, on => Switch(s => s with { ReduceTransparency = on }));
        rows.Add(ReduceTransparency.Row);
        TranslucentSidebar = new CzToggleRow(GlassText.TranslucentSidebar, GlassText.TranslucentSidebarDetail,
            () => read().TranslucentSidebar, on => Switch(s => s with { TranslucentSidebar = on }));
        rows.Add(TranslucentSidebar.Row);
        // ED-NEW-glass-alive-2 (rows #87, #32): the desktop behind the window, Clear (the default) or Frosted by Windows (the Acrylic it was).
        Desktop = new CzSegmentRow(GlassText.Desktop, GlassText.DesktopDetail, [("clear", GlassText.DesktopClear), ("frosted", GlassText.DesktopFrosted)],
            DesktopId(settings), id =>
            {
                if (glide.IsMoving) glide.Jump(1);
                write(read() with { Desktop = id == "frosted" ? DesktopBackdrop.Frosted : DesktopBackdrop.Clear });
                commit();
            });
        rows.Add(Desktop.Row);

        Reset = new CzCapsuleButton(GlassText.Reset, detail: GlassText.ResetDetail);
        Reset.Invoked += () => Glide(GlassSettings.Default);
        Reset.Dimmed = settings == GlassSettings.Default;
        rows.Add(new CzRow(GlassText.Defaults, GlassText.ResetDetail, Reset));
        rows.Add(new CzNoteRow(GlassText.Note));

        Section = new CzSection(Heading, rows, open: true);
    }

    private CzSlider Slider(GlassKnob knob, double value, Action<double> edit, Func<double, string>? format = null)
    {
        var slider = new CzSlider(knob.Name, knob.Min, knob.Max, knob.Step, value, format: format ?? knob.Format, detail: knob.Description);
        slider.ValueChanged += v =>
        {
            if (showing || v is not { } picked) return;
            if (glide.IsMoving) glide.Jump(1); // a slider touched mid-glide: the glide lands first, then the slider's value
            edit(picked);
        };
        // a drag has ended, or a key has been pressed and lifted: the settings' coalesced write goes out now
        slider.AddHandler(InputElement.PointerReleasedEvent, (_, _) => commit(), RoutingStrategies.Bubble, handledEventsToo: true);
        slider.AddHandler(InputElement.KeyUpEvent, (_, _) => commit(), RoutingStrategies.Bubble, handledEventsToo: true);
        return slider;
    }

    private static string IdOf(GlassTintColour colour) => GlassSettings.TintColours.First(c => c.Value == colour).Id;

    private static bool ReducedMotion(GlassSettings settings) => settings.ReducedMotion ?? GlassLooks.System.ReduceMotion;

    /// <summary>Shows what the setting says now in every row (a change from elsewhere, a glide's frame, a slider's own echo); during a glide, the
    /// glide's own point.</summary>
    internal void Apply() => Show(glide.IsMoving ? glideFrom.Toward(glideTo, glide.Value) : read().Held());

    private void Show(GlassSettings settings)
    {
        showing = true;
        try
        {
            if (Transparency.Value != settings.Transparency) Transparency.Update(settings.Transparency, null);
            if (Intensity.Value != settings.Intensity) Intensity.Update(settings.Intensity, null);
            if (SeeThrough.Value != readSeeThrough()) SeeThrough.Update(readSeeThrough(), null);
            foreach (var (id, slider) in Sliders)
            {
                var value = settings.Get(id);
                if (slider.Value != value) slider.Update(value, null);
            }
            if (settings.Look is { } look)
            {
                var id = GlassSettings.IdOf(look);
                if (Look.Control.SelectedId != id) Look.Apply(id, GlassText.LookDetail(settings));
            }
            else
            {
                Look.Control.ShowNone();
                Look.Row.SetDetail(GlassText.LookDetail(settings));
            }
            var tint = IdOf(settings.TintColour);
            if (TintColour.SelectedId != tint) TintColour.Show(tint);
            var motion = ReducedMotion(settings) ? "reduced" : "full";
            if (Motion.Control.SelectedId != motion) Motion.Apply(motion, GlassText.MotionDetail);
            var engine = GlassSettings.IdOf(settings.Engine);
            if (Engine.Control.SelectedId != engine) Engine.Apply(engine, GlassText.EngineDetail);
            foreach (var (id, slider) in RizzySliders)
            {
                var value = settings.Rizzy.Get(id);
                if (slider.Value != value) slider.Update(value, null);
            }
            if (PresetId(settings.Rizzy) is { } preset)
            {
                if (RizzyPreset.Control.SelectedId != preset) RizzyPreset.Apply(preset, GlassText.RizzyPresetDetail(settings.Rizzy));
            }
            else
            {
                RizzyPreset.Control.ShowNone();
                RizzyPreset.Row.SetDetail(GlassText.RizzyPresetDetail(settings.Rizzy));
            }
            var desktop = DesktopId(settings);
            if (Desktop.Control.SelectedId != desktop) Desktop.Apply(desktop, GlassText.DesktopDetail);
            foreach (var (id, slider) in DasherSliders)
                if (slider.Value != settings.Dasher.Get(id)) slider.Update(settings.Dasher.Get(id), null);
            if (DasherPresetId(settings.Dasher) is { } dasher) { if (DasherPreset.Control.SelectedId != dasher) DasherPreset.Apply(dasher, GlassText.EnginePresetDetail(false)); }
            else { DasherPreset.Control.ShowNone(); DasherPreset.Row.SetDetail(GlassText.EnginePresetDetail(true)); }
            foreach (var (id, slider) in HqSliders)
                if (slider.Value != settings.GlassHq.Get(id)) slider.Update(settings.GlassHq.Get(id), null);
            if (settings.GlassHq.Preset is { } hq) { if (HqPreset.Control.SelectedId != hq) HqPreset.Apply(hq, GlassText.EnginePresetDetail(false)); }
            else { HqPreset.Control.ShowNone(); HqPreset.Row.SetDetail(GlassText.EnginePresetDetail(true)); }
            var material = GlassHqGlass.Materials.First(m => m.Value == settings.GlassHq.Material).Id;
            if (HqMaterial.Control.SelectedId != material) HqMaterial.Apply(material, GlassText.HqMaterialDetail);
            var appearance = GlassHqGlass.Appearances.First(m => m.Value == settings.GlassHq.Appearance).Id;
            if (HqAppearance.Control.SelectedId != appearance) HqAppearance.Apply(appearance, GlassText.HqAppearanceDetail);
            var hqMotion = GlassHqGlass.Motions.First(m => m.Value == settings.GlassHq.Motion).Id;
            if (HqMotion.Control.SelectedId != hqMotion) HqMotion.Apply(hqMotion, GlassText.HqMotionDetail);
            var hqTint = IdOf(settings.GlassHq.Tint);
            if (HqTint.SelectedId != hqTint) HqTint.Show(hqTint);
            foreach (var (slot, rowEngine) in engineRows) slot.Open = settings.Drawn == rowEngine;
        }
        finally { showing = false; }
        var custom = read().IsCustom;
        ResetFineTuning.Dimmed = !custom;
        FineTuning.SetDetail(GlassText.FineTuningDetail(custom));
        Reset.Dimmed = read().Held() == GlassSettings.Default;
    }

    private static string? DasherPresetId(DasherGlass dasher) => dasher.Preset is { } preset ? DasherGlass.Presets.First(p => p.Value == preset).Id : null;

    private static string? PresetId(RizzyGlass rizzy) => rizzy.Preset is { } preset ? RizzyGlass.Presets.First(p => p.Value == preset).Id : null;

    private static string DesktopId(GlassSettings settings) => settings.Desktop == DesktopBackdrop.Frosted ? "frosted" : "clear";

    private void Pick(string id)
    {
        var look = GlassSettings.Looks.FirstOrDefault(l => l.Id == id);
        if (look.Id is null) return;
        var now = glide.IsMoving ? glideTo : read();
        if (now.Look == look.Value && !now.IsCustom) return;
        Glide(now.At(look.Value)); // every fine-tuning value the stop's own (the canvas's setG); the switches stay
    }

    private void Switch(Func<GlassSettings, GlassSettings> change)
    {
        var now = glide.IsMoving ? glideTo : read();
        var next = change(now);
        if (next != now) Glide(next);
    }

    /// <summary>Glides from where the glass is to <paramref name="to"/> (0.4 s, the page's easing; at once under Reduce motion): the setting is
    /// <paramref name="to"/> at once (stored when the glide ends), every surface crossfades to it (GlassLooks.Blend), and the sliders glide.</summary>
    internal void Glide(GlassSettings to)
    {
        if (glide.IsMoving) glide.Jump(1);
        glideFrom = read().Held();
        glideTo = to.Held();
        if (glideFrom == glideTo) return;
        GlassLooks.Blend(glideFrom, 0);
        write(glideTo);
        glide.Jump(0);
        glide.To(1, CzMotion.Ez(0.4));
    }

    private void Glided(double t)
    {
        if (t <= 0) return;
        GlassLooks.Blend(glideFrom, t);
        Show(glideFrom.Toward(glideTo, t));
        if (t >= 1) commit();
    }
}

/// <summary>The words of the Glass section: the design canvas's (OD-0147).</summary>
/// <summary>ED-NEW-glass-layers-1: the See-through slider: the window's Transparency (its backdrop's opacity) read the other way round, so it and
/// the Window page's Transparency are one setting.</summary>
internal static class GlassSeeThrough
{
    internal const double DefaultOpacity = 0.8;

    internal static GlassKnob Knob { get; } = new("seeThrough", "See-through",
        "How much of the Windows desktop shows behind the whole app, from solid to fully see-through", 0, 1, 0.01, GlassUnit.Plain3);

    /// <summary>The slider's value for a backdrop opacity, and back.</summary>
    internal static double Of(double opacity) => Math.Clamp(1 - opacity, 0, 1);

    internal static double OpacityOf(double seeThrough) => Math.Clamp(1 - seeThrough, 0, 1);
}

internal static class GlassText
{
    internal const string Look = "Look";
    /// <summary>The Transparency slider's accessible name (the canvas's aria-label).</summary>
    internal const string TransparencyName = "Liquid Glass, more clear to more tinted";
    internal const string FineTuningCaption = "Fine-tuning";
    internal const string FineTuning = "Fine-tuning";
    internal const string ResetFineTuning = "Reset";
    internal const string ResetFineTuningDetail = "Every fine-tuning value back to the look's own.";
    internal const string BehaviourCaption = "Behaviour";
    internal const string TintColour = "Tint colour";
    internal const string TintColourDetail = "A small colour cast, capped so it stays glass.";
    internal const string Motion = "Motion";
    internal const string MotionDetail = "Bulge on press and lens knobs.";
    internal const string MotionFull = "Full";
    internal const string MotionReduced = "Reduced";
    internal const string IncreaseContrast = "Increase contrast";
    internal const string IncreaseContrastDetail = "Sharper edges and a stronger line of light.";
    internal const string ReduceTransparency = "Reduce transparency";
    internal const string ReduceTransparencyDetail = "Solid surfaces instead of glass.";
    internal const string TranslucentSidebar = "Translucent sidebar";
    internal const string TranslucentSidebarDetail = "Let the window behind tint the sidebar.";
    internal const string Desktop = "Desktop behind the window";
    internal const string DesktopDetail = "Clear shows the desktop as it is; Frosted lets Windows blur it.";
    internal const string DesktopClear = "Clear";
    internal const string DesktopFrosted = "Frosted";
    internal const string Engine = "Engine";
    internal const string EngineDetail = "The liquid glass every surface is drawn with: Rizzy, Dasher or Glass HQ, each with its own options; Canvas is the classic material.";
    internal const string RizzyCaption = "Rizzy (rizzytoday/liquid-glass)";
    internal const string LiquidGlass = "Liquid glass";
    internal const string LiquidGlassDetail = "The liquid lens on every glass surface. Off, the glass is the classic canvas material.";
    internal const string DasherCaption = "Dasher (dashersw/liquid-glass-js)";
    internal const string HqCaption = "Glass HQ (Glass-HQ/liquid-glass)";
    internal static string EnginePresetDetail(bool custom) => custom ? "Custom" : "The repository's options, and steps around them.";
    internal const string Warp = "Warp";
    internal const string WarpDetail = "The centre bends too, not only the rim.";
    internal const string HqMaterial = "Material";
    internal const string HqMaterialDetail = "Regular frosts and fills for text; Clear is the most see-through.";
    internal const string HqAppearance = "Appearance";
    internal const string HqAppearanceDetail = "The glass's light or dark tone; Theme follows nth's.";
    internal const string HqTint = "Tint";
    internal const string HqTintDetail = "A glass colour; text and icons stay as they are.";
    internal const string HqConcentric = "Concentric";
    internal const string HqConcentricDetail = "A piece on glass follows its container, and its bend stays within its inset.";
    internal const string HqInteractive = "Interactive";
    internal const string HqInteractiveDetail = "Press and pointer feedback: the light follows the pointer.";
    internal const string HqMotion = "Motion";
    internal const string HqMotionDetail = "Full swells the press; Reduced only fades; None is still. Reduce Motion always applies.";
    internal const string RizzyPreset = "Preset";
    internal static string RizzyPresetDetail(RizzyGlass rizzy) => rizzy.Preset is null ? "Custom" : "rizzytoday/liquid-glass's options, stepped around its own.";
    internal const string Defaults = "Defaults";
    internal const string Reset = "Reset to defaults";
    internal const string ResetDetail = "Regular, every value at its default, the switches following Windows.";
    internal const string Note = "Glass changes apply live to every glass surface in every window. Text on glass keeps a contrast of at least 4.5:1 at any setting.";

    /// <summary>The Look row's detail (the canvas's): what the slider's position is.</summary>
    internal static string LookDetail(GlassSettings settings) => settings.Look switch
    {
        GlassVariant.Clear => "More clear: the most see-through.",
        GlassVariant.Tinted => "More tinted: frostier, reads best.",
        GlassVariant.Regular => "Default, the iOS 27 midpoint.",
        _ => "Between two looks.",
    };

    /// <summary>The Transparency slider's value as the canvas shows it: the Look at a stop ("Regular, default" at the default), else the nearest
    /// look's name, then the position: "Regular, default (50)", "More clear (12)".</summary>
    internal static string TransparencyValue(double transparency)
    {
        var at = Math.Round(transparency);
        var stop = GlassSettings.Looks.FirstOrDefault(l => Math.Abs(l.At - at) < 0.5);
        var name = stop.Id is null ? GlassSettings.LookName(at) : stop.Value == GlassVariant.Regular ? "Regular, default" : stop.Name;
        return $"{name} ({at.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
    }

    /// <summary>The fine-tuning row's detail: the canvas's Custom mark when a value differs from the look's own.</summary>
    internal static string FineTuningDetail(bool custom) => custom ? "Custom" : "Every value the look's own.";
}
