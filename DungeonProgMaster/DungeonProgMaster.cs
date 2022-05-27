using System;
using DungeonProgMaster.Model;
using System.Drawing;
using Timers = System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.Threading;

namespace DungeonProgMaster
{
    public partial class DungeonProgMaster : Form
    {
        Level level;
        readonly PlayerAnimator playerAnimator;
        private PointF WorldPlayerPosition;
        private SizeF WorldPlayerSize;
        private readonly Timers.Timer pieceAnimator;
        private float frameTimeSpeed = 1f;//чем больше тем медленнее
        private readonly Piece pieceData;
        private CancellationTokenSource stopTask;

        public DungeonProgMaster()
        {
            level = Levels.GetLevel(4);
            playerAnimator = new PlayerAnimator(level.Player.Movement);
            InitializeComponent();
            InitializeDesign();

            pieceData = new Piece();
            pieceAnimator = new Timers.Timer(100);
            pieceAnimator.Elapsed += PieceUpdateFrame;
            pieceAnimator.Start();

            MessageBox.Show("Пожалуйста, прочитайте Инструкцию, если вы впервые заходите в эту игру.", "Важно!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        /// <summary>
        /// Сбрасывает состояние карты к исходному (включая персонажа)
        /// </summary>
        private void LevelReset()
        {
            level.Reset();
            WindowResize();
            playerAnimator.Reset(level.Player.Movement);
            
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
            var player = level.Player;
            if (player.Position == player.TargetPosition)
                return;

            var frame = 1.0f / playerAnimator.Anim.Count;
            player.Move(frame); 
            playerAnimator.UpdatePlayerFrame(player.Movement);

            if ((playerAnimator.CurrentFrame == 2 || playerAnimator.CurrentFrame == 4) && sounds.TryGetValue("Floor", out (WaveOut wave, string audio) floor))
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
        private void PlayerMovement(ref bool isStopped)
        {
            CheckOnStopped(ref isStopped);
            if (isStopped) return;
            PlayerMove();

            SetPlayerWorldPositionAndSize(sizer);
            var player = level.Player;
            
            gamePlace.Invalidate();
            if (player.Position != player.TargetPosition)
            {
                Thread.Sleep((int)(100 * frameTimeSpeed));
                PlayerMovement(ref isStopped);
            }
            if (player.NextMovement != player.Movement)
            {
                Thread.Sleep((int)(150 * frameTimeSpeed));
                player.Rotate();
                playerAnimator.UpdateMovement(player.Movement);
                gamePlace.Invalidate();
            }
        }

        private void PlayButtonClick(object sender, EventArgs args)
        {
            if (playerAnimator.IsAnimated) return;
            stopTask = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                var isStoped = false;
                playerAnimator.IsAnimated = true;
                SetEnabledControls(false, menu.Controls);
                stopButton.BeginInvoke(new Action(() => stopButton.Enabled = true));
                var scripts = level.GetScripts();
                if(level.scripts.Count > level.MaxScriptCount)
                {
                    ExceedingNumberScripts();
                    SetEnabledControls(true, menu.Controls);
                    stopButton.BeginInvoke(new Action(() => stopButton.Enabled = false));
                    LevelReset();
                    return;
                }
                for (var i = 0; i < scripts.Count; i++)
                {
                    if (isStoped)
                    {
                        SetEnabledControls(true, menu.Controls);
                        stopButton.BeginInvoke(new Action(() => stopButton.Enabled = false));
                        LevelReset();
                        return;
                    }

                    scripts[i].Play(level.Player);
                    if (!CheckWallAndBlank())
                    {
                        SetEnabledControls(true, menu.Controls);
                        stopButton.BeginInvoke(new Action(() => stopButton.Enabled = false));
                        return;
                    }
                    PlayerMovement(ref isStoped);
                }
                playerAnimator.IsAnimated = false;
                Finished();

                SetEnabledControls(true, menu.Controls);
                stopButton.BeginInvoke(new Action(() => stopButton.Enabled = false));

            }, stopTask.Token);
        }

        private void CheckOnStopped(ref bool isStoped)
        {
            try
            {
                if (stopTask.IsCancellationRequested)
                    stopTask.Token.ThrowIfCancellationRequested();
            }
            catch
            {
                isStoped = true;
            }
        }

        private void StopButtonClick(object sender, EventArgs args)
        {
            stopTask.Cancel();
        }

        #region Messages

        private bool CheckWallAndBlank()
        {
            var target = level.WatchOnTarget();
            if (target == Tales.Wall)
            {
                MessageBox.Show("Похоже вы пытались выйти за пределы карты, чего делать нельзя. Будьте осторожней.", "Ой",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LevelReset();
                return false;
            }
            else if (target == Tales.Blank)
            {
                MessageBox.Show("Вы чуть не упали в дыру в полу. Будьте осторожней!", "Ой",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LevelReset();
                return false;
            }
            return true;
        }

        private void ExceedingNumberScripts()
        {
            MessageBox.Show($"Данный уровень можно пройти за {level.MaxScriptCount} строк кода. Попробуйте ещё раз!", "Ой",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        private void Finished()
        {
            if (!level.AllPiecesAssembled())
            {
                MessageBox.Show("Для перехода на следующий уровень нужно собрать все монеты!", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LevelReset();
            }
            else if (level.IsFinished())
            {
                var message = MessageBox.Show("Уровень пройден! Перейти на следующий уровень? ", "Ура!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                if (message == DialogResult.Yes)
                {
                    try
                    {
                        level = Levels.GetLevel(level.id + 1);
                    }
                    catch
                    {
                        MessageBox.Show("Благодарю за прохождение демо версии игры. " +
                            "Здесь много чего ещё можно улучшить и добавить," +
                            " но главная идея и механика созданы и функционируют.", "Спасибо за игру!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }
                    LevelReset();
                    notepad.BeginInvoke(new Action(() => notepad.ResetText()));
                }
                else LevelReset();
            }
            else
            {
                MessageBox.Show("Похоже вы не дошли до финиша, попробуйте ещё раз!", "Опля",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                LevelReset();
            }
        }

        #endregion

        #region Actions with Scripts

        private void AddButtonMenu_ItemClick(object sender, ToolStripItemClickedEventArgs args)
        {
            var start = notepad.SelectionStart;
            var end = start == start + notepad.SelectionLength ? start : start + notepad.SelectionLength - 1;
            FindSelectedScripts(start, end, out int startS, out int endS);

            var command = level.openedScripts[contextMenu.Items.IndexOf(args.ClickedItem)].Move;
            if (notepad.SelectionLength == 0) 
            {
                level.ScriptsInsert(startS, new Script(command));
            }
            else
            {
                level.ScriptsRemove(startS, endS - startS);
                level.ScriptsInsert(startS - 1 < 0 ? 0 : startS - 1, new Script(command));
            }
            notepad.Text = ScriptsWrite();
            notepad.SelectionStart = end + notepad.Lines[startS].Length + 1;
        }

        private void NotepadResetClick(object sender, EventArgs args)
        {
            level.ScriptsClear();
            notepad.Text = ScriptsWrite();
        }

        private void NotepadRemoveItem()
        {
            var start = notepad.SelectionStart;
            var end = start == start + notepad.SelectionLength ? start : start + notepad.SelectionLength - 1;

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
                if (i > 0) sum++;
                sum += str[i].Length;
                if (startS == -1 && sum >= start) startS = i;
                if (startS != -1 && sum >= end) endS = i;
                
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
