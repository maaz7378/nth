using System.Runtime.CompilerServices;
using Marrow.Core.Port.Settings;
using Marrow.UI.Port;

namespace Marrow.UI.Tests.Fixtures;

// ED-NEW-glass-alive-8: a fresh profile draws with the Rizzy liquid engine now. The tests written for the approved canvas material (OD-0147) measure
// that material, so this assembly starts on it, and every test that resets the glass resets it to this; the liquid engines' own tests set their
// engine explicitly, and GlassLiquidDefaultTests checks the shipped default (GlassSettings.Default) on every surface.
internal static class ClassicGlass
{
    /// <summary>The shipped settings drawn by the canvas engine.</summary>
    public static GlassSettings Default { get; } = GlassSettings.Default with { Engine = GlassEngine.Canvas };

    [ModuleInitializer]
#pragma warning disable CA2255 // A module initializer in a test library is the point: it must run before any test.
    internal static void Install() => GlassLooks.Tuning = Default;
#pragma warning restore CA2255
}
