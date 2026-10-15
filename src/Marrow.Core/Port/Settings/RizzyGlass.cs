// PORT-ONLY: ED-NEW-glass-alive-5 (phase 1a): rizzytoday/liquid-glass (MIT, Riz Roze; THIRD_PARTY_NOTICES.md "liquid-glass") as its own glass
// engine, in its own words. Every option of its LiquidGlassOptions (core/liquid-glass.ts, commit 841af14) is here with the repository's default
// (resolveConfig) and the range of the repository's own demo slider (demo/index.html; displaceBlur, a README option with no slider, 0 to 20). The
// presets step those same options up and down around the repository's values; Repo Default is the repository's values untouched. width, height and
// filterId are the element's own (the piece's size and the material), and fallbackFilter is the browsers' without SVG filters (every piece here
// draws the full effect), so none of them is a setting.
using System.Globalization;
using System.Text.Json;

namespace Marrow.Core.Port.Settings;

/// <summary>The glass engines the Glass settings switch between. Each draws with its own repository's model and is set in its own words.</summary>
public enum GlassEngine
{
    /// <summary>The approved design canvas's material (OD-0147), with the Look and fine-tuning values of the Glass settings.</summary>
    Canvas,
    /// <summary>rizzytoday/liquid-glass: a displacement map with three per-channel displacements (chromatic aberration).</summary>
    Rizzy,
}

/// <summary>The presets of an engine's options: the repository's own values, and three steps around them.</summary>
public enum GlassEnginePreset
{
    /// <summary>The repository's own defaults, untouched.</summary>
    Repo,
    /// <summary>Less bend, a little frost.</summary>
    Subtle,
    /// <summary>A visible body: a little more bend, a wider rim, some frost (the engine's first options).</summary>
    Balanced,
    /// <summary>The most bend and fringe.</summary>
    Strong,
}

/// <summary>
/// rizzytoday/liquid-glass's options (LiquidGlassOptions), by the repository's own names: <c>borderRadius</c>, <c>scale</c>, <c>aberration</c>
/// [r, g, b], <c>blur</c>, <c>border</c>, <c>lightness</c>, <c>alpha</c>, <c>frost</c>, <c>saturation</c> and <c>displaceBlur</c>. A piece's own corner
/// is its shape's; <see cref="BorderRadius"/> is the corner of a piece that has none of its own (the repository's default pill).
/// </summary>
public sealed record RizzyGlass
{
    /// <summary>Border radius in px (the repository's default 50: a pill); a piece with a corner of its own keeps it.</summary>
    public double BorderRadius { get; init; } = 50;

    /// <summary>Base displacement strength; negative bends inward (default -180).</summary>
    public double Scale { get; init; } = -180;

    /// <summary>The red channel's offset on <see cref="Scale"/> (aberration[0], default 0).</summary>
    public double AberrationR { get; init; }

    /// <summary>The green channel's offset on <see cref="Scale"/> (aberration[1], default 10).</summary>
    public double AberrationG { get; init; } = 10;

    /// <summary>The blue channel's offset on <see cref="Scale"/> (aberration[2], default 20).</summary>
    public double AberrationB { get; init; } = 20;

    /// <summary>Edge blur in px of the centre's neutral zone (default 11).</summary>
    public double Blur { get; init; } = 11;

    /// <summary>The neutral zone's inset as a fraction of the smaller side (default 0.07).</summary>
    public double Border { get; init; } = 0.07;

    /// <summary>The centre's lightness, 0 to 100 (default 50: no displacement).</summary>
    public double Lightness { get; init; } = 50;

    /// <summary>The centre's opacity, 0 to 1 (default 0.93); under 1 the body bends too.</summary>
    public double Alpha { get; init; } = 0.93;

    /// <summary>The frost overlay's opacity, 0 to 1 (default 0: clear glass).</summary>
    public double Frost { get; init; }

    /// <summary>The backdrop's saturation multiplier (default 1).</summary>
    public double Saturation { get; init; } = 1;

    /// <summary>Post-displacement blur in px (default 0).</summary>
    public double DisplaceBlur { get; init; }

    /// <summary>The repository's defaults (resolveConfig).</summary>
    public static RizzyGlass Repo { get; } = new();

    /// <summary>Every option in the repository's order, with its id (the repository's name), its words, its range and its step.</summary>
    public static IReadOnlyList<GlassKnob> Knobs { get; } =
    [
        new("borderRadius", "Border radius", "Corner radius in px of a piece without a corner of its own.", 0, 100, 1, GlassUnit.Pixels0),
        new("scale", "Scale", "Displacement strength. More negative is more warp.", -300, -20, 10, GlassUnit.Whole),
        new("aberrationR", "Aberration, red", "The red channel's offset on the scale: colour at the rim.", -40, 40, 1, GlassUnit.Whole),
        new("aberrationG", "Aberration, green", "The green channel's offset on the scale.", -40, 40, 1, GlassUnit.Whole),
        new("aberrationB", "Aberration, blue", "The blue channel's offset on the scale.", -40, 40, 1, GlassUnit.Whole),
        new("blur", "Edge blur", "How softly the rim's bend fades into the flat centre.", 0, 30, 1, GlassUnit.Pixels0),
        new("border", "Border", "How wide the bending rim is, as a share of the smaller side.", 0, 0.3, 0.01, GlassUnit.Plain),
        new("lightness", "Lightness", "The centre's lightness. 50 is neutral; away from it the whole centre shifts.", 0, 100, 1, GlassUnit.Whole),
        new("alpha", "Alpha", "The centre's opacity. Below 1 the centre bends too: a lens body, not only a rim.", 0, 1, 0.01, GlassUnit.Plain),
        new("frost", "Frost", "The frost over the glass. 0 is clear glass.", 0, 0.5, 0.01, GlassUnit.Plain),
        new("saturation", "Saturation", "The backdrop's saturation.", 0, 3, 0.1, GlassUnit.Times),
        new("displaceBlur", "Displace blur", "Blur in px after the displacement.", 0, 20, 0.1, GlassUnit.Pixels1),
    ];

    /// <summary>The presets in the page's order, with their ids and names.</summary>
    public static IReadOnlyList<(string Id, string Name, GlassEnginePreset Value)> Presets { get; } =
        [("repo", "Repo Default", GlassEnginePreset.Repo), ("subtle", "Subtle", GlassEnginePreset.Subtle), ("balanced", "Balanced", GlassEnginePreset.Balanced),
            ("strong", "Strong", GlassEnginePreset.Strong)];

    /// <summary>The options of <paramref name="preset"/>.</summary>
    public static RizzyGlass Of(GlassEnginePreset preset) => preset switch
    {
        GlassEnginePreset.Subtle => Repo with { Scale = -90, AberrationG = 4, AberrationB = 8, Blur = 7, Frost = 0.14, Saturation = 1.05 },
        GlassEnginePreset.Balanced => Repo with { Scale = -200, Border = 0.09, Alpha = 0.9, Frost = 0.24, Saturation = 1.15 },
        GlassEnginePreset.Strong => Repo with { Scale = -300, AberrationG = 18, AberrationB = 34, Blur = 20, Frost = 0.42, Saturation = 1.3 },
        _ => Repo,
    };

    /// <summary>The preset these options are, or null when they are the user's own (Custom).</summary>
    public GlassEnginePreset? Preset => Presets.Select(p => (GlassEnginePreset?)p.Value).FirstOrDefault(p => Of(p!.Value) == Held());

    /// <summary>An option by its id.</summary>
    public double Get(string id) => id switch
    {
        "borderRadius" => BorderRadius, "scale" => Scale, "aberrationR" => AberrationR, "aberrationG" => AberrationG, "aberrationB" => AberrationB,
        "blur" => Blur, "border" => Border, "lightness" => Lightness, "alpha" => Alpha, "frost" => Frost, "saturation" => Saturation,
        "displaceBlur" => DisplaceBlur,
        _ => throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown liquid-glass option."),
    };

    /// <summary>The same options with <paramref name="id"/> at <paramref name="value"/>, held to its range.</summary>
    public RizzyGlass With(string id, double value)
    {
        var knob = Knobs.FirstOrDefault(k => k.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown liquid-glass option.");
        var v = knob.Hold(value);
        return id switch
        {
            "borderRadius" => this with { BorderRadius = v }, "scale" => this with { Scale = v }, "aberrationR" => this with { AberrationR = v },
            "aberrationG" => this with { AberrationG = v }, "aberrationB" => this with { AberrationB = v }, "blur" => this with { Blur = v },
            "border" => this with { Border = v }, "lightness" => this with { Lightness = v }, "alpha" => this with { Alpha = v },
            "frost" => this with { Frost = v }, "saturation" => this with { Saturation = v }, _ => this with { DisplaceBlur = v },
        };
    }

    /// <summary>Every option held to its range.</summary>
    public RizzyGlass Held()
    {
        var held = this;
        foreach (var knob in Knobs) held = held.With(knob.Id, held.Get(knob.Id));
        return held;
    }

    /// <summary>The options <paramref name="t"/> of the way from these to <paramref name="to"/>'s; at 1 exactly <paramref name="to"/>.</summary>
    public RizzyGlass Toward(RizzyGlass to, double t)
    {
        ArgumentNullException.ThrowIfNull(to);
        if (t >= 1) return to;
        if (t <= 0) return this;
        var o = this;
        foreach (var knob in Knobs) o = o.With(knob.Id, Get(knob.Id) + (to.Get(knob.Id) - Get(knob.Id)) * t);
        return o;
    }

    internal void Write(Utf8JsonWriter w, RizzyGlass baseline)
    {
        var held = Held();
        foreach (var knob in Knobs)
            if (Math.Abs(held.Get(knob.Id) - baseline.Get(knob.Id)) > 1e-9) w.WriteNumber(knob.Id, Math.Round(held.Get(knob.Id), 4));
    }

    internal static RizzyGlass Read(JsonElement element, RizzyGlass baseline)
    {
        if (element.ValueKind != JsonValueKind.Object) return baseline;
        var read = baseline;
        foreach (var knob in Knobs)
            if (element.TryGetProperty(knob.Id, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var number) && double.IsFinite(number))
                read = read.With(knob.Id, number);
        return read;
    }

    /// <inheritdoc />
    public override string ToString() => string.Join(", ", Knobs.Select(k => k.Id + " " + Get(k.Id).ToString(CultureInfo.InvariantCulture)));
}
