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
        public const int TRIBORDER_BOT = 1 + 15 * 4;
        public const int TRIBORDER_LEFT = 1 + 12 * 4;
        public const int TRIBORDER_RIGHT = 1 + 13 * 4;
        public const int TRIBORDER_TOP = 1 + 14 * 4;
        public static IList<IList<Texture>> TilesTextures;
        private static RectangleShape shape;

        public static void Draw(RenderTarget target, int tile, Vector2f position, int version)
        {
            shape.Texture = TilesTextures[tile][version];
            shape.Position = position;
            target.Draw(shape, RenderStates.Default);
        }

        public static void Draw(RenderTarget target, int tile, Vector2f position, int version, RenderStates states)
        {
            shape.Texture = TilesTextures[tile][version];
            shape.Position = position;
            target.Draw(shape, states);
        }

        public static void InitTiles()
        {
            shape = new RectangleShape(new Vector2f(48, 48));
            TilesTextures = new IList<Texture>[TRIBORDER_BOT + 4];
            var imgSrc = new Image("assets/images/tiles.png");
            //lava
            TilesTextures[0] = new Texture[] { new Texture(imgSrc, new IntRect(0, 0, 16, 16)), new Texture(imgSrc, new IntRect(16, 0, 16, 16)), new Texture(imgSrc, new IntRect(32, 0, 16, 16)) };

            List<Texture> getBreakVariants(int pos, int breakState, int rotation)
            {
                var res = new List<Texture>();
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        res.AddRange(new Texture[]
                        {
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j,  new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                            ))),
                        new Texture(Utilities.Multiply(
                            imgSrc.Rotate(j, new IntRect(16 * breakState, 128, 16, 16)),
                            imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                            imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                            )))
                        });
                return res;
            }
            List<Texture> getVariants(int pos, int rotation)
            {
                var res = new List<Texture>();
                for (int i = 0; i < 4; i++)
                    res.AddRange(new Texture[]
                    {
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(0, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(16, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(0, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(16, 16, 16, 16)
                        ))),
                    new Texture(Utilities.Multiply(
                        imgSrc.Rotate(rotation, new IntRect(32, pos, 16, 16)),
                        imgSrc.Rotate(i, new IntRect(32, 16, 16, 16)
                        )))
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
    }
}