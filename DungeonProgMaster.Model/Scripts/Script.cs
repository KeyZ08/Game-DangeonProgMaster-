using System;
using System.Collections.Generic;

namespace DungeonProgMaster.Model
{
    public class Script
    {
        public readonly Command Move;

        public readonly string Sketch;

        public readonly string Declaration;

        private readonly Action<Player> Doing;

        public Script(Command move)
        {
            Move = move;
            Sketch = data[move].sketch;
            Declaration = data[move].declaration;
            Doing = Commands.commands[move];
        }

        public void Play(Player player)
        {
            Doing.Invoke(player);
        }

        private static readonly Dictionary<Command, (string sketch, string declaration)> data = new()
        {
            { Command.Forward, ("Player.Forward();", "Player.Forward();") },
            { Command.Rotate, ("Player.Rotate();", "Player.Rotate();") },
            { Command.RepeatStart, ("Repeat(3){", "Repeat(3)") },
            { Command.RepeatEnd, ("}", "EndRepeat()") },
        };
    }
}
