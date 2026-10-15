// PORT-ONLY: ED-NEW-glass-alive-6 (phase 1b): dashersw/liquid-glass-js (MIT, Armagan Amcalar; THIRD_PARTY_NOTICES.md "liquid-glass-js") as its own
// glass engine, in its own words. Every option of its glass controls (controls.js window.glassControls and container.js, commit 78cb6cc) is here
// with the repository's default and the range of the repository's own demo slider (index.html): edgeIntensity, rimIntensity, baseIntensity,
// edgeDistance, rimDistance, baseDistance, cornerBoost, rippleEffect, blurRadius, tintOpacity, the warp switch, and the container's borderRadius
// (its type, rounded, circle or pill, is the piece's own shape). hideButtons is the demo page's, not the glass's, so it is not a setting.
using System.Globalization;
using System.Text.Json;

namespace Marrow.Core.Port.Settings;

/// <summary>
/// dashersw/liquid-glass-js's options, by the repository's own names. Its refraction is the shape's outward normal times three exponential bands
/// of the distance from the edge (base, edge and rim), a corner boost and a ripple along the rim, as a share of the page; then a Gaussian blur of
/// <see cref="BlurRadius"/>, a white-to-grey tint and a tint sampled from the page, both at <see cref="TintOpacity"/>.
/// </summary>
public sealed record DasherGlass
{
    /// <summary>The edge band's strength (default 0.01).</summary>
    public double EdgeIntensity { get; init; } = 0.01;

    /// <summary>The rim band's strength (default 0.05).</summary>
    public double RimIntensity { get; init; } = 0.05;

    /// <summary>The base (centre) band's strength, with <see cref="Warp"/> on (default 0.01).</summary>
    public double BaseIntensity { get; init; } = 0.01;

    /// <summary>The edge band's fall-off per px (default 0.15).</summary>
    public double EdgeDistance { get; init; } = 0.15;

    /// <summary>The rim band's fall-off per px (default 0.8).</summary>
    public double RimDistance { get; init; } = 0.8;

    /// <summary>The base band's rise per px (default 0.1).</summary>
    public double BaseDistance { get; init; } = 0.1;

    /// <summary>The bend near the corners (default 0.02).</summary>
    public double CornerBoost { get; init; } = 0.02;

    /// <summary>The ripple along the rim (default 0.1).</summary>
    public double RippleEffect { get; init; } = 0.1;

    /// <summary>The blur radius (default 5).</summary>
    public double BlurRadius { get; init; } = 5;

    /// <summary>The tint's opacity (default 0.2).</summary>
    public double TintOpacity { get; init; } = 0.2;

    /// <summary>The container's corner in px for a piece without one of its own (default 48).</summary>
    public double BorderRadius { get; init; } = 48;

    /// <summary>Warp: the centre bends too (the base band; default off).</summary>
    public bool Warp { get; init; }

    /// <summary>The repository's defaults.</summary>
    public static DasherGlass Repo { get; } = new();

    /// <summary>Every slider in the repository's order, with its id, words, range and step (index.html).</summary>
    public static IReadOnlyList<GlassKnob> Knobs { get; } =
    [
        new("edgeIntensity", "Edge intensity", "How strongly the band near the edge bends.", 0, 0.1, 0.001, GlassUnit.Plain3),
        new("rimIntensity", "Rim intensity", "How strongly the thin rim bends.", 0, 0.2, 0.001, GlassUnit.Plain3),
        new("baseIntensity", "Base intensity", "How strongly the centre bends, with Warp on.", 0, 0.05, 0.001, GlassUnit.Plain3),
        new("edgeDistance", "Edge distance", "How fast the edge band fades inward. Lower reaches further.", 0.05, 0.5, 0.01, GlassUnit.Plain),
        new("rimDistance", "Rim distance", "How fast the rim fades inward. Lower is a wider rim.", 0.1, 2, 0.05, GlassUnit.Plain),
        new("baseDistance", "Base distance", "How fast the centre's bend rises from the edge.", 0.05, 0.3, 0.01, GlassUnit.Plain),
        new("cornerBoost", "Corner boost", "Extra bend in the corners.", 0, 0.1, 0.001, GlassUnit.Plain3),
        new("rippleEffect", "Ripple effect", "A ripple along the rim.", 0, 0.5, 0.01, GlassUnit.Plain),
        new("blurRadius", "Blur radius", "The blur of what shows through.", 1, 15, 0.5, GlassUnit.Pixels1),
        new("tintOpacity", "Tint opacity", "The white-to-grey tint and the tint taken from behind.", 0, 1, 0.01, GlassUnit.Plain),
        new("borderRadius", "Border radius", "Corner radius in px of a piece without a corner of its own.", 0, 100, 1, GlassUnit.Pixels0),
    ];

    /// <summary>The presets in the page's order.</summary>
    public static IReadOnlyList<(string Id, string Name, GlassEnginePreset Value)> Presets => RizzyGlass.Presets;

    /// <summary>The options of <paramref name="preset"/>: Balanced is tint 0.2, blur 4.5, rim 0.1 at 0.6 with a visible edge band.</summary>
    public static DasherGlass Of(GlassEnginePreset preset) => preset switch
    {
        GlassEnginePreset.Subtle => Repo with { EdgeIntensity = 0.006, RimIntensity = 0.03, RippleEffect = 0.05, BlurRadius = 4, TintOpacity = 0.15 },
        GlassEnginePreset.Balanced => Repo with { EdgeIntensity = 0.02, RimIntensity = 0.1, RimDistance = 0.6, BlurRadius = 4.5, TintOpacity = 0.2 },
        GlassEnginePreset.Strong => Repo with { EdgeIntensity = 0.04, RimIntensity = 0.16, RimDistance = 0.4, BaseIntensity = 0.02, CornerBoost = 0.05, RippleEffect = 0.2, BlurRadius = 6, TintOpacity = 0.25, Warp = true },
        _ => Repo,
    };

    /// <summary>The preset these options are, or null (Custom).</summary>
    public GlassEnginePreset? Preset => Presets.Select(p => (GlassEnginePreset?)p.Value).FirstOrDefault(p => Of(p!.Value) == Held());

    /// <summary>An option by its id.</summary>
    public double Get(string id) => id switch
    {
        "edgeIntensity" => EdgeIntensity, "rimIntensity" => RimIntensity, "baseIntensity" => BaseIntensity, "edgeDistance" => EdgeDistance,
        "rimDistance" => RimDistance, "baseDistance" => BaseDistance, "cornerBoost" => CornerBoost, "rippleEffect" => RippleEffect,
        "blurRadius" => BlurRadius, "tintOpacity" => TintOpacity, "borderRadius" => BorderRadius,
        _ => throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown liquid-glass-js option."),
    };

    /// <summary>The same options with <paramref name="id"/> at <paramref name="value"/>, held to its range.</summary>
    public DasherGlass With(string id, double value)
    {
        var knob = Knobs.FirstOrDefault(k => k.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown liquid-glass-js option.");
        var v = knob.Hold(value);
        return id switch
        {
            "edgeIntensity" => this with { EdgeIntensity = v }, "rimIntensity" => this with { RimIntensity = v },
            "baseIntensity" => this with { BaseIntensity = v }, "edgeDistance" => this with { EdgeDistance = v },
            "rimDistance" => this with { RimDistance = v }, "baseDistance" => this with { BaseDistance = v },
            "cornerBoost" => this with { CornerBoost = v }, "rippleEffect" => this with { RippleEffect = v },
            "blurRadius" => this with { BlurRadius = v }, "tintOpacity" => this with { TintOpacity = v }, _ => this with { BorderRadius = v },
        };
    }

    /// <summary>Every option held to its range.</summary>
    public DasherGlass Held()
    {
        var held = this;
        foreach (var knob in Knobs) held = held.With(knob.Id, held.Get(knob.Id));
        return held;
    }

    /// <summary>The options <paramref name="t"/> of the way to <paramref name="to"/>'s; the switch is the nearer end's.</summary>
    public DasherGlass Toward(DasherGlass to, double t)
    {
        ArgumentNullException.ThrowIfNull(to);
        if (t >= 1) return to;
        if (t <= 0) return this;
        var o = this with { Warp = t < 0.5 ? Warp : to.Warp };
        foreach (var knob in Knobs) o = o.With(knob.Id, Get(knob.Id) + (to.Get(knob.Id) - Get(knob.Id)) * t);
        return o;
    }

    internal void Write(Utf8JsonWriter w, DasherGlass baseline)
    {
        var held = Held();
        foreach (var knob in Knobs)
            if (Math.Abs(held.Get(knob.Id) - baseline.Get(knob.Id)) > 1e-9) w.WriteNumber(knob.Id, Math.Round(held.Get(knob.Id), 4));
        if (held.Warp != baseline.Warp) w.WriteBoolean("warp", held.Warp);
    }

    internal static DasherGlass Read(JsonElement element, DasherGlass baseline)
    {
        if (element.ValueKind != JsonValueKind.Object) return baseline;
        var read = baseline;
        foreach (var knob in Knobs)
            if (element.TryGetProperty(knob.Id, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var number) && double.IsFinite(number))
                read = read.With(knob.Id, number);
        if (element.TryGetProperty("warp", out var warp) && warp.ValueKind is JsonValueKind.True or JsonValueKind.False) read = read with { Warp = warp.GetBoolean() };
        return read;
    }

    /// <inheritdoc />
    public override string ToString() =>
        string.Join(", ", Knobs.Select(k => k.Id + " " + Get(k.Id).ToString(CultureInfo.InvariantCulture))) + ", warp " + (Warp ? "on" : "off");
}
