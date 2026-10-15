// PORT-ONLY ED-NEW-shader-gradient-1 (row #87; Maaz, 2026-10-15, overnight brief item 7): the Gradient wallpaper's settings, every option
// ruucm/shadergradient's ShaderGradient component offers (its props: type, color1 to color3, uSpeed, uStrength, uDensity, uFrequency, uAmplitude,
// grain, brightness, reflection, lightType, envPreset, cAzimuthAngle, cPolarAngle, cDistance, cameraZoom, fov, positionX to Z, rotationX to Z,
// animate, range, rangeStart, rangeEnd, wireframe) with its "halo" defaults. shadergradient carries no licence, so this takes its option set and
// default values only; nth's own shader draws them. The setting is compact JSON of what differs from the defaults, read leniently.
using System.Globalization;
using System.Text.Json;

namespace Marrow.Core.Port.Settings;

/// <summary>The Gradient wallpaper's shape.</summary>
public enum GradientShape
{
    /// <summary>A flowing plane (shadergradient's "plane").</summary>
    Plane,
    /// <summary>A flowing sphere ("sphere").</summary>
    Sphere,
    /// <summary>A water plane seen at an angle ("waterPlane").</summary>
    WaterPlane,
}

/// <summary>How the Gradient is lit.</summary>
public enum GradientLighting
{
    /// <summary>A 3D light ("3d").</summary>
    Light3D,
    /// <summary>An environment's light ("env").</summary>
    Environment,
}

/// <summary>The environment that lights the Gradient when it is lit by one.</summary>
public enum GradientEnvironment
{
    /// <summary>"city": cool and even.</summary>
    City,
    /// <summary>"dawn": warm and low.</summary>
    Dawn,
    /// <summary>"lobby": soft and bright.</summary>
    Lobby,
}

/// <summary>One number of the Gradient's settings: its id (the component's prop), its name and description, its range and step.</summary>
public sealed record GradientKnob(string Id, string Name, string Description, double Min, double Max, double Step)
{
    /// <summary>The value held to the range (not a number: the default the caller gives).</summary>
    public double Hold(double value, double fallback) => Math.Clamp(double.IsFinite(value) ? value : fallback, Min, Max);
}

/// <summary>The Gradient wallpaper's settings.</summary>
public sealed record ShaderGradientSettings
{
    /// <summary>The store's key.</summary>
    public const string StoreKey = "shaderGradient";

    public GradientShape Shape { get; init; } = GradientShape.Plane;
    public string Color1 { get; init; } = "#ff5005";
    public string Color2 { get; init; } = "#dbba95";
    public string Color3 { get; init; } = "#d0bce1";
    public bool Animate { get; init; } = true;
    public bool Grain { get; init; } = true;
    public bool Wireframe { get; init; }
    public GradientLighting Lighting { get; init; } = GradientLighting.Light3D;
    public GradientEnvironment Environment { get; init; } = GradientEnvironment.City;
    /// <summary>Range: the time the flow runs between (rangeStart to rangeEnd), looping; off, it runs on.</summary>
    public bool Range { get; init; }

    /// <summary>The numbers, by knob id.</summary>
    public IReadOnlyDictionary<string, double> Numbers { get; init; } = Knobs.ToDictionary(k => k.Id, k => Defaults[k.Id]);

    /// <summary>Every number the component offers, in the page's order.</summary>
    public static IReadOnlyList<GradientKnob> Knobs { get; } =
    [
        new("uSpeed", "Speed", "How fast the colours flow", 0, 2, 0.01),
        new("uStrength", "Strength", "How far the flow lifts and folds the surface", 0, 10, 0.1),
        new("uDensity", "Density", "How many folds the surface holds", 0, 7, 0.1),
        new("uFrequency", "Frequency", "How quickly the folds change across the surface", 0, 10, 0.1),
        new("uAmplitude", "Amplitude", "How tall the waves on the folds are", 0, 7, 0.1),
        new("brightness", "Brightness", "How bright the light on the surface is", 0, 3, 0.05),
        new("reflection", "Reflection", "How much the surface mirrors its light", 0, 1, 0.01),
        new("grainBlending", "Grain amount", "How strong the film grain is", 0, 1, 0.01),
        new("cAzimuthAngle", "Camera azimuth", "The camera's angle around the surface (degrees)", 0, 360, 1),
        new("cPolarAngle", "Camera polar angle", "The camera's angle down from above (degrees)", 0, 180, 1),
        new("cDistance", "Camera distance", "How far the camera stands from the surface", 0.5, 12, 0.1),
        new("cameraZoom", "Zoom", "How far the camera zooms in", 0.2, 5, 0.05),
        new("fov", "Field of view", "How wide the camera sees (degrees)", 10, 120, 1),
        new("positionX", "Position X", "Where the surface sits, left to right", -5, 5, 0.1),
        new("positionY", "Position Y", "Where the surface sits, down to up", -5, 5, 0.1),
        new("positionZ", "Position Z", "Where the surface sits, far to near", -5, 5, 0.1),
        new("rotationX", "Rotation X", "How far the surface turns about its X axis (degrees)", -180, 180, 1),
        new("rotationY", "Rotation Y", "How far the surface turns about its Y axis (degrees)", -180, 180, 1),
        new("rotationZ", "Rotation Z", "How far the surface turns about its Z axis (degrees)", -180, 180, 1),
        new("rangeStart", "Range start", "Where the flow's range starts (seconds)", 0, 40, 0.1),
        new("rangeEnd", "Range end", "Where the flow's range ends (seconds)", 0, 40, 0.1),
    ];

    /// <summary>shadergradient's "halo" values.</summary>
    public static IReadOnlyDictionary<string, double> Defaults { get; } = new Dictionary<string, double>
    {
        ["uSpeed"] = 0.4, ["uStrength"] = 4, ["uDensity"] = 1.3, ["uFrequency"] = 5.5, ["uAmplitude"] = 1, ["brightness"] = 1.2, ["reflection"] = 0.1,
        ["grainBlending"] = 0.3, ["cAzimuthAngle"] = 180, ["cPolarAngle"] = 90, ["cDistance"] = 3.6, ["cameraZoom"] = 1, ["fov"] = 45,
        ["positionX"] = -1.4, ["positionY"] = 0, ["positionZ"] = 0, ["rotationX"] = 0, ["rotationY"] = 10, ["rotationZ"] = 50, ["rangeStart"] = 0, ["rangeEnd"] = 40,
    };

    public static ShaderGradientSettings Default { get; } = new();

    public static IReadOnlyList<(string Id, string Name, GradientShape Value)> Shapes { get; } =
        [("plane", "Plane", GradientShape.Plane), ("sphere", "Sphere", GradientShape.Sphere), ("waterPlane", "Water", GradientShape.WaterPlane)];

    public static IReadOnlyList<(string Id, string Name, GradientLighting Value)> Lightings { get; } =
        [("3d", "3D light", GradientLighting.Light3D), ("env", "Environment", GradientLighting.Environment)];

    public static IReadOnlyList<(string Id, string Name, GradientEnvironment Value)> Environments { get; } =
        [("city", "City", GradientEnvironment.City), ("dawn", "Dawn", GradientEnvironment.Dawn), ("lobby", "Lobby", GradientEnvironment.Lobby)];

    /// <summary>A number by knob id.</summary>
    public double Get(string id) => Numbers.TryGetValue(id, out var v) ? v : Defaults[id];

    /// <summary>These settings with one number changed (held to its range).</summary>
    public ShaderGradientSettings With(string id, double value)
    {
        var knob = Knobs.FirstOrDefault(k => k.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown gradient option.");
        var numbers = Knobs.ToDictionary(k => k.Id, k => Get(k.Id));
        numbers[id] = knob.Hold(value, Defaults[id]);
        return this with { Numbers = numbers };
    }

    /// <summary>Every value held: the numbers to their ranges, the colours to #rrggbb (else the default's), the choices to the defined ones.</summary>
    public ShaderGradientSettings Held() => this with
    {
        Shape = Enum.IsDefined(Shape) ? Shape : GradientShape.Plane,
        Lighting = Enum.IsDefined(Lighting) ? Lighting : GradientLighting.Light3D,
        Environment = Enum.IsDefined(Environment) ? Environment : GradientEnvironment.City,
        Color1 = Hex(Color1) ?? Default.Color1, Color2 = Hex(Color2) ?? Default.Color2, Color3 = Hex(Color3) ?? Default.Color3,
        Numbers = Knobs.ToDictionary(k => k.Id, k => k.Hold(Get(k.Id), Defaults[k.Id])),
    };

    /// <summary>A colour as #rrggbb, lower case, or null.</summary>
    public static string? Hex(string? value)
    {
        if (value is not { Length: 7 } || value[0] != '#') return null;
        foreach (var c in value.AsSpan(1)) if (!Uri.IsHexDigit(c)) return null;
        return value.ToLowerInvariant();
    }

    /// <summary>A colour's linear red, green and blue (0 to 1).</summary>
    public static (double R, double G, double B) Linear(string hex)
    {
        var h = Hex(hex) ?? "#000000";
        double L(int at)
        {
            var v = int.Parse(h.AsSpan(at, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;
            return v <= 0.04045 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }
        return (L(1), L(3), L(5));
    }

    public bool Equals(ShaderGradientSettings? other) =>
        other is not null && Shape == other.Shape && Color1 == other.Color1 && Color2 == other.Color2 && Color3 == other.Color3 && Animate == other.Animate
        && Grain == other.Grain && Wireframe == other.Wireframe && Lighting == other.Lighting && Environment == other.Environment && Range == other.Range
        && Knobs.All(k => Get(k.Id) == other.Get(k.Id));

    public override int GetHashCode() => HashCode.Combine(Shape, Color1, Color2, Color3, Animate, Lighting, Environment, Knobs.Sum(k => Get(k.Id)));

    /// <summary>Compact JSON of what differs from the defaults.</summary>
    public byte[] ToJson()
    {
        var held = Held();
        using var stream = new MemoryStream();
        using (var w = new Utf8JsonWriter(stream))
        {
            w.WriteStartObject();
            if (held.Shape != Default.Shape) w.WriteString("type", Shapes.First(s => s.Value == held.Shape).Id);
            if (held.Color1 != Default.Color1) w.WriteString("color1", held.Color1);
            if (held.Color2 != Default.Color2) w.WriteString("color2", held.Color2);
            if (held.Color3 != Default.Color3) w.WriteString("color3", held.Color3);
            if (!held.Animate) w.WriteBoolean("animate", false);
            if (!held.Grain) w.WriteBoolean("grain", false);
            if (held.Wireframe) w.WriteBoolean("wireframe", true);
            if (held.Range) w.WriteBoolean("range", true);
            if (held.Lighting != Default.Lighting) w.WriteString("lightType", Lightings.First(l => l.Value == held.Lighting).Id);
            if (held.Environment != Default.Environment) w.WriteString("envPreset", Environments.First(e => e.Value == held.Environment).Id);
            foreach (var k in Knobs) if (held.Get(k.Id) != Defaults[k.Id]) w.WriteNumber(k.Id, held.Get(k.Id));
            w.WriteEndObject();
        }
        return stream.ToArray();
    }

    /// <summary>The lenient read: anything missing or wrong is the default's.</summary>
    public static ShaderGradientSettings FromJson(ReadOnlySpan<byte> json)
    {
        try
        {
            using var document = JsonDocument.Parse(json.ToArray());
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object) return Default;
            string? Text(string id) => root.TryGetProperty(id, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
            bool? Flag(string id) => root.TryGetProperty(id, out var v) && v.ValueKind is JsonValueKind.True or JsonValueKind.False ? v.GetBoolean() : null;
            var numbers = Knobs.ToDictionary(k => k.Id, k => root.TryGetProperty(k.Id, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var d)
                ? k.Hold(d, Defaults[k.Id]) : Defaults[k.Id]);
            return new ShaderGradientSettings
            {
                Shape = Shapes.FirstOrDefault(s => s.Id == Text("type")) is { Id: not null } shape ? shape.Value : Default.Shape,
                Color1 = Hex(Text("color1")) ?? Default.Color1, Color2 = Hex(Text("color2")) ?? Default.Color2, Color3 = Hex(Text("color3")) ?? Default.Color3,
                Animate = Flag("animate") ?? true, Grain = Flag("grain") ?? true, Wireframe = Flag("wireframe") ?? false, Range = Flag("range") ?? false,
                Lighting = Lightings.FirstOrDefault(l => l.Id == Text("lightType")) is { Id: not null } light ? light.Value : Default.Lighting,
                Environment = Environments.FirstOrDefault(e => e.Id == Text("envPreset")) is { Id: not null } env ? env.Value : Default.Environment,
                Numbers = numbers,
            };
        }
        catch (JsonException)
        {
            return Default;
        }
    }

    public override string ToString() => System.Text.Encoding.UTF8.GetString(ToJson());
}
