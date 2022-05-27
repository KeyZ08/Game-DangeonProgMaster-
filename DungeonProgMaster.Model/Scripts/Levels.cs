using System;
using System.Collections.Generic;
using System.Drawing;

namespace DungeonProgMaster.Model
{
    public static class Levels
    {
        static readonly List<Level> levels = new()
        {
            {
                new Level(0,
                    new int[,]
                {
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,2,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                }, 
                    new Player(new Point(2,4), PlayerMoveAnim.Right),
                    Array.Empty<Point>(), new Command[] {Command.Forward})
            },
            {
                new Level(1,
                    new int[,]
                {
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,2,1},
                },
                    new Player(new Point(2, 4), PlayerMoveAnim.Right),
                    Array.Empty<Point>(), new Command[] { Command.Forward, Command.Rotate })
            },
            {
                new Level(2,
                    new int[,]
                {
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,0,1,1,1,1,1,1},
                    { 1,1,0,1,1,1,1,1,1},
                    { 1,1,0,2,1,1,1,1,1},
                    { 1,1,0,0,0,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                },
                    new Player(new Point(1, 4), PlayerMoveAnim.Right),
                    Array.Empty<Point>(), new Command[] { Command.Forward, Command.Rotate })
            },
            {
                new Level(3,
                    new int[,]
                {
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,2,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                },
                    new Player(new Point(1, 4), PlayerMoveAnim.Right),
                    new Point[] { new Point(1,7), new Point(4,7), new Point(4,4)}, 
                    new Command[] { Command.Forward, Command.Rotate, Command.RepeatStart, Command.RepeatEnd })
            },
            {
                new Level(4,
                    new int[,]
                {
                    { 0,0,0,0,0,0,0,0,0},
                    { 1,0,1,1,2,1,0,1,1},
                    { 1,1,1,1,1,1,0,1,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,0,1,1,1,1,1},
                    { 1,1,0,1,1,1,1,1,1},
                    { 1,1,1,1,1,1,1,0,1},
                    { 1,1,1,1,1,1,1,1,1},
                    { 1,1,1,0,1,1,1,1,1},
                }, 
                    new Player(new Point(7,1), PlayerMoveAnim.Bottom),
                    Array.Empty<Point>(), Array.Empty<Command>(), 6)
            },
            {
                new Level(5,
                    new int[,]
                {
                    { 1,1,0,1,1,0,0,1},
                    { 0,1,1,1,1,1,1,1},
                    { 1,1,1,1,0,1,0,1},
                    { 0,1,1,1,1,1,1,1},
                    { 1,1,1,1,0,1,1,1},
                    { 1,1,1,1,1,1,1,1},
                    { 0,0,1,1,1,1,1,0},
                    { 1,2,1,1,0,1,1,1},
                }, 
                    new Player(new Point(7,7), PlayerMoveAnim.Left),
                    new Point[] { 
                        new Point(5,6), new Point(5,4), new Point(5,2),
                        new Point(7,5), new Point(7,3), new Point(7,1),
                        new Point(3,6), new Point(3,4), new Point(3,2),
                        new Point(1,5), new Point(1,3), new Point(1,1),
                    }, 
                    Array.Empty<Command>(), 13)
            },
        };

        public static Level GetLevel(int id)
        {
            if (levels.Count > id)
                return levels[id];
            else throw new ArgumentOutOfRangeException($"Уровня с ID:{id} ещё нет!");
        }
    }
}
