// PORT-ONLY ED-NEW-shader-gradient-1: the Gradient wallpaper's options as its shader takes them (u_g), from the app's Gradient settings. The app
// sets Current (as it sets the glass's tuning); a change wakes every wallpaper so a paused one draws it at once.
using Marrow.Core.Port.Settings;

namespace Marrow.UI.Theme.Wallpapers;

/// <summary>The Gradient wallpaper's settings, for its shader.</summary>
public static class ShaderGradientParams
{
    /// <summary>The Gradient wallpaper's id in the library.</summary>
    public const string WallpaperId = "shader-gradient";

    /// <summary>How many float4 the shader reads.</summary>
    public const int Rows = 9;

    private static ShaderGradientSettings current = ShaderGradientSettings.Default;

    /// <summary>The settings drawn now.</summary>
    public static ShaderGradientSettings Current
    {
        get => current;
        set
        {
            var held = (value ?? ShaderGradientSettings.Default).Held();
            if (held.Equals(current)) return;
            current = held;
            Changed?.Invoke();
        }
    }

    /// <summary>Raised after <see cref="Current"/> changes.</summary>
    public static event Action? Changed;

    private const double Degrees = Math.PI / 180;

    /// <summary>The shader's u_g rows for <paramref name="s"/> (see shader-gradient's header).</summary>
    public static float[] Uniforms(ShaderGradientSettings s)
    {
        var u = new float[Rows * 4];
        void Row(int i, double x, double y, double z, double w) { u[i * 4] = (float)x; u[i * 4 + 1] = (float)y; u[i * 4 + 2] = (float)z; u[i * 4 + 3] = (float)w; }
        var (r1, g1, b1) = ShaderGradientSettings.Linear(s.Color1);
        var (r2, g2, b2) = ShaderGradientSettings.Linear(s.Color2);
        var (r3, g3, b3) = ShaderGradientSettings.Linear(s.Color3);
        Row(0, (int)s.Shape, s.Animate ? s.Get("uSpeed") : 0, s.Get("uStrength"), s.Get("uDensity"));
        Row(1, s.Get("uFrequency"), s.Get("uAmplitude"), s.Get("brightness"), s.Get("reflection"));
        Row(2, s.Grain ? s.Get("grainBlending") : 0, s.Lighting == GradientLighting.Environment ? 1 : 0, (int)s.Environment, s.Wireframe ? 1 : 0);
        Row(3, s.Get("cAzimuthAngle") * Degrees, s.Get("cPolarAngle") * Degrees, s.Get("cDistance"), s.Get("cameraZoom"));
        Row(4, s.Get("positionX"), s.Get("positionY"), s.Get("positionZ"), s.Get("fov") * Degrees);
        Row(5, s.Get("rotationX") * Degrees, s.Get("rotationY") * Degrees, s.Get("rotationZ") * Degrees, s.Range ? 1 : 0);
        Row(6, r1, g1, b1, s.Get("rangeStart"));
        Row(7, r2, g2, b2, s.Get("rangeEnd"));
        Row(8, r3, g3, b3, 0);
        return u;
    }
}
