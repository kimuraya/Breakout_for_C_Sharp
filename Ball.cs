using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breakout_for_C_Sharp
{
    class Ball
    {
            double x; //ボールの座標
	        double y; //ボールの座標
	        const int ballHeight = 15; //ボールの高さ
	        const int ballWidth = 15; //ボールの横幅
	        double speed; //ボールのスピード
	        double angle; //ボールの角度

            public Ball()
            {
		        //ボールのスピード、角度を設定
		        this.speed = 4;
	        }

            public double getX()
            {
		        return this.x;
	        }

            public void setX(double x)
            {
		        this.x = x;
	        }

            public double getY()
            {
		        return this.y;
	        }

            public void setY(double y)
            {
		        this.y = y;
	        }

            public int getBallHeight()
            {
                return Ball.ballHeight;
	        }

            public int getBallWidth()
            {
                return Ball.ballWidth;
	        }

            public double getSpeed()
            {
		        return this.speed;
	        }

            public void setSpeed(double speed)
            {
		        this.speed = speed;
	        }

            public double getAngle()
            {
		        return this.angle;
	        }

            public void setAngle(double angle)
            {
		        this.angle = angle;
	        }

	        //1フレーム毎のボールの移動
            public void move()
            {

		        //ラジアンを求める
                double radian = (angle / 360) * (Math.PI * 2);

                double x = speed * Math.Cos(radian);
                double y = -(speed * Math.Sin(radian));

		        this.x += x;
		        this.y += y;

		        return;
	        }
    }
}
