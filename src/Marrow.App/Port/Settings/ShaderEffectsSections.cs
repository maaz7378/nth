// PORT-ONLY ED-NEW-shader-effects-1 (row #87; Maaz, 2026-10-15, overnight brief item 8): Appearance's Shaders section, its own section beside
// Gradient: the shader-effects-inc effects (MIT) ported so far, a picker for the one to edit, a button that shows it as the wallpaper, every one of
// its own options (its colours as wells, its numbers as sliders, its choices as segments) live, and a reset of that effect's options.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port.Settings;
using Marrow.UI.Theme.Wallpapers;

namespace Marrow.App.Port.Settings;

internal sealed class ShaderEffectsSections
{
    private readonly Func<ShaderEffectSettings> read;
    private readonly Action<ShaderEffectSettings> write;
    private readonly Action commit;
    private readonly List<(string Effect, CzRowSlot Slot)> slots = [];
    private readonly Dictionary<(string Effect, string Prop), Control> controls = [];
    private bool showing;

    internal CzSection Section { get; }
    internal CzMenuButton Effect { get; }
    internal CzCapsuleButton Use { get; }
    internal CzCapsuleButton Reset { get; }
    internal CzWellPicker Picker { get; } = new();

    /// <summary>The effect the rows edit.</summary>
    internal ShaderEffect Shown => ShaderEffects.Find(read().Shown) ?? ShaderEffects.All[0];

    /// <summary>The control that edits an option of an effect.</summary>
    internal Control ControlFor(string effect, string prop) => controls[(effect, prop)];

    internal ShaderEffectsSections(Func<ShaderEffectSettings> read, Action<ShaderEffectSettings> write, Action commit, Action<string> use)
    {
        this.read = read;
        this.write = write;
        this.commit = commit;
        var rows = new CzRowStack();
        Effect = new CzMenuButton("Effect", ShaderEffects.All.Select(e => (e.Id, e.Name)).ToArray(), Shown.Id, "The effect the options below edit");
        Effect.SelectionChanged += id => { if (!showing) { write(read() with { Shown = id }); commit(); Apply(); } };
        rows.Add(new CzRow("Effect", "shader-effects-inc's effects, each its own live wallpaper", Effect));
        Use = new CzCapsuleButton("Use as wallpaper", detail: "Shows the effect behind the window");
        Use.Invoked += () => use(Shown.Id);
        rows.Add(new CzRow("Wallpaper", "Shows this effect behind the window", Use));
        rows.Add(Picker);

        foreach (var e in ShaderEffects.All)
            foreach (var p in e.Props)
            {
                var (effect, prop) = (e.Id, p);
                Control control;
                CzRow row;
                switch (p.Kind)
                {
                    case "color":
                    {
                        var well = new CzColorWell(p.Label, "", p.Colour, Picker.Toggle);
                        well.ColorChanged += hex => Edit(s => s.With(effect, prop.Id, ShaderGradientSettings.Hex(hex) ?? prop.Colour), now: true);
                        control = well;
                        row = new CzRow(p.Label, p.Detail, well);
                        break;
                    }
                    case "select":
                    {
                        var options = p.Choices.Select((c, i) => (i.ToString(System.Globalization.CultureInfo.InvariantCulture), c)).ToArray();
                        var seg = new CzSegmentRow(p.Label, p.Detail, options, ((int)p.Number).ToString(System.Globalization.CultureInfo.InvariantCulture),
                            id => Edit(s => s.With(effect, prop.Id, double.Parse(id, System.Globalization.CultureInfo.InvariantCulture)), now: true));
                        control = seg.Control;
                        row = seg.Row;
                        break;
                    }
                    default:
                    {
                        var slider = new CzSlider(p.Label, p.Min, p.Max, p.Step, p.Number, format: v => ShaderEffects.Format(prop, v), detail: p.Detail);
                        slider.ValueChanged += v => { if (v is { } x) Edit(s => s.With(effect, prop.Id, x)); };
                        slider.AddHandler(InputElement.PointerReleasedEvent, (_, _) => commit(), RoutingStrategies.Bubble, handledEventsToo: true);
                        slider.AddHandler(InputElement.KeyUpEvent, (_, _) => commit(), RoutingStrategies.Bubble, handledEventsToo: true);
                        control = slider;
                        row = new CzRow(p.Label, p.Detail, slider, dimmed => slider.Dimmed = dimmed);
                        break;
                    }
                }
                Avalonia.Automation.AutomationProperties.SetName(control, $"{e.Name} {p.Label}");
                controls[(e.Id, p.Id)] = control;
                slots.Add((e.Id, rows.Add(row, open: e.Id == Shown.Id)));
            }

        Reset = new CzCapsuleButton("Reset effect");
        Reset.Invoked += () => { write(read().Without(Shown.Id)); commit(); Apply(); };
        rows.Add(new CzRow("Defaults", "This effect's options back to the package's", Reset));
        Section = new CzSection("Shaders", rows, open: true);
        Apply();
    }

    private void Edit(Func<ShaderEffectSettings, ShaderEffectSettings> change, bool now = false)
    {
        if (showing) return;
        write(change(read()));
        if (now) commit();
    }

    /// <summary>Shows what the setting says now: the effect picked, its rows open, every value.</summary>
    internal void Apply()
    {
        var s = read();
        var shown = Shown;
        showing = true;
        try
        {
            if (Effect.SelectedId != shown.Id) Effect.Show(shown.Id);
            foreach (var (effect, slot) in slots) slot.Open = effect == shown.Id;
            foreach (var e in ShaderEffects.All)
                foreach (var p in e.Props)
                    switch (controls[(e.Id, p.Id)])
                    {
                        case CzColorWell well when well.Hex != s.Colour(e.Id, p.Id, p.Colour): well.Display(s.Colour(e.Id, p.Id, p.Colour)); break;
                        case CzSlider slider when slider.Value != s.Number(e.Id, p.Id, p.Number): slider.Update(Math.Clamp(s.Number(e.Id, p.Id, p.Number), p.Min, p.Max), null); break;
                        case CzSegmentedControl seg:
                            var id = ((int)s.Number(e.Id, p.Id, p.Number)).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            if (seg.SelectedId != id) seg.Select(id, focus: false);
                            break;
                    }
        }
        finally { showing = false; }
    }
}
