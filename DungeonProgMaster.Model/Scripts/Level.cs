using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DungeonProgMaster.Model
{
    public class Level
    {
        public readonly int id;
        public readonly int[,] map;
        readonly Player reservePlayer;
        public Player player;
        public readonly List<Point> pieces;
        public LinkedList<Script> scripts;
        public HashSet<Point> pickedPieces;
        public readonly Script[] openedScripts;

        public Level(int id, int[,] map, Player player, Point[] pieces, Command[] openedCommands)
        {
            CheckLevelOnCorrect(map, pieces);

            this.map = map;

            if (!InMap(player.TargetPosition) || map[player.TargetPosition.Y, player.TargetPosition.X] != (int)Tales.Ground)
                throw new Exception($"Уровень с ID:{id}. Игрок не на земле!");

            this.id = id;
            this.player = player;
            reservePlayer = new Player(player.TargetPosition, player.Movement);
            pickedPieces = new HashSet<Point>();
            scripts = new LinkedList<Script>();

            this.pieces = new List<Point>(pieces);

            if (openedCommands.Length == 0)
            {
                openedScripts = new Script[4];
                openedScripts[0] = new Script(Command.Forward);
                openedScripts[1] = new Script(Command.Rotate);
                openedScripts[2] = new Script(Command.RepeatStart);
                openedScripts[3] = new Script(Command.RepeatEnd);
            }
            else
            {
                openedScripts = new Script[openedCommands.Length];
                for (var i = 0; i < openedCommands.Length; i++)
                {
                    openedScripts[i] = new Script(openedCommands[i]);
                }
            }
        }

        private static void CheckLevelOnCorrect(int[,] map, Point[] pieces)
        {
            if (map.GetLength(0) != map.GetLength(1)) throw new Exception("Карта должна быть квадратной!");
            bool containsFinish = false;
            foreach (var i in map)
                if ((int)Tales.Finish == i) { containsFinish = true; break; }
            if (!containsFinish) throw new Exception("Не существует выхода с уровня!");
            
            foreach (var i in pieces)
                if (i.X < 0 || i.Y < 0 || i.X >= map.GetLength(0) || i.Y >= map.GetLength(1))
                    throw new Exception($"Монета с координатами {i} находится за пределами карты!");
        }
       
        public void Reset()
        {
            player = new Player(Point.Ceiling(reservePlayer.Position), reservePlayer.Movement, player);
            pickedPieces.Clear();
        }

        public void TakePeace()
        {
            if (player.Position != player.TargetPosition) throw new Exception("Ошибка расположения игрока");
            if (pickedPieces.Contains(player.TargetPosition) || !pieces.Contains(player.TargetPosition)) throw new Exception("Монета уже подобрана, либо её нет на этом месте");
            pickedPieces.Add(player.TargetPosition);
        }

        #region Action with Scripts

        public void ScriptsClear() => scripts.Clear();

        public void ScriptsRemove(int startS, int count)
        {
            var node = scripts.First;
            for (var i = 0; i < scripts.Count; i++)
            {
                if (i == startS)
                {
                    for (var j = 0; j <= count; j++)
                    {
                        var next = node.Next;
                        scripts.Remove(node);
                        node = next;
                    }
                    break;
                }
                node = node.Next;
            }
        }

        public void ScriptsInsert(int startS, Script script)
        {
            if (scripts.Count == 0) { scripts.AddLast(script); return; };
            var node = scripts.First;
            for (var i = 0; i < scripts.Count; i++)
            {
                if (i == startS)
                {
                    scripts.AddAfter(node, script);
                    break;
                }
                node = node.Next;
            }
        }

        public List<Script> GetScripts()
        {
            return ScriptParse(scripts.ToList());
        }

        private List<Script> ScriptParse(List<Script> scripts)
        {
            if (scripts == null || scripts.Count == 0) return new List<Script>();
            int hooks = 0;
            var result = new List<Script>();
            var buffer = new LinkedList<List<Script>>();
            foreach (var i in scripts)
            {
                if (i.Move == Command.RepeatStart)
                {
                    hooks++;
                    buffer.AddLast(new List<Script>());
                    buffer.Last.Value.Add(i);
                }
                else if (i.Move == Command.RepeatEnd)
                {
                    hooks--;
                    if (hooks < 0) continue;
                    buffer.Last.Value.Add(i);
                    ScriptRepeatCreate(hooks, result, buffer);
                }
                else if (hooks == 0) result.Add(i);
                else buffer.Last.Value.Add(i);
            }
            return result;
        }

        private void ScriptRepeatCreate(int hooks, List<Script> result, LinkedList<List<Script>> buffer)
        {
            if (hooks == 0)
            {
                buffer.Last.Value.RemoveAt(0);
                buffer.Last.Value.RemoveAt(buffer.Last.Value.Count - 1);
                var comboScript = new List<Script>();
                for (var j = 0; j < 4; j++)
                    comboScript.AddRange(buffer.Last.Value);
                result.AddRange(comboScript);
            }
            else
            {
                var comboScript = ScriptParse(buffer.Last.Value);
                buffer.RemoveLast();
                buffer.Last.Value.AddRange(comboScript);
            }
        }

        #endregion

        #region Check

        public Tales WatchOnTarget()
        {
            var pos = player.TargetPosition;
            if (!InMap(pos))
            {
                return Tales.Wall;
            }
            else if (map[pos.Y, pos.X] == (int)Tales.Blank)
            {
                return Tales.Blank;
            }
            else return Tales.Ground;
        }

        public bool InMap(Point pos)
        {
            return !(pos.X < 0 || pos.Y < 0 || pos.X >= map.GetLength(0) || pos.Y >= map.GetLength(1));
        }

        public bool ItIsPiece()
        {
            if (player.Position != player.TargetPosition) return false;
            return pieces.Contains(player.TargetPosition);
        }

        public bool ItIsPickedPiece()
        {
            if (player.Position != player.TargetPosition) return false;
            return pickedPieces.Contains(player.TargetPosition);
        }

        public bool AllPiecesAssembled()
        {
            return pickedPieces.Count == pieces.Count;
        }

        public bool IsFinished()
        {
            return map[(int)player.Position.Y, (int)player.Position.X] == (int)Tales.Finish;
        }

        #endregion
    }

    public enum Tales
    {
        Blank = 0,
        Ground = 1,
        Finish = 2,
        Wall = 4,
    }

    //шаблон
    //{
    //    new Level(0,
    //        new int[,]
    //    {
    //        { 0,0,0,0,0,0,0,0,0,0,0,0 },
    //        { 1,1,1,1,1,1,1,1,1,2,1,1 },
    //        { 1,1,1,1,1,1,1,1,1,1,1,1 },
    //        { 1,1,1,1,1,1,1,1,1,1,1,1 },
    //        { 1,1,1,1,1,0,1,0,1,1,1,1 },
    //        { 1,1,1,1,1,1,1,1,1,1,1,1 },
    //        { 1,1,1,1,1,1,1,1,1,1,1,1 },
    //        { 0,0,0,0,0,0,0,0,1,1,1,1 },
    //        { 0,0,0,0,0,0,0,0,1,1,1,1 },
    //        { 0,0,0,0,0,0,0,0,1,1,1,1 },
    //        { 0,0,0,0,0,0,0,0,1,1,1,1 },
    //        { 0,0,0,0,0,0,0,0,1,1,1,1 }
    //    }, 
    //        new Player(new Point(1, 1), PlayerMoveAnim.Right),
    //        new Point[] { new Point(4, 1), new Point(5, 1), new Point(6, 1), new Point(7, 1) }, new Command[0])
    //},
}
