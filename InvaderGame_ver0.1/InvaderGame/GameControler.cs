using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace InvaderGame
{
    /// <summary>
    /// ゲーム全体の処理を管理する
    /// </summary>
    class GameControler
    {
        private System.Timers.Timer autoEventThread;
        private List<GameCharacter> gameCharacters;
        private int stage;
        private int totalScore;
        private bool isGameOver;
        private Ship ship;
        private GameCharacter shipBullet;
        private int moveSpeed = 33;
        private int shotBulletInterval;
        private bool changeSpeed = false;
        GameCharacter[] hitTestArray; // 当たり判定用配列

        /// <summary>
        /// ゲームを開始する
        /// </summary>
        public void GameStart()
        {
            // ゲーム開始時の初期値
            stage = 1;
            totalScore = 0;
            isGameOver = false;

            InitializeCharacters();

            // キーボードでのShipの操作を受け付ける
            Thread shipEventThread = new Thread(new ThreadStart(ShipEvent));
            shipEventThread.Start();

            // 操作と無関係のイベントを定期的に起こす
            autoEventThread = new System.Timers.Timer(60);
            autoEventThread.Elapsed += new ElapsedEventHandler(AutoEvent);
            autoEventThread.Start();            
        }

        /// <summary>
        /// キャラクタを初期化する
        /// </summary>
        private  void InitializeCharacters()
        {
            Console.Clear();

            // Shipの作成
            ship = new Ship();

            // 弾を撃っていない状態にする
            shipBullet = null;

            // キャラクタリストの初期化
            gameCharacters = new List<GameCharacter>();
            hitTestArray = new GameCharacter[1000];
            var point = 60;
            shotBulletInterval = moveSpeed / 3;

            // 敵軍の作成
            // ステージごとに開始位置を変更
            for (int line = (stage % 6 + 3); line < (stage % 6 + 2*6 + 3); line += 2)
            {
                for (int column = 0; column < 4*12; column += 4)
                {
                    var inv = new Enemy(column, line, point, moveSpeed);
                    gameCharacters.Add(inv);
                }
                point -= 10;
            }
            
            // ブロックの作成
            for (int y = 20; y < 24; y++)
            {
                for (int x = 4; x < 14; x += 2)
                {
                    var block = new Block(x, y);
                    gameCharacters.Add(block);
                }

                for (int x = 20; x < 30; x += 2)
                {
                    var block = new Block(x, y);
                    gameCharacters.Add(block);
                }

                for (int x = 36; x < 46; x += 2)
                {
                    var block = new Block(x, y);
                    gameCharacters.Add(block);
                }

                for (int x = 52; x < 62; x += 2)
                {
                    var block = new Block(x, y);
                    gameCharacters.Add(block);
                }
            }
        }

        /// <summary>
        /// 自機の移動、自弾の発射の処理を行う
        /// </summary>
        private  void ShipEvent()
        {
            while (!isGameOver)
            {
                lock (ship)
                {
                    if (!Console.KeyAvailable) { continue; }

                    var key = Console.ReadKey().Key;
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            ship.MoveLeft();
                            continue;
                        case ConsoleKey.RightArrow:
                            ship.MoveRight();
                            continue;
                        case ConsoleKey.Spacebar:
                            if (shipBullet != null) { break; ; }
                            shipBullet = ship.ShotBullet();

                            // 弾を作成した位置に敵・敵弾があった場合は対消滅させる
                            if (hitTestArray[shipBullet.GetIndex()] != null)
                            {
                                hitTestArray[shipBullet.GetIndex()].HitTest(shipBullet);
                                shipBullet = null;
                                continue; ;
                            }
                            gameCharacters.Add(shipBullet);
                            continue;
                        case ConsoleKey.Enter:
                            isGameOver = true;
                            continue;
                    }
                }
            }
            
            // ゲームオーバー後の処理
            while (true)
            {
                if (!Console.KeyAvailable) { continue; }
                var key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.Q: // ゲームをやめる
                        return;
                    case ConsoleKey.Enter: // 新しくゲームを始める
                        GameStart();
                        return;
                    default:
                        continue;
                }
            }
        }

        /// <summary>
        /// 自動で発生するイベント(敵の移動など)の処理を行う
        /// </summary>
        /// <param name="sender">イベントの発生元</param>
        /// <param name="e">発生したイベントのデータ</param>
        private  void AutoEvent(object sender, ElapsedEventArgs e)
        {
            lock (gameCharacters)
            {
                Move();
                ShotEnemyBullet();
                GoNextStage();
                ShowTotalScore();
            }
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        private void Move()
        {
            lock (ship)
            {
                MoveCharacterExceptShip();
                CountScore();

                // 自機の当たり判定
                foreach (var ch in gameCharacters)
                {
                    //Shipの先端より上にいるキャラクタは判定しない
                    if (ch.GetIndex() < (Console.WindowWidth * 25) / 2) { continue; }

                    //敵がShipの先端と同じ段に来たらゲームオーバー
                    if (ch.Score != 0 && ch.Score != 300)
                    {
                        isGameOver = true;
                        return;
                    }
                    // Shipと当たったらゲームオーバー
                    if (ship.HitTest(ch))
                    {
                        isGameOver = true;
                        return;
                    }
                }

                CheckNecessityOfChangeSpeed();
            }
        }

        /// <summary>
        /// 自機以外のキャラクタの移動
        /// </summary>
        private void MoveCharacterExceptShip()
        {
            foreach (var ch in gameCharacters)
            {
                if (changeSpeed) { ch.ChangeSpeed(CalcMoveSpeed()); }
                // 移動前のindexの配列の中身を消す
                hitTestArray[ch.GetIndex()] = null;

                ch.Move();

                if (!ch.IsLived()) { continue; }

                // 当たり判定
                var checkChara = hitTestArray[ch.GetIndex()];
                if (checkChara == null)
                {
                    hitTestArray[ch.GetIndex()] = ch;
                    continue;
                }
                else
                {
                    checkChara.HitTest(ch);
                    if (!checkChara.IsLived())
                    {
                        hitTestArray[ch.GetIndex()] = null;
                    }
                    if (ch.IsLived())
                    {
                        hitTestArray[ch.GetIndex()] = ch;
                    }

                    if (hitTestArray[ch.GetIndex()] != null) { hitTestArray[ch.GetIndex()].UpdateDrawing(); }
                }
            }

            // 画面にShipの弾は一つだけ
            if (shipBullet != null && !shipBullet.IsLived())
            {
                shipBullet = null;
            }
        }

        /// <summary>
        /// キャラクタ移動時に獲得したスコアをカウントする
        /// </summary>
        private void CountScore()
        {
            var score = 0;
            // リストのうちHP = 0になったもののスコアをカウントする
            for (int i = 0; i < gameCharacters.Count; i++)
            {
                if (gameCharacters[i].IsLived()) { continue; }
                // リストから削除するインスタンス
                var deletech = gameCharacters[i];
                gameCharacters.Remove(gameCharacters[i]);

                // 今回獲得した得点のカウント
                score += deletech.Score;

                // 列の一番下のEnemyが消えた場合、弾を発射できるEnemyを変更する
                if (!deletech.IsBottom) { continue; }

                // 同列の一番下の敵が弾を撃てるようにする
                var width = Console.WindowWidth / 2;
                var newLowChara = gameCharacters.FindLast(ch => ch.IsLived() && (ch.GetIndex() < deletech.GetIndex()) && (ch.GetIndex() % width == deletech.GetIndex() % width));
                if (newLowChara != null) { newLowChara.IsBottom = true; }
            }

            if (totalScore / 1000 < (totalScore + score) / 1000)
            {
                gameCharacters.Add(new UFO(moveSpeed));
            }
            totalScore += score;
        }

        /// <summary>
        /// キャラクタの移動速度を変更する必要があるか調べる
        /// </summary>
        private void CheckNecessityOfChangeSpeed()
        {
            foreach (var ch in gameCharacters)
            {
                if (!ch.NeedChangeSpeed()) { continue; }
                changeSpeed = true;
                return;
            }
            changeSpeed = false;
        }

        /// <summary>
        /// キャラクタの移動速度を計算する
        /// </summary>
        /// <returns>移動速度</returns>
        private int CalcMoveSpeed()
        {
            int count = 0;
            foreach (var ch in gameCharacters)
            {
                if (ch.Score != 0 && ch.Score != 300)
                {
                    count++;
                }
            }
            if (count == 0) { return 1; }

             var sp = (count / 2) >= moveSpeed ? moveSpeed : (count / 2);
            
            return sp > 0 ? sp : 1;
        }

        /// <summary>
        /// 敵の弾を撃つ
        /// </summary>
        private void ShotEnemyBullet()
        {
            if (shotBulletInterval > 0)
            {
                shotBulletInterval--;
                return;
            }

            var listx = gameCharacters.OrderBy(i => Guid.NewGuid()).Take(10).ToList();
            foreach (var ch in listx)
            {
                var bullet = ch.ShotBullet();
                // 打たれなかった場合
                if (bullet == null) { continue; }

                if (hitTestArray[bullet.GetIndex()] != null)
                {
                    hitTestArray[bullet.GetIndex()].UpdateDrawing();
                    continue;
                }

                gameCharacters.Add(bullet);
                break;
            }
            shotBulletInterval = moveSpeed / 3;
        }

        /// <summary>
        /// 次のステージに進む
        /// </summary>
        private void GoNextStage()
        {            
            foreach (var ch in gameCharacters)
            {
                if (ch.Score != 0 && ch.Score != 300)
                {
                    return;
                }
            }
            
            autoEventThread.Stop();
            
            if (stage == 6)
            {
                moveSpeed = moveSpeed + 3 * 6;
                stage = 1;
            }
            else
            {
                moveSpeed -= 3;
                stage += 1;
            }
            
            InitializeCharacters();
            autoEventThread.Start();
        }

        /// <summary>
        /// 合計スコアを表示する
        /// </summary>
        private void ShowTotalScore()
        {
            if (!isGameOver) { return; }

            autoEventThread.Stop();

            Console.Clear();
            Console.SetCursorPosition(20, 8);
            Console.Write("GameOver");
            Console.SetCursorPosition(20, 10);
            Console.WriteLine("Max stage   : " + stage);
            Console.SetCursorPosition(20, 11);
            Console.WriteLine("Total score : " + totalScore);
            Console.SetCursorPosition(20, 16);
            Console.WriteLine("Play new game : push Enter");
            Console.SetCursorPosition(20, 17);
            Console.WriteLine("Quit game     : push Q");
        }
    }
}
