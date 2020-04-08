using System;
using System.Collections.Generic;
using System.Text;
using SFML;
using SFML.System;
using SFML.Graphics;

namespace Spleef
{
    public class Miner : Transformable, Drawable
    {
        private RectangleShape body;
        private Time elapsed;
        private int frame;

        public Miner(bool ennemy)
        {
            frame = 0;
            elapsed = Time.Zero;
            body = new RectangleShape(new Vector2f(64, 128));
            body.Origin = new Vector2f(0, 64);
            if (ennemy)
                body.Texture = Game.ennemyTexture;
            else
                body.Texture = Game.playerTexture;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(body, states);
        }

        public void Update(Time delta)
        {
            elapsed += delta;
            if (elapsed > Time.FromMilliseconds(150))
            {
                frame++;
                frame %= 4;
                elapsed = Time.Zero;
            }
            body.TextureRect = frame switch
            {
                0 => new IntRect(0, 0, 32, 64),
                1 => new IntRect(32, 0, 32, 64),
                2 => new IntRect(64, 0, 32, 64),
                3 => new IntRect(96, 0, 32, 64)
            };
        }
    }
}