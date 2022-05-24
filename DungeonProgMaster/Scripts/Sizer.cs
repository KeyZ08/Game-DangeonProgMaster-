using System.Drawing;

namespace DungeonProgMaster
{
    class Sizer
    {
        public readonly int rows;
        public readonly int columns;
        public readonly float coeff;
        public readonly SizeF floorSize;

        public Sizer(int rows, int columns, float coeff, SizeF floorSize)
        {
            this.rows = rows;
            this.columns = columns;
            this.coeff = coeff;
            this.floorSize = floorSize;
        }

        /// <summary>
        /// Устанавливает мировые координаты соответственно размеру мира
        /// </summary>
        public PointF GetWorldPosition(PointF position, SizeF size)
        {
            var pos = position;
            var center = floorSize / 2;
            var inWorldPosition = new PointF(floorSize.Width * pos.X + (center.Width - size.Width / 2),
                floorSize.Height * pos.Y + (center.Height - size.Height / 1.6f));
            return inWorldPosition;
        }

        /// <summary>
        /// Устанавливает размер соответственно размеру мира 
        /// </summary>
        public SizeF GetWorldSize(Size size)
        {
            var wight = size.Width * (coeff * 1.2f);
            var height = size.Height * (coeff * 1.2f);
            var inWorldSize = new SizeF(wight, height);
            return inWorldSize;
        }

    }
}
