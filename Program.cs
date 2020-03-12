using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Spleef
{
    internal class Program
    {
        public static RenderWindow App;
        public static Texture SparkTexture = new Texture("assets/images/spark.png") { Smooth = true };

        private static void Main(string[] args)
        {
            App = new RenderWindow(new VideoMode(900, 600), "SPLEEF!", Styles.Close);
            App.SetVerticalSyncEnabled(true);
            App.Closed += (sender, e) => App.Close();
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
            var rotation = 0f;

            while (App.IsOpen)
            {
                App.DispatchEvents();

                var delta = clock.Restart();

                if (bigger)
                {
                    title.Scale *= 1 + .05f * delta.AsSeconds();
                    startMessage.Scale *= 1 + .2f * delta.AsSeconds();
                    if (startMessage.Scale.X > 1.2f)
                        bigger = false;
                }
                else
                {
                    title.Scale /= 1 + .05f * delta.AsSeconds();
                    startMessage.Scale /= 1 + .2f * delta.AsSeconds();
                    if (startMessage.Scale.X < 1)
                        bigger = true;
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
                spark.Scale = startMessage.Scale;
                clock.Restart();

                App.Clear(new Color(255, (byte)(bgGreen * 255), 0));

                App.Draw(spark, new RenderStates(shrinker));
                App.Draw(startMessage);
                App.Draw(title);

                App.Display();
            }
        }
    }
}