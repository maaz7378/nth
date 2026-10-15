// PORT-ONLY ED-NEW-shader-effects-1: the Shaders section's effects (shader-effects-inc/shaders, MIT): each live wallpaper of tier "shaders" and
// its own options (its "props" in live-wallpapers.json: colours, numbers and choices, in the order the shader reads them from u_e), and the
// uniforms the shader takes for the user's settings. The app sets Current; a change wakes every wallpaper so a paused one draws it at once.
using System.Globalization;
using System.Text.Json;
using Marrow.Core.Port.Settings;

namespace Marrow.UI.Theme.Wallpapers;

/// <summary>One option of a shader effect.</summary>
public sealed record ShaderEffectProp(string Id, string Label, string Kind, string Detail, double Min, double Max, double Step, double Number, string Colour, IReadOnlyList<string> Choices)
{
    /// <summary>How many floats it takes in u_e (a colour four, anything else one).</summary>
    public int Width => Kind == "color" ? 4 : 1;
}

/// <summary>One shader effect of the Shaders section.</summary>
public sealed record ShaderEffect(string Id, string Name, string Desc, IReadOnlyList<ShaderEffectProp> Props);

/// <summary>The Shaders section's effects and their uniforms.</summary>
public static class ShaderEffects
{
    /// <summary>The wallpaper tier of the Shaders section.</summary>
    public const string Tier = "shaders";

    /// <summary>How many float4 the shaders read.</summary>
    public const int Rows = 8;

    private static IReadOnlyList<ShaderEffect>? all;
    private static ShaderEffectSettings current = ShaderEffectSettings.Default;

    /// <summary>Every effect, in the library's order.</summary>
    public static IReadOnlyList<ShaderEffect> All => all ??= Load();

    /// <summary>An effect by id, or null.</summary>
    public static ShaderEffect? Find(string id) => All.FirstOrDefault(e => e.Id == id);

    /// <summary>The settings drawn now.</summary>
    public static ShaderEffectSettings Current
    {
        get => current;
        set
        {
            var next = value ?? ShaderEffectSettings.Default;
            if (next.Equals(current)) return;
            current = next;
            Changed?.Invoke();
        }
    }

    /// <summary>Raised after <see cref="Current"/> changes.</summary>
    public static event Action? Changed;

    /// <summary>The shader's u_e for <paramref name="effect"/> under <paramref name="settings"/>: each option in order, a colour as linear rgba.</summary>
    public static float[] Uniforms(ShaderEffect effect, ShaderEffectSettings settings)
    {
        var u = new float[Rows * 4];
        var at = 0;
        foreach (var p in effect.Props)
        {
            if (at + p.Width > u.Length) break;
            if (p.Kind == "color")
            {
                var (r, g, b) = ShaderGradientSettings.Linear(settings.Colour(effect.Id, p.Id, p.Colour));
                (u[at], u[at + 1], u[at + 2], u[at + 3]) = ((float)r, (float)g, (float)b, 1f);
            }
            else u[at] = (float)Math.Clamp(settings.Number(effect.Id, p.Id, p.Number), p.Min, p.Max);
            at += p.Width;
        }
        return u;
    }

    private static IReadOnlyList<ShaderEffect> Load()
    {
        using var stream = typeof(ShaderEffects).Assembly.GetManifestResourceStream("Marrow.Wallpapers.live.json")
            ?? throw new InvalidOperationException("The live wallpapers are missing.");
        using var document = JsonDocument.Parse(stream);
        var list = new List<ShaderEffect>();
        foreach (var w in document.RootElement.GetProperty("wallpapers").EnumerateArray())
        {
            if (w.GetProperty("tier").GetString() != Tier) continue;
            var props = new List<ShaderEffectProp>();
            foreach (var p in w.GetProperty("props").EnumerateArray())
            {
                var kind = p.GetProperty("kind").GetString()!;
                double N(string name, double fallback) => p.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : fallback;
                var choices = p.TryGetProperty("options", out var o) ? o.EnumerateArray().Select(x => x.GetString()!).ToArray() : [];
                props.Add(new ShaderEffectProp(p.GetProperty("id").GetString()!, p.GetProperty("label").GetString()!, kind,
                    p.TryGetProperty("detail", out var d) ? d.GetString()! : "",
                    N("min", 0), N("max", kind == "select" ? Math.Max(0, choices.Length - 1) : 1), N("step", 1),
                    kind == "color" ? 0 : N("default", 0), kind == "color" ? p.GetProperty("default").GetString()! : "", choices));
            }
            list.Add(new ShaderEffect(w.GetProperty("id").GetString()!, w.GetProperty("name").GetString()!, w.GetProperty("desc").GetString()!, props));
        }
        return list;
    }

    /// <summary>A number as the page shows it.</summary>
    public static string Format(ShaderEffectProp p, double v) =>
        v.ToString(p.Step < 0.01 ? "0.000" : p.Step < 0.1 ? "0.00" : p.Step < 1 ? "0.0" : "0", CultureInfo.InvariantCulture);
}
