// ED-NEW-composer-centre-1 (row #87 phase 2): the composer stands centred on a new page and glides to the bottom when work starts.
using Avalonia.Controls;
using Marrow.App.Tests.Fixtures;
using Marrow.App.Views.Chat;

namespace Marrow.App.Tests.Views.Chat;

[Collection("Headless")]
public sealed class ComposerCentreTests(HeadlessSession headless)
{
    [Fact]
    [Trait("Decision", "ED-NEW-composer-centre-1")]
    public void A_new_page_is_a_thread_with_no_turn_no_run_and_an_empty_draft()
    {
        Assert.True(ComposerCentre.IsNewPage(0, false, true));
        Assert.False(ComposerCentre.IsNewPage(0, false, false)); // the first letter typed
        Assert.False(ComposerCentre.IsNewPage(0, true, true));
        Assert.False(ComposerCentre.IsNewPage(3, false, true));
        Assert.Equal(300, ComposerCentre.CentredLift(800, 200));
        Assert.Equal(0, ComposerCentre.CentredLift(100, 200));
    }

    [Fact]
    [Trait("Decision", "ED-NEW-composer-centre-1")]
    public void It_rises_to_the_middle_and_comes_down_to_the_bottom_when_work_starts() => headless.OnUiThread(() =>
    {
        var composer = new Border { Height = 120 };
        var window = new Window { Content = composer, Width = 600, Height = 700 };
        window.Show();
        try
        {
            var centre = new ComposerCentre(composer);
            centre.Update(700, 120, newPage: true);
            Assert.True(centre.Centred);
            centre.Update(700, 120, newPage: false);
            Assert.False(centre.Centred);
        }
        finally { window.Close(); }
    });
}
