using System;
using NUnit.Framework;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace DungeonProgMaster.Model.Tests
{
    public class LevelTests
    {
        [Test]
        public void PlayerIsNull() 
        {
            Assert.Throws<ArgumentNullException>(() => new Level(0,
            new int[,]
            {
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,2},
            },
                null,
                Array.Empty<Point>(),
                Array.Empty<Command>())
            );
        }

        [Test]
        public void PlayerGoesOffMap()
        {
            var exception = Assert.Throws<Exception>(() => new Level(0,
            new int[,]
            {
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,2},
            },
                new Player(new Point(-1,-1), PlayerMoveAnim.Right),
                Array.Empty<Point>(),
                Array.Empty<Command>())
            );
            Assert.AreEqual($"Уровень с ID:{0}. Игрок не на земле!", exception.Message);
        }

        [Test]
        public void PlayerOnBlank()
        {
            var exception = Assert.Throws<Exception>(() => new Level(0,
            new int[,]
            {
                { 1,0,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,2},
            },
                new Player(new Point(1, 0), PlayerMoveAnim.Right),
                Array.Empty<Point>(),
                Array.Empty<Command>())
            );
            Assert.AreEqual($"Уровень с ID:{0}. Игрок не на земле!", exception.Message);
        }

        [Test]
        public void MapWithoutExit()
        {
            var exception = Assert.Throws<Exception>(() => new Level(0,
            new int[,]
            {
                { 1,0,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
                { 1,1,1,1,1},
            },
                new Player(new Point(0, 0), PlayerMoveAnim.Right),
                Array.Empty<Point>(),
                Array.Empty<Command>())
            );

            Assert.AreEqual("Не существует выхода с уровня!", exception.Message);
        }

        [Test]
        public void MapIsNotSquare()
        {
            var exception = Assert.Throws<Exception>(() => new Level(0,
            new int[,]
            {
                { 1,1,1,1,1,1},
                { 1,1,1,1,1,2},
                { 1,1,1,1,1,1},
            },
                new Player(new Point(1, 0), PlayerMoveAnim.Right),
                Array.Empty<Point>(),
                Array.Empty<Command>())
            );
            Assert.AreEqual("Карта должна быть квадратной!", exception.Message);
        }

        [Test]
        public void PieceGoesOffMap()
        {
            var exception = Assert.Throws<Exception>(() => 
            {
                new Level(0,
                new int[,]
                {
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,2},
                },
                    new Player(new Point(1, 0), PlayerMoveAnim.Right),
                    new Point[] { new Point(-1,1)},
                    Array.Empty<Command>());
            });

            Assert.AreEqual($"Монета с координатами {new Point(-1, 1)} находится за пределами карты!", exception.Message);
        }

        [Test]
        public void CorrectGetScripts()
        {
            var level = new Level(0,
                new int[,]
                {
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,1},
                    { 1,1,1,1,1,2},
                },
                    new Player(new Point(1, 0), PlayerMoveAnim.Right),
                    Array.Empty<Point>(),
                    Array.Empty<Command>());
            level.ScriptsInsert(0,new Script(Command.Forward));
            level.ScriptsInsert(0,new Script(Command.RepeatStart));
            level.ScriptsInsert(1,new Script(Command.Forward));
            level.ScriptsInsert(2,new Script(Command.RepeatStart));
            level.ScriptsInsert(3,new Script(Command.Rotate));
            level.ScriptsInsert(4,new Script(Command.RepeatEnd));
            level.ScriptsInsert(5, new Script(Command.Forward));
            level.ScriptsRemove(6, 0);
            level.ScriptsInsert(5, new Script(Command.Forward));
            level.ScriptsInsert(6,new Script(Command.RepeatEnd));
            level.ScriptsInsert(7,new Script(Command.Forward));
            var actual = level.GetScripts().Select(x => x.Move).ToArray();
            var result = new Command[]
            {
                Command.Forward, 
                Command.Forward, Command.Rotate, Command.Rotate, Command.Rotate, Command.Forward,
                Command.Forward, Command.Rotate, Command.Rotate, Command.Rotate, Command.Forward,
                Command.Forward, Command.Rotate, Command.Rotate, Command.Rotate, Command.Forward,
                Command.Forward,
            };
            Assert.AreEqual(result, actual);
        }
    }
}
