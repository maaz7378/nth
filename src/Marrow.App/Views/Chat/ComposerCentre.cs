// PORT-ONLY: ED-NEW-composer-centre-1 (row #87 phase 2; Maaz, 2026-10-15): on a new thread the composer sits in the middle of the conversation pane,
// its liquid glass over the wallpaper, and glides down to its place at the bottom as soon as work starts: the first letter typed, an attachment,
// a turn, or a run. Clearing the draft of a thread with no turns brings it back up. Under Reduce Motion it is placed at once (OD-0076).
// ED-NEW-composer-centre-2 (overnight brief, item 3): centred, it stands under the new page's heading with a gap (the heading and it are one group
// centred in the pane, so they never overlap), and it is a bigger, softly rounded rectangle (ComposerSurface.Idle 1) that morphs into the bottom
// pill (Idle 0) on the same spring as the glide: position, height and corner move together and land with a slight overshoot.
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
    private readonly AnimatedScalar idle;
    private bool centred;
    private double pane, composer, prompt;

    /// <summary>The glide down (and back up): a half-second spring with a little bounce, so it lands with a slight liquid settle.</summary>
    internal static Animation Glide => Animation.Spring(duration: 0.55, bounce: 0.14);

    /// <summary>The gap between the new page's heading and the centred composer (pt).</summary>
    internal const double HeadingGap = 28;

    /// <param name="host">What moves (the composer's row).</param>
    /// <param name="morph">Where the composer's shape takes how idle it is (1 the centred rectangle, 0 the bottom pill).</param>
    internal ComposerCentre(Control host, Action<double>? morph = null)
    {
        this.host = host;
        host.RenderTransform = shift;
        lift = new AnimatedScalar(host, 0, v => shift.Y = -v);
        idle = new AnimatedScalar(host, 0, v => morph?.Invoke(Math.Max(0, v)));
    }

    /// <summary>The distance the composer is raised now (0 at the bottom).</summary>
    internal double Lift => lift.Value;

    /// <summary>How far the composer's shape is the centred rectangle now (1) rather than the bottom pill (0).</summary>
    internal double Idle => idle.Value;

    /// <summary>Where the shape is heading (1 the centred rectangle, 0 the pill).</summary>
    internal double IdleTarget => idle.Target;

    /// <summary>Where the lift is heading.</summary>
    internal double LiftTarget => lift.Target;

    /// <summary>Whether it stands (or is heading) in the middle.</summary>
    internal bool Centred => centred;

    /// <summary>A thread is a new page while it has no turn, no run and an empty draft.</summary>
    internal static bool IsNewPage(int turns, bool running, bool draftEmpty) => turns == 0 && !running && draftEmpty;

    /// <summary>How far up the composer stands centred: its middle on the pane's middle.</summary>
    internal static double CentredLift(double paneHeight, double composerHeight) => Math.Max(0, (paneHeight - composerHeight) / 2);

    /// <summary>ED-NEW-composer-centre-2: how far up the composer stands under a heading <paramref name="promptHeight"/> tall that is centred in the
    /// pane above it: the heading, the gap and the composer centred together, so the composer's top is the gap below the heading's bottom.</summary>
    internal static double CentredLift(double paneHeight, double composerHeight, double promptHeight) =>
        promptHeight <= 0 ? CentredLift(paneHeight, composerHeight) : Math.Max(0, (paneHeight - composerHeight - promptHeight - HeadingGap) / 2);

    /// <summary>ED-NEW-composer-centre-2: how far the heading rises from its own centre in the pane above the composer, for the group to be centred.</summary>
    internal static double PromptRise => HeadingGap / 2;

    /// <summary>Takes the pane's, the composer's and the heading's heights and whether the thread is a new page; glides when the page changes,
    /// follows a resize at once (or, mid-glide, retargets the glide).</summary>
    internal void Update(double paneHeight, double composerHeight, bool newPage, double promptHeight = 0)
    {
        var resized = paneHeight != pane || composerHeight != composer || promptHeight != prompt;
        (pane, composer, prompt) = (paneHeight, composerHeight, promptHeight);
        var target = newPage ? CentredLift(paneHeight, composerHeight, promptHeight) : 0;
        if (newPage == centred && !resized && lift.Target == target) return;
        var turned = newPage != centred;
        var glide = (turned && host.IsArrangeValid) || lift.IsRunning ? Glide : null;
        centred = newPage;
        lift.Set(target, glide);
        if (turned || !idle.IsRunning) idle.Set(newPage ? 1 : 0, turned && host.IsArrangeValid ? Glide : null);
    }
}
