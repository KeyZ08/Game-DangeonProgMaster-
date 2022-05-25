using System;
using System.Collections.Generic;

namespace DungeonProgMaster.Model
{
    public class Script
    {
        public Command Move { get; private set; }

        public string Sketch { get; private set; }

        public string Declaration { get; private set; }

        private readonly Action<Player> Doing;

        public Script(Command move)
        {
            Move = move;
            Sketch = Sketches.data[move].sketch;
            Declaration = Sketches.data[move].declaration;
            Doing = Commands.commands[move];
        }

        public void Play(Player player)
        {
            Doing.Invoke(player);
        }
    }

    public static class Sketches
    {
        public static Dictionary<Command, (string sketch, string declaration)> data = new()
        {
            { Command.Forward, ("Player.Forward();", "Player.Forward();") },
            { Command.Rotate, ("Player.Rotate();", "Player.Rotate();") },
            { Command.RepeatStart, ("Repeat(3){", "Repeat(3)") },
            { Command.RepeatEnd, ("}", "EndRepeat()") },
        };
    }
}
