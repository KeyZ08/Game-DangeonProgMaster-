using System.Collections.Generic;
using System;
using System.Drawing;
using System.IO;

namespace DungeonProgMaster.Model
{
    public class Player
    {
        readonly Dictionary<PlayerMoveAnim, List<Bitmap>> animations;
        private PointF _position;
        public PointF Position => _position;

        private Point _target;
        public Point TargetPosition => _target;
        public PlayerMoveAnim Movement { get; private set; }
        public PlayerMoveAnim NextMovement { get; private set; }
        public int CurrentFrame { get; private set; }
        public List<Bitmap> Anim { get; private set; }
        public bool IsAnimated { get; set; }

        public Player(Point position, PlayerMoveAnim defaultAnim)
        {
            _position = position;
            _target = position;
            var images = new Bitmap(Path.GetFullPath(@"..\..\..\Resources\Character_SpriteSheet.png"));
            animations = new Dictionary<PlayerMoveAnim, List<Bitmap>>();
            CreateAnimations(images, PlayerMoveAnim.Top);
            CreateAnimations(images, PlayerMoveAnim.Bottom);
            CreateAnimations(images, PlayerMoveAnim.Left);
            CreateAnimations(images, PlayerMoveAnim.Right);
            Movement = defaultAnim;
            Anim = animations[Movement];
            NextMovement = Movement;
            CurrentFrame = 0;
            IsAnimated = false;
        }

        public Player(Point position, PlayerMoveAnim defaultAnim, Player p)
        {
            _position = position;
            Movement = defaultAnim;
            _target = position;
            animations = p.animations;
            Anim = animations[Movement];
            NextMovement = Movement;
            CurrentFrame = 0;
            IsAnimated = false;
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
            CurrentFrame = 0;
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
            CurrentFrame = 0;
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
            Anim = animations[Movement];
        }

        /// <summary>
        /// Обновляет картинку персонажа
        /// </summary>
        public void UpdatePlayerFrame()
        {
            //вычисление анимации
            var anim = PlayerMoveAnimations(Movement);
            if (CurrentFrame >= 0) CurrentFrame++;
            if (CurrentFrame >= anim.Count)
                CurrentFrame = 0;
            this.Anim = anim;
        }

        public List<Bitmap> PlayerMoveAnimations(PlayerMoveAnim move)
        {
            return animations.TryGetValue(move, out var result) ? result: throw new ArgumentException($"{move} нет в словаре");
        }

        /// <summary>
        /// Разбивает общий спрайт на его части, группируя в списки для анимации соответственно перемещению
        /// </summary>
        /// <param name="move">Направление движения</param>
        private void CreateAnimations(Bitmap images, PlayerMoveAnim move)
        {
            var list = new List<Bitmap>();
            for(var i = 0; i < 5; i++)
                list.Add(images.Clone(new Rectangle(new Point(i*64, (int)move * 64), new Size(64,64)), images.PixelFormat));
            animations.Add(move, list);
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
