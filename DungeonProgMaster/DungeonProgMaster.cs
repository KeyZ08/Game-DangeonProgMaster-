using System;
using DungeonProgMaster.Model;
using System.Drawing;
using Timers = System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace DungeonProgMaster
{
    public partial class DungeonProgMaster : Form
    {
        Level level;

        private PointF WorldPlayerPosition;
        private SizeF WorldPlayerSize;
        private readonly Timers.Timer pieceAnimator;
        private float frameTimeSpeed = 1f;//чем больше тем медленнее
        private readonly MapData.Piece pieceData;

        public DungeonProgMaster()
        {
            level = Levels.GetLevel(0);
            InitializeComponent();
            InitializeDesign();

            pieceData = new MapData.Piece();
            pieceAnimator = new Timers.Timer(100 * frameTimeSpeed);
            pieceAnimator.Elapsed += PieceUpdateFrame;
            pieceAnimator.Start();
        }

        /// <summary>
        /// Сбрасывает состояние карты к исходному (включая персонажа)
        /// </summary>
        private void LevelReset()
        {
            level.Reset();
            SetPlayerWorldPositionAndSize(sizer);
            WindowResize();
            gamePlace.Invalidate();
        }

        private void OnKeyDownNotepad(object sender, KeyEventArgs args)
        {
            var key = args.KeyCode;
            if(key == Keys.Back)
            {
                NotepadRemoveItem();
            }
        }

        public void PlayerMove()
        {
            var player = level.player;
            if (player.Position == player.TargetPosition)
                return;

            var frame = 1.0f / player.Anim.Count;
            player.Move(frame);

            if ((player.CurrentFrame == 2 || player.CurrentFrame == 4) && sounds.TryGetValue("Floor", out (WaveOut wave, string audio) floor))
            {
                floor.wave.Init(new AudioFileReader(floor.audio));
                floor.wave.Play();
            };

            if (player.Position == player.TargetPosition)
            {
                if (level.ItIsPiece() && !level.ItIsPickedPiece())
                {
                    level.TakePeace();
                    if (sounds.TryGetValue("Money", out (WaveOut wave, string audio) money))
                    {
                        money.wave.Init(new AudioFileReader(money.audio));
                        money.wave.Play();
                    }
                }
            }
        }

        /// <summary>
        /// Перемещает персонажа по карте соответственно скорости анимации
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void PlayerMovement()
        {
            PlayerMove();

            SetPlayerWorldPositionAndSize(sizer);
            var player = level.player;
            player.UpdatePlayerFrame();

            if (player.Position != player.TargetPosition)
            {
                System.Threading.Thread.Sleep((int)(100 * frameTimeSpeed));
                PlayerMovement();
            }
            if (player.NextMovement != player.Movement)
            {
                System.Threading.Thread.Sleep((int)(150 * frameTimeSpeed));
                player.Rotate();
            }

            gamePlace.Invalidate();
        }

        private bool WatсhWallAndBlank()
        {
            var target = level.WatchOnTarget();
            if (target == MapData.Tales.Wall)
            {
                MessageBox.Show("Похоже вы пытались выйти за пределы карты, чего делать нельзя. Будьте осторожней.", "Ой",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LevelReset();
                return false;
            }
            else if (target == MapData.Tales.Blank)
            {
                MessageBox.Show("Вы чуть не упали в дыру в полу. Будьте осторожней!", "Ой",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LevelReset();
                return false;
            }
            return true;
        }

        private void Finished()
        {
            if(!level.AllPiecesAssembled())
            {
                MessageBox.Show("Для перехода на следующий уровень нужно собрать все монеты!", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LevelReset();
            }
            else if (level.IsFinished())
            {
                var message = MessageBox.Show("Уровень пройден! Перейти на следующий уровень? ", "Ура!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (message == DialogResult.Yes)
                {
                    level = Levels.GetLevel(level.id + 1);
                    LevelReset();
                    notepad.BeginInvoke(new Action(() => notepad.ResetText()));
                }
                else LevelReset();
            }
            else
            {
                MessageBox.Show("Похоже вы не дошли до финиша, попробуйте ещё раз!", "Опля",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LevelReset();
            }
        }

        private void PlayButtonClick(object sender, EventArgs args)
        {
            if (level.player.IsAnimated) return;

            var task = Task.Run(() =>
            {
                level.player.IsAnimated = true;
                SetEnabledControls(false, menu.Controls);
                var scripts = level.GetScripts();
                for (var i = 0; i < scripts.Count; i++)
                {
                    scripts[i].Play(level.player);
                    if (!WatсhWallAndBlank())
                    {
                        SetEnabledControls(true, menu.Controls);
                        return;
                    }
                    PlayerMovement();
                }
                level.player.IsAnimated = false;
                Finished();

                SetEnabledControls(true, menu.Controls);
                notepad.BeginInvoke(new Action(() => notepad.Enabled = true));
            });
        }

        #region Actions with Scripts

        private void AddButtonMenu_ItemClick(object sender, ToolStripItemClickedEventArgs args)
        {
            var start = notepad.SelectionStart - 1;
            var end = start + notepad.SelectionLength;
            FindSelectedScripts(start, end, out int startS, out int endS);

            var command = level.openedScripts[contextMenu.Items.IndexOf(args.ClickedItem)].Move;
            if (notepad.SelectionLength == 0) 
            {
                level.ScriptsInsert(startS, new Script(command));
            }
            else
            {
                level.ScriptsRemove(startS, endS - startS);
                level.ScriptsInsert(startS - (endS - startS), new Script(command));
            }
            notepad.Text = ScriptsWrite();
            notepad.Select(end + notepad.Lines[endS].Length, 0);
        }

        private void NotepadResetClick(object sender, EventArgs args)
        {
            level.ScriptsClear();
            notepad.Text = ScriptsWrite();
        }

        private void NotepadRemoveItem()
        {
            var start = notepad.SelectionStart;
            var end = start + notepad.SelectionLength - 1;

            FindSelectedScripts(start, end, out int startS, out int endS);

            level.ScriptsRemove(startS, endS - startS);
            notepad.Text = ScriptsWrite();
            notepad.SelectionStart = start;
        }

        private void FindSelectedScripts(int start, int end, out int startS, out int endS)
        {
            var sum = 0;
            startS = -1;
            endS = -1;
            
            var str = notepad.Lines;
            for (var i = 0; i < str.Length; i++)
            {
                if (i != 0) sum += 2;//2 = '\n'.Length
                if (startS == -1 && sum + str[i].Length >= start) startS = i;
                if (startS != -1 && sum + str[i].Length >= end) endS = i;
                sum += str[i].Length;
                if (endS != -1) break;
            }
        }

        private void AddButtonClick(object sender, EventArgs args)
        {
            contextMenu = new ContextMenuStrip();
            for (var i = 0; i < level.openedScripts.Length; i++)
            {
                contextMenu.Items.Add(level.openedScripts[i].Declaration);
            }

            contextMenu.ItemClicked += new ToolStripItemClickedEventHandler(AddButtonMenu_ItemClick);
            contextMenu.Show(addButton, new Point(addButton.Height, 0));
        }

        #endregion
    }
}
