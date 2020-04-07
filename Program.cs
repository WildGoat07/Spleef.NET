using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Spleef
{
    internal class Program
    {
        public static RenderWindow App;
        public static float DefaultVolume = 25;
        public static Random Generator;
        public static SoundBuffer MainTheme;
        public static SoundBuffer[] Musics;
        public static bool SoundEnabled = true;

        private static void Main(string[] args)
        {
            Generator = new Random();
            App = new RenderWindow(new VideoMode(900, 600), "SPLEEF!", Styles.Close);
            App.SetVerticalSyncEnabled(true);
            App.Closed += (sender, e) => App.Close();
            App.SetIcon(256, 256, Utilities.Icon.Pixels);
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
            var spark = new RectangleShape(new Vector2f(startMessage.LocalBounds.Width, startMessage.LocalBounds.Width)) { Texture = Utilities.SparkTexture };
            spark.Origin = spark.Size / 2;
            var shrinker = Transform.Identity;
            shrinker.Translate(startMessage.Position);
            shrinker.Scale(1.5f, 20f / startMessage.LocalBounds.Width * 2.5f);
            var cursor = new RightCursor(64);
            {
                void stopAll(object sender, EventArgs e) => Environment.Exit(0);
                App.Closed += stopAll;
                var text = new CustomText("Chargement des musiques...") { Height = 40 };
                text.Origin = new Vector2f(text.LocalBounds.Width / 2, text.LocalBounds.Height / 2);
                text.Position = (Vector2f)App.Size / 2;
                var t = Task.Run(() =>
                {
                    //Utilities.InitMusics();
                    text.Text = "Chargement des textures...";
                    text.Origin = new Vector2f(text.LocalBounds.Width / 2, text.LocalBounds.Height / 2);
                    text.Position = (Vector2f)App.Size / 2;
                    Tile.InitTiles();
                });
                while (!t.IsCompleted)
                {
                    App.DispatchEvents();
                    App.Clear(new Color(255, 150, 0));
                    App.Draw(text);
                    App.Display();
                }
                App.Closed -= stopAll;
            }

            var mainMusic = new Sound(MainTheme) { Loop = true, Volume = DefaultVolume };

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
            var sound = new CustomText("$")
            {
                Height = 40
            };
            sound.Position = new Vector2f(900 - sound.LocalBounds.Width, 550);
            sound.Origin = new Vector2f(sound.LocalBounds.Width / 2, 20);
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
                    if (Utilities.IsUpKey(e.Code))
                        if (selection == play)
                            selection = quit;
                        else if (selection == quit)
                            selection = howTo;
                        else if (selection == howTo)
                            selection = play;
                        else if (selection == sound)
                            selection = quit;
                    if (Utilities.IsDownKey(e.Code))
                        if (selection == play)
                            selection = howTo;
                        else if (selection == howTo)
                            selection = quit;
                        else if (selection == quit)
                            selection = play;
                        else if (selection == sound)
                            selection = play;
                    if (Utilities.IsRightKey(e.Code) || Utilities.IsLeftKey(e.Code))
                        if (selection == sound)
                            selection = play;
                        else
                            selection = sound;
                    if (Utilities.IsCancelKey(e.Code))
                        titleTime = true;
                    if (Utilities.IsConfirmKey(e.Code))
                    {
                        if (selection == play)
                            Game.launch();
                        if (selection == quit)
                            App.Close();
                        if (selection == sound)
                        {
                            SoundEnabled = !SoundEnabled;
                            sound.Text = SoundEnabled ? "$" : "£";
                            if (SoundEnabled)
                                mainMusic.Play();
                            else
                                mainMusic.Pause();
                        }
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
                    if (play.GlobalBounds.Contains(App.MapPixelToCoords(e)))
                        Game.launch();
                    else if (howTo.GlobalBounds.Contains(App.MapPixelToCoords(e)))
                        ;
                    else if (quit.GlobalBounds.Contains(App.MapPixelToCoords(e)))
                        App.Close();
                    else if (sound.GlobalBounds.Contains(App.MapPixelToCoords(e)))
                    {
                        SoundEnabled = !SoundEnabled;
                        sound.Text = SoundEnabled ? "$" : "£";
                        if (SoundEnabled)
                            mainMusic.Play();
                        else
                            mainMusic.Pause();
                    }
                }
            }
            App.KeyPressed += keyPressed;
            App.MouseButtonPressed += mouseClick;

            mainMusic.Play();
            clock.Restart();
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

                if (play.GlobalBounds.Contains(App.MapPixelToCoords(Mouse.GetPosition(App))))
                    selection = play;
                if (howTo.GlobalBounds.Contains(App.MapPixelToCoords(Mouse.GetPosition(App))))
                    selection = howTo;
                if (quit.GlobalBounds.Contains(App.MapPixelToCoords(Mouse.GetPosition(App))))
                    selection = quit;
                if (sound.GlobalBounds.Contains(App.MapPixelToCoords(Mouse.GetPosition(App))))
                    selection = sound;
                {
                    play.Scale = new Vector2f(1, 1);
                    howTo.Scale = new Vector2f(1, 1);
                    quit.Scale = new Vector2f(1, 1);
                    sound.Scale = new Vector2f(1, 1);
                    play.Color = Color.White;
                    howTo.Color = Color.White;
                    quit.Color = Color.White;
                    selection.Scale *= 1.3f;
                    cursor.Position = new Vector2f(selection.GlobalBounds.Left - 10, selection.Position.Y);
                    if (selection == play)
                        cursor.Color = Utilities.Green;
                    else if (selection == howTo)
                        cursor.Color = Utilities.Blue;
                    else if (selection == quit)
                        cursor.Color = Utilities.Red;
                    else if (selection == sound)
                        cursor.Color = SoundEnabled ? Utilities.Blue : Utilities.Red;
                    selection.Color = cursor.Color;
                    sound.Color = Color.White;
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
                    App.Draw(sound);
                }
                App.Draw(title);

                App.Display();
            }
        }
    }
}