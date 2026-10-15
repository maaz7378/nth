// PORT-ONLY: ED-NEW-glass-alive-7 (phase 1c): Glass-HQ/liquid-glass (MIT; THIRD_PARTY_NOTICES.md "Glass-HQ liquid-glass") as its own glass engine,
// in its own words. Every material option of its surfaces (core/materials.ts MaterialOptions and the site's API reference) is here with the
// repository's default: material (clear or regular), appearance (the scene's, or light or dark), radius, refraction (displacement in px, 60),
// concentric (a nested surface follows its container and limits its refraction to its inset), tint (a colour; none by default), and the motion
// options (interactive press feedback, motion full, reduced or none). The material's own numbers (core/materials.ts: blur, fill, tone, shadow lift,
// highlight roll-off) and the maps' (gpu/maps.wgsl: the bezel, the specular and the rim shade) are the engine's, not settings, as there.
using System.Text.Json;

namespace Marrow.Core.Port.Settings;

/// <summary>Glass-HQ's materials.</summary>
public enum GlassHqMaterial
{
    /// <summary>Regular: a frost of 7.33 px, a wide fill and a calibrated tone (the repository's default for panels and menus).</summary>
    Regular,
    /// <summary>Clear: no frost, a shadow lift and a highlight roll-off only.</summary>
    Clear,
}

/// <summary>Glass-HQ's appearance: the scene's (nth's theme), or held light or dark.</summary>
public enum GlassHqAppearance
{
    /// <summary>The scene's: nth's own light or dark theme.</summary>
    Scene,
    /// <summary>Light.</summary>
    Light,
    /// <summary>Dark.</summary>
    Dark,
}

/// <summary>Glass-HQ's motion option.</summary>
public enum GlassHqMotion
{
    /// <summary>Full: the press swells and the light follows.</summary>
    Full,
    /// <summary>Reduced: no travel, a fade only.</summary>
    Reduced,
    /// <summary>None.</summary>
    None,
}

/// <summary>Glass-HQ/liquid-glass's surface options, by the repository's own names.</summary>
public sealed record GlassHqGlass
{
    /// <summary>The material (default regular).</summary>
    public GlassHqMaterial Material { get; init; } = GlassHqMaterial.Regular;

    /// <summary>The appearance (default: the scene's).</summary>
    public GlassHqAppearance Appearance { get; init; } = GlassHqAppearance.Scene;

    /// <summary>The continuous corner radius in px of a piece without a corner of its own (the repository's surface default, 8).</summary>
    public double Radius { get; init; } = 8;

    /// <summary>The displacement in px (default 60; nested surfaces limit it to their inset).</summary>
    public double Refraction { get; init; } = 60;

    /// <summary>Concentric: a nested surface follows its container (default on).</summary>
    public bool Concentric { get; init; } = true;

    /// <summary>The concentric inset in px (how far a nested surface sits in; it also limits its refraction).</summary>
    public double Inset { get; init; } = 8;

    /// <summary>The glass colour; none by default (text and icons unchanged).</summary>
    public GlassTintColour Tint { get; init; } = GlassTintColour.None;

    /// <summary>Interactive: press and drag feedback (default on for controls).</summary>
    public bool Interactive { get; init; } = true;

    /// <summary>Motion (default full; the system's reduced motion always applies).</summary>
    public GlassHqMotion Motion { get; init; } = GlassHqMotion.Full;

    /// <summary>The repository's defaults.</summary>
    public static GlassHqGlass Repo { get; } = new();

    /// <summary>The number options, with their ids, words, ranges and steps.</summary>
    public static IReadOnlyList<GlassKnob> Knobs { get; } =
    [
        new("radius", "Radius", "Continuous corner radius in px of a piece without a corner of its own.", 0, 64, 1, GlassUnit.Pixels0),
        new("refraction", "Refraction", "Displacement in px at the edge.", 0, 120, 1, GlassUnit.Pixels0),
        new("inset", "Concentric inset", "How far a nested surface sits in; it limits the nested refraction too.", 0, 32, 1, GlassUnit.Pixels0),
    ];

    /// <summary>The materials, appearances and motions in the page's order.</summary>
    public static IReadOnlyList<(string Id, string Name, GlassHqMaterial Value)> Materials { get; } =
        [("regular", "Regular", GlassHqMaterial.Regular), ("clear", "Clear", GlassHqMaterial.Clear)];

    /// <summary>The appearances.</summary>
    public static IReadOnlyList<(string Id, string Name, GlassHqAppearance Value)> Appearances { get; } =
        [("scene", "Theme", GlassHqAppearance.Scene), ("light", "Light", GlassHqAppearance.Light), ("dark", "Dark", GlassHqAppearance.Dark)];

    /// <summary>The motions.</summary>
    public static IReadOnlyList<(string Id, string Name, GlassHqMotion Value)> Motions { get; } =
        [("full", "Full", GlassHqMotion.Full), ("reduced", "Reduced", GlassHqMotion.Reduced), ("none", "None", GlassHqMotion.None)];

    /// <summary>The presets: the repository's two materials as it ships them, and a stronger bend.</summary>
    public static IReadOnlyList<(string Id, string Name, GlassHqGlass Value)> Presets { get; } =
    [
        ("regular", "Regular", Repo), ("clear", "Clear", Repo with { Material = GlassHqMaterial.Clear }),
        ("strong", "Strong", Repo with { Refraction = 96 }),
    ];

    /// <summary>A number option by its id.</summary>
    public double Get(string id) => id switch
    {
        "radius" => Radius, "refraction" => Refraction, "inset" => Inset,
        _ => throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown Glass-HQ option."),
    };

    /// <summary>The same options with <paramref name="id"/> at <paramref name="value"/>, held to its range.</summary>
    public GlassHqGlass With(string id, double value)
    {
        var knob = Knobs.FirstOrDefault(k => k.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown Glass-HQ option.");
        var v = knob.Hold(value);
        return id switch { "radius" => this with { Radius = v }, "refraction" => this with { Refraction = v }, _ => this with { Inset = v } };
    }

    /// <summary>Every option held to its range, every choice one of the repository's.</summary>
    public GlassHqGlass Held()
    {
        var held = this with
        {
            Material = Enum.IsDefined(Material) ? Material : GlassHqMaterial.Regular,
            Appearance = Enum.IsDefined(Appearance) ? Appearance : GlassHqAppearance.Scene,
            Tint = Enum.IsDefined(Tint) ? Tint : GlassTintColour.None,
            Motion = Enum.IsDefined(Motion) ? Motion : GlassHqMotion.Full,
        };
        foreach (var knob in Knobs) held = held.With(knob.Id, held.Get(knob.Id));
        return held;
    }

    /// <summary>The preset these options are, or null (Custom).</summary>
    public string? Preset => Presets.FirstOrDefault(p => p.Value == Held()).Id;

    /// <summary>The options <paramref name="t"/> of the way to <paramref name="to"/>'s; each choice is the nearer end's.</summary>
    public GlassHqGlass Toward(GlassHqGlass to, double t)
    {
        ArgumentNullException.ThrowIfNull(to);
        if (t >= 1) return to;
        if (t <= 0) return this;
        var o = t < 0.5 ? this : to;
        foreach (var knob in Knobs) o = o.With(knob.Id, Get(knob.Id) + (to.Get(knob.Id) - Get(knob.Id)) * t);
        return o;
    }

    internal void Write(Utf8JsonWriter w, GlassHqGlass baseline)
    {
        var held = Held();
        foreach (var knob in Knobs)
            if (Math.Abs(held.Get(knob.Id) - baseline.Get(knob.Id)) > 1e-9) w.WriteNumber(knob.Id, Math.Round(held.Get(knob.Id), 4));
        if (held.Material != baseline.Material) w.WriteString("material", Materials.First(m => m.Value == held.Material).Id);
        if (held.Appearance != baseline.Appearance) w.WriteString("appearance", Appearances.First(m => m.Value == held.Appearance).Id);
        if (held.Motion != baseline.Motion) w.WriteString("motion", Motions.First(m => m.Value == held.Motion).Id);
        if (held.Tint != baseline.Tint) w.WriteString("tint", GlassSettings.TintColours.First(c => c.Value == held.Tint).Id);
        if (held.Concentric != baseline.Concentric) w.WriteBoolean("concentric", held.Concentric);
        if (held.Interactive != baseline.Interactive) w.WriteBoolean("interactive", held.Interactive);
    }

    internal static GlassHqGlass Read(JsonElement e, GlassHqGlass baseline)
    {
        if (e.ValueKind != JsonValueKind.Object) return baseline;
        var read = baseline;
        foreach (var knob in Knobs)
            if (e.TryGetProperty(knob.Id, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var number) && double.IsFinite(number))
                read = read.With(knob.Id, number);
        string? S(string id) => e.TryGetProperty(id, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
        bool? B(string id) => e.TryGetProperty(id, out var v) && v.ValueKind is JsonValueKind.True or JsonValueKind.False ? v.GetBoolean() : null;
        if (Materials.FirstOrDefault(m => m.Id == S("material")) is { Id: not null } m) read = read with { Material = m.Value };
        if (Appearances.FirstOrDefault(a => a.Id == S("appearance")) is { Id: not null } a) read = read with { Appearance = a.Value };
        if (Motions.FirstOrDefault(x => x.Id == S("motion")) is { Id: not null } mo) read = read with { Motion = mo.Value };
        if (GlassSettings.TintColours.FirstOrDefault(c => c.Id == S("tint")) is { Id: not null } c) read = read with { Tint = c.Value };
        if (B("concentric") is { } concentric) read = read with { Concentric = concentric };
        if (B("interactive") is { } interactive) read = read with { Interactive = interactive };
        return read;
    }
}
