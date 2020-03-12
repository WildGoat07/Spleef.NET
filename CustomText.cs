using System;
using System.Collections.Generic;
using System.Text;
using SFML.System;
using SFML.Graphics;
using System.Linq;

namespace Spleef
{
    public class CustomText : Transformable, Drawable
    {
        public static Image FontImage = new Image("assets/images/font.png");
        public static Texture FontTexture = new Texture(FontImage);

        public CustomText()
        {
            Height = 10;
            Color = Color.White;
            Text = "";
        }

        public CustomText(string text) : this()
        {
            Text = text;
        }

        public Color Color { get; set; }
        public FloatRect GlobalBounds => Transform.TransformRect(LocalBounds);
        public int Height { get; set; }

        public FloatRect LocalBounds
        {
            get
            {
                var max_x = 0f;
                var offset = new Vector2f();
                foreach (var item in Text)
                {
                    if (item == '\n')
                    {
                        offset.Y += Height;
                        offset.X = 0;
                    }
                    else if (item == ' ')
                    {
                        offset.X += Height * 4f / 10;
                    }
                    else
                    {
                        var texRect = GetCharRect(item);
                        offset.X += texRect.Width * Height / 10f - Height / 10f;
                    }
                    max_x = Math.Max(max_x, offset.X);
                }
                return new FloatRect(0, 0, max_x, offset.Y + 10);
            }
        }

        public string Text { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            var offset = new Vector2f();
            var rect = new RectangleShape() { Texture = FontTexture, FillColor = Color };
            foreach (var item in Text)
            {
                if (item == '\n')
                {
                    offset.Y += Height;
                    offset.X = 0;
                }
                else if (item == ' ')
                {
                    offset.X += Height * 4f / 10;
                }
                else
                {
                    rect.Position = offset;
                    var texRect = GetCharRect(item);
                    rect.Size = new Vector2f(texRect.Width * Height / 10f, Height);
                    rect.TextureRect = texRect;
                    target.Draw(rect, states);
                    offset.X += rect.Size.X - Height / 10f;
                }
            }
        }

        private static IntRect GetCharRect(char c)
        {
            if ("éèêë".Contains(c))
                c = 'e';
            if ("ÉÊËÈ".Contains(c))
                c = 'e';
            if (c == 'à')
                c = 'a';
            if (c == 'ç')
                c = 'c';
            c = char.ToLower(c);
            var index = "abcdefghijklmnopqrstuvwxyz!?.+-:,".IndexOf(c);
            if (index == -1)
                return default;
            int x = 0;
            while (index >= 0)
            {
                if (FontImage.GetPixel((uint)x, 10).R == 0)
                    index--;
                x++;
            }
            var result = new IntRect(x - 1, 0, 0, 10);
            index = x;
            while (index < FontImage.Size.X && FontImage.GetPixel((uint)index, 10).R == 255)
                index++;
            result.Width = index - x + 1;
            return result;
        }
    }
}