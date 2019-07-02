using System;

namespace InvaderGame
{
    /// <summary>
    /// 敵に関する処理を定義
    /// </summary>
    class Enemy : GameCharacter
    {
        private bool moveLine = false;
        private bool moveLeft = false;
        public override bool IsBottom
        {
            get;set;
        }

        /// <summary>
        /// 敵の作成
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <param name="score">敵のスコア</param>
        /// <param name="interval">移動速度</param>
        public Enemy(int x, int y, int score, int interval)
        {
            xPos = x;
            yPos = y;
            Score = score;
            moveInterval =  maxMoveInterval = interval;
            drawCharacter = (y % 2 == 0 ? "央" : "中");
            hitPoint = 1;
            AttackPower = 4;
            if (Score == 10)
            {
                IsBottom = true;
            }
            UpdateDrawing();
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="ch">判定するキャラクタ</param>
        public override void UpdateHitPoint(GameCharacter ch)
        {
            if (ch.GetType() == typeof(EnemyBullet))
            { return; }

            Erase(xPos, yPos);
            hitPoint -= ch.AttackPower;
            ch.ReduceHitPoint(AttackPower);
        }

        /// <summary>
        /// キャラクタの移動
        /// </summary>
        public override void Move()
        {
            // 移動まで間隔をあける
            if (moveInterval > 0)
            {
                moveInterval--;
                return;
            }

            if (hitPoint <= 0) { return; }

            drawCharacter = drawCharacter.Equals("央") ? "中" : "央";

            // 行を移動する
            if (moveLine)
            {
                 yPos++;
                UpdateDrawing(xPos, yPos - 1);

                moveLeft = !moveLeft;
                moveInterval = maxMoveInterval;
                return;
            }
           
            // 左に移動
            if (moveLeft)
            {
                xPos -= 2;
                UpdateDrawing(xPos + 2, yPos);
            }

            // 右に移動
            if (!moveLeft)
            {
                xPos += 2;
                UpdateDrawing(xPos - 2, yPos);
            }

            moveInterval = maxMoveInterval;
        }

        /// <summary>
        /// 移動速度を変える必要があるか調べる
        /// </summary>
        /// <returns>true:速度変更が必要、false:変更する必要はない</returns>
        public override bool NeedChangeSpeed()
        {
            if (moveInterval < maxMoveInterval) { return false; }
            if (hitPoint <= 0) { return false; }

            // 行を移動した直後はスピードの変更はしない
            if (moveLine)
            {
               moveLine = false;
                return false;
            }

            // 画面左端かつ左に移動、または、画面右端かつ右に移動の時
            // 行を下に移動し、移動速度を変更する必要がある
            if( (xPos == 0 && moveLeft) || (xPos == 64 && !moveLeft) )
            {
                moveLine = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 弾を撃つ処理
        /// </summary>
        /// <returns>弾のインスタンス(撃たなかった場合はnull)</returns>
        public override GameCharacter ShotBullet()
        {
            // 同列の敵の中で一番下にいなければ弾を撃たない
            if (!IsBottom)
            {
                return null;
            }

            // 画面の一番下の行に来たら撃たない
            if (yPos >= Console.WindowHeight - 2){ return null; }

            return new EnemyBullet(xPos, yPos + 2);
        }

        /// <summary>
        /// 移動速度を変更する
        /// </summary>
        /// <param name="speed">変更する移動速度</param>
        public override void ChangeSpeed(int speed)
        {
            moveLine = true;
            maxMoveInterval = speed;
            // 速度の変更に伴ってmoveIntervalのカウントもはじめから行う
            moveInterval = maxMoveInterval;
        }

    }
}
