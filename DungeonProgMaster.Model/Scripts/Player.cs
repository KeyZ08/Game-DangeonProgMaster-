using System.Collections.Generic;
using System;
using System.Drawing;
using System.IO;

namespace DungeonProgMaster.Model
{
    public class Player
    {
        private PointF _position;
        public PointF Position => _position;

        private Point _target;
        public Point TargetPosition => _target;
        public PlayerMoveAnim Movement { get; private set; }
        public PlayerMoveAnim NextMovement { get; private set; }

        public Player(Point position, PlayerMoveAnim defaultAnim)
        {
            _position = position;
            _target = position;
            Movement = defaultAnim;
            NextMovement = Movement;
        }

        public Player(Point position, PlayerMoveAnim defaultAnim, Player p)
        {
            _position = position;
            Movement = defaultAnim;
            _target = position;
            NextMovement = Movement;
        }

        public void GetNextMovement()
        {
            if (Movement == PlayerMoveAnim.Right)
                NextMovement = PlayerMoveAnim.Bottom;
            else if (Movement == PlayerMoveAnim.Left)
                NextMovement = PlayerMoveAnim.Top;
            else if (Movement == PlayerMoveAnim.Top)
                NextMovement = PlayerMoveAnim.Right;
            else if (Movement == PlayerMoveAnim.Bottom)
                NextMovement = PlayerMoveAnim.Left;
        }

        public void GetNextTargetPosition()
        {
            if (Movement == PlayerMoveAnim.Right)
                _target.X += 1;
            else if (Movement == PlayerMoveAnim.Left)
                _target.X -= 1;
            else if (Movement == PlayerMoveAnim.Top)
                _target.Y -= 1;
            else if (Movement == PlayerMoveAnim.Bottom)
                _target.Y += 1;
        }

        public void Move(float distance)
        {
            var pos = Position;
            if (Movement == PlayerMoveAnim.Right) pos.X += distance;
            else if (Movement == PlayerMoveAnim.Left) pos.X -= distance;
            else if (Movement == PlayerMoveAnim.Top) pos.Y -= distance;
            else if (Movement == PlayerMoveAnim.Bottom) pos.Y += distance;
            _position.X = (float)Math.Round(pos.X, 2);
            _position.Y = (float)Math.Round(pos.Y, 2);
        }

        public void Rotate()
        {
            Movement = NextMovement;
        }
    }

    public enum PlayerMoveAnim
    {
        Top = 1,
        Bottom = 0,
        Left = 3,
        Right = 2,
    }
}
