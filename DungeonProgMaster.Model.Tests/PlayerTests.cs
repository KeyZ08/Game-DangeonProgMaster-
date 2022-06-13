using NUnit.Framework;
using System;
using System.Drawing;

namespace DungeonProgMaster.Model.Tests
{
    class PlayerTests
    {
        [Test]
        public void Rotate()
        {
            var player = new Player(new Point(0,0), PlayerMoveAnim.Right);
            player.GetNextMovement();
            Assert.AreEqual(PlayerMoveAnim.Bottom, player.NextMovement);

            player.Rotate();
            player.GetNextMovement();
            Assert.AreEqual(PlayerMoveAnim.Left, player.NextMovement);

            player.Rotate();
            player.GetNextMovement();
            Assert.AreEqual(PlayerMoveAnim.Top, player.NextMovement);

            player.Rotate();
            player.GetNextMovement();
            Assert.AreEqual(PlayerMoveAnim.Right, player.NextMovement);

            player.Rotate();
            player.GetNextMovement();
            Assert.AreEqual(PlayerMoveAnim.Bottom, player.NextMovement);
        }

        [Test]
        public void Move()
        {
            var player = new Player(new Point(0, 0), PlayerMoveAnim.Right);
            player.Move(1f);
            Assert.AreEqual(new PointF(0,0), player.Position);

            player.GetNextTargetPosition();
            player.GetNextTargetPosition();
            Assert.AreEqual(new Point(1,0), player.TargetPosition);

            player.Move(1f);
            Assert.AreEqual(new PointF(1,0), player.Position);

            player.Move(1f);
            Assert.AreEqual(new PointF(1, 0), player.Position);

            player.GetNextTargetPosition();
            player.Move(0.25f);
            Assert.AreEqual(new PointF(1.25f, 0), player.Position);
            player.Move(0.75f);
            Assert.AreEqual(new PointF(2, 0), player.Position);
        }
    }
}
