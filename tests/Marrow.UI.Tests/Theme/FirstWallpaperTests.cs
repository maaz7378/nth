// ED-NEW-glass-alive-9: a fresh profile starts with a live wallpaper behind the glass; a profile that has chosen anything is never touched.
using Marrow.Testing;
using Marrow.UI.Theme.Wallpapers;
using Xunit;

namespace Marrow.UI.Tests.Theme;

public sealed class FirstWallpaperTests
{
    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-9")]
    public void A_fresh_profile_starts_on_the_default_live_wallpaper_once()
    {
        var settings = new InMemorySettingsStore();
        Assert.True(FirstWallpaper.Seed(settings));
        var chosen = new WallpaperLibraryStore(settings).Selection;
        Assert.Equal((WallpaperKind.Animated, FirstWallpaper.DefaultId), (chosen.Kind, chosen.Id));
        // the user picks None: it sticks, the seed never comes back
        new WallpaperLibraryStore(settings).Selection = new WallpaperSelection();
        Assert.False(FirstWallpaper.Seed(settings));
        Assert.Equal(WallpaperKind.None, new WallpaperLibraryStore(settings).Selection.Kind);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-9")]
    public void A_profile_with_its_own_picture_or_choice_is_left_alone()
    {
        var picture = new InMemorySettingsStore();
        picture.Set("wallpaperFile", "mine.png");
        Assert.False(FirstWallpaper.Seed(picture));
        Assert.Equal(WallpaperKind.None, new WallpaperLibraryStore(picture).Selection.Kind);
        var chosen = new InMemorySettingsStore();
        new WallpaperLibraryStore(chosen).Select(WallpaperKind.Animated, "lagoon");
        Assert.False(FirstWallpaper.Seed(chosen));
        Assert.Equal("lagoon", new WallpaperLibraryStore(chosen).Selection.Id);
    }

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-9")]
    public void The_default_is_a_live_wallpaper_of_the_library() =>
        Assert.Contains(WallpaperCatalogue.All, w => w.Kind == WallpaperKind.Animated && w.Id == FirstWallpaper.DefaultId);
}
