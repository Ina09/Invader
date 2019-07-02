using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvaderGame
{
    /// <summary>
    /// 自機の弾に関する処理を定義
    /// </summary>
    class ShipBullet : GameCharacter
    {
        /// <summary>
        /// 自機の弾の作成
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public ShipBullet(int x, int y)
        {
            xPos = x;
            yPos = y;
            drawCharacter = "§";
            hitPoint = 1;
            AttackPower = 1;
            UpdateDrawing();
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="ch">判定するキャラクタ</param>
        public override void UpdateHitPoint(GameCharacter ch)
        {
            Erase(xPos, yPos);
            hitPoint = 0;
            ch.ReduceHitPoint(AttackPower);
        }

        /// <summary>
        /// キャラクタの移動
        /// </summary>
        public override void Move()
        {
            if (hitPoint <= 0) { return; }

            var beforeYPos = yPos;
            yPos--;
            if (yPos < 1)
            {
                hitPoint = 0;
                yPos = 1;
                Erase(xPos, 1);
                return;
            }
            UpdateDrawing(xPos, beforeYPos);
        }
    }
}
