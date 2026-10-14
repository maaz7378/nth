// ED-NEW-glass-alive-3 (cousin/glass-alive, batch 3: round glass buttons drew square): a Border with a fill of its own (a tint, a press colour) under
// a Capsule or Circle glass drew that fill to its own corners, square, because the glass shape rounds only the material: the corner past the
// shape showed the fill where the material showed none. These tests render the piece and read its corner, its centre and the backdrop.
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Marrow.UI.Port;
using Marrow.UI.Tests.Fixtures;
using Xunit;

namespace Marrow.UI.Tests.Port;

[Collection("Headless")]
public sealed class GlassRoundFillTests(HeadlessSession headless)
{
    private const int Width = 300, Height = 200, Left = 40, Top = 40;
    private static readonly Color Grey = Color.FromRgb(128, 128, 128);
    private static readonly Color Fill = Color.FromRgb(40, 120, 255);

    private static Border Piece(GlassShape shape, double width, double height) => new()
    {
        Background = new SolidColorBrush(Fill), Width = width, Height = height, HorizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(Left, Top, 0, 0),
    };

    private static Func<int, int, Color> Render(Control piece, Action<Window>? after = null)
    {
        var window = new Window
        {
            Width = Width, Height = Height, Background = Brushes.Black,
            Content = new Panel { Children = { new Panel { Background = new SolidColorBrush(Grey) }, piece } },
        };
        window.Show();
        try
        {
            for (var i = 0; i < 4; i++) GlassScene.Drain();
            after?.Invoke(window);
            for (var i = 0; i < 4; i++) GlassScene.Drain();
            using var bitmap = window.CaptureRenderedFrame()!;
            var size = bitmap.PixelSize;
            var bytes = new byte[size.Width * size.Height * 4];
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { bitmap.CopyPixels(new PixelRect(size), handle.AddrOfPinnedObject(), bytes.Length, size.Width * 4); }
            finally { handle.Free(); }
            var rgba = bitmap.Format == Avalonia.Platform.PixelFormat.Rgba8888;
            return (x, y) =>
            {
                var p = (y * size.Width + x) * 4;
                return Color.FromArgb(bytes[p + 3], bytes[p + (rgba ? 0 : 2)], bytes[p + 1], bytes[p + (rgba ? 2 : 0)]);
            };
        }
        finally { window.Close(); }
    }

    private static double Distance(Color a, Color b) => Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);

    public static TheoryData<string, bool> Shapes() => new() { { "circle", false }, { "circle", true }, { "capsule", false }, { "capsule", true } };

    private static (GlassShape Shape, double Width, double Height) ShapeOf(string name) => name == "circle"
        ? (GlassShape.Circle, 80, 80)
        : (GlassShape.Capsule(RoundedCornerStyle.Continuous), 160, 40);

    [Theory]
    [Trait("Decision", "ED-NEW-glass-alive-3")] // a round glass piece's own fill is round: its corner shows the backdrop, its centre the fill
    [MemberData(nameof(Shapes))]
    public void A_round_glass_pieces_own_fill_stays_inside_its_shape(string name, bool interactive) => headless.OnUiThread(() =>
    {
        var (shape, width, height) = ShapeOf(name);
        var piece = Piece(shape, width, height);
        var glass = GlassEffect.Regular(shape);
        SwiftGlass.SetEffect(piece, interactive ? glass.Interactive() : glass);
        var at = Render(piece);
        var corner = at(Left + 2, Top + 2);
        Assert.True(Distance(corner, Fill) > 120, $"the {name}'s corner, outside its shape, shows {corner}: the piece's fill drawn square");
        Assert.True(Distance(corner, Grey) <= 30, $"the {name}'s corner shows {corner}, not the backdrop");
        Assert.True(Distance(at(Left + (int)width / 2, Top + (int)height / 2), Fill) <= 30, $"the {name}'s centre still shows its fill");
    });

    [Fact]
    [Trait("Decision", "ED-NEW-glass-alive-3")] // the fill follows a resize, and a piece that leaves the round shape keeps the corners it was given
    public void The_round_fill_follows_a_resize_and_lets_go_of_a_rectangle() => headless.OnUiThread(() =>
    {
        var piece = Piece(GlassShape.Capsule(RoundedCornerStyle.Continuous), 160, 40);
        SwiftGlass.SetEffect(piece, GlassEffect.Regular(GlassShape.Capsule(RoundedCornerStyle.Continuous)));
        var at = Render(piece, _ => piece.Height = 80);
        Assert.Equal(new CornerRadius(40), piece.CornerRadius);
        Assert.True(Distance(at(Left + 2, Top + 2), Grey) <= 30, "after the resize the capsule's corner still shows the backdrop");

        var rectangle = Piece(GlassShape.Capsule(RoundedCornerStyle.Continuous), 160, 40);
        SwiftGlass.SetEffect(rectangle, GlassEffect.Regular(GlassShape.Capsule(RoundedCornerStyle.Continuous)));
        Render(rectangle, _ =>
        {
            SwiftGlass.SetEffect(rectangle, GlassEffect.Regular(GlassShape.RoundedRectangle(6, RoundedCornerStyle.Circular)));
            rectangle.CornerRadius = new CornerRadius(6);
            rectangle.Height = 60;
        });
        Assert.Equal(new CornerRadius(6), rectangle.CornerRadius);
    });
}
