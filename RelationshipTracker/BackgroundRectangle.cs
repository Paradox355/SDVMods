using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDVMods.RelationshipTracker
{
    class BackgroundRectangle
    {
        public int X { get; set; } // x coord of block
        public int Y { get; set; } // y coord of block
        public int Width { get; set; } // Width of block
        public int Height { get; set; } // Height of block
        public Color Color { get; set; } // Color of the block

        private Texture2D Pixel { get; set; }
        private SpriteBatch SpriteBatch;
        private readonly GraphicsDevice GraphicsDevice;

        public BackgroundRectangle(int x, int y, int width, int height, Color color, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D pixel)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.Color = color;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDevice = graphicsDevice;
            Pixel = pixel;
            Pixel.SetData(new Color[] { Color.White });
        }

        public void Draw()
        {
            SpriteBatch.Draw(Pixel, new Rectangle(X, Y, Width, Height), null, Color);
        }

        public void DrawBorder()
        {
            SpriteBatch.Draw(Pixel, new Rectangle(X+1, Y+1, Width - 2, 2), new Color(143, 69, 30)); //top border
            SpriteBatch.Draw(Pixel, new Rectangle(X+1, Y+1, 2, Height - 2), new Color(143, 69, 30)); // left border
            SpriteBatch.Draw(Pixel, new Rectangle(X+1, Y+52, Width - 2, 2), new Color(143, 69, 30)); // heading border
            SpriteBatch.Draw(Pixel, new Rectangle(Width - 1, Y+1, 2, Height - 2), new Color(143, 69, 30)); // right border
            SpriteBatch.Draw(Pixel, new Rectangle(X + 1, Y + Height - 3, Width - 2, 2), new Color(143, 69, 30)); // bottom border
        }
    }
}
