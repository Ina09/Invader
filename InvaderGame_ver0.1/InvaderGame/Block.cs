using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvaderGame
{
    /// <summary>
    /// 防御用のブロックに関する処理を定義
    /// </summary>
    class Block : GameCharacter
    {
        private static string[] drawCharacterList = new string[] { "　", "－", "＋", "※", "□" };

        /// <summary>
        /// ブロックの作成
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public Block(int x, int y)
        {
            xPos = x;
            yPos = y;
            hitPoint = 4;
            AttackPower = 1;
            drawCharacter = drawCharacterList[hitPoint];
            UpdateDrawing();
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="ch">判定するキャラクタのインスタンス</param>
        public override void UpdateHitPoint(GameCharacter ch)
        {
            Erase(xPos, yPos);
            hitPoint -= ch.AttackPower;
            if (ch.AttackPower ==  4)
            {
                ch.IsBottom = false;
                return;
            }
            ch.ReduceHitPoint(AttackPower);
        }

        /// <summary>
        /// キャラクタの移動
        /// </summary>
        public override void Move()
        {
            if (hitPoint <= 0) { return; }
        }

        /// <summary>
        /// キャラクタの描画
        /// </summary>
        protected override void Draw()
        {
            drawCharacter = drawCharacterList[hitPoint];
            base.Draw();
        }
    }
}
