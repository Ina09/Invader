using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvaderGame
{
    /// <summary>
    /// 敵の弾に関する処理を定義
    /// </summary>
    class EnemyBullet : GameCharacter
    {
        /// <summary>
        /// 敵弾の作成
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public EnemyBullet(int x, int y)
        {
            xPos = x;
            yPos = y;
            drawCharacter = "↓";
            hitPoint = 1;
            AttackPower = 1;
            moveInterval = maxMoveInterval = 2;
            UpdateDrawing();
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="ch">判定するキャラクタ</param>
        public override void HitTest(GameCharacter ch)
        {
            if (ch.GetType() == typeof(Enemy))
            { return; }

            Erase(xPos, yPos);
            hitPoint = 0;
            ch.ReduceHitPoint(AttackPower);
        }

        /// <summary>
        /// キャラクタの移動
        /// </summary>
        public override void Move()
        {
            if (moveInterval > 0)
            {
                moveInterval--;
                return;
            }

            if (hitPoint <= 0) { return; }

            var beforeYPos = yPos;
            yPos++;
            if (yPos > 26)
            {
                hitPoint = 0;
                yPos = 26;
                Erase(xPos, 26);
                return;
            }
            UpdateDrawing(xPos, beforeYPos);
            moveInterval = maxMoveInterval;
        }
    }
}
