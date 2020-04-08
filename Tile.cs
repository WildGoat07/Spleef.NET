using System;
using System.Collections.Generic;
using System.Text;
using SFML;
using SFML.System;
using SFML.Graphics;

namespace Spleef
{
    public class Tile
    {
        public const int BASE = 1;

        public const int BORDER_BOT = 1 + 4;

        public const int BORDER_LEFT = 1 + 2 * 4;

        public const int BORDER_RIGHT = 1 + 4 * 4;

        public const int BORDER_TOP = 1 + 3 * 4;

        public const int CORNER_BOTLEFT = 1 + 9 * 4;

        public const int CORNER_BOTRIGHT = 1 + 10 * 4;

        public const int CORNER_TOPLEFT = 1 + 7 * 4;

        public const int CORNER_TOPRIGHT = 1 + 8 * 4;

        public const int ISLE = 1 + 11 * 4;

        public const int LAVA = 0;

        public const int STRAIGHT_HOR = 1 + 6 * 4;

        public const int STRAIGHT_VER = 1 + 5 * 4;

        public const int TILE_SIZE = 64;

        public const int TRIBORDER_BOT = 1 + 15 * 4;

        public const int TRIBORDER_LEFT = 1 + 12 * 4;

        public const int TRIBORDER_RIGHT = 1 + 13 * 4;

        public const int TRIBORDER_TOP = 1 + 14 * 4;

        public static Vector2i lavaOffset;
        public static IList<IList<Variant>> TilesTextures;
        private static float offsetRotation;
        private static RectangleShape shape;

        public static void Draw(RenderTarget target, int tile, Vector2f position, int version, int lavaVersion) => Draw(target, tile, position, version, lavaVersion, RenderStates.Default);

        public static void Draw(RenderTarget target, int tile, Vector2f position, int version, int lavaVersion, RenderStates states)
        {
            shape.TextureRect = new IntRect(lavaOffset.X, lavaOffset.Y, 16, 16);
            shape.Texture = TilesTextures[LAVA][lavaVersion].Texture;
            shape.Position = position;
            shape.FillColor = new Color(255, 255, 255, (byte)(Math.Abs(180 - offsetRotation) / 180 * 100 + 155));
            target.Draw(shape, states);
            if (tile != LAVA)
            {
                shape.FillColor = Color.White;
                shape.TextureRect = new IntRect(0, 0, 16, 16);
                shape.Texture = TilesTextures[tile][version].Texture;
                target.Draw(shape, states);
            }
        }

        public static void DrawLights(RenderTarget target, int tile, Vector2f position, int version) => DrawLights(target, tile, position, version, RenderStates.Default);

        public static void DrawLights(RenderTarget target, int tile, Vector2f position, int version, RenderStates states)
        {
            states.BlendMode = BlendMode.Add;
            shape.FillColor = Color.White;
            shape.TextureRect = new IntRect(0, 0, 16, 16);
            shape.Texture = TilesTextures[tile][version].LightMask;
            shape.Position = position;
            target.Draw(shape, states);
        }

        public static void InitTiles()
        {
            shape = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            TilesTextures = new IList<Variant>[TRIBORDER_BOT + 4];
            var imgSrc = new Image("assets/images/tiles.png");
            //lava
            TilesTextures[LAVA] = new Variant[]
            {
                new Variant { Texture = new Texture(imgSrc, new IntRect(0, 0, 16, 16)){ Repeated = true} },
                new Variant { Texture = new Texture(imgSrc, new IntRect(16, 0, 16, 16)){ Repeated = true} },
                new Variant { Texture = new Texture(imgSrc, new IntRect(32, 0, 16, 16)){ Repeated = true} }
            };

            List<Variant> getBreakVariants(int pos, int breakState, int rotation)
            {
                var res = new List<Variant>();
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        res.AddRange(new Variant[]
                        {
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j,  new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                            ))) },
                        new Variant{Texture = new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                            ))) }
                        });
                return res;
            }
            List<Variant> getVariants(int pos, int rotation)
            {
                var res = new List<Variant>();
                for (int i = 0; i < 4; i++)
                    res.AddRange(new Variant[]
                    {
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                        ))) },
                    new Variant{Texture = new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                        ))) }
                    });
                return res;
            }
            void addBreakVariant(int pos, int index, int rotation)
            {
                TilesTextures[index] = getVariants(pos, rotation);
                TilesTextures[index + 1] = getBreakVariants(pos, 0, rotation);
                TilesTextures[index + 2] = getBreakVariants(pos, 1, rotation);
                TilesTextures[index + 3] = getBreakVariants(pos, 2, rotation);
            }
            addBreakVariant(16 * 2, BASE, 0);
            addBreakVariant(16 * 3, BORDER_BOT, 0);
            addBreakVariant(16 * 3, BORDER_RIGHT, 1);
            addBreakVariant(16 * 3, BORDER_TOP, 2);
            addBreakVariant(16 * 3, BORDER_LEFT, 3);
            addBreakVariant(16 * 4, CORNER_BOTRIGHT, 0);
            addBreakVariant(16 * 4, CORNER_TOPRIGHT, 1);
            addBreakVariant(16 * 4, CORNER_TOPLEFT, 2);
            addBreakVariant(16 * 4, CORNER_BOTLEFT, 3);
            addBreakVariant(16 * 5, STRAIGHT_HOR, 0);
            addBreakVariant(16 * 5, STRAIGHT_VER, 1);
            addBreakVariant(16 * 6, TRIBORDER_RIGHT, 0);
            addBreakVariant(16 * 6, TRIBORDER_TOP, 1);
            addBreakVariant(16 * 6, TRIBORDER_LEFT, 2);
            addBreakVariant(16 * 6, TRIBORDER_BOT, 3);
            addBreakVariant(16 * 7, ISLE, 0);
            foreach (var variant in TilesTextures[LAVA])
                variant.LightMask = new Texture(new Image(16, 16, Color.White));
            for (int i = LAVA + 1; i < TilesTextures.Count; i++)
                foreach (var variant in TilesTextures[i])
                    variant.LightMask = new Texture(variant.Texture.CopyToImage().Modify((x, y, c) => new Color(
                        (byte)(255 - c.A * 100f / 255),
                        (byte)(255 - c.A * 100f / 255),
                        (byte)(255 - c.A * 100f / 255))));
            /*
            foreach (var item in TilesTextures)
            {
                int n = 0;
                foreach (var item2 in item.Value)
                {
                    item2.CopyToImage().SaveToFile("out/" + item.Key + "_" + n + ".png");
                    n++;
                }
            }
            */
        }

        public static void UpdateLava(Time delta)
        {
            offsetRotation += delta.AsSeconds() * 100;
            offsetRotation %= 360;
            lavaOffset.X = (int)(Math.Cos(offsetRotation * Math.PI / 180f) * 4);
            lavaOffset.Y = (int)(Math.Sin(offsetRotation * Math.PI / 180f) * 4);
        }

        public class Variant
        {
            public Texture LightMask;
            public Texture Texture;
        }
    }
}