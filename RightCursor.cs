using SFML.Graphics;
using SFML.System;
using System;

namespace Spleef
{
    internal class RightCursor : Transformable, Drawable
    {
        private static Texture ColorCursorTexture = new Texture("assets/images/colorCursor.png");
        private static Texture CursorTexture = new Texture("assets/images/cursor.png");

        public RightCursor()
        {
            ColorCursorRect = new RectangleShape(new Vector2f(32, 32)) { Texture = ColorCursorTexture };
            CursorRect = new RectangleShape(new Vector2f(32, 32)) { Texture = CursorTexture };
        }

        public RightCursor(float size) : this()
        {
            Size = size;
        }

        public Color Color { get => ColorCursorRect.FillColor; set => ColorCursorRect.FillColor = value; }

        public float Size
        {
            get => CursorRect.Size.X;
            set
            {
                ColorCursorRect.Size = new Vector2f(value, value);
                CursorRect.Size = ColorCursorRect.Size;
            }
        }

        private RectangleShape ColorCursorRect { get; set; }
        private RectangleShape CursorRect { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            ColorCursorRect.Position = new Vector2f(-ColorCursorRect.Size.X, -ColorCursorRect.Size.Y / 2);
            CursorRect.Position = new Vector2f(-CursorRect.Size.X, -CursorRect.Size.Y / 2);
            target.Draw(ColorCursorRect, states);
            target.Draw(CursorRect, states);
        }
    }
}