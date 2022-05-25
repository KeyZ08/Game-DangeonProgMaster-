using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DungeonProgMaster
{
    public class Piece
    {
        private int currentFrame = 0;

        public int CurrentFrame
        {
            get { return currentFrame; }
            private set
            {
                if (value >= Frames.Count) currentFrame = 0;
                else if (value < 0) currentFrame = Frames.Count - 1;
                else currentFrame = value;
            }
        }

        public readonly List<Image> Frames;

        public Piece()
        {
            Frames = new List<Image>();
            var images = new Bitmap(Application.StartupPath + @"..\..\..\Resources\Piece_Images.png");
            for (var i = 0; i < 10; i++)
                Frames.Add(images.Clone(new Rectangle(new Point(i * 32, 0), new Size(32, 32)), images.PixelFormat));
        }

        public void FrameUpdate()
        {
            CurrentFrame++;
        }
    }
}
