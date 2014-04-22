using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DxLibDLL;

namespace Breakout_for_C_Sharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(640, 480);
            DX.SetUserWindow(this.Handle);

            //ログの出力を停止する
            DX.SetOutApplicationLogValidFlag(0);

            DX.DxLib_Init();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DX.DxLib_End();
        }

        public void MainLoop()
        {

           	//描画先グラフィック領域の指定
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);

	            //ウィンドウのタイトルを変更する
            string titleStr = "Breakout";
            DX.SetMainWindowText(titleStr);

            GameMain gameMain = new GameMain(); //ゲーム本体を作成

                
	            // 裏画面を表画面に反映、ウインドウのメッセージを処理、画面を消す
            while (DX.ScreenFlip() == 0 && DX.ProcessMessage() == 0 && DX.ClearDrawScreen() == 0)
            {

	            switch (gameMain.gameMode) {

		            case mode.TITLE:
			            gameMain.showTitleScreenDraw(); //タイトル画面の表示
			            break;
		            case mode.NEWGAME:
			            gameMain.gameInitialize(); //ゲーム新規開始準備
			            break;
		            case mode.MOVEINPUT:
			            gameMain.calc(); //ゲーム内部の計算フェーズ
			            gameMain.gameMainScreenDraw(); //ゲーム画面の描画フェーズ
			            break;
		            case mode.MISS:
			            gameMain.gameMainScreenDraw(); //ゲーム画面の描画フェーズ
			            gameMain.missScreenDraw(); //ミスした際の画面の描画フェーズ
			            break;
		            case mode.GAME_OVER:
			            gameMain.gameMainScreenDraw(); //ゲーム画面の描画フェーズ
			            gameMain.gameOverScreenDraw(); //ゲームオーバー画面の表示
			            break;
		            case mode.STAGE_CLEAR:
			            gameMain.gameMainScreenDraw(); //ゲーム画面の描画フェーズ
			            gameMain.stageClearScreenDraw(); //ステージクリア画面の描画フェーズ
			            break;
		            case mode.GAME_CLEAR:
			            gameMain.gameMainScreenDraw(); //ゲーム画面の描画フェーズ
			            gameMain.gameClearScreenDraw(); //ゲームクリア画面の描画フェーズ
			            break;
		            default:
			            gameMain.showTitleScreenDraw(); //タイトル画面の表示
			            break;
	            }

                //メッセージ・キューにあるWindowsメッセージを全て処理する
                Application.DoEvents();
            }
        }
    }
}
