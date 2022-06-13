using NUnit.Framework;
using System;

namespace DungeonProgMaster.Model.Tests
{
    public class LevelsTests
    {
        [Test]
        public void Levels_OutRange1()
            => Assert.Throws<ArgumentOutOfRangeException>(()=> Levels.GetLevel(999));

        [Test]
        public void Levels_OutRange2() 
            => Assert.Throws<ArgumentOutOfRangeException>(() => Levels.GetLevel(-1));

        [Test]
        public void Levels_NotOutRange() 
            => Assert.DoesNotThrow(() => Levels.GetLevel(0));
    }
}