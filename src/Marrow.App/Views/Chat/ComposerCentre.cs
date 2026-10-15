// PORT-ONLY: ED-NEW-composer-centre-1 (row #87 phase 2; Maaz, 2026-10-15): on a new thread the composer sits in the middle of the conversation pane,
// its liquid glass over the wallpaper, and glides down to its place at the bottom as soon as work starts: the first letter typed, an attachment,
// a turn, or a run. Clearing the draft of a thread with no turns brings it back up. Only a translation moves (no layout pass, no new glass
// program); under Reduce Motion it is placed at once (OD-0076).
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Marrow.UI.Port;

namespace Marrow.App.Views.Chat;

/// <summary>Where the composer stands in its pane: centred on an empty new thread, at the bottom once work starts.</summary>
internal sealed class ComposerCentre
{
    private readonly Control host;
    private readonly TranslateTransform shift = new();
    private readonly AnimatedScalar lift;
    private bool centred;
    private double pane, composer;

    /// <summary>The glide down (and back up): Apple's smooth spring, half a second.</summary>
    internal static Animation Glide => Animation.Smooth(0.5);

    internal ComposerCentre(Control host)
    {
        this.host = host;
        host.RenderTransform = shift;
        lift = new AnimatedScalar(host, 0, v => shift.Y = -v);
    }

    /// <summary>The distance the composer is raised now (0 at the bottom).</summary>
    internal double Lift => lift.Value;

    /// <summary>Whether it stands (or is heading) in the middle.</summary>
    internal bool Centred => centred;

    /// <summary>A thread is a new page while it has no turn, no run and an empty draft.</summary>
    internal static bool IsNewPage(int turns, bool running, bool draftEmpty) => turns == 0 && !running && draftEmpty;

    /// <summary>How far up the composer stands centred: its middle on the pane's middle.</summary>
    internal static double CentredLift(double paneHeight, double composerHeight) => Math.Max(0, (paneHeight - composerHeight) / 2);

    /// <summary>Takes the pane's and the composer's heights and whether the thread is a new page; glides when the page changes, follows a resize at once.</summary>
    internal void Update(double paneHeight, double composerHeight, bool newPage)
    {
        var resized = paneHeight != pane || composerHeight != composer;
        (pane, composer) = (paneHeight, composerHeight);
        var target = newPage ? CentredLift(paneHeight, composerHeight) : 0;
        if (newPage == centred && !resized && lift.Target == target) return;
        var glide = newPage != centred && host.IsArrangeValid;
        centred = newPage;
        lift.Set(target, glide ? Glide : null);
    }
}
