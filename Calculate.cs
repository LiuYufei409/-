using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 用五点光滑法进行曲线拟合
{
    class Calculate
    {
        public static double GetDistance(Point a, Point b)
        {
            double d = Math.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
            return d;
        }

        public static List<Point> Supply_Points(List<Point> orgPoints, bool is_closed)
        {
            List<Point> tempPoints = new List<Point>();
            orgPoints.ForEach(o => tempPoints.Add(o));

            if (is_closed)
            {
                int length = tempPoints.Count;
                tempPoints.Insert(0, tempPoints[length - 1]);
                tempPoints.Insert(0, tempPoints[length - 1]);
                tempPoints.Add(tempPoints[2]);
                tempPoints.Add(tempPoints[3]);
                tempPoints.Add(tempPoints[4]);
            }

            else
            {
                Point a = new Point();
                Point b = new Point();
                Point p1 = tempPoints[0];
                Point p2 = tempPoints[1];
                Point p3 = tempPoints[2];

                a.x = p3.x - 3 * p2.x + 3 * p1.x;
                a.y = p3.y - 3 * p2.y + 3 * p1.y;
                b.x = p2.x - 3 * p1.x + 3 * a.x;
                b.y = p2.y - 3 * p1.y + 3 * a.y;
                tempPoints.Insert(0, a);
                tempPoints.Insert(0, b);

                int length = tempPoints.Count;
                Point c = new Point();
                Point d = new Point();
                Point pn = tempPoints[length - 1];
                Point pn_1 = tempPoints[length - 2];
                Point pn_2 = tempPoints[length - 3];

                c.x = pn_2.x - 3 * pn_1.x + 3 * pn.x;
                c.y = pn_2.y - 3 * pn_1.y + 3 * pn.y;
                d.x = pn_1.x - 3 * pn.x + 3 * c.x;
                d.y = pn_1.y - 3 * pn.y + 3 * c.y;
                tempPoints.Add(c);
                tempPoints.Add(d);

            }
            return tempPoints;
        }
        public static void GetSinCos(Point p1, Point p2, Point p3,Point p4,Point p5,ref double Sin,ref double Cos)
        {
            double a0, a1, a2, a3, a4, b0, b1, b2, b3, b4, w2, w3;

            a1 = p2.x - p1.x;
            a2 = p3.x - p2.x;
            a3 = p4.x - p3.x;
            a4 = p5.x - p4.x;
            b1 = p2.y - p1.y;
            b2 = p3.y - p2.y;
            b3 = p4.y - p3.y;
            b4 = p5.y - p4.y;
            w2 = Math.Abs(a3 * b4 - a4 * b3);
            w3 = Math.Abs(a1 * b2 - a2 * b1);
            a0 = w2 * a2 + w3 * a3;
            b0 = w2 * b2 + w3 * b3;

            Cos = a0 / Math.Sqrt(a0 * a0 + b0 * b0);
            Sin = b0 / Math.Sqrt(a0 * a0 + b0 * b0);
        }

        public static List<Curve> Built(List<Point> orgPoints, bool is_closed)
        {
            List<Curve> answer = new List<Curve>();
            List<Point> Supply = Supply_Points(orgPoints, is_closed);

            for(int i=0;i<Supply.Count-5;i++)
            {
                double cos0 = 0, sin0 = 0, cos1 = 0, sin1 = 0;
                Curve curve = new Curve();
                double r = GetDistance(Supply[i+2], Supply[i+3]);

                GetSinCos(Supply[i], Supply[i+1], Supply[i+2], Supply[i+3], Supply[i+3], ref sin0, ref cos0);
                GetSinCos(Supply[i+1], Supply[i + 2], Supply[i + 3], Supply[i + 4], Supply[i + 5], ref sin1, ref cos1);

                curve.E0 = Supply[i + 2].x;
                curve.E1 = r*cos0; 
                curve.E2=3*(Supply[i+3].x-Supply[i+2].x)-r*(cos1+2*cos0);
                curve.E3 = -2 * (Supply[i + 3].x - Supply[i + 2].x) + r * (cos1 + cos0);
                curve.F0 = Supply[i + 2].y;
                curve.F1 = r * sin0;
                curve.F2 = 3 * (Supply[i + 3].y - Supply[i + 2].y) - r * (sin1 + 2 * sin0);
                curve.F3 = -2 * (Supply[i + 3].y - Supply[i + 2].y) + r * (sin1 + sin0);

                curve.start = Supply[i + 2];
                curve.end = Supply[i + 3];
                answer.Add(curve);

            }

            return answer;
        }
    }

}  