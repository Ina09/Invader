using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvaderGame
{
    /// <summary>
    /// UFOに関する処理を定義
    /// </summary>
    class UFO : GameCharacter
    {
        /// <summary>
        /// UFOクラスの作成
        /// </summary>
        /// <param name="speed">移動スピード</param>
        public UFO(int speed)
        {
            drawCharacter = "興";
            xPos = 0;
            yPos = 1;
            hitPoint = 1;
            AttackPower = 1;
            Score = 300;
            moveInterval = maxMoveInterval = speed / 2;
            UpdateDrawing();
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="ch">判定するキャラクタのインスタンス</param>
        public override void HitTest(GameCharacter ch)
        {
            Erase(xPos, yPos);
            hitPoint -= ch.AttackPower;
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

            xPos += 2;
            if (xPos > 64)
            {
                hitPoint = 0;
                Score = 0;
                xPos = 64;
                Erase(64, yPos);
                return;
            }

            UpdateDrawing(xPos - 2, yPos);
            moveInterval = maxMoveInterval;
        }
    }
}
