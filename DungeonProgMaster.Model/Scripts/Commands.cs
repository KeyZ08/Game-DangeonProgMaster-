using System;
using System.Collections.Generic;

namespace DungeonProgMaster.Model
{
    public class Commands
    {
        public static void ForwardMove(Player player)
        {
            player.GetNextTargetPosition();
        }

        public static void Rotate(Player player)
        {
            player.GetNextMovement();
        }

        public static void RepeatStart(Player player)
        { }

        public static void RepeatEnd(Player player)
        { }

        public static Dictionary<Command, Action<Player>> commands = new()
        {
            {
                Command.Forward,
                ForwardMove
            },
            {
                Command.Rotate,
                Rotate
            },
            {
                Command.RepeatStart,
                RepeatStart
            },
            {
                Command.RepeatEnd,
                RepeatEnd
            }
        };
    }

    public enum Command
    {
        Rotate = 0,
        Forward = 1,
        RepeatStart = 2,
        RepeatEnd = 3
    }
}
