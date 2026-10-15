// PORT-ONLY ED-NEW-shader-gradient-1 (row #87; Maaz, 2026-10-15, overnight brief item 7): Appearance's Gradient section, every option of the
// Gradient wallpaper (ruucm/shadergradient's whole option set, ShaderGradientSettings) live: its shape, its three colours, its flow (animate,
// speed, strength, density, frequency, amplitude, the range), its look (brightness, reflection, grain, wireframe), its light (a 3D light or an
// environment and which), its camera (azimuth, polar angle, distance, zoom, field of view) and its placing (position and rotation), a button that
// shows it as the wallpaper, and a reset. A change reaches the wallpaper at once; a drag or a key writes the setting when it ends.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port.Settings;

namespace Marrow.App.Port.Settings;

internal sealed class GradientSections
{
    private readonly Func<ShaderGradientSettings> read;
    private readonly Action<ShaderGradientSettings> write;
    private readonly Action commit;
    private bool showing;

    internal CzSection Section { get; }
    internal CzSegmentRow Shape { get; }
    internal IReadOnlyList<CzColorWell> Wells { get; }
    internal CzWellPicker Picker { get; } = new();
    internal CzToggleRow Animate { get; }
    internal CzToggleRow Range { get; }
    internal CzToggleRow Grain { get; }
    internal CzToggleRow Wireframe { get; }
    internal CzSegmentRow Lighting { get; }
    internal CzSegmentRow Environment { get; }
    internal IReadOnlyDictionary<string, CzSlider> Sliders { get; }
    internal CzCapsuleButton Use { get; }
    internal CzCapsuleButton Reset { get; }

    /// <param name="read">The Gradient setting now.</param>
    /// <param name="write">Puts a new setting in the app's settings (the wallpaper follows at once).</param>
    /// <param name="commit">The settings' coalesced write goes out now.</param>
    /// <param name="use">Shows the Gradient as the wallpaper.</param>
    internal GradientSections(Func<ShaderGradientSettings> read, Action<ShaderGradientSettings> write, Action commit, Action use)
    {
        this.read = read;
        this.write = write;
        this.commit = commit;
        var s = read().Held();
        var rows = new CzRowStack();

        Use = new CzCapsuleButton("Use as wallpaper", detail: "Shows the Gradient behind the window");
        Use.Invoked += use;
        rows.Add(new CzRow("Gradient wallpaper", "A plane, sphere or water plane that flows in three colours", Use));

        Shape = new CzSegmentRow("Shape", "The surface the colours flow over", ShaderGradientSettings.Shapes.Select(x => (x.Id, x.Name)).ToArray(),
            ShaderGradientSettings.Shapes.First(x => x.Value == s.Shape).Id,
            id => Edit(g => g with { Shape = ShaderGradientSettings.Shapes.First(x => x.Id == id).Value }, now: true));
        rows.Add(Shape.Row);

        var wells = new[]
        {
            new CzColorWell("Colour 1", "1", s.Color1, Picker.Toggle), new CzColorWell("Colour 2", "2", s.Color2, Picker.Toggle), new CzColorWell("Colour 3", "3", s.Color3, Picker.Toggle),
        };
        wells[0].ColorChanged += hex => Edit(g => g with { Color1 = ShaderGradientSettings.Hex(hex) ?? g.Color1 }, now: true);
        wells[1].ColorChanged += hex => Edit(g => g with { Color2 = ShaderGradientSettings.Hex(hex) ?? g.Color2 }, now: true);
        wells[2].ColorChanged += hex => Edit(g => g with { Color3 = ShaderGradientSettings.Hex(hex) ?? g.Color3 }, now: true);
        Wells = wells;
        rows.Add(new CzRow("Colours", "The three colours the surface flows through",
            new StackPanel { Orientation = Orientation.Horizontal, Spacing = 14, Children = { wells[0], wells[1], wells[2], Picker } }));

        var sliders = new Dictionary<string, CzSlider>();
        void Slider(string id)
        {
            var k = ShaderGradientSettings.Knobs.Single(x => x.Id == id);
            var slider = new CzSlider(k.Name, k.Min, k.Max, k.Step, s.Get(id), format: v => CzText.Fmt(v, k.Step < 0.1 ? 2 : k.Step < 1 ? 1 : 0), detail: k.Description);
            slider.ValueChanged += v => { if (v is { } x) Edit(g => g.With(id, x)); };
            slider.AddHandler(InputElement.PointerReleasedEvent, (_, _) => commit(), RoutingStrategies.Bubble, handledEventsToo: true);
            slider.AddHandler(InputElement.KeyUpEvent, (_, _) => commit(), RoutingStrategies.Bubble, handledEventsToo: true);
            Avalonia.Automation.AutomationProperties.SetName(slider, k.Name);
            sliders[id] = slider;
            rows.Add(new CzRow(k.Name, k.Description, slider, dimmed => slider.Dimmed = dimmed));
        }

        rows.Add(new CzCaptionRow("Flow"), caption: true);
        Animate = new CzToggleRow("Animate", "The colours flow; off, they hold still", () => read().Animate, on => Edit(g => g with { Animate = on }, now: true));
        rows.Add(Animate.Row);
        foreach (var id in new[] { "uSpeed", "uStrength", "uDensity", "uFrequency", "uAmplitude" }) Slider(id);
        Range = new CzToggleRow("Range", "The flow swings between a start and an end", () => read().Range, on => Edit(g => g with { Range = on }, now: true));
        rows.Add(Range.Row);
        Slider("rangeStart");
        Slider("rangeEnd");

        rows.Add(new CzCaptionRow("Look"), caption: true);
        Slider("brightness");
        Slider("reflection");
        Grain = new CzToggleRow("Grain", "A fine film grain over the colours", () => read().Grain, on => Edit(g => g with { Grain = on }, now: true));
        rows.Add(Grain.Row);
        Slider("grainBlending");
        Wireframe = new CzToggleRow("Wireframe", "Draws the surface as its grid", () => read().Wireframe, on => Edit(g => g with { Wireframe = on }, now: true));
        rows.Add(Wireframe.Row);

        rows.Add(new CzCaptionRow("Light"), caption: true);
        Lighting = new CzSegmentRow("Lighting", "A 3D light, or an environment's light", ShaderGradientSettings.Lightings.Select(x => (x.Id, x.Name)).ToArray(),
            ShaderGradientSettings.Lightings.First(x => x.Value == s.Lighting).Id,
            id => Edit(g => g with { Lighting = ShaderGradientSettings.Lightings.First(x => x.Id == id).Value }, now: true));
        rows.Add(Lighting.Row);
        Environment = new CzSegmentRow("Environment", "The environment that lights it", ShaderGradientSettings.Environments.Select(x => (x.Id, x.Name)).ToArray(),
            ShaderGradientSettings.Environments.First(x => x.Value == s.Environment).Id,
            id => Edit(g => g with { Environment = ShaderGradientSettings.Environments.First(x => x.Id == id).Value }, now: true));
        rows.Add(Environment.Row);

        rows.Add(new CzCaptionRow("Camera"), caption: true);
        foreach (var id in new[] { "cAzimuthAngle", "cPolarAngle", "cDistance", "cameraZoom", "fov" }) Slider(id);

        rows.Add(new CzCaptionRow("Placing"), caption: true);
        foreach (var id in new[] { "positionX", "positionY", "positionZ", "rotationX", "rotationY", "rotationZ" }) Slider(id);
        Sliders = sliders;

        Reset = new CzCapsuleButton("Reset to defaults");
        Reset.Invoked += () => { write(ShaderGradientSettings.Default); commit(); Apply(); };
        rows.Add(new CzRow("Defaults", "Back to the Gradient as it ships (shadergradient's Halo)", Reset));

        Section = new CzSection("Gradient", rows, open: true);
        Apply();
    }

    private void Edit(Func<ShaderGradientSettings, ShaderGradientSettings> change, bool now = false)
    {
        if (showing) return;
        write(change(read()).Held());
        if (now) commit();
        Apply();
    }

    /// <summary>Shows what the setting says now in every row.</summary>
    internal void Apply()
    {
        var s = read().Held();
        showing = true;
        try
        {
            foreach (var (id, slider) in Sliders) if (slider.Value != s.Get(id)) slider.Update(s.Get(id), null);
            var shape = ShaderGradientSettings.Shapes.First(x => x.Value == s.Shape).Id;
            if (Shape.Control.SelectedId != shape) Shape.Apply(shape, "The surface the colours flow over");
            var light = ShaderGradientSettings.Lightings.First(x => x.Value == s.Lighting).Id;
            if (Lighting.Control.SelectedId != light) Lighting.Apply(light, "A 3D light, or an environment's light");
            var env = ShaderGradientSettings.Environments.First(x => x.Value == s.Environment).Id;
            if (Environment.Control.SelectedId != env) Environment.Apply(env, "The environment that lights it");
            if (Wells[0].Hex != s.Color1) Wells[0].Display(s.Color1);
            if (Wells[1].Hex != s.Color2) Wells[1].Display(s.Color2);
            if (Wells[2].Hex != s.Color3) Wells[2].Display(s.Color3);
            // what an option does not act on, dimmed: the environment without its light, the grain's amount without grain, the range's ends without it
            Environment.Row.Dimmed = s.Lighting != GradientLighting.Environment;
            Sliders["grainBlending"].Dimmed = !s.Grain;
            Sliders["rangeStart"].Dimmed = Sliders["rangeEnd"].Dimmed = !s.Range;
            Sliders["uSpeed"].Dimmed = !s.Animate;
            Animate.Apply("The colours flow; off, they hold still");
            Range.Apply("The flow swings between a start and an end");
            Grain.Apply("A fine film grain over the colours");
            Wireframe.Apply("Draws the surface as its grid");
        }
        finally { showing = false; }
    }
}
