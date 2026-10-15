// PORT-ONLY ED-NEW-shader-effects-1 (row #87; Maaz, 2026-10-15, overnight brief item 8): the Shaders section's settings: for each
// shader-effects-inc effect, the values of its own options the user changed (a colour as #rrggbb, a number or a choice as a number), and which
// effect the section shows. Stored as compact JSON under "shaderEffects", read leniently; an option the user never touched is the effect's default.
using System.Globalization;
using System.Text.Json;

namespace Marrow.Core.Port.Settings;

/// <summary>The Shaders section's settings.</summary>
public sealed record ShaderEffectSettings
{
    /// <summary>The store's key.</summary>
    public const string StoreKey = "shaderEffects";

    /// <summary>The effect the section shows.</summary>
    public string Shown { get; init; } = "";

    /// <summary>Each effect's changed options: effect id, then option id, then its value (a colour "#rrggbb", else a number as text).</summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Values { get; init; } = new Dictionary<string, IReadOnlyDictionary<string, string>>();

    public static ShaderEffectSettings Default { get; } = new();

    /// <summary>An option's stored value, or null when the user never changed it.</summary>
    public string? Get(string effect, string option) => Values.TryGetValue(effect, out var o) && o.TryGetValue(option, out var v) ? v : null;

    /// <summary>A number option's value, or <paramref name="fallback"/>.</summary>
    public double Number(string effect, string option, double fallback) =>
        Get(effect, option) is { } v && double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) && double.IsFinite(d) ? d : fallback;

    /// <summary>A colour option's value, or <paramref name="fallback"/>.</summary>
    public string Colour(string effect, string option, string fallback) => ShaderGradientSettings.Hex(Get(effect, option)) ?? fallback;

    /// <summary>These settings with one option of one effect set.</summary>
    public ShaderEffectSettings With(string effect, string option, string value)
    {
        var all = Values.ToDictionary(e => e.Key, e => e.Value.ToDictionary(o => o.Key, o => o.Value));
        if (!all.TryGetValue(effect, out var options)) all[effect] = options = [];
        options[option] = value;
        return this with { Values = all.ToDictionary(e => e.Key, e => (IReadOnlyDictionary<string, string>)e.Value) };
    }

    /// <summary>These settings with a number option set (invariant text).</summary>
    public ShaderEffectSettings With(string effect, string option, double value) => With(effect, option, value.ToString("R", CultureInfo.InvariantCulture));

    /// <summary>These settings with every option of one effect back to its defaults.</summary>
    public ShaderEffectSettings Without(string effect) =>
        this with { Values = Values.Where(e => e.Key != effect).ToDictionary(e => e.Key, e => e.Value) };

    public bool Equals(ShaderEffectSettings? other) =>
        other is not null && Shown == other.Shown && Values.Count == other.Values.Count
        && Values.All(e => other.Values.TryGetValue(e.Key, out var o) && e.Value.Count == o.Count && e.Value.All(v => o.TryGetValue(v.Key, out var w) && w == v.Value));

    public override int GetHashCode() => HashCode.Combine(Shown, Values.Count);

    /// <summary>Compact JSON: {"shown": id, "values": {effect: {option: value}}}.</summary>
    public byte[] ToJson()
    {
        using var stream = new MemoryStream();
        using (var w = new Utf8JsonWriter(stream))
        {
            w.WriteStartObject();
            if (Shown.Length > 0) w.WriteString("shown", Shown);
            if (Values.Count > 0)
            {
                w.WriteStartObject("values");
                foreach (var (effect, options) in Values.OrderBy(e => e.Key, StringComparer.Ordinal))
                {
                    w.WriteStartObject(effect);
                    foreach (var (option, value) in options.OrderBy(o => o.Key, StringComparer.Ordinal)) w.WriteString(option, value);
                    w.WriteEndObject();
                }
                w.WriteEndObject();
            }
            w.WriteEndObject();
        }
        return stream.ToArray();
    }

    /// <summary>The lenient read.</summary>
    public static ShaderEffectSettings FromJson(ReadOnlySpan<byte> json)
    {
        try
        {
            using var document = JsonDocument.Parse(json.ToArray());
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return Default;
            var values = new Dictionary<string, IReadOnlyDictionary<string, string>>();
            if (root.TryGetProperty("values", out var v) && v.ValueKind == JsonValueKind.Object)
                foreach (var effect in v.EnumerateObject())
                    if (effect.Value.ValueKind == JsonValueKind.Object)
                        values[effect.Name] = effect.Value.EnumerateObject().Where(o => o.Value.ValueKind == JsonValueKind.String).ToDictionary(o => o.Name, o => o.Value.GetString()!);
            return new ShaderEffectSettings
            {
                Shown = root.TryGetProperty("shown", out var s) && s.ValueKind == JsonValueKind.String ? s.GetString()! : "",
                Values = values,
            };
        }
        catch (JsonException)
        {
            return Default;
        }
    }

    public override string ToString() => System.Text.Encoding.UTF8.GetString(ToJson());
}
