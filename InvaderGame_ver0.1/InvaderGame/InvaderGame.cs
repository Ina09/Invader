using System;

namespace InvaderGame
{
    /// <summary>
    /// 起動時に呼び出される
    /// </summary>
    class InvaderGame
    {
        static void Main(string[] args)
        {
            // コンソールの大きさの設定
            Console.Title = "InvaderGame";
            Console.CursorVisible = false;
            Console.SetWindowSize(66,26);

            var controler = new GameControler();
            
            // ゲーム開始
            controler.GameStart();
        }
    }
}
