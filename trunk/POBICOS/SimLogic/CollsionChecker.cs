using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POBICOS.SimLogic;
using Microsoft.Xna.Framework;
using System.Collections;

namespace POBICOS
{
    public class CollsionChecker
    {
        private static Obstacles obstacles = Obstacles.Instance;
        private const float radius = 0.2F;
       

        public static void Initialize()
        {
            Console.WriteLine("Radius equals {0}", radius);
            obstacles = Obstacles.Instance;
            
        }

        //  if move is possible, human will do it
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
            /* #region stare 
                        bftmp.X = before.X + radius;
                        bftmp.Y = before.Y +radius;
                        aftmp.X = after.X + radius;
                        aftmp.Y = after.Y +radius;
                        beforeList.Add(bftmp, aftmp);

                        bftmp = new Vector2();
                        aftmp = new Vector2();

                        bftmp.X = before.X - radius;
                        bftmp.Y = before.Y - radius;
                        aftmp.X = after.X - radius;
                        aftmp.Y = after.Y - radius;
                        beforeList.Add(bftmp, aftmp);

                        bftmp = new Vector2();
                        aftmp = new Vector2();

                        bftmp.X = before.X +radius;
                        bftmp.Y = before.Y - radius;
                        aftmp.X = after.X + radius;
                        aftmp.Y = after.Y - radius;
                        beforeList.Add(bftmp, aftmp);

                        bftmp = new Vector2();
                        aftmp = new Vector2();

                        bftmp.X = before.X -radius;
                        bftmp.Y = before.Y + radius;
                        aftmp.X = after.X - radius;
                        aftmp.Y = after.Y + radius;
                        beforeList.Add(bftmp, aftmp);
            #endregion*/
            
            #region test innego rozwiazania
            // przod after 
            bftmp = new Vector2();
            aftmp = new Vector2();
            bftmp.X = after.X - radius;
            bftmp.Y = after.Y - radius;
            aftmp.X = after.X - radius;
            aftmp.Y = after.Y + radius;
            beforeList.Add(bftmp, aftmp);

            // przod after 
            bftmp = new Vector2();
            aftmp = new Vector2();
            bftmp.X = after.X + radius;
            bftmp.Y = after.Y + radius;
            aftmp.X = after.X + radius;
            aftmp.Y = after.Y - radius;
            beforeList.Add(bftmp, aftmp);

            //3
            if (forward)
            {
                bftmp = new Vector2();
                aftmp = new Vector2();
                bftmp.X = after.X - radius;
                bftmp.Y = after.Y + radius;
                aftmp.X = after.X + radius;
                aftmp.Y = after.Y + radius;
                beforeList.Add(bftmp, aftmp);
            }
            else
            {
                // przod after 
                bftmp = new Vector2();
                aftmp = new Vector2();
                bftmp.X = after.X + radius;
                bftmp.Y = after.Y - radius;
                aftmp.X = after.X - radius;
                aftmp.Y = after.Y - radius;
                beforeList.Add(bftmp, aftmp);
            }

            #endregion

            foreach (KeyValuePair<Vector2, Vector2> pair in beforeList)
            {
                if (CrossChecker(pair.Key.X, pair.Key.Y, pair.Value.X, pair.Value.Y))
                    return;
            }
            MakeMove(ref human,forward);

        }


        private static void MakeMove(ref Human human, bool forward)
        {
            if (forward)
            human.model.Translate += human.direction * human.movementSpeed;
            else
                human.model.Translate -= human.direction * human.movementSpeed;
        }

        // if true to sie przecinaja
        static private bool CrossChecker(double x1, double z1, double x2, double z2)
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
                    // Console.WriteLine("Przecinaja sie");
                    return crossing;
                }
            }
            return crossing;
        }
    }

}
