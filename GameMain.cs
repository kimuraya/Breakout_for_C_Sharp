using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DxLibDLL;

namespace Breakout_for_C_Sharp
{

//マップ上のオブジェクトの性質
enum NatureObject{
	OBJ_SPACE,
	OBJ_BLOCK,
	OBJ_UNKNOWN,
};

//ゲームの状態
enum mode {
	TITLE,
	NEWGAME,
	MOVEINPUT,
	MISS,
	GAME_OVER,
	STAGE_CLEAR,
	GAME_CLEAR,
};

    class GameMain
    {
        	    //ゲームで使用する各種定数
	    const int stageHeight = 32; //マップの高さ
	    const int stageWidth = 11; //マップの横幅
	    const int ballInitialCoordinateX = 250; //ボールの初期座標（x軸）
	    const int ballInitialCoordinateY = 400; //ボールの初期座標（y軸）
	    const int ballWidth = 15; //ボールの横幅
	    const double InitialAngle = 45; //ボールの初期角度
	    const int stickInitialCoordinateX = 190; //スティックの初期座標（x軸）
	    const int stickInitialCoordinateY = 440; //スティックの初期座標（y軸）
	    const int stickSpeed = 4; //スティックが移動するスピード
	    const int mapObjectHeight = 15; //ブロックの高さ
	    const int mapObjectWidth = 40; //ブロックの横幅
	    const int locationOfWall = 441; //壁の配置場所
	    const int stageTotalNumber = 5; //ゲームのステージの総数

	    //ゲームで使用する各種変数
	    public mode gameMode; //ゲームの状態を表す
	    int[] key; // キーが押されているフレーム数を格納する
        MapObject[,] stageMap = new MapObject[stageHeight, stageWidth]; //ステージのマップ
	    int currentStageNumber; //現在のステージ数
	    int life = 5; //スティックの残機数

	    //スティックとボール
	    Stick stick;
	    Ball ball;

	    //フォントの色とフォントの指定
	    int fontType = 0;
	    int white = 0;

	    //ゲームのキャラクター
	    int stickbuf = 0;
	    int ballbuf = 0;
	    int wallbuf = 0;
	    int blockbuf = 0;

	    //コンストラクタ
        public GameMain()
        {
		    //変数の初期化
		    this.key = new int[256];
		    this.gameMode = mode.TITLE; //ゲームモードをゲームの新規開始にする

		    //フォントの色とフォントの指定（ゲームクリア時に使用）
		    this.fontType = DX.CreateFontToHandle(null, 64, 5, -1);
		    this.white = DX.GetColor(255, 255, 255);

		    //画面のキャラクターを読み込む
		    this.stickbuf = DX.LoadGraph("gamedata\\stick.png");
		    this.ballbuf = DX.LoadGraph("gamedata\\ball.png");
		    this.wallbuf = DX.LoadGraph("gamedata\\wall.png");
		    this.blockbuf = DX.LoadGraph("gamedata\\block.png");

		    //ボールとスティックを作る
		    this.ball = new Ball();
		    this.stick = new Stick();
	    }

	    //タイトル画面の作成と表示
	    public void showTitleScreenDraw() {

		    byte[] tmpKey = new byte[256]; // 現在のキーの入力状態を格納する

		    string titleStr = "Breakout";
		    DX.DrawStringToHandle(180, 100, titleStr, white, fontType);

		    //Enterキー入力待ちのメッセージ
		    DX.DrawString(200, 300, "Please Press the Enter key", white);

		    //全てのキーの入力状態を得る
		    DX.GetHitKeyStateAll(out tmpKey[0]);

		    //Enterキーが押された場合
		    if (tmpKey[DX.KEY_INPUT_RETURN] == 1) {
			    this.gameMode = mode.NEWGAME;
		    }

		    return;
	    }

	    //ゲームの開始準備
	    public void gameInitialize() {

		    //マップをファイルから読み込む
		    this.stageInitialize("gamedata\\stage1.txt");

		    //ゲームの初期化処理が終わった為、ゲーム本編の画面に移動する
		    gameMode = mode.MOVEINPUT;

		    //現在のステージ数を初期化
		    currentStageNumber = 1;

		    //ボールとスティックを初期配置場所に配置し、ボールの角度を初期化する
		    this.ball.setX(ballInitialCoordinateX);
		    this.ball.setY(ballInitialCoordinateY);
		    this.ball.setAngle(InitialAngle);

		    this.stick.setX(stickInitialCoordinateX);
		    this.stick.setY(stickInitialCoordinateY);

		    return;
	    }

	    //マップの読み込みと初期化
	    private void stageInitialize(string fileName) {

		    char[,] tempStageMap = new char[stageHeight, stageWidth]; //ファイルから読み込んだマップ

		    //ファイルを読み込む
		    StreamReader reader = new StreamReader(fileName);

            for (int i = 0; i < stageHeight; i++)
            {

			    string line;
			    line = reader.ReadLine();

                for (int j = 0; j < stageWidth; j++)
                {
				    tempStageMap[i, j] = line[j];
			    }
		    }

		    //ファイルから読み込んだマップをゲーム内部のマップに変換する
            for (int i = 0; i < stageHeight; i++)
            {

                for (int j = 0; j < stageWidth; j++)
                {

				    switch(tempStageMap[i, j]) {
					    case '#':
						    MapObject blockMapObject = new MapObject();
						    NatureObject blockNatureObject = NatureObject.OBJ_BLOCK;
                            blockMapObject.setNatureObject(blockNatureObject);
                            stageMap[i, j] = blockMapObject;
						    break;
					    case '.':
						    MapObject spaceObject = new MapObject();
						    NatureObject natureObject = NatureObject.OBJ_SPACE;
                            spaceObject.setNatureObject(natureObject);
                            stageMap[i, j] = spaceObject;
						    break;
					    default:
						    MapObject mapObject = new MapObject();
						    NatureObject unknownNatureObject = NatureObject.OBJ_UNKNOWN;
                            mapObject.setNatureObject(unknownNatureObject);
						    stageMap[i, j] = mapObject;
						    break;
				    }
			    }
		    }

		    //マップのオブジェクトの座標を設定する
		    for (int i = 0; i < stageHeight; i++) {

			    for (int j = 0; j < stageWidth; j++) {
				    switch(stageMap[i, j].getNatureObject()) {
					    case NatureObject.OBJ_BLOCK:
						    stageMap[i, j].setX(j * mapObjectWidth);
						    stageMap[i, j].setY(i * mapObjectHeight);
						    break;
					    case NatureObject.OBJ_SPACE:
						    stageMap[i, j].setX(j * mapObjectWidth);
						    stageMap[i, j].setY(i * mapObjectHeight);
						    break;
					    case NatureObject.OBJ_UNKNOWN:
						    stageMap[i, j].setX(j * mapObjectWidth);
						    stageMap[i, j].setY(i * mapObjectHeight);
						    break;
					    default:
						    break;
				    }
			    }
		    }

		    return;
	    }

	    //ゲーム内部の計算フェーズ
	    public void calc() {

		    //ゲームのクリアチェック
		    if (this.checkClear()) {
			    //フラグを更新し、ゲームのクリア画面へ移動
			    gameMode = mode.STAGE_CLEAR;
			    return;
		    }

		    //ゲーム内部の更新処理
		    this.upDate();

		    return;
	    }

	    //ゲームのクリアチェック
	    private bool checkClear() {
		    //画面上にブロックが無ければ、クリアしている
		    for (int i = 0; i < stageHeight; i++) {
			    for (int j= 0; j < stageWidth; j++) {
				    if (stageMap[i, j].getNatureObject() == NatureObject.OBJ_BLOCK) {
					    return false;
				    }
			    }
		    }

		    return true;
	    }

	    //ゲームのアップデート処理
	    private void upDate() {

		    //ユーザーの入力を取得
		    byte[] tmpKey = new byte[256]; // 現在のキーの入力状態を格納する
		    DX.GetHitKeyStateAll(out tmpKey[0]); // 全てのキーの入力状態を得る

		    //スティックの移動
		    if (tmpKey[DX.KEY_INPUT_RIGHT] == 1) { //右キーが押された
			    int x = stick.getX();
			    x += stickSpeed;
			    stick.setX(x);
		    }

		    if (tmpKey[DX.KEY_INPUT_LEFT] == 1) { //左キーが押された
			    int x = stick.getX();
			    x += -stickSpeed;
			    stick.setX(x);
		    }

		    //スティックが右端に来た時
		    if (stick.getRightEnd() > locationOfWall) {
			    stick.setX(365);
		    }

		    //スティックが左端に来た時
		    if (stick.getX() < 0) {
			    stick.setX(0);
		    }

		    //ボールの移動
		    ball.move();

		    //ボールの反射（天井）
		    if (ball.getY() < 0) {
			    double angle = ball.getAngle();
			    ball.setAngle(-angle);
		    }

		    //ボールの反射（右壁）
		    if (ball.getX() + ballWidth > locationOfWall) {
			    double angle = ball.getAngle();
			    double angleOfReflection = (2 * 90) - angle; //反射角を求める
			    ball.setAngle(angleOfReflection);
		    }

		    //ボールの反射（左壁）
		    if (ball.getX() < 0) {
			    double angle = ball.getAngle();
			    double angleOfReflection = (2 * 90) - angle; //反射角を求める
			    ball.setAngle(angleOfReflection);
		    }

		    //ミスの処理。残機を減らし、ミスした際の画面へ移動
		    if (ball.getY() > 480) {
			    this.gameMode = mode.MISS;
			    life--;
			    return;
		    }

		    //ボールの反射（スティック）
		    stick.ballCollisionCheck(ref this.ball);

		    //マップ上にある全てのオブジェクトとボールの現在座標を比較し、
		    //ブロックの矩形にボールが侵入していたら、ブロックを消去する
		    for (int i= 0; i < stageHeight; i++) {
			    for (int j= 0; j < stageWidth; j++) {
				    switch(stageMap[i, j].getNatureObject()) {
					    case NatureObject.OBJ_BLOCK:
						    stageMap[i, j].ballCollisionCheck(ref this.ball);
						    break;
					    default:
						    break;
				    }
			    }
		    }

		    return;
	    }

	    //ゲーム画面の描画フェーズ
	    public void gameMainScreenDraw() {

		    //ゲームの説明等の表示
		    string stageStr = String.Format("STAGE {0}", currentStageNumber);
		    DX.DrawString(480, 50, stageStr, white);

		    string messageStr = "Please input key";
		    DX.DrawString(480, 80, messageStr, white);

		    string rightLeftStr = "←　→";
		    DX.DrawString(480, 110, rightLeftStr, white);

		    string lifeStr = String.Format("LIFE : {0}", life);
		    DX.DrawString(480, 140, lifeStr, white);

		    //壁を表示させる
		    for (int i = 0; i < 480; i++) {
			    DX.DrawGraph(locationOfWall, i, wallbuf, 1);
		    }

		    //マップの配置通りにグラフィックを描画する
		    for (int i= 0; i < stageHeight; i++) {
			    for (int j= 0; j < stageWidth; j++) {
				    switch(stageMap[i, j].getNatureObject()) {
					    case NatureObject.OBJ_BLOCK:
						    DX.DrawGraph(j * mapObjectWidth, i * mapObjectHeight, blockbuf, 1);
						    break;
					    default:
						    break;
				    }
			    }
		    }

		    //スティックとボールを描画する
		    DX.DrawGraph(stick.getX(), stick.getY(), stickbuf, 1);
		    DX.DrawGraph((int)ball.getX(), (int)ball.getY(), ballbuf, 1);

		    return;
	    }

	    //ミスした際の画面の描画フェーズ
	    public void missScreenDraw() {

		    //残機が0になったら、ゲームオーバーの処理に移る
		    if (life == 0) {
			    this.gameMode = mode.GAME_OVER;
			    return;
		    }

		    //ステージクリア画面の表示
		    string stageStr = "ｍｉｓｓ！！";
		    DX.DrawStringToHandle(29, 179, stageStr, white, fontType);

		    //Enterキー入力待ちのメッセージ
		    string enterMessageStr = "Please Press the Enter key";
		    DX.DrawString(120, 300, enterMessageStr, white);

		    //ここにEnterキーを押したら、次のステージのマップを生成し、新しいステージを始める処理を書く
		    byte[] tmpKey = new byte[256]; // 現在のキーの入力状態を格納する
		    DX.GetHitKeyStateAll(out tmpKey[0]); // 全てのキーの入力状態を得る

		    //Enterキーが押された場合
		    if (tmpKey[DX.KEY_INPUT_RETURN] == 1) {

			    //ボールとスティックを初期配置場所に配置し、ボールの角度を初期化する
			    this.ball.setX(ballInitialCoordinateX);
			    this.ball.setY(ballInitialCoordinateY);
			    this.ball.setAngle(InitialAngle);

			    this.stick.setX(stickInitialCoordinateX);
			    this.stick.setY(stickInitialCoordinateY);

			    this.gameMode = mode.MOVEINPUT;
		    }

		    return;
	    }

	    //ゲームオーバー画面の表示
	    public void gameOverScreenDraw() {

		    //ゲームオーバー画面の表示
		    string gameStr = " ＧＡＭＥ";
		    DX.DrawStringToHandle(40, 179, gameStr, white, fontType);

		    string clearStr = "ＯＶＥＲ！";
		    DX.DrawStringToHandle(40, 229, clearStr, white, fontType);

		    //Enterキー入力待ちのメッセージ
		    string enterMessageStr = "Please Press the Enter key";
		    DX.DrawString(120, 300, enterMessageStr, white);

		    //ここにEnterキーを押したら、次のステージのマップを生成し、新しいステージを始める処理を書く
		    byte[] tmpKey = new byte[256]; // 現在のキーの入力状態を格納する
		    DX.GetHitKeyStateAll(out tmpKey[0]); // 全てのキーの入力状態を得る

		    //Enterキーが押された場合
		    if (tmpKey[DX.KEY_INPUT_RETURN] == 1) {
			    life = 5; //自機の数を元に戻す
			    this.gameMode = mode.TITLE;
		    }

		    return;
	    }

	    //ステージクリア画面の描画フェーズ
	    public void stageClearScreenDraw() {

		    //クリアしたステージが規定のステージ数を超えたら、ゲームはクリアした状態になる
		    if (currentStageNumber == stageTotalNumber) {
			    this.gameMode = mode.GAME_CLEAR;
			    return;
		    }

		    //クリアしたステージ数が規定の数に達しなければ、次のステージに進む
		    if (currentStageNumber < stageTotalNumber) {

			    //ステージクリア画面の表示
			    string stageStr = " ＳＴＡＧＥ";
			    DX.DrawStringToHandle(29, 179, stageStr, white, fontType);

			    string clearStr = "ＣＬＥＡＲ！";
			    DX.DrawStringToHandle(29, 229, clearStr, white, fontType);

			    //Enterキー入力待ちのメッセージ
			    string enterMessageStr = "Please Press the Enter key";
			    DX.DrawString(120, 300, enterMessageStr, white);

			    //ここにEnterキーを押したら、次のステージのマップを生成し、新しいステージを始める処理を書く
			    byte[] tmpKey = new byte[256]; // 現在のキーの入力状態を格納する
			    DX.GetHitKeyStateAll(out tmpKey[0]); // 全てのキーの入力状態を得る

			    //Enterキーが押された場合
			    if (tmpKey[DX.KEY_INPUT_RETURN] == 1) {

				    //現在のステージを更新
				    this.currentStageNumber++;

				    //新しいマップを読み込む
				    string fileName = String.Format("gamedata\\stage{0}.txt", currentStageNumber);

				    //マップをファイルから読み込む
				    this.stageInitialize(fileName);

				    //ボールとスティックを初期配置場所に配置し、ボールの角度を初期化する
				    this.ball.setX(ballInitialCoordinateX);
				    this.ball.setY(ballInitialCoordinateY);
				    this.ball.setAngle(InitialAngle);

				    this.stick.setX(stickInitialCoordinateX);
				    this.stick.setY(stickInitialCoordinateY);

				    this.gameMode = mode.MOVEINPUT;
			    }
		    }

		    return;
	    }

	    //ステージクリア画面の描画フェーズ
	    public void gameClearScreenDraw() {

		    //ゲームクリア画面の表示
		    string gameStr = "ＧＡＭＥ";
		    DX.DrawStringToHandle(29, 179, gameStr, white, fontType);

		    string clearStr = "ＣＬＥＡＲ！";
		    DX.DrawStringToHandle(29, 229, clearStr, white, fontType);

		    return;
	    }
    }
}
