using System;
using System.Collections.Generic;
using System.Text;
using SFML;
using SFML.System;
using SFML.Graphics;

namespace Spleef
{
    public class BlocSelector : Transformable, Drawable
    {
        private static Texture texture = new Texture("assets/images/blocSelection.png");
        private int decal;
        private RectangleShape shape;
        private Time timer;

        public BlocSelector()
        {
            decal = 0;
            timer = Time.Zero;
            shape = new RectangleShape(new Vector2f(64, 128))
            {
                Texture = texture,
                Origin = new Vector2f(0, 64)
            };
        }

        public Color Color { get => shape.FillColor; set => shape.FillColor = value; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(shape, states);
        }

        public void Update(Time delta)
        {
            timer += delta;
            if (timer > Time.FromMilliseconds(150))
            {
                timer = Time.Zero;
                decal++;
                decal %= 8;
                shape.TextureRect = new IntRect(decal * 16, 0, 16, 32);
            }
        }
    }
}