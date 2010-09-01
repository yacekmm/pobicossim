using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POBICOS.SimLogic;
using Microsoft.Xna.Framework;
using System.Collections;
using POBICOS.SimLogic.Scenarios;

namespace POBICOS.SimBase
{
    /// <summary>
    /// Class handles collision detection in 3D world.
    /// </summary>
    public class CollsionChecker
    {
        private static Obstacles obstacles = Obstacles.Instance;
        //private const float radius = 0.2F;
        private static float radiusX, radiusY;


        /// <summary>
        /// Method initializes all obstacles in simulator
        /// </summary>
        public static void Initialize()
        {
            //Console.WriteLine("Radius equals {0}", radius);
            obstacles = Obstacles.Instance;
            Human tmp = SimScenario.Instance.GetActiveHuman();
            radiusY = (tmp.model.BoundingBox.Max.Z - tmp.model.BoundingBox.Min.Z) / 2;
            radiusX = (tmp.model.BoundingBox.Max.X - tmp.model.BoundingBox.Min.X) / 2;

            foreach (SimObject obj in SimScenario.pobicosObjectList)
            {
                if (obj.model.Transformation.Translate.Y < 0.69)
                {
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Min.Z, obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Max.Z));
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Min.Z, obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Min.Z));
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Max.Z, obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Max.Z));
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Min.Z, obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Max.Z));
                    // Console.WriteLine("Obiekt: {0}; Max: {1}, Min: {2} .", obj.name, obj.model.BoundingBox.Max, obj.model.BoundingBox.Min);
                }

            }
            foreach (SimObject obj in SimScenario.furnitureList)
            {
                if (obj.model.Transformation.Translate.Y < 0.69)
                {
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Min.Z, obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Max.Z));
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Min.Z, obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Min.Z));
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Min.X, obj.model.BoundingBox.Max.Z, obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Max.Z));
                    obstacles.AddWall(new Wall(obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Min.Z, obj.model.BoundingBox.Max.X, obj.model.BoundingBox.Max.Z));
                    //    Console.WriteLine("Obiekt: {0}; Max: {1}, Min: {2} .", obj.name, obj.model.BoundingBox.Max, obj.model.BoundingBox.Min);
                }

            }

        }

        /// <summary>
        /// Moves human if move is possible
        /// </summary>
        /// <param name="human"></param>
        /// <param name="forward"></param>  
        public static void Move(ref Human human, bool forward)
        {
            Vector3 after3;
            if (forward)
                after3 = human.model.Translate + human.direction * human.movementSpeed;
            else
                after3 = human.model.Translate - human.direction * human.movementSpeed;

            //Vector2 before = (new Vector2(human.model.Translate.X, human.model.Translate.Z));
            Vector2 after = new Vector2(after3.X, after3.Z);
            Dictionary<Vector2, Vector2> beforeList = new Dictionary<Vector2, Vector2>();


            Vector2 bftmp = new Vector2();
            Vector2 aftmp = new Vector2();   

            #region test innego rozwiazania
            // przod after 
            bftmp = new Vector2();
            aftmp = new Vector2();
            bftmp.X = after.X - radiusX;
            bftmp.Y = after.Y - radiusY;
            aftmp.X = after.X - radiusX;
            aftmp.Y = after.Y + radiusY;
            beforeList.Add(bftmp, aftmp);

            // przod after 
            bftmp = new Vector2();
            aftmp = new Vector2();
            bftmp.X = after.X + radiusX;
            bftmp.Y = after.Y + radiusY;
            aftmp.X = after.X + radiusX;
            aftmp.Y = after.Y - radiusY;
            beforeList.Add(bftmp, aftmp);

            //3
            if (forward)
            {
                bftmp = new Vector2();
                aftmp = new Vector2();
                bftmp.X = after.X - radiusX;
                bftmp.Y = after.Y + radiusY;
                aftmp.X = after.X + radiusX;
                aftmp.Y = after.Y + radiusY;
                beforeList.Add(bftmp, aftmp);
            }
            else
            {
                // przod after 
                bftmp = new Vector2();
                aftmp = new Vector2();
                bftmp.X = after.X + radiusX;
                bftmp.Y = after.Y - radiusY;
                aftmp.X = after.X - radiusX;
                aftmp.Y = after.Y - radiusY;
                beforeList.Add(bftmp, aftmp);
            }

            #endregion

            foreach (KeyValuePair<Vector2, Vector2> pair in beforeList)
            {
                Wall tmp = CrossChecker(pair.Key.X, pair.Key.Y, pair.Value.X, pair.Value.Y);
                if (null != tmp)
                {
                    slidingMove(ref human, forward, tmp);
                    return;
                }
            }
            MakeMove(ref human, forward);

        }

        private static void slidingMove(ref Human human, bool forward, Wall obstacle)
        {
            Vector2 wall = new Vector2((float)(obstacle.x2 - obstacle.x1), (float)(obstacle.z2 - obstacle.z1));
            wall.Normalize();            
            Vector3 wallDirection = new Vector3(Math.Abs(wall.X), 0, Math.Abs(wall.Y));           
            Vector3 tmp = human.direction * wallDirection;

            //TODO nie sprawdza czy nie koliduje z niczym
            if (forward)
                tmp = human.model.Translate + tmp * human.movementSpeed;
            else
                tmp = human.model.Translate - tmp * human.movementSpeed;



            Vector2 after = new Vector2(tmp.X, tmp.Z);
            Dictionary<Vector2, Vector2> beforeList = new Dictionary<Vector2, Vector2>();


            Vector2 bftmp = new Vector2();
            Vector2 aftmp = new Vector2();

            #region test innego rozwiazania
            // przod after 
            bftmp = new Vector2();
            aftmp = new Vector2();
            bftmp.X = after.X - radiusX;
            bftmp.Y = after.Y - radiusY;
            aftmp.X = after.X - radiusX;
            aftmp.Y = after.Y + radiusY;
            beforeList.Add(bftmp, aftmp);

            // przod after 
            bftmp = new Vector2();
            aftmp = new Vector2();
            bftmp.X = after.X + radiusX;
            bftmp.Y = after.Y + radiusY;
            aftmp.X = after.X + radiusX;
            aftmp.Y = after.Y - radiusY;
            beforeList.Add(bftmp, aftmp);

            //3
            if (forward)
            {
                bftmp = new Vector2();
                aftmp = new Vector2();
                bftmp.X = after.X - radiusX;
                bftmp.Y = after.Y + radiusY;
                aftmp.X = after.X + radiusX;
                aftmp.Y = after.Y + radiusY;
                beforeList.Add(bftmp, aftmp);
            }
            else
            {
                // przod after 
                bftmp = new Vector2();
                aftmp = new Vector2();
                bftmp.X = after.X + radiusX;
                bftmp.Y = after.Y - radiusY;
                aftmp.X = after.X - radiusX;
                aftmp.Y = after.Y - radiusY;
                beforeList.Add(bftmp, aftmp);
            }

            #endregion

            foreach (KeyValuePair<Vector2, Vector2> pair in beforeList)
            {
                Wall tmpe = CrossChecker(pair.Key.X, pair.Key.Y, pair.Value.X, pair.Value.Y);
                if (null != tmpe)
                {
                    return;
                }
            }
            human.model.Translate = tmp;


        }


        private static void MakeMove(ref Human human, bool forward)
        {
            
            if (forward)
                human.model.Translate += human.direction * human.movementSpeed;
            else
                human.model.Translate -= human.direction * human.movementSpeed;
        }

        // if true to sie przecinaja
        static private Wall CrossChecker(double x1, double z1, double x2, double z2)
        {
            double Bh, Ch, Ah = 1, As, Bs, Cs;
            bool crossing = false;
            if (x1 == x2)
            {
                Ah = 0;
                Bh = -1;
                Ch = x1;

            }
            else
            {
                Bh = -(z2 - z1) / (x2 - x1);
                Ch = -z1 - Bh * x1;
            }
            // Console.WriteLine("rownanie prostej ludka to 0 =  {0} y +  {1} x + {2}", Ah, Bh, Ch);

            foreach (Wall wall in obstacles.Walls)
            {
                if (wall.x1 == wall.x2)
                {
                    Cs = wall.x1;
                    Bs = -1;
                    As = 0;
                }
                else
                {
                    Bs = 0;
                    Cs = -wall.z1;
                    As = 1;
                }
                // Console.WriteLine("rownanie prostej sciany  to 0 =  {0} y + {1} x + {2}", As,Bs, Cs);

                crossing = true;
                //Console.WriteLine((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs));
                // Console.WriteLine((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch));
                if ((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs) >= 0)
                {
                    // Console.WriteLine("Tutaj");
                    //    Console.WriteLine((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs));
                    crossing = false;
                }
                if ((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch) >= 0)
                {
                    // Console.WriteLine("Tutaj");
                    //  Console.WriteLine((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch));
                    crossing = false;
                }
                if (crossing)
                {
                    double angleCos = (Ah * As + Bh * Bs) / (Math.Sqrt(Math.Pow(Ah, 2) + Math.Pow(Bh, 2)) * Math.Sqrt(Math.Pow(As, 2) + Math.Pow(Bs, 2)));

                    //  Console.WriteLine("Przecinaja sie pod  kątem {0} ", angleCos);
                    return wall;
                }
            }
            return null;
        }
    }

}
