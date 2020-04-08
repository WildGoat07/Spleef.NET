using System;
using System.Collections.Generic;
using System.Text;
using SFML;
using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System.Linq;

namespace Spleef
{
    public static class Game
    {
        public static RenderTexture LightMap;
        public static RenderWindow App => Program.App;
        public static Random Generator => Program.Generator;

        public static void launch()
        {
            var tiles = new GameTile[40, 30];
            for (int x = 0; x < 40; x++)
                for (int y = 0; y < 30; y++)
                {
                    tiles[x, y] = new GameTile
                    {
                        type = Tile.BASE,
                        lavaVersion = Generator.Next(3),
                        breakState = 0,
                        erosionLevel = 0
                    };
                }
            //lava borders
            for (int x = 0; x < 40; x++)
                for (int y = 0; y < 2; y++)
                    tiles[x, y].type = Tile.LAVA;
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 30; y++)
                    tiles[x, y].type = Tile.LAVA;
            for (int x = 38; x < 40; x++)
                for (int y = 0; y < 30; y++)
                    tiles[x, y].type = Tile.LAVA;
            for (int x = 0; x < 40; x++)
                for (int y = 28; y < 30; y++)
                    tiles[x, y].type = Tile.LAVA;

            {
                const int TOP = 1;
                const int LEFT = 1 << 1;
                const int BOT = 1 << 2;
                const int RIGHT = 1 << 3;
                for (int x = 0; x < 40; x++)
                    for (int y = 0; y < 30; y++)
                    {
                        if (tiles[x, y].type != Tile.LAVA)
                        {
                            int borders = 0;
                            if (x > 0 && tiles[x - 1, y].type == Tile.LAVA)
                                borders |= LEFT;
                            if (x < 49 && tiles[x + 1, y].type == Tile.LAVA)
                                borders |= RIGHT;
                            if (y > 0 && tiles[x, y - 1].type == Tile.LAVA)
                                borders |= TOP;
                            if (y < 49 && tiles[x, y + 1].type == Tile.LAVA)
                                borders |= BOT;
                            if (borders == LEFT)
                                tiles[x, y].type = Tile.BORDER_LEFT;
                            if (borders == RIGHT)
                                tiles[x, y].type = Tile.BORDER_RIGHT;
                            if (borders == TOP)
                                tiles[x, y].type = Tile.BORDER_TOP;
                            if (borders == BOT)
                                tiles[x, y].type = Tile.BORDER_BOT;
                            if (borders == (RIGHT | LEFT))
                                tiles[x, y].type = Tile.STRAIGHT_VER;
                            if (borders == (TOP | BOT))
                                tiles[x, y].type = Tile.STRAIGHT_HOR;
                            if (borders == (TOP | LEFT))
                                tiles[x, y].type = Tile.CORNER_TOPLEFT;
                            if (borders == (BOT | LEFT))
                                tiles[x, y].type = Tile.CORNER_BOTLEFT;
                            if (borders == (TOP | RIGHT))
                                tiles[x, y].type = Tile.CORNER_TOPRIGHT;
                            if (borders == (BOT | RIGHT))
                                tiles[x, y].type = Tile.CORNER_BOTRIGHT;
                            if (borders == (BOT | RIGHT | TOP))
                                tiles[x, y].type = Tile.TRIBORDER_LEFT;
                            if (borders == (BOT | LEFT | TOP))
                                tiles[x, y].type = Tile.TRIBORDER_RIGHT;
                            if (borders == (RIGHT | LEFT | TOP))
                                tiles[x, y].type = Tile.TRIBORDER_BOT;
                            if (borders == (RIGHT | LEFT | BOT))
                                tiles[x, y].type = Tile.TRIBORDER_TOP;
                            if (borders == 0b1111)
                                tiles[x, y].type = Tile.ISLE;
                        }
                        tiles[x, y].version = Generator.Next(Tile.TilesTextures[tiles[x, y].type].Count);
                    }
            }
            var multStates = RenderStates.Default;
            multStates.BlendMode = BlendMode.Multiply;
            var addStates = RenderStates.Default;
            addStates.BlendMode = BlendMode.Add;
            var dummyShape = new VertexArray(PrimitiveType.TriangleFan, 10);
            var clock = new Clock();
            var lightOpacity = 1f;
            var delta = Time.Zero;
            var oldTimer = Time.Zero;
            var gameView = new View(App.GetView());
            var viewObjective = new Vector2f();
            void mouseClick(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == Mouse.Button.Right)
                {
                    App.SetMouseCursorGrabbed(true);
                    App.SetMouseCursorVisible(false);
                    Mouse.SetPosition((Vector2i)App.Size / 2, App);
                }
            }
            void mouseRelease(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == Mouse.Button.Right)
                {
                    App.SetMouseCursorGrabbed(false);
                    App.SetMouseCursorVisible(true);
                }
            }
            App.MouseButtonPressed += mouseClick;
            App.MouseButtonReleased += mouseRelease;
            while (App.IsOpen)
            {
                delta = clock.Restart();
                oldTimer += delta;
                App.DispatchEvents();
                if (oldTimer > Time.FromMilliseconds(120))
                {
                    oldTimer = Time.Zero;
                    lightOpacity = .8f + (float)Generator.NextDouble() * .2f;
                }
                if (Mouse.IsButtonPressed(Mouse.Button.Right))
                {
                    if (App.MapPixelToCoords(new Vector2i(900, 600)).X - App.MapPixelToCoords(new Vector2i()).X < 900 * 2)
                        gameView.Zoom(1 + 5 * delta.AsSeconds());
                    gameView.Move((App.MapPixelToCoords(Mouse.GetPosition(App)) - App.MapPixelToCoords((Vector2i)App.Size / 2)) * -2);
                    Mouse.SetPosition((Vector2i)App.Size / 2, App);
                }
                else
                {
                    if (App.MapPixelToCoords(new Vector2i(900, 600)).X - App.MapPixelToCoords(new Vector2i()).X > 900)
                        gameView.Zoom(1 - 2f * delta.AsSeconds());
                    var toObjective = viewObjective - gameView.Center;
                    var dist = (float)Math.Sqrt(toObjective.X * toObjective.X + toObjective.Y * toObjective.Y);
                    if (dist > 5)
                        gameView.Move(toObjective / dist * 5000 * delta.AsSeconds());
                }
                Tile.UpdateLava(delta);

                App.Clear(new Color(255, 255, 50));
                LightMap.Clear();
                var visibleBlocks = new FloatRect(App.MapPixelToCoords(new Vector2i()), App.MapPixelToCoords(new Vector2i(900, 600)));
                visibleBlocks.Left = Math.Max(0, visibleBlocks.Left / 64 - 2);
                visibleBlocks.Top = Math.Max(0, visibleBlocks.Top / 64 - 2);
                visibleBlocks.Width = Math.Min(40, visibleBlocks.Width / 64 + 3);
                visibleBlocks.Height = Math.Min(30, visibleBlocks.Height / 64 + 3);

                App.SetView(gameView);

                {
                    var topleft = App.MapPixelToCoords(new Vector2i());
                    if (topleft.X < 0)
                        gameView.Move(new Vector2f(-topleft.X, 0));
                    if (topleft.Y < 0)
                        gameView.Move(new Vector2f(0, -topleft.Y));
                }
                {
                    var botright = App.MapPixelToCoords(new Vector2i(900, 600));
                    if (botright.X > 64 * 40)
                        gameView.Move(new Vector2f(64 * 40 - botright.X, 0));
                    if (botright.Y > 64 * 30)
                        gameView.Move(new Vector2f(0, 64 * 30 - botright.Y));
                }
                LightMap.SetView(gameView);
                App.SetView(gameView);

                for (int x = (int)visibleBlocks.Left; x < (int)visibleBlocks.Width; x++)
                    for (int y = (int)visibleBlocks.Top; y < (int)visibleBlocks.Height; y++)
                    {
                        if (tiles[x, y].type == Tile.LAVA)
                        {
                            dummyShape[0] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32), new Color(255, 150, 60));
                            for (int i = 1; i <= 9; i++)
                                dummyShape[(uint)i] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32) + new Vector2f((float)Math.Cos(i * Math.PI * 2 / 8), (float)Math.Sin(i * Math.PI * 2 / 8)) * 256 * lightOpacity, Color.Black);
                            LightMap.Draw(dummyShape, addStates);
                        }
                        if (((tiles[x, y].type - 1) % 4) == 3)
                        {
                            dummyShape[0] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32), new Color(150, 80, 30));
                            for (int i = 1; i <= 9; i++)
                                dummyShape[(uint)i] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32) + new Vector2f((float)Math.Cos(i * Math.PI * 2 / 8), (float)Math.Sin(i * Math.PI * 2 / 8)) * 94 * lightOpacity, Color.Black);
                            LightMap.Draw(dummyShape, addStates);
                        }
                        Tile.Draw(App, tiles[x, y].type + tiles[x, y].breakState, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE), tiles[x, y].version, tiles[x, y].lavaVersion);
                        Tile.DrawLights(LightMap, tiles[x, y].type + tiles[x, y].breakState, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE), tiles[x, y].version);
                    }

                LightMap.Display();
                App.SetView(App.DefaultView);
                App.Draw(new Sprite(LightMap.Texture), multStates);
                App.SetView(gameView);
                App.Display();
            }
            App.MouseButtonPressed -= mouseClick;
            App.MouseButtonReleased -= mouseRelease;
        }
    }
}