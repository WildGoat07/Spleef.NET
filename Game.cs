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
        public static Texture ennemyTexture = new Texture("assets/images/ennemy.png");
        public static RenderTexture LightMap;
        public static Miner player;
        public static Texture playerTexture = new Texture("assets/images/miner.png");
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
                int nbHoles = Generator.Next(20);
                for (int i = 0; i < nbHoles; i++)
                    tiles[Generator.Next(40), Generator.Next(30)].type = Tile.LAVA;
            }
            void updateSingleTile(int x, int y, bool randomize)
            {
                const int TOP = 1;
                const int LEFT = 1 << 1;
                const int BOT = 1 << 2;
                const int RIGHT = 1 << 3;
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
                        tiles[x, y].type = Tile.TRIBORDER_RIGHT;
                    if (borders == (BOT | LEFT | TOP))
                        tiles[x, y].type = Tile.TRIBORDER_LEFT;
                    if (borders == (RIGHT | LEFT | TOP))
                        tiles[x, y].type = Tile.TRIBORDER_TOP;
                    if (borders == (RIGHT | LEFT | BOT))
                        tiles[x, y].type = Tile.TRIBORDER_BOT;
                    if (borders == 0b1111)
                        tiles[x, y].type = Tile.ISLE;
                }
                if (randomize)
                    tiles[x, y].version = Generator.Next(Tile.TilesTextures[tiles[x, y].type].Count);
            }
            void updateTiles(bool randomize)
            {
                for (int x = 0; x < 40; x++)
                    for (int y = 0; y < 30; y++)
                        updateSingleTile(x, y, randomize);
            }
            updateTiles(true);
            var multStates = RenderStates.Default;
            multStates.BlendMode = BlendMode.Multiply;
            var addStates = RenderStates.Default;
            addStates.BlendMode = BlendMode.Add;
            var dummyShape = new VertexArray(PrimitiveType.TriangleFan, 6);
            var lightOpacity = 1f;
            var delta = Time.Zero;
            var lightTimer = Time.Zero;
            var gameView = new View(App.GetView());
            var viewObjective = new Vector2f();
            player = new Miner(false);
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
            var stage = 0;
            player.Position = new Vector2f(8 * 64, 8 * 64);
            var selectedBloc = player.Position;
            var movementAvailable = 0;
            var stage2Clock = new Clock();
            Vector2f initialStage2PlayerPos = default;
            void keyDown(object sender, KeyEventArgs e)
            {
                if (stage == 0)
                {
                    if (Utilities.IsUpKey(e.Code))
                        selectedBloc -= new Vector2f(0, 64);
                    if (Utilities.IsDownKey(e.Code))
                        selectedBloc += new Vector2f(0, 64);
                    if (Utilities.IsLeftKey(e.Code))
                        selectedBloc -= new Vector2f(64, 0);
                    if (Utilities.IsRightKey(e.Code))
                        selectedBloc += new Vector2f(64, 0);
                    selectedBloc.X = Math.Min(39 * 64, selectedBloc.X);
                    selectedBloc.X = Math.Max(0, selectedBloc.X);
                    selectedBloc.Y = Math.Min(29 * 64, selectedBloc.Y);
                    selectedBloc.Y = Math.Max(0, selectedBloc.Y);
                    if (Utilities.IsConfirmKey(e.Code) && movementAvailable < 2)
                    {
                        stage2Clock.Restart();
                        stage = 1;
                        initialStage2PlayerPos = player.Position;
                        updateBreakState();
                        var currTile = tiles[(int)initialStage2PlayerPos.X >> 6, (int)initialStage2PlayerPos.Y >> 6];
                        if (currTile.breakState == 0 && currTile.type != Tile.LAVA)
                            currTile.breakState++;
                    }
                }
            }
            App.MouseButtonPressed += mouseClick;
            App.MouseButtonReleased += mouseRelease;
            App.KeyPressed += keyDown;
            gameView.Center = player.Position;
            void updateBlocGraphics(int x, int y)
            {
                var current = tiles[x, y];
                if (current.breakState >= 4)
                {
                    current.type = Tile.LAVA;
                    current.version = 0;
                    current.breakState = 0;
                }
            }
            void updateSingleBreakState(int x, int y)
            {
                var current = tiles[x, y];
                if (current.type != Tile.LAVA)
                {
                    if (current.breakState > 0)
                        current.breakState++;
                    updateBlocGraphics(x, y);
                    if (current.breakState == 0 && current.type != Tile.LAVA)
                    {
                        if (tiles[x - 1, y].type == Tile.LAVA)
                            current.erosionLevel++;
                        if (tiles[x + 1, y].type == Tile.LAVA)
                            current.erosionLevel++;
                        if (tiles[x, y - 1].type == Tile.LAVA)
                            current.erosionLevel++;
                        if (tiles[x, y + 1].type == Tile.LAVA)
                            current.erosionLevel++;
                        if (current.erosionLevel >= 4)
                        {
                            current.breakState = 1;
                            current.version = Generator.Next(Tile.TilesTextures[current.type].Count);
                        }
                    }
                }
            }
            void updateBreakState()
            {
                for (int x = 2; x < 38; x++)
                    for (int y = 2; y < 28; y++)
                        updateSingleBreakState(x, y);
                updateTiles(false);
            }
            var selector = new BlocSelector();
            var clock = new Clock();
            while (App.IsOpen)
            {
                delta = clock.Restart();
                lightTimer += delta;
                App.DispatchEvents();
                player.Update(delta);
                selector.Update(delta);
                selector.Position = selectedBloc;
                if (stage == 0)
                {
                    viewObjective = selectedBloc + new Vector2f(32, 32);
                    if (selector.Position == player.Position + new Vector2f(64, 0)
                        || selector.Position == player.Position + new Vector2f(-64, 0)
                        || selector.Position == player.Position + new Vector2f(0, 64)
                        || selector.Position == player.Position + new Vector2f(0, -64)
                        || selector.Position == player.Position)
                        movementAvailable = 0;
                    else if (selector.Position == player.Position + new Vector2f(128, 0)
                        || selector.Position == player.Position + new Vector2f(-128, 0)
                        || selector.Position == player.Position + new Vector2f(0, 128)
                        || selector.Position == player.Position + new Vector2f(0, -128))
                        movementAvailable = 1;
                    else
                        movementAvailable = 2;
                    switch (movementAvailable)
                    {
                        case 0:
                            selector.Color = new Color(50, 250, 150);
                            break;

                        case 1:
                            selector.Color = new Color(250, 150, 50);
                            break;

                        case 2:
                            selector.Color = new Color(250, 50, 150);
                            break;
                    }
                }
                else if (stage == 1)
                {
                    if (movementAvailable == 0)
                    {
                        if (stage2Clock.ElapsedTime < Time.FromMilliseconds(250))
                        {
                            var perc = stage2Clock.ElapsedTime.AsSeconds() / Time.FromMilliseconds(250).AsSeconds();
                            var height = (float)Math.Sqrt(1 - 2 * Math.Abs(.5f - perc));
                            player.Position = initialStage2PlayerPos + perc * (selectedBloc - initialStage2PlayerPos) + new Vector2f(0, -height * 16);
                        }
                        else
                        {
                            player.Position = selectedBloc;
                            stage = 0;
                        }
                    }
                    else
                    {
                        if (stage2Clock.ElapsedTime < Time.FromMilliseconds(250))
                        {
                            var perc = stage2Clock.ElapsedTime.AsSeconds() / Time.FromMilliseconds(250).AsSeconds();
                            var height = (float)Math.Sqrt(1 - 2 * Math.Abs(.5f - perc));
                            player.Position = initialStage2PlayerPos + perc * (selectedBloc - initialStage2PlayerPos) + new Vector2f(0, -height * 48);
                        }
                        else
                        {
                            player.Position = selectedBloc;
                            stage = 0;
                            updateSingleBreakState((int)selectedBloc.X >> 6, (int)selectedBloc.Y >> 6);
                            if (tiles[(int)selectedBloc.X >> 6, (int)selectedBloc.Y >> 6].type == Tile.LAVA)
                            {
                                updateSingleTile(((int)selectedBloc.X >> 6) - 1, (int)selectedBloc.Y >> 6, false);
                                updateSingleTile(((int)selectedBloc.X >> 6) + 1, (int)selectedBloc.Y >> 6, false);
                                updateSingleTile((int)selectedBloc.X >> 6, ((int)selectedBloc.Y >> 6) + 1, false);
                                updateSingleTile((int)selectedBloc.X >> 6, ((int)selectedBloc.Y >> 6) - 1, false);
                            }
                        }
                    }
                }
                if (lightTimer > Time.FromMilliseconds(120))
                {
                    lightTimer = Time.Zero;
                    lightOpacity = .8f + (float)Generator.NextDouble() * .2f;
                }
                if (Mouse.IsButtonPressed(Mouse.Button.Right))
                {
                    if (App.MapPixelToCoords(new Vector2i(900, 600)).X - App.MapPixelToCoords(new Vector2i()).X < 900 * 1.5f)
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
                    if (dist > 10)
                        gameView.Move(toObjective / dist * Math.Max(5, dist * 10 * delta.AsSeconds()));
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
                        if (tiles[x, y].type == Tile.LAVA && x > 0 && y > 0 && x < 39 && y < 29)
                        {
                            if (tiles[x + 1, y].type != Tile.LAVA
                                || tiles[x - 1, y].type != Tile.LAVA
                                || tiles[x, y + 1].type != Tile.LAVA
                                || tiles[x, y - 1].type != Tile.LAVA)
                            {
                                dummyShape[0] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32), new Color(255, 150, 60));
                                for (int i = 1; i <= 5; i++)
                                    dummyShape[(uint)i] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32) + new Vector2f((float)Math.Cos(i * Math.PI * 2 / 4), (float)Math.Sin(i * Math.PI * 2 / 4)) * 256 * lightOpacity, Color.Black);
                                LightMap.Draw(dummyShape, addStates);
                            }
                        }
                        if (((tiles[x, y].type - 1) % 4) == 3)
                        {
                            dummyShape[0] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32), new Color(150, 80, 30));
                            for (int i = 1; i <= 5; i++)
                                dummyShape[(uint)i] = new Vertex(new Vector2f(x * 64 + 32, y * 64 + 32) + new Vector2f((float)Math.Cos(i * Math.PI * 2 / 4), (float)Math.Sin(i * Math.PI * 2 / 4)) * 94 * lightOpacity, Color.Black);
                            LightMap.Draw(dummyShape, addStates);
                        }
                        Tile.Draw(App, tiles[x, y].type + tiles[x, y].breakState, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE), tiles[x, y].version, tiles[x, y].lavaVersion);
                        Tile.DrawLights(LightMap, tiles[x, y].type + tiles[x, y].breakState, new Vector2f(x * Tile.TILE_SIZE, y * Tile.TILE_SIZE), tiles[x, y].version);
                    }

                LightMap.Display();
                App.Draw(player);
                App.SetView(App.DefaultView);
                App.Draw(new Sprite(LightMap.Texture), multStates);
                App.SetView(gameView);
                if (stage == 0)
                    App.Draw(selector);
                App.Display();
            }
            App.MouseButtonPressed -= mouseClick;
            App.MouseButtonReleased -= mouseRelease;
            App.KeyPressed -= keyDown;
        }
    }
}