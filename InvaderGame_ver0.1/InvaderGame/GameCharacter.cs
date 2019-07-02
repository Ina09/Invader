using System;

namespace InvaderGame
{
    /// <summary>
    /// ゲームに登場するキャラクタに共通する処理を定義
    /// </summary>
    abstract class GameCharacter
    {
        
        public int AttackPower { get; set; }
        public int Score { get; set; }
        public virtual bool IsBottom
        {
            get { return false; }
            set { }
        }
        protected int xPos;
        protected int yPos;
        protected string drawCharacter;
        protected int hitPoint;
        protected int maxMoveInterval;
        protected int moveInterval;
        

        /// <summary>
        /// 移動処理
        /// </summary>
        public abstract void Move();

        /// <summary>
        /// ヒットポイントの更新を行う
        /// </summary>
        /// <param name="ch">当たった相手のキャラクタ</param>
        public abstract void UpdateHitPoint(GameCharacter ch);

        /// <summary>
        /// 弾を撃つ処理
        /// </summary>
        /// <returns>弾のインスタンス(撃たなかった場合はnull)</returns>
        public virtual GameCharacter ShotBullet()
        {
            return null;
        }

        /// <summary>
        /// 移動速度を変える必要があるか調べる
        /// </summary>
        /// <returns>true:速度変更が必要、false:変更する必要はない</returns>
        public virtual bool NeedChangeSpeed()
        {
            return false;
        }

        /// <summary>
        /// 移動速度を変更する
        /// </summary>
        /// <param name="speed">変更する移動速度</param>
        public virtual void ChangeSpeed(int speed)
        {
            return;
        }

        /// <summary>
        /// ヒットポイントを攻撃力の分減らす
        /// </summary>
        /// <param name="attackPower">攻撃力</param>
        public void ReduceHitPoint(int attackPower)
        {
            hitPoint -= attackPower;
            Erase(xPos, yPos);
        }

        /// <summary>
        /// 画面内での位置インデックスを取得する
        /// </summary>
        /// <returns>位置インデックス</returns>
        public int GetIndex()
        {
            var index = (Console.WindowWidth * yPos + xPos) / 2;
            return index;
        }

        /// <summary>
        /// キャラクタのヒットポイントが残っているか
        /// </summary>
        /// <returns>true:残っている、false:残っていない</returns>
        public bool IsAlive()
        {
            if (hitPoint <= 0)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 位置を変えずにキャラクタの描画を更新する
        /// </summary>
        public void UpdateDrawing()
        {
            UpdateDrawing(xPos,yPos);
        }

        /// <summary>
        /// 描画位置を変更してキャラクタの描画を更新する
        /// </summary>
        /// <param name="x">変更前のx座標</param>
        /// <param name="y">変更前のy座標</param>
        protected void UpdateDrawing(int x, int y)
        {
            Erase(x, y);
            Draw();
        }

        /// <summary>
        /// キャラクタを描画する
        /// </summary>
        protected virtual void Draw()
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(drawCharacter);
        }

        /// <summary>
        /// 指定した位置のキャラクタの描画を消す
        /// </summary>
        /// <param name="x">消すキャラクタのx座標</param>
        /// <param name="y">消すキャラクタのy座標</param>
        protected void Erase(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("　");
        }
    }
}
