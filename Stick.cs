using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;

namespace Breakout_for_C_Sharp

{
    class Stick
    {
        int x; //スティックのx軸
	    int y; //スティックのy軸
	    int rightEnd;//スティックの右端
	    const int stickHeight = 15; //スティックの高さ
	    const int stickWidth = 75; //スティックの横幅

        public Stick()
        {
	    }

        public int getX()
        {
		    return this.x;
	    }

        public void setX(int x)
        {
		    this.x = x;
	    }

        public int getY()
        {
		    return this.y;
	    }

        public void setY(int y)
        {
		    this.y = y;
	    }

        public int getRightEnd()
        {
		    return this.x + stickWidth;
	    }

	    //ボールがスティックに衝突しているかをチェックする
        public void ballCollisionCheck(ref Ball ball)
        {

		    //ボールの座標がスティックの矩形に重なっていないかをチェックする

		    //ボールのX座標がオブジェクトの矩形より大きく、ボールのX座標が矩形のX座標の幅よりも小さい場合、
		    //または矩形のX座標がボールのX座標よりも大きく、かつ、矩形のX座標よりもボールのX座標がボールの横幅よりも大きい場合、
		    //かつ、ボールのY座標が矩形のY座標よりも大きく、ボールのY座標が矩形のY座標の高さよりも大きい場合、
		    //または矩形のY座標がボールのY座標よりも大きく、さらに矩形のY座標よりもボールのY座標がボールの縦幅よりも大きい場合、
            if (((ball.getX() > this.x) && (ball.getX() < (this.x + Stick.stickWidth)) ||
			     (this.x > ball.getX()) && (this.x < (ball.getX() + ball.getBallWidth()))) &&
                ((ball.getY() > this.y) && (ball.getY() < (this.y + Stick.stickHeight)) ||
			     (this.y > ball.getY()) && (this.y < (ball.getY() + ball.getBallHeight()))))
		    {
			    //スティックの矩形と座標が重なっていたら、ボールを反射させる
			    double angle = ball.getAngle();

			    if (DX.GetRand(10) > 5) {
				    angle -=  DX.GetRand(10);
			    } else {
				    angle +=  DX.GetRand(10);
			    }

			    ball.setAngle(-angle);
		    }

		    return;
	    }
    }
}
