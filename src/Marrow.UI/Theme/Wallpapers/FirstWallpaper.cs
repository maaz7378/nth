// PORT-ONLY: ED-NEW-glass-alive-9 (Maaz, 2026-10-15: "a good live wallpaper is ON by default on a fresh profile, so the liquid glass has something
// to bend"): once, on a profile that has never chosen a wallpaper (no library choice, no picture of its own), the library's default live wallpaper
// is chosen, as if the user had picked it. A profile that has chosen anything, None included, is never touched, and the seed is written once
// whatever happens next, so choosing None later sticks.
using Marrow.Core.Port;

namespace Marrow.UI.Theme.Wallpapers;

/// <summary>The live wallpaper a fresh profile starts with.</summary>
public static class FirstWallpaper
{
    /// <summary>The default: Borealis, the library's aurora (moving light and colour that bends visibly through the glass).</summary>
    public const string DefaultId = "borealis";

    internal const string SeededKey = "wallpaperLibrarySeeded", LibraryKey = "wallpaperLibrary", PictureKey = "wallpaperFile";

    /// <summary>Whether <paramref name="settings"/> is a profile that has never chosen a wallpaper and has not been seeded.</summary>
    public static bool Fresh(ISettingsStore settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return settings.Object(SeededKey) is null && settings.Object(LibraryKey) is null
            && settings.Object(PictureKey) is not string { Length: > 0 };
    }

    /// <summary>Chooses the default live wallpaper on a fresh profile (and marks the profile seeded either way); true when it chose it.</summary>
    public static bool Seed(ISettingsStore settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var fresh = Fresh(settings);
        if (settings.Object(SeededKey) is null) settings.Set(SeededKey, true);
        if (!fresh) return false;
        new WallpaperLibraryStore(settings).Select(WallpaperKind.Animated, DefaultId);
        return true;
    }
}
