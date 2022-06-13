using DungeonProgMaster.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DungeonProgMaster
{
    class PlayerAnimator
    {
        readonly Dictionary<PlayerMoveAnim, List<Bitmap>> animations;
        public int CurrentFrame { get; private set; }
        public List<Bitmap> Anim { get; private set; }
        public bool IsAnimated { get; set; }

        public PlayerAnimator(PlayerMoveAnim movement)
        {
            var images = new Bitmap(Application.StartupPath + @"..\..\..\Resources\Character_SpriteSheet.png");
            animations = new Dictionary<PlayerMoveAnim, List<Bitmap>>();
            CreateAnimations(images, PlayerMoveAnim.Top);
            CreateAnimations(images, PlayerMoveAnim.Bottom);
            CreateAnimations(images, PlayerMoveAnim.Left);
            CreateAnimations(images, PlayerMoveAnim.Right);

            Anim = animations[movement];
            CurrentFrame = 0;
            IsAnimated = false;
        }

        /// <summary>
        /// Обновляет картинку персонажа
        /// </summary>
        public void UpdatePlayerFrame(PlayerMoveAnim movement)
        {
            //вычисление анимации
            var anim = PlayerMoveAnimations(movement);
            if (CurrentFrame >= 0) CurrentFrame++;
            if (CurrentFrame >= anim.Count)
                CurrentFrame = 0;
            Anim = anim;
        }

        public void UpdateMovement(PlayerMoveAnim movement)
        {
            Anim = PlayerMoveAnimations(movement);
        }

        public void Reset(PlayerMoveAnim movement)
        {
            Anim = PlayerMoveAnimations(movement);
            CurrentFrame = 0;
            IsAnimated = false;
        }

        public List<Bitmap> PlayerMoveAnimations(PlayerMoveAnim move)
        {
            return animations.TryGetValue(move, out var result) ? result : throw new ArgumentException($"{move} нет в словаре");
        }

        /// <summary>
        /// Разбивает общий спрайт на его части, группируя в списки для анимации соответственно перемещению
        /// </summary>
        /// <param name="move">Направление движения</param>
        private void CreateAnimations(Bitmap images, PlayerMoveAnim move)
        {
            var list = new List<Bitmap>();
            for (var i = 0; i < 5; i++)
                list.Add(images.Clone(new Rectangle(new Point(i * 64, (int)move * 64), new Size(64, 64)), images.PixelFormat));
            animations.Add(move, list);
        }
    }
}
