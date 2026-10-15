// ED-NEW-jelly-pill-2 (row #87; overnight brief item 4): the Settings navigation's selected highlight is a liquid glass drop that the pointer can
// drag along the list: it follows the pointer, stretches with its speed, lands on the nearest row and opens it; clicks and arrow keys move it too.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Marrow.UI.Port;
using Marrow.UI.Port.Settings;
using Marrow.Testing;
using Marrow.UI.Tests.Fixtures;
using Xunit;
using SidebarRow = Marrow.UI.Chrome.SidebarRow;

namespace Marrow.UI.Tests.Port.Settings;

[Collection("Headless")]
public sealed class JellyNavTests(HeadlessSession headless)
{
    private sealed class Rig
    {
        public readonly List<SidebarRow> Rows = [];
        public int Selected;
        public int Picks;
        public JellyNav Nav = null!;
        public Window Window = null!;
        public readonly ManualFrameSource Frames = new();
        public readonly ManualTimeProvider Time = new();

        /// <summary>Time and the frame clock move on together, a 60 Hz frame at a time.</summary>
        public void Step(int frames = 1)
        {
            for (var i = 0; i < frames; i++) { Time.Advance(TimeSpan.FromMilliseconds(16)); Frames.Advance(TimeSpan.FromMilliseconds(16)); }
            GlassScene.Drain();
        }

        /// <summary>A second of frames: every spring has landed.</summary>
        public void Settle() => Step(90);
    }

    private static void Run(Action<Rig> body)
    {
        var rig = new Rig();
        using var frames = FrameClock.Source.Override(rig.Frames);
        using var clock = Marrow.Core.Port.SwiftClock.Provider.Override(rig.Time);
        Show(rig);
        try { body(rig); }
        finally { rig.Window.Close(); }
    }

    private static void Show(Rig rig, int count = 6)
    {
        var list = new StackPanel { Spacing = 1, Margin = new Thickness(8, 12, 8, 8) };
        for (var i = 0; i < count; i++)
        {
            var index = i;
            var row = new SidebarRow($"Page {i}", () => { rig.Selected = index; rig.Picks++; foreach (var r in rig.Rows) r.IsSelected = ReferenceEquals(r, rig.Rows[index]); rig.Nav.Sync(); },
                () => new Border(), _ => new Border(), isSelected: i == 0, rowHeight: 30) { SelectionDrawnElsewhere = true };
            rig.Rows.Add(row);
            list.Children.Add(row);
        }
        rig.Nav = new JellyNav(list, () => rig.Rows, () => rig.Rows[rig.Selected], row => ((SidebarRow)row).Invoke())
        {
            Width = 220, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
        };
        rig.Window = new Window { Content = rig.Nav, Width = 300, Height = 400 };
        rig.Window.Show();
        rig.Settle();
    }

    private static Rect RowRect(Rig rig, int i) => new(rig.Rows[i].TranslatePoint(default, rig.Nav)!.Value, rig.Rows[i].Bounds.Size);

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-2")]
    public void The_highlight_is_a_third_layer_liquid_glass_drop_over_the_selected_row() => headless.OnUiThread(() => Run(rig =>
    {
        {
            var effect = SwiftGlass.GetEffect(rig.Nav.DropControl);
            Assert.NotNull(effect);
            Assert.Equal(GlassSurface.Inner, effect!.Surface);
            Assert.Equal(3, SwiftGlass.GetLayer(rig.Nav.DropControl));
            Assert.Equal(RowRect(rig, 0).Y, rig.Nav.Drop.Y, 3);
            Assert.Equal(RowRect(rig, 0).Height, rig.Nav.Drop.Height, 3);
            Assert.True(rig.Rows[0].SelectionDrawnElsewhere);
        }
    }));

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-2")]
    public void Dragging_the_drop_follows_the_pointer_stretches_and_opens_the_row_it_lands_on() => headless.OnUiThread(() => Run(rig =>
    {
        {
            var from = RowRect(rig, 0).Center;
            var to = RowRect(rig, 3).Center;
            rig.Window.MouseDown(from, MouseButton.Left, RawInputModifiers.None);
            Assert.True(rig.Nav.Dragging);
            var mid = new Point(from.X, (from.Y + to.Y) / 2);
            rig.Time.Advance(TimeSpan.FromMilliseconds(16));
            rig.Window.MouseMove(mid, RawInputModifiers.LeftMouseButton);
            // the drop's middle is where the pointer took it, whatever row is under it
            Assert.Equal(mid.Y, rig.Nav.Drop.Center.Y, 1);
            Assert.True(rig.Nav.Jelly > 0, "a fast drag stretches the drop");
            Assert.Equal(0, rig.Selected); // nothing opens while it is held
            rig.Window.MouseMove(new Point(from.X, to.Y + 4), RawInputModifiers.LeftMouseButton);
            rig.Window.MouseUp(new Point(from.X, to.Y + 4), MouseButton.Left, RawInputModifiers.None);
            Assert.False(rig.Nav.Dragging);
            Assert.Equal(3, rig.Selected);
            Assert.Equal(1, rig.Picks);
            rig.Settle();
            Assert.Equal(RowRect(rig, 3).Y, rig.Nav.Drop.Y, 1); // landed on the row
            Assert.Equal(0, rig.Nav.Jelly, 2); // and the wobble has died
        }
    }));

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-2")]
    public void A_drag_past_the_list_lands_on_the_last_row_and_never_sticks() => headless.OnUiThread(() => Run(rig =>
    {
        {
            var from = RowRect(rig, 0).Center;
            rig.Window.MouseDown(from, MouseButton.Left, RawInputModifiers.None);
            rig.Window.MouseMove(new Point(from.X + 200, 2000), RawInputModifiers.LeftMouseButton);
            rig.Window.MouseUp(new Point(from.X + 200, 2000), MouseButton.Left, RawInputModifiers.None);
            Assert.False(rig.Nav.Dragging);
            Assert.Equal(5, rig.Selected);
        }
    }));

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-2")]
    public void A_click_and_the_arrow_keys_move_the_drop_to_their_row() => headless.OnUiThread(() => Run(rig =>
    {
        {
            var at = RowRect(rig, 2).Center;
            rig.Window.MouseDown(at, MouseButton.Left, RawInputModifiers.None);
            rig.Window.MouseUp(at, MouseButton.Left, RawInputModifiers.None);
            Assert.Equal(2, rig.Selected);
            rig.Step(3);
            var between = rig.Nav.Drop.Y;
            Assert.True(between > RowRect(rig, 0).Y && between != RowRect(rig, 2).Y, $"the drop slides there ({between:F1})");
            rig.Settle();
            Assert.Equal(RowRect(rig, 2).Y, rig.Nav.Drop.Y, 1);
            rig.Rows[2].Focus();
            rig.Window.KeyPress(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null);
            Assert.Equal(3, rig.Selected);
            rig.Window.KeyPress(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null);
            Assert.Equal(2, rig.Selected);
        }
    }));

    [Fact]
    [Trait("Decision", "ED-NEW-jelly-pill-2")]
    public void The_stretch_keeps_the_area_grows_with_speed_and_is_none_at_rest()
    {
        Assert.Equal((1.0, 1.0), JellyNav.Stretch(0));
        var (x, y) = JellyNav.Stretch(1);
        Assert.True(y > 1.2 && x < 1);
        Assert.Equal(1, x * y, 9);
        Assert.Equal(0, JellyNav.StretchFor(0));
        Assert.True(JellyNav.StretchFor(600) < JellyNav.StretchFor(1200));
        Assert.Equal(1.3, JellyNav.StretchFor(1e6));
    }
}
