using System;

namespace InvaderGame
{
    /// <summary>
    /// 自機に関する処理を定義
    /// </summary>
    class Ship
    {
        // 要変更
        public int xPos;
        public int yPos;

        /// <summary>
        /// 自機の作成
        /// </summary>
        public Ship()
        {
            xPos = 32;
            yPos = 25;
            UpdateDrawing(xPos, yPos);
        }

        /// <summary>
        /// 左に移動する
        /// </summary>
        public void MoveLeft()
        {
            var beforeXPos = xPos;

            xPos -= 2;
            if (xPos < 0)
            {
                xPos = 0;
                return;
            }
            UpdateDrawing(beforeXPos, yPos);
        }

        /// <summary>
        /// 右に移動する
        /// </summary>
        public void MoveRight()
        {
            var beforeXPos = xPos;

            xPos += 2;
            if (xPos > 60)
            {
                xPos = 60;
                return;
            }
            UpdateDrawing(beforeXPos, yPos);
        }

        /// <summary>
        /// 弾を撃つ
        /// </summary>
        /// <returns>弾のインスタンス</returns>
        public GameCharacter ShotBullet()
        {
            return new ShipBullet(xPos + 2, yPos - 1);
        }

        /// <summary>
        /// 当たり判定
        /// </summary>
        /// <param name="hitTestArray">当たり判定用配列</param>
        /// <returns>true:当たった、false:当たっていない</returns>
        public bool HitTest(GameCharacter[] hitTestArray)
        {
            var triangleIndex = (Console.WindowWidth * yPos + xPos + 2) / 2;
            var squareIndex = (Console.WindowWidth * (yPos + 1) + xPos) / 2;

            if (hitTestArray[triangleIndex] != null ||
                hitTestArray[squareIndex] != null || hitTestArray[squareIndex + 1] != null || hitTestArray[squareIndex + 2] != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 指定した位置の描画を更新する
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        private void UpdateDrawing(int x, int y)
        {
            Erase(x, y);
            Draw();
        }

        /// <summary>
        /// 自機を描画する
        /// </summary>
        private void Draw()
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write("　▲　");
            Console.SetCursorPosition(xPos, yPos + 1);
            Console.Write("■■■");
            //Console.SetCursorPosition(0, 2);
        }

        /// <summary>
        /// 指定した位置の描画を消す
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        private void Erase(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("　　　");
            Console.SetCursorPosition(x, y + 1);
            Console.Write("　　　");
            //Console.SetCursorPosition(0, 2);
        }

        // 要消去
        public void UpdateDrawing()
        {
            UpdateDrawing(xPos, yPos);
        }
    }
}
