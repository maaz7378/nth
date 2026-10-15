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
        var window = new Avalonia.Controls.Window { Content = composer, Width = 600, Height = 700 };
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

    [Theory]
    [Trait("Decision", "ED-NEW-composer-centre-2")]
    [InlineData(800, 180, 92)]
    [InlineData(1400, 210, 92)]
    [InlineData(520, 180, 92)]
    [InlineData(330, 180, 92)]
    public void Centred_it_stands_under_the_heading_and_never_overlaps_it(double pane, double composer, double prompt)
    {
        var lift = ComposerCentre.CentredLift(pane, composer, prompt);
        // the composer's row is the pane's foot; the heading is centred in the room above it, then rises half the gap
        var composerTop = pane - composer - lift;
        var promptBottom = (pane - composer) / 2 + prompt / 2 - ComposerCentre.PromptRise;
        Assert.True(lift >= 0);
        if (lift > 0)
        {
            Assert.Equal(promptBottom + ComposerCentre.HeadingGap, composerTop, 6);
            // the heading, the gap and the composer are centred together in the pane
            var groupTop = promptBottom - prompt;
            Assert.Equal(pane - (composerTop + composer), groupTop, 6);
        }
        else Assert.True(composerTop >= promptBottom, "too short a pane leaves the composer at the bottom, below the heading");
    }

    [Fact]
    [Trait("Decision", "ED-NEW-composer-centre-2")]
    public void The_centred_rectangle_morphs_into_the_bottom_pill_on_the_glide() => headless.OnUiThread(() =>
    {
        var composer = new Border { Height = 120 };
        var window = new Avalonia.Controls.Window { Content = composer, Width = 600, Height = 700 };
        window.Show();
        try
        {
            var shown = new List<double>();
            var centre = new ComposerCentre(composer, shown.Add);
            centre.Update(700, 120, newPage: true, promptHeight: 92);
            Assert.Equal(1, centre.IdleTarget, 6); // heading for the centred rectangle (on the glide, or at once before the first layout)
            Assert.Equal(ComposerCentre.CentredLift(700, 120, 92), centre.LiftTarget, 6);
            centre.Update(700, 120, newPage: false, promptHeight: 92);
            Assert.False(centre.Centred);
            Assert.Equal(0, centre.IdleTarget, 6); // the shape morphs back into the pill on the same spring as the glide down
            Assert.Equal(0, centre.LiftTarget, 6);
            Assert.Equal(26, Marrow.App.Views.Composer.ComposerSurface.CornerAt(1), 6);
            Assert.Equal(22, Marrow.App.Views.Composer.ComposerSurface.CornerAt(0), 6);
            Assert.Equal(24, Marrow.App.Views.Composer.ComposerSurface.CornerAt(0.5), 6);
            Assert.True(Marrow.App.Views.Composer.ComposerSurface.IdleHeight > 46 * 2, "the start box is clearly bigger than the pill");
        }
        finally { window.Close(); }
    });
}

