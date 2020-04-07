using System;
using System.Collections.Generic;
using System.Text;
using SFML;
using SFML.Window;
using SFML.System;
using SFML.Graphics;

namespace Spleef
{
    public static class Game
    {
        public static RenderWindow App => Program.App;
        public static Random Generator => Program.Generator;

        public static void launch()
        {
            var tiles = new (int, int)[20, 20];
            for (int x = 0; x < 20; x++)
                for (int y = 0; y < 20; y++)
                {
                    var version = Tile.BASE + Generator.Next(3);
                    tiles[x, y] = (version, Generator.Next(Tile.TilesTextures[version].Count));
                }
            while (App.IsOpen)
            {
                App.DispatchEvents();

                App.Clear(Color.Yellow);

                for (int x = 0; x < 20; x++)
                    for (int y = 0; y < 20; y++)
                        Tile.Draw(App, tiles[x, y].Item1, new Vector2f(x * 48, y * 48), tiles[x, y].Item2);

                App.Display();
            }
        }
    }
}