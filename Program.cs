using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Linq;

namespace Spleef
{
    internal class Program
    {
        public static RenderWindow App;
        public static Image Icon = new Image("assets/images/icon.png");
        public static Texture SparkTexture = new Texture("assets/images/spark.png") { Smooth = true };

        public static bool IsCancelKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Escape, Keyboard.Key.Backspace, Keyboard.Key.A }.Contains(key);

        public static bool IsConfirmKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Enter, Keyboard.Key.Space, Keyboard.Key.E }.Contains(key);

        public static bool IsDownKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Down, Keyboard.Key.S }.Contains(key);

        public static bool IsLeftKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Left, Keyboard.Key.Q }.Contains(key);

        public static bool IsRightKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Right, Keyboard.Key.D }.Contains(key);

        public static bool IsUpKey(Keyboard.Key key) => new Keyboard.Key[] { Keyboard.Key.Up, Keyboard.Key.Z }.Contains(key);

        private static void Main(string[] args)
        {
            App = new RenderWindow(new VideoMode(900, 600), "SPLEEF!", Styles.Close);
            App.SetVerticalSyncEnabled(true);
            App.Closed += (sender, e) => App.Close();
            App.SetIcon(256, 256, Icon.Pixels);
            var title = new RectangleShape(new Vector2f(43, 26) * 400 / 43) { Texture = new Texture("assets/images/spleef.png") };
            title.Origin = title.Size / 2;
            title.Position = (Vector2f)App.Size / 2;
            var startMessage = new CustomText("Appuyer sur une touche") { Height = 20 };
            startMessage.Origin = new Vector2f(startMessage.LocalBounds.Width / 2, 10);
            startMessage.Position = new Vector2f(App.Size.X / 2, 560);
            bool bigger = true;
            var clock = new Clock();
            var darker = true;
            var bgGreen = .6f;
            var spark = new RectangleShape(new Vector2f(startMessage.LocalBounds.Width, startMessage.LocalBounds.Width)) { Texture = SparkTexture };
            spark.Origin = spark.Size / 2;
            var shrinker = Transform.Identity;
            shrinker.Translate(startMessage.Position);
            shrinker.Scale(1.5f, 20f / startMessage.LocalBounds.Width * 2.5f);
            var cursor = new RightCursor(64);

            var rotation = 0f;

            bool titleTime = true;

            var play = new CustomText("Jouer")
            {
                Height = 40,
                Position = new Vector2f(450, 250)
            };
            play.Origin = new Vector2f(play.LocalBounds.Width / 2, 20);
            var howTo = new CustomText("Comment jouer")
            {
                Height = 40,
                Position = new Vector2f(450, 350)
            };
            howTo.Origin = new Vector2f(howTo.LocalBounds.Width / 2, 20);
            var quit = new CustomText("Quitter")
            {
                Height = 40,
                Position = new Vector2f(450, 450)
            };
            quit.Origin = new Vector2f(quit.LocalBounds.Width / 2, 20);
            CustomText selection = play;

            void keyPressed(object sender, KeyEventArgs e)
            {
                if (titleTime)
                {
                    titleTime = false;
                    selection = play;
                }
                else
                {
                    if (IsUpKey(e.Code))
                        if (selection == play)
                            selection = quit;
                        else if (selection == quit)
                            selection = howTo;
                        else if (selection == howTo)
                            selection = play;
                    if (IsDownKey(e.Code))
                        if (selection == play)
                            selection = howTo;
                        else if (selection == howTo)
                            selection = quit;
                        else if (selection == quit)
                            selection = play;
                    if (IsCancelKey(e.Code))
                        titleTime = true;
                    if (IsConfirmKey(e.Code))
                    {
                        if (selection == quit)
                            App.Close();
                    }
                }
            }
            void mouseClick(object sender, MouseButtonEventArgs e)
            {
                if (titleTime)
                {
                    titleTime = false;
                    selection = play;
                }
                else
                {
                }
            }
            App.KeyPressed += keyPressed;
            App.MouseButtonPressed += mouseClick;

            while (App.IsOpen)
            {
                App.DispatchEvents();

                var delta = clock.Restart();

                if (titleTime)
                {
                    if (title.Position.Y < App.Size.Y / 2)
                        title.Position += new Vector2f(0, delta.AsSeconds() * 450);
                    if (bigger)
                    {
                        startMessage.Scale *= 1 + .2f * delta.AsSeconds();
                        if (startMessage.Scale.X > 1.2f)
                            bigger = false;
                    }
                    else
                    {
                        startMessage.Scale /= 1 + .2f * delta.AsSeconds();
                        if (startMessage.Scale.X < 1)
                            bigger = true;
                    }
                    title.Scale = startMessage.Scale * .9f;
                }
                else
                {
                    if (title.Position.Y > 100)
                        title.Position -= new Vector2f(0, delta.AsSeconds() * 450);
                    if (bigger)
                    {
                        title.Scale *= 1 + .1f * delta.AsSeconds();
                        if (title.Scale.X > .7f)
                            bigger = false;
                    }
                    else
                    {
                        if (title.Scale.X > .7f)
                            title.Scale /= 1 + .8f * delta.AsSeconds();
                        title.Scale /= 1 + .1f * delta.AsSeconds();
                        if (title.Scale.X < .6f)
                            bigger = true;
                    }
                }

                if (darker)
                {
                    bgGreen /= 1 + .2f * delta.AsSeconds();
                    if (bgGreen < .3f)
                        darker = false;
                }
                else
                {
                    bgGreen *= 1 + .2f * delta.AsSeconds();
                    if (bgGreen > .65f)
                        darker = true;
                }
                rotation += delta.AsSeconds() * 60;
                spark.Rotation = rotation;
                spark.Scale = selection.Scale;

                {
                    play.Scale = new Vector2f(1, 1);
                    howTo.Scale = new Vector2f(1, 1);
                    quit.Scale = new Vector2f(1, 1);
                    selection.Scale *= 1.3f;
                    cursor.Position = new Vector2f(selection.GlobalBounds.Left - 10, selection.Position.Y);
                    if (selection == play)
                        cursor.Color = new Color(0, 255, 132);
                    else if (selection == howTo)
                        cursor.Color = new Color(0, 192, 255);
                    else if (selection == quit)
                        cursor.Color = new Color(255, 0, 84);
                }

                App.Clear(new Color(255, (byte)(bgGreen * 255), 0));

                if (titleTime)
                {
                    App.Draw(spark, new RenderStates(shrinker));
                    App.Draw(startMessage);
                }
                else
                {
                    App.Draw(play);
                    App.Draw(howTo);
                    App.Draw(quit);
                    App.Draw(cursor);
                }
                App.Draw(title);

                App.Display();
            }
        }
    }
}