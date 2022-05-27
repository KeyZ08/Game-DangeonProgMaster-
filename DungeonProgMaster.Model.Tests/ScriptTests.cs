using NUnit.Framework;
using System.Drawing;
using System;

namespace DungeonProgMaster.Model.Tests
{
    class ScriptTests
    {
        [Test]
        public void Forward()
        {
            var player = new Player(new Point(0,0), PlayerMoveAnim.Bottom);
            var script = new Script(Command.Forward);
            script.Play(player);
            Assert.AreEqual(new Point(0,1), player.TargetPosition);
        }

        [Test]
        public void Rotate()
        {
            var player = new Player(new Point(0, 0), PlayerMoveAnim.Bottom);
            var script = new Script(Command.Rotate);
            script.Play(player);
            Assert.AreEqual(PlayerMoveAnim.Left, player.NextMovement);
        }
    }
}
