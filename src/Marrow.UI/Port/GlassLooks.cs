// PORT-ONLY: ED-NEW-glasslook-1 (OD-0107 and its amendment: the per-surface look set in one place), ED-NEW-glasslook-2 (the owner's Glass settings in
// Customization) and OD-0147 / ED-NEW-glasslook-4 (the owner approved the Glass UI canvas, 2026-10-11: its material ships at its defaults, every
// setting the user's). The one table that gives each surface its look: the canvas's material for the surface's kind (GlassCanvas) under the user's
// Glass settings and Windows' switches (GlassTuning), or a piece's own knob look (GlassLook: the release material, kept for a piece that names it).
// Every value is a uniform or a CPU-side size: a change here needs no new GPU program (GlassMaterial.WarmupPrograms stays whole), so a slider drags
// with no recompile.
using System.Runtime.InteropServices;
using Marrow.Core.Port.Settings;

namespace Marrow.UI.Port;

/// <summary>The kinds of glass surface the look table (<see cref="GlassLooks"/>) is set for.</summary>
public enum GlassSurface
{
    /// <summary>A window's own glass over its backdrop or wallpaper (a rectangle with no corner: Chrome's window glass layer, the tour's backdrop).</summary>
    Window,
    /// <summary>A rounded panel or card (the head panel, the usage panel, the floating sidebar, the minimap, sheets, request cards).</summary>
    Panel,
    /// <summary>The composer pill (its corner measured circular in ED-275's band).</summary>
    Composer,
    /// <summary>A capsule or circle that is not a button (a quiet glass shape).</summary>
    Capsule,
    /// <summary>A glass button or toolbar pill: a capsule or circle that responds to the pointer or carries a tint (Chrome's glass buttons).</summary>
    Button,
    /// <summary>ED-NEW-glasslook-3: a glass piece inside another glass piece (a control on the composer, a button on a panel). The owner's principles
    /// (2026-10-11): only the container is glass; what is on it is glyphs and text with quiet hover fills (<see cref="GlassLook.QuietLook"/>), a hero
    /// control excepted.</summary>
    Inner,
    /// <summary>OD-0147: the sidebar (the floating sidebar panel): the canvas's sidebar glass, which the Translucent sidebar switch turns all but solid.</summary>
    Sidebar,
}

/// <summary>
/// ED-NEW-glasslook-1: the material's look. Each knob scales the calibrated release material (ED-356, ED-358); 1 (0 for <see cref="Clarity"/>) is the
/// look the release drew. The repos' names for each are in brackets. OD-0147: a look with a <see cref="Canvas"/> is the approved canvas's material
/// instead (every surface's under the user's settings, <see cref="GlassLooks.For(GlassSurface)"/>); the knob looks stay for a piece that names one.
/// </summary>
/// <param name="Clarity">How clear the lens rim is, 0 to 1: the share of the unblurred backdrop the rim band bends (rizzytoday's displacement of
/// the backdrop itself, its filter without blur), times the bend's own fall-off; 0 bends the frost only, as the release did.</param>
/// <param name="Bend">The bend at the edge and the rim ring, times the piece's own (rizzytoday's scale; dashersw's rimIntensity), held so that
/// bend and ring together read at most 60 pt past the edge (inside the 72 pt the material reads around itself).</param>
/// <param name="Band">The width of the bend's fall-off, times the piece's own (rizzytoday's blur of its neutral centre), 0.5 to 1.25.</param>
/// <param name="Fringe">The red, green and blue split, times rizzytoday's aberration (0 / 10 / 20 on 180), 0 to 3.</param>
/// <param name="Rim">The specular rim and the light inner edge, times the Mac-calibrated light, 0 to 2.</param>
/// <param name="Blur">The backdrop blur's sigma, times the piece's own (dashersw's blurRadius), at most 12 pt.</param>
/// <param name="Tint">The film's opacity, times the calibrated film (dashersw's tintOpacity), 0 to 2.</param>
/// <param name="Saturation">The saturation lift of what the glass shows, times the calibrated lift (rizzytoday's saturate()), 0 to 2.</param>
/// <param name="Shadow">The contact line just outside the edge, times the calibrated line, 0 to 2.</param>
/// <param name="Profile">ED-NEW-glasslook-3: the bezel's lens, 0 (the release's logistic fall-off with dashersw's rim ring) to 1 (a convex squircle
/// refracting by Snell's law: the bend smooth from the edge to an undistorted centre).</param>
/// <param name="Glow">The light inner edge (an inner bevel), times the calibrated one, 0 to 2; 0 draws none.</param>
/// <param name="Hairline">The rim, 0 (the release's line at the top and the bottom) to 1 (a specular hairline lit from the top left, fading around the
/// shape, with a faint counter-highlight).</param>
/// <param name="FringeCap">The most the red and blue reads part, in pt (0: uncapped, as the release); a cap of about 1 keeps the fringe a hint.</param>
/// <param name="Lift">How much brighter the glass shows what lies behind it, toward white, 0 to 0.25 (no fill: the backdrop's own colour, lifted).</param>
/// <param name="Depth">A soft, wide, faint shadow under the glass (its darkest share just outside the edge), 0 to 0.4; the tight contact line is
/// <paramref name="Shadow"/>'s.</param>
public sealed record GlassLook(double Clarity = 0, double Bend = 1, double Band = 1, double Fringe = 1, double Rim = 1, double Blur = 1, double Tint = 1,
    double Saturation = 1, double Shadow = 1, double Profile = 0, double Glow = 1, double Hairline = 0, double FringeCap = 0, double Lift = 0, double Depth = 0)
{
    /// <summary>The look the release drew: the bend and fringe through the frost (ED-356).</summary>
    public static GlassLook Release { get; } = new();

    /// <summary>The repos' clear lens rim over the Mac-calibrated body (ED-NEW-glasslook-1's measured proposal, for the design canvas).</summary>
    public static GlassLook Liquid { get; } = new(Clarity: 0.9, Bend: 2, Fringe: 1.5, Rim: 1.3);

    /// <summary>ED-NEW-glasslook-3: the owner's premium principles (2026-10-11, relayed by the lead: "plasticky, not premium"), a candidate until the
    /// design canvas reports its numbers: glass from what it does to the backdrop (a clear squircle bezel, a slight lift, a little frost, a small
    /// tint), a hairline specular rim with a counter-highlight and no inner bevel, a fringe of at most 1 pt, a soft wide faint shadow and the tight
    /// contact line.</summary>
    public static GlassLook Premium { get; } = new(Clarity: 0.85, Bend: 1.2, Fringe: 1, Rim: 1, Blur: 0.6, Tint: 0.25, Saturation: 1.15, Shadow: 0.8,
        Profile: 1, Glow: 0, Hairline: 1, FringeCap: 1, Lift: 0.05, Depth: 0.12);

    /// <summary>ED-NEW-glasslook-3: the candidate for Apple's Tinted Liquid Glass (iOS 27, the owner's target), until the canvas reports its numbers:
    /// the premium finish with a more opaque, frosted body for contrast.</summary>
    public static GlassLook TintedCandidate { get; } = new(Clarity: 0.5, Bend: 1, Fringe: 1, Rim: 1, Blur: 1, Tint: 1.8, Saturation: 1, Shadow: 0.8,
        Profile: 1, Glow: 0, Hairline: 1, FringeCap: 1, Lift: 0, Depth: 0.14);

    /// <summary>ED-NEW-glass-alive-1 (row #87 (a), row #32): the window's glass layer over a wallpaper (Chrome's WindowBackdrop overlay). The canvas
    /// drew it as the Settings sheet's glass, the widest frost and the heaviest tint, so the wallpaper was washed out before any panel's lens read it
    /// and the rims had nothing bright to bend. This look keeps the layer (the same one pass, no new program) but nearly clear: a quarter of the
    /// blur, a fifth of the film, a 1.15 saturation lift, and no bend, fringe, rim, inner light or contact line at the window's own edge.</summary>
    public static GlassLook WallpaperClear { get; } = new(Clarity: 1, Bend: 0, Fringe: 0, Rim: 0, Blur: 0.25, Tint: 0.2, Saturation: 1.15, Shadow: 0,
        Glow: 0);

    /// <summary>The look <paramref name="t"/> of the way from this one to <paramref name="to"/> (a Look's crossfade): every knob in between; at 1 exactly <paramref name="to"/>.</summary>
    public GlassLook Toward(GlassLook to, double t)
    {
        ArgumentNullException.ThrowIfNull(to);
        if (t >= 1) return to;
        if (t <= 0) return this;
        double M(double a, double b) => a + (b - a) * t;
        return new GlassLook(M(Clarity, to.Clarity), M(Bend, to.Bend), M(Band, to.Band), M(Fringe, to.Fringe), M(Rim, to.Rim), M(Blur, to.Blur), M(Tint, to.Tint),
            M(Saturation, to.Saturation), M(Shadow, to.Shadow), M(Profile, to.Profile), M(Glow, to.Glow), M(Hairline, to.Hairline), M(FringeCap, to.FringeCap),
            M(Lift, to.Lift), M(Depth, to.Depth))
        {
            Quiet = Quiet && to.Quiet,
            Canvas = Canvas is { } a && to.Canvas is { } b && a.Kind == b.Kind
                ? a with { Tuning = a.Tuning.Toward(b.Tuning, t), Rizzy = a.Rizzy.Toward(b.Rizzy, t), Dasher = a.Dasher.Toward(b.Dasher, t), GlassHq = a.GlassHq.Toward(b.GlassHq, t), Engine = t < 0.5 ? a.Engine : b.Engine } : t < 0.5 ? Canvas : to.Canvas,
        };
    }

    /// <summary>The soft shadow's blur (pt) under a piece whose smaller side is <paramref name="smaller"/> pt.</summary>
    internal static double DepthRadius(double smaller) => Math.Clamp(0.25 * smaller, 6, 20);

    /// <summary>How far the soft shadow drops (pt).</summary>
    internal static double DepthOffset(double smaller) => 0.3 * DepthRadius(smaller);

    internal double ClarityValue => Math.Clamp(Clarity, 0, 1);
    internal double BendValue => Math.Clamp(Bend, 0, 4);
    internal double BandValue => Math.Clamp(Band, 0.5, 1.25);
    internal double FringeValue => Math.Clamp(Fringe, 0, 3);
    internal double RimValue => Math.Clamp(Rim, 0, 2);
    internal double BlurValue => Math.Clamp(Blur, 0, 4);
    internal double TintValue => Math.Clamp(Tint, 0, 2);
    internal double SaturationValue => Math.Clamp(Saturation, 0, 2);
    internal double ShadowValue => Math.Clamp(Shadow, 0, 2);
    /// <summary>ED-NEW-glasslook-3: a quiet look draws no glass at all (what lies on another glass is glyphs and text, not glass on glass).</summary>
    public bool Quiet { get; init; }

    /// <summary>The look that draws no glass.</summary>
    public static GlassLook QuietLook { get; } = new() { Quiet = true };

    internal double ProfileValue => Math.Clamp(Profile, 0, 1);
    internal double GlowValue => Math.Clamp(Glow, 0, 2);
    internal double HairlineValue => Math.Clamp(Hairline, 0, 1);
    internal double FringeCapValue => Math.Clamp(FringeCap, 0, 4);
    internal double LiftValue => Math.Clamp(Lift, 0, 0.25);
    internal double DepthValue => Math.Clamp(Depth, 0, 0.4);

    /// <summary>OD-0147 (the approved design canvas): the look is the canvas's material, its kind and its inputs; null for a look drawn with the knobs
    /// above (an effect that names its own look).</summary>
    public GlassCanvas? Canvas { get; init; }

    /// <summary>The canvas's material for <paramref name="kind"/> with <paramref name="tuning"/>.</summary>
    public static GlassLook Of(GlassKind kind, GlassTuning tuning) => new() { Canvas = new GlassCanvas(kind, tuning) };
}

/// <summary>OD-0147: the kinds of the approved material (the design canvas's glassSpec kinds, and its flat controls).</summary>
public enum GlassKind
{
    /// <summary>The composer bar.</summary>
    Composer,
    /// <summary>A toolbar capsule or a glass button.</summary>
    Capsule,
    /// <summary>The sidebar: a wider frost, a slimmer bezel, no pointer glow.</summary>
    Sidebar,
    /// <summary>A menu, popover or floating panel: more frost and a little more tint (it reads over busy threads), a lifted shadow.</summary>
    Popover,
    /// <summary>A sheet or a window's glass: the widest bezel, the most frost and tint, the deepest shadow.</summary>
    Settings,
    /// <summary>A control on glass (no glass on glass): a glyph or label with a flat capsule under the pointer, no bezel, rim or shadow.</summary>
    Flat,
}

/// <summary>
/// OD-0147: the approved material's inputs: the user's Glass settings (<see cref="GlassSettings"/>) with each switch resolved (Windows' own where the
/// user has set none) and as a number from 0 to 1, so a change of either glides, and the tint colour as its colour and amount.
/// </summary>
public readonly record struct GlassTuning(double Refraction, double Bezel, double Frost, double Fringe, double Rim, double Tint, double Saturation,
    double Shadow, double Contrast, double Reduce, double SidebarOff, double Motion, double TintR, double TintG, double TintB, double TintAmount)
{
    /// <summary>The canvas's tint colour amount (its m10.w): a small cast, capped so it stays glass.</summary>
    internal const double TintColourAmount = 0.16;

    /// <summary>The inputs of <paramref name="settings"/> on <paramref name="system"/>.</summary>
    public static GlassTuning Of(GlassSettings settings, IGlassSystem system)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(system);
        var s = settings.Held();
        var colour = GlassSettings.TintColours.First(c => c.Value == s.TintColour);
        static double On(bool on) => on ? 1 : 0;
        return new GlassTuning(s.Refraction, s.Bezel, s.Frost, s.Fringe, s.Rim, s.Tint, s.Saturation, s.Shadow,
            On(s.IncreaseContrast ?? system.IncreaseContrast), On(s.ReduceTransparency ?? system.ReduceTransparency), On(!s.TranslucentSidebar),
            On(!(s.ReducedMotion ?? system.ReduceMotion)), colour.R, colour.G, colour.B, s.TintColour == GlassTintColour.None ? 0 : TintColourAmount);
    }

    /// <summary><paramref name="t"/> of the way to <paramref name="to"/>; a tint colour from none takes the new colour at once and fades its amount in.</summary>
    public GlassTuning Toward(GlassTuning to, double t)
    {
        if (t >= 1) return to;
        if (t <= 0) return this;
        double M(double a, double b) => a + (b - a) * t;
        var (r, g, b) = TintAmount <= 0 ? (to.TintR, to.TintG, to.TintB) : to.TintAmount <= 0 ? (TintR, TintG, TintB) : (M(TintR, to.TintR), M(TintG, to.TintG), M(TintB, to.TintB));
        return new GlassTuning(M(Refraction, to.Refraction), M(Bezel, to.Bezel), M(Frost, to.Frost), M(Fringe, to.Fringe), M(Rim, to.Rim), M(Tint, to.Tint),
            M(Saturation, to.Saturation), M(Shadow, to.Shadow), M(Contrast, to.Contrast), M(Reduce, to.Reduce), M(SidebarOff, to.SidebarOff), M(Motion, to.Motion),
            r, g, b, M(TintAmount, to.TintAmount));
    }
}

/// <summary>Windows' own accessibility switches the Glass settings follow until the user sets theirs (one source of truth: OD-0147's linking).</summary>
public interface IGlassSystem
{
    /// <summary>Windows' Animation effects is off (ED-084's read).</summary>
    bool ReduceMotion { get; }

    /// <summary>A Windows contrast theme is on.</summary>
    bool IncreaseContrast { get; }

    /// <summary>Windows' Transparency effects is off.</summary>
    bool ReduceTransparency { get; }
}

/// <summary>The Windows 11 answers: Animation effects (SPI_GETCLIENTAREAANIMATION, Chrome's ReduceMotionPlatform), contrast themes
/// (SPI_GETHIGHCONTRAST) and Transparency effects (HKCU Personalize EnableTransparency, 0 when off). Read only; nothing in Windows is changed.</summary>
public sealed class WindowsGlassSystem : IGlassSystem
{
    private const uint SpiGetHighContrast = 0x0042, HighContrastOn = 0x1;

    [StructLayout(LayoutKind.Sequential)]
    private struct HighContrast
    {
        public uint Size;
        public uint Flags;
        public IntPtr Scheme;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint action, uint parameter, ref HighContrast value, uint update);

    /// <inheritdoc />
    public bool ReduceMotion => Marrow.UI.Chrome.Chrome.ReduceMotionPlatform.Current.AccessibilityDisplayShouldReduceMotion;

    /// <inheritdoc />
    public bool IncreaseContrast
    {
        get
        {
            if (!OperatingSystem.IsWindows()) return false;
            var value = new HighContrast { Size = (uint)Marshal.SizeOf<HighContrast>() };
            return SystemParametersInfo(SpiGetHighContrast, value.Size, ref value, 0) && (value.Flags & HighContrastOn) != 0;
        }
    }

    /// <inheritdoc />
    public bool ReduceTransparency
    {
        get
        {
            if (!OperatingSystem.IsWindows()) return false;
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                return key?.GetValue("EnableTransparency") is int enabled && enabled == 0;
            }
            catch (Exception e) when (e is System.Security.SecurityException or UnauthorizedAccessException or IOException) { return false; }
        }
    }
}

/// <summary>
/// ED-NEW-glasslook-1 and OD-0147: THE ONE PLACE each surface's glass look is set. The owner approved the design canvas
/// (E:/Droppy/work/design/glass-ui) and asked for it to ship with its own defaults: every surface is the canvas's material for its kind, with the
/// user's Glass settings (<see cref="Tuning"/>, ED-NEW-glasslook-2) as its inputs. A piece whose effect names its own <see cref="GlassEffect.Look"/>
/// takes that instead (the knob looks: the release's, and the candidates the canvas replaced, kept for what names them).
/// </summary>
public static class GlassLooks
{
    /// <summary>The canvas kind a surface is drawn as: the composer the composer, a capsule or button the toolbar capsule, the sidebar the sidebar, a
    /// rounded panel or card the popover (a floating panel over the thread), a window's glass the Settings sheet, and a piece inside another glass
    /// the flat control (no glass on glass).</summary>
    public static GlassKind KindOf(GlassSurface surface) => surface switch
    {
        GlassSurface.Composer => GlassKind.Composer,
        GlassSurface.Capsule or GlassSurface.Button => GlassKind.Capsule,
        GlassSurface.Sidebar => GlassKind.Sidebar,
        GlassSurface.Window => GlassKind.Settings,
        GlassSurface.Inner => GlassKind.Flat,
        _ => GlassKind.Popover,
    };

    /// <summary>The look of <paramref name="surface"/> under the user's settings now.</summary>
    public static GlassLook For(GlassSurface surface) => Engined(GlassLook.Of(KindOf(surface), Shown), ShownSettings);

    /// <summary>The look of <paramref name="surface"/> at <paramref name="variant"/>'s stop (every fine-tuning value the stop's own), the user's
    /// switches kept.</summary>
    public static GlassLook For(GlassSurface surface, GlassVariant variant) =>
        Engined(GlassLook.Of(KindOf(surface), GlassTuning.Of(tuning.At(variant), system)), tuning);

    // ED-NEW-glass-alive-5, -8: the canvas material's look drawn by the user's engine with its own options (the canvas with Liquid glass off).
    private static GlassLook Engined(GlassLook look, GlassSettings settings) =>
        settings.Drawn != GlassEngine.Canvas && look.Canvas is { } canvas
            ? look with { Canvas = canvas with { Engine = settings.Drawn, Rizzy = settings.Rizzy, Dasher = settings.Dasher, GlassHq = settings.GlassHq } } : look;

    /// <summary>ED-NEW-glass-alive-5, -8: the engine's options every surface is drawn with now (a glide's in between; the engine and the switch are
    /// the tuning's).</summary>
    public static GlassSettings ShownSettings => blendFrom is { } from ? from.Toward(tuning, blendProgress) with { Engine = tuning.Engine, Liquid = tuning.Liquid } : tuning;

    /// <summary>ED-NEW-glass-alive-5: liquid-glass's options every surface is drawn with now (a glide's in between).</summary>
    public static RizzyGlass ShownRizzy => ShownSettings.Rizzy;

    // A glide (Blend): from which settings, and how far toward the tuning's.
    private static GlassSettings? blendFrom;
    private static double blendProgress = 1;

    /// <summary>ED-NEW-glasslook-2: a change glides: every surface's look is <paramref name="progress"/> of the way from its look under
    /// <paramref name="from"/> to its look under the tuning (each frame of the page's glide: a Look, a switch, a reset), with no pop; 1 ends the glide.
    /// Uniforms only.</summary>
    public static void Blend(GlassSettings from, double progress)
    {
        ArgumentNullException.ThrowIfNull(from);
        var t = Math.Clamp(progress, 0, 1);
        (GlassSettings?, double) next = t >= 1 ? (null, 1) : (from.Held(), t);
        if (next == (blendFrom, blendProgress)) return;
        (blendFrom, blendProgress) = next;
        Refresh();
    }

    private static GlassSettings tuning = GlassSettings.Default;
    private static IGlassSystem system = new WindowsGlassSystem();
    private static GlassTuning? shown;
    private static bool watching;

    /// <summary>
    /// ED-NEW-glasslook-2: the user's Glass settings, over every surface's look (the app mirrors AppSettings.Glass here, as it mirrors the theme into
    /// ThemeManager). A change redraws every glass piece of every window with new uniforms on the next composition update: no layout pass, no new
    /// program (queued on the UI thread when set from another).
    /// </summary>
    public static GlassSettings Tuning
    {
        get => tuning;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            Watch();
            var held = value.Held();
            if (held == tuning) return;
            tuning = held;
            Changed?.Invoke();
            Refresh();
        }
    }

    /// <summary>Windows' switches the settings follow until the user sets theirs (tests set their own).</summary>
    public static IGlassSystem System
    {
        get => system;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            system = value;
            Refresh();
        }
    }

    /// <summary>The inputs every surface is drawn with now (a glide's in between).</summary>
    public static GlassTuning Shown => shown ??= blendFrom is { } from
        ? GlassTuning.Of(from, system).Toward(GlassTuning.Of(tuning, system), blendProgress)
        : GlassTuning.Of(tuning, system);

    /// <summary>Windows' switches may have changed (a contrast theme, the colours): every glass reads them again.</summary>
    public static void SystemChanged() => Refresh();

    private static void Refresh()
    {
        shown = null;
        GlassMaterial.LookChanged();
    }

    // Windows' contrast themes and colour changes reach the app as the platform's colour values changing (Avalonia's WM_SETTINGCHANGE handling).
    private static void Watch()
    {
        if (watching || Avalonia.Application.Current?.PlatformSettings is not { } platform) return;
        watching = true;
        platform.ColorValuesChanged += (_, _) => SystemChanged();
    }

    /// <summary>Raised after <see cref="Tuning"/> changes, on the thread that changed it.</summary>
    public static event Action? Changed;

    /// <summary>The surface a glass is on: its effect's own <see cref="GlassEffect.Surface"/>, else told from its shape: a rectangle with no corner
    /// is a window's glass, the composer's measured-circular corner the composer, a capsule or circle a button when it responds to the pointer or
    /// carries a tint and a quiet capsule otherwise, any other rounded rectangle a panel.</summary>
    public static GlassSurface SurfaceOf(GlassEffect effect)
    {
        ArgumentNullException.ThrowIfNull(effect);
        if (effect.Surface is { } surface) return surface;
        var shape = effect.Shape;
        if (shape.Kind == GlassShapeKind.RoundedRectangle)
            return shape.CircularInBand ? GlassSurface.Composer : (shape.CornerRadius ?? 0) <= 0 ? GlassSurface.Window : GlassSurface.Panel;
        return effect.IsInteractive || effect.TintColor is not null ? GlassSurface.Button : GlassSurface.Capsule;
    }

    /// <summary>The look a glass is drawn with: its own, else its surface's under the user's settings.</summary>
    public static GlassLook Of(GlassEffect effect) => Of(effect, nested: false);

    /// <summary>The look a glass is drawn with, <paramref name="nested"/> inside another glass piece: its own, else the Inner surface's (a flat
    /// control: no glass on glass) when it is nested and names no surface itself, else its surface's; under the user's settings.</summary>
    public static GlassLook Of(GlassEffect effect, bool nested)
    {
        ArgumentNullException.ThrowIfNull(effect);
        if (effect.Look is { } own) return own;
        return For(nested && effect.Surface is null ? GlassSurface.Inner : SurfaceOf(effect));
    }
}

/// <summary>
/// OD-0147: the approved material (the design canvas's glassSpec and its material, matL), for one kind with one set of inputs. Every number is the
/// canvas's; the shader (GlassMaterial, the canvas branch) is its fragment shader's Liquid glass branch. ED-NEW-glasslook-4: the canvas is dark only;
/// in a light appearance the body's two greys mirror (the compression's mid 0.22 to 0.78, the neutral grey 0.17 to 0.83) and the text scrim lifts
/// what is dark instead of calming what is bright.
/// </summary>
public sealed record GlassCanvas(GlassKind Kind, GlassTuning Tuning)
{
    /// <summary>ED-NEW-glass-alive-5: the engine that draws this piece (the canvas's own material by default).</summary>
    public GlassEngine Engine { get; init; } = GlassEngine.Canvas;

    /// <summary>ED-NEW-glass-alive-5: rizzytoday/liquid-glass's options, when <see cref="Engine"/> is <see cref="GlassEngine.Rizzy"/>.</summary>
    public RizzyGlass Rizzy { get; init; } = RizzyGlass.Repo;

    /// <summary>ED-NEW-glass-alive-6: dashersw/liquid-glass-js's options, when <see cref="Engine"/> is <see cref="GlassEngine.Dasher"/>.</summary>
    public DasherGlass Dasher { get; init; } = DasherGlass.Repo;

    /// <summary>ED-NEW-glass-alive-7: Glass-HQ/liquid-glass's options, when <see cref="Engine"/> is <see cref="GlassEngine.GlassHq"/>.</summary>
    public GlassHqGlass GlassHq { get; init; } = GlassHqGlass.Repo;

    /// <summary>ED-NEW-glass-alive-5, -8: the piece is drawn by a liquid engine. ED-NEW-glass-alive-8: a piece on glass is liquid too (Maaz,
    /// 2026-10-15: a drop of liquid on the panel, double lensing): it reads the glass under it and bends it again.</summary>
    internal bool Liquid => Engine != GlassEngine.Canvas && Tuning.Reduce < 0.5;

    /// <summary>ED-NEW-glass-alive-8: a piece on glass is a second lens (the compound lens: its bend times 1.3, its centre magnified 4.5 %).</summary>
    internal bool Nested => Kind == GlassKind.Flat;

    /// <summary>ED-NEW-glass-alive-8: the nested lens's gain on the bend.</summary>
    internal double NestedGain => Nested ? 1.3 : 1;

    /// <summary>ED-NEW-glass-alive-8: the nested lens's magnification of its centre (the shift toward its centre per pt from it).</summary>
    internal double NestedMagnify => Nested ? 0.045 : 0;

    /// <summary>ED-NEW-glass-alive-8: whether the liquid engines draw the shared finish (the readability veil and the rim light) over their own
    /// model. Always on in the app; an engine's own pixel tests turn it off to measure the repository's model alone.</summary>
    internal static bool Finish { get; set; } = true;

    /// <summary>ED-NEW-glass-alive-8: the rim light every liquid engine draws (the Glass settings' Rim light; more with Increase contrast).</summary>
    internal double RimLight => Math.Clamp(Tuning.Rim + 0.2 * Tuning.Contrast, 0, 1.2);

    /// <summary>ED-NEW-glass-alive-8: the readability veil of the kind (over the bend, under the text), and what it adds where the window behind
    /// it is clear (the bare desktop: its icons must never fight the text). The sidebar is all but solid with Translucent sidebar off.</summary>
    internal (double Veil, double Clear) Veil
    {
        get
        {
            var (veil, clear) = Kind switch
            {
                GlassKind.Sidebar => (0.42, 0.5), GlassKind.Settings => (0.36, 0.5), GlassKind.Popover => (0.34, 0.5),
                GlassKind.Composer => (0.18, 0.45), GlassKind.Capsule => (0.12, 0.4), _ => (0.0, 0.25),
            };
            if (Kind == GlassKind.Sidebar) veil += (0.88 - veil) * Tuning.SidebarOff;
            return (Math.Min(0.9, veil + 0.25 * Tuning.Tint + 0.1 * Tuning.Contrast), clear);
        }
    }

    /// <summary>ED-NEW-glass-alive-8: how much of liquid-glass's centre reads the frosted backdrop (the rim stays sharp), by kind: the panels that
    /// carry text frost their body so it reads; the composer, the buttons and a drop on glass stay clear.</summary>
    internal double BodyFrost => Kind switch { GlassKind.Sidebar => 1, GlassKind.Settings => 0.85, GlassKind.Popover => 0.7, GlassKind.Composer => 0.35, _ => 0 };

    /// <summary>ED-NEW-glass-alive-8: the frost's sigma (pt) of liquid-glass's body, by kind.</summary>
    internal double BodySigma => Kind switch { GlassKind.Sidebar => 10, GlassKind.Settings => 9, GlassKind.Popover => 7, GlassKind.Composer => 4, _ => 0 };

    /// <summary>ED-NEW-glass-alive-5: the farthest a liquid engine reads past a point (pt). It is held so that it stays inside what the material
    /// reads around itself (GlassMaterial's sampled margin).</summary>
    internal const double LiquidReach = 104;

    /// <summary>ED-NEW-glass-alive-5: liquid-glass's red, green and blue scales (scale plus each aberration; a nested lens's times 1.3), held
    /// together to <see cref="LiquidReach"/>.</summary>
    internal (double R, double G, double B) LiquidScales
    {
        get
        {
            var r = Rizzy.Held();
            var g = NestedGain;
            var (sr, sg, sb) = ((r.Scale + r.AberrationR) * g, (r.Scale + r.AberrationG) * g, (r.Scale + r.AberrationB) * g);
            var most = 0.5 * Math.Max(Math.Abs(sr), Math.Max(Math.Abs(sg), Math.Abs(sb)));
            var k = most > LiquidReach ? LiquidReach / most : 1;
            return (sr * k, sg * k, sb * k);
        }
    }

    /// <summary>ED-NEW-glass-alive-8: how far the engine's bend reads (pt), for the material's reach.</summary>
    internal double LiquidBend => Engine switch
    {
        GlassEngine.GlassHq => GlassHq.Held().Refraction * 1.2 * 0.5 + 16,
        GlassEngine.Dasher => LiquidReach,
        _ => 0.5 * Math.Max(Math.Abs(LiquidScales.R), Math.Max(Math.Abs(LiquidScales.G), Math.Abs(LiquidScales.B))),
    };

    /// <summary>ED-NEW-glass-alive-7: Glass-HQ's material numbers (core/materials.ts; dark or light), as the shader takes them.</summary>
    internal static (double FillOpacity, double ToneA, double ToneB, double ToneQ, double Chroma, double ShadowLift, double HighlightRolloff, double Blur) HqMaterial(bool dark) =>
        dark ? (0.65, 0.105, 0.83, -0.2091, 1.0648, 0.15, 0.20, 7.33) : (0.34, 0.36, 0.67, -0.1113, 1.1031, 0.18, 0, 7.33);

    // The canvas's BODY: the compression target and the neutral grey (dark), the white lift, the local weight of the neutral tint, the rim blur's
    // outer band and where the full frost begins.
    internal const double Mid = 0.22, Grey = 0.17, Lift = 0.06, Local = 0.75, SharpTo = 4.5, FullAt = 10;
    // Magnification is flat by 2.2 bezels in; the light comes from the upper left (the canvas's lightT).
    internal const double MagnifyWidth = 2.2, LightX = -0.6, LightY = -0.8;
    /// <summary>The frost's Gaussian sigma per pt of its radius (the canvas's note: about 0.48), and the most the material reads (pt): three sigmas
    /// and the bend stay inside its sampled margin.</summary>
    internal const double SigmaPerRadius = 0.48, MaxSigma = 24;

    /// <summary>The frost radius (pt) of this kind (the canvas's blur; F never below 20).</summary>
    internal double BlurRadius
    {
        get
        {
            var f = Math.Max(20, Tuning.Frost);
            return Kind switch { GlassKind.Capsule => f * 0.55, GlassKind.Sidebar or GlassKind.Settings => f * 1.4, GlassKind.Popover => f * 1.2, _ => f };
        }
    }

    /// <summary>The frost's sigma (pt), held to <see cref="MaxSigma"/> (ED-NEW-glasslook-4: the canvas's Tinted sidebar and sheet ask up to 30).</summary>
    internal double Sigma => Liquid ? Engine switch
        {
            GlassEngine.Dasher => Math.Min(MaxSigma, Math.Pow(Dasher.Held().BlurRadius / 2, 2)),
            GlassEngine.GlassHq => GlassHq.Held().Material == GlassHqMaterial.Clear ? 0 : HqMaterial(true).Blur,
            _ => Math.Min(MaxSigma, Math.Max(Rizzy.Held().DisplaceBlur, Finish ? BodySigma : 0)),
        }
        : Kind == GlassKind.Flat ? 0 : Math.Min(MaxSigma, SigmaPerRadius * BlurRadius);

    /// <summary>The material's numbers for a piece whose smaller side is <paramref name="s"/> pt, in an appearance <paramref name="darkness"/> dark
    /// (0 light, 1 dark), <paramref name="hover"/> and <paramref name="press"/> of the way under the pointer; <paramref name="pixel"/> pt is one device pixel.</summary>
    internal GlassCanvasSpec Spec(double s, double darkness, double hover, double press, double pixel)
    {
        var p = Tuning;
        var ic = p.Contrast;
        var red = p.Reduce;
        var f = Math.Max(20, p.Frost);
        var bez = p.Bezel;
        double[] sh = [0, 1, 0, 0];
        var glow = 0.045;
        var band = Math.Clamp(0.065 * s, 2, 4.5);
        var tint = p.Tint;
        switch (Kind)
        {
            case GlassKind.Composer: sh = [6, 18, 0.20, 0.14]; break;
            case GlassKind.Capsule: bez = Math.Min(0.3 * s, Math.Max(4, p.Bezel * 0.57)); sh = [3, 9, 0.18, 0.12]; break;
            case GlassKind.Sidebar: bez = p.Bezel * 0.85; glow = 0; band = 3.5; break;
            case GlassKind.Popover: sh = [12, 30, 0.30, 0.14]; glow = 0; tint += 0.12; break;
            case GlassKind.Settings: bez = p.Bezel * 1.15; sh = [20, 48, 0.38, 0.16]; glow = 0; tint += 0.14; break;
        }
        // an interactive piece (only those ease a hover or a press) shows the pointer's glow whatever its kind: the canvas's sidebar, popover and
        // sheet carry none because nothing there is interactive (ED-NEW-glasslook-4); the glow is weighed by the hover, so it fades in and out
        if (hover > 0 || press > 0) glow = Math.Max(glow, 0.045);
        bez = Math.Max(1, Math.Min(bez, 0.45 * s));
        var q = 0.6 + 0.4 * Math.Min(p.Shadow, 1.6);
        var low = Math.Clamp((36 - f) / 16, 0, 1);
        var bend = Math.Min(Math.Min(p.Refraction * bez, 30), 0.5 * s) * (1 + 0.2 * press);
        var tintT = tint;
        var tintB = tint * 1.15 + 0.01;
        if (Kind == GlassKind.Sidebar) { tintT += (0.9 - tintT) * p.SidebarOff; tintB += (0.9 - tintB) * p.SidebarOff; }
        tintT += (0.93 - tintT) * red;
        tintB += (0.95 - tintB) * red;
        bend *= 1 - red;
        var mag = 1 + 0.075 * p.Refraction;
        // the shape swells a little on hover and press when motion is full (the canvas's iOS 27 interactive glass)
        var swell = p.Motion * (Kind == GlassKind.Flat ? 0.8 * press : Math.Clamp(0.06 * s, 1, 3) * press + 0.4 * hover);
        var dark = Math.Clamp(darkness, 0, 1);
        return new GlassCanvasSpec(
            Bend: bend, Bezel: bez, Fringe: p.Fringe * (1 - red), Magnify: mag + (1 - mag) * red,
            Grey: 0.83 + (Grey - 0.83) * dark, TintTop: tintT, TintBottom: tintB,
            Mid: 0.78 + (Mid - 0.78) * dark, Compress: Math.Clamp(0.06 + 1.6 * tint, 0, 0.7), Saturation: p.Saturation, Lift: Lift * (1 - red),
            Band: band, BandDark: Math.Min(0.38, 0.22 + 0.45 * tint) + 0.14 * ic, Spec: p.Rim + 0.2 * ic, Counter: 0.05,
            ShadowY: sh[0] * q, ShadowSigma: sh[1] * q, ShadowAlpha: sh[2] * p.Shadow, ContactAlpha: sh[3] * Math.Min(p.Shadow, 1.4),
            Hair: 0.07 + 0.1 * ic, Glow: glow, GlowSigma: Math.Clamp(s * 0.9, 20, 60), Dark: dark,
            Glint: 0.30, Scrim: (Math.Clamp(0.32 - 0.5 * tint, 0.12, 0.32) + 0.22 * low) * (1 - red), Line: Math.Max(0.5, 1.05 * pixel),
            TintR: p.TintR, TintG: p.TintG, TintB: p.TintB, TintAmount: p.TintAmount, Hover: hover, Press: press, Swell: swell);
    }

    /// <summary>How far past its edge the piece draws (pt): its shadow (the drop and three sigmas, as the canvas's tanh falls to under 1 %), the
    /// contact shadow, and the swell.</summary>
    internal double Reach(double s)
    {
        var spec = Spec(s, 1, 1, 1, 1);
        if (Kind == GlassKind.Flat) return spec.Swell + 1;
        if (Liquid) return 1;
        return Math.Max(spec.ShadowAlpha > 0 ? spec.ShadowY + 3 * spec.ShadowSigma : 0, spec.ContactAlpha > 0 ? 1 + 3 * 2.5 : 0) + spec.Swell + 1;
    }
}

/// <summary>OD-0147: one piece's numbers in the approved material (the canvas's u_m rows, by name).</summary>
internal readonly record struct GlassCanvasSpec(double Bend, double Bezel, double Fringe, double Magnify, double Grey, double TintTop, double TintBottom,
    double Mid, double Compress, double Saturation, double Lift, double Band, double BandDark, double Spec, double Counter, double ShadowY, double ShadowSigma,
    double ShadowAlpha, double ContactAlpha, double Hair, double Glow, double GlowSigma, double Dark, double Glint, double Scrim, double Line,
    double TintR, double TintG, double TintB, double TintAmount, double Hover, double Press, double Swell);
