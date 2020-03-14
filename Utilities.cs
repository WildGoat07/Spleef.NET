using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spleef
{
    public static class Utilities
    {
        public static Color Blue = new Color(0, 192, 255);

        public static Color Green = new Color(0, 255, 132);

        public static Image Icon = new Image("assets/images/icon.png");

        public static Color Red = new Color(255, 0, 84);

        public static Texture SparkTexture = new Texture("assets/images/spark.png") { Smooth = true };

        public static bool Contains(this FloatRect rect, Vector2f vector) => rect.Contains(vector.X, vector.Y);

        public static void InitMusics()
        {
            try
            {
                Program.MainTheme = new SoundBuffer("assets/audio/ost (in flames)/voices.ogg");
                var list = new List<SoundBuffer>();
                list.Add(new SoundBuffer("assets/audio/ost (in flames)/burn.ogg"));
                list.Add(new SoundBuffer("assets/audio/ost (in flames)/call my name.ogg"));
                list.Add(new SoundBuffer("assets/audio/ost (in flames)/deep inside.ogg"));
                list.Add(new SoundBuffer("assets/audio/ost (in flames)/i am above.ogg"));
                list.Add(new SoundBuffer("assets/audio/ost (in flames)/i the mask.ogg"));
                list.Add(new SoundBuffer("assets/audio/ost (in flames)/house.ogg"));
                Program.Musics = list.Shuffle().ToArray();
            }
            catch (Exception) { }
        }

        public static bool IsCancelKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Escape, Keyboard.Key.Backspace, Keyboard.Key.A }.Contains(key);

        public static bool IsConfirmKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Enter, Keyboard.Key.Space, Keyboard.Key.E }.Contains(key);

        public static bool IsDownKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Down, Keyboard.Key.S }.Contains(key);

        public static bool IsLeftKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Left, Keyboard.Key.Q }.Contains(key);

        public static bool IsRightKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Right, Keyboard.Key.D }.Contains(key);

        public static bool IsUpKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Up, Keyboard.Key.Z }.Contains(key);

        public static Vector2f MapPixelToCoords(this RenderWindow window, MouseButtonEventArgs e) => window.MapPixelToCoords(new Vector2i(e.X, e.Y));

        public static Vector2f MapPixelToCoords(this RenderWindow window, MouseButtonEventArgs e, View view) => window.MapPixelToCoords(new Vector2i(e.X, e.Y), view);

        public static Vector2f MapPixelToCoords(this RenderWindow window, MouseMoveEventArgs e) => window.MapPixelToCoords(new Vector2i(e.X, e.Y));

        public static Vector2f MapPixelToCoords(this RenderWindow window, MouseMoveEventArgs e, View view) => window.MapPixelToCoords(new Vector2i(e.X, e.Y), view);

        public static ICollection<T> Shuffle<T>(this ICollection<T> collection)
        {
            var copy = collection.ToList();
            var randomizer = new Random();
            var list = new List<T>();
            while (copy.Count > 0)
            {
                var index = randomizer.Next(copy.Count);
                list.Add(copy[index]);
                copy.RemoveAt(index);
            }
            return list;
        }
    }
}