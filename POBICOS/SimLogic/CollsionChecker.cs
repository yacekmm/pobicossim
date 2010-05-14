using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POBICOS.SimLogic;
using Microsoft.Xna.Framework;

namespace POBICOS
{
    public class CollsionChecker
    {
        private static Obstacles obstacles = Obstacles.Instance;
        
        //  if move is possible, human will do it
        public static void Move(ref Human human, Vector3 direction)
        {

            Vector3 after3 = human.model.Translate + direction * human.movementSpeed;
            Vector2 before = new Vector2(human.model.Translate.X, human.model.Translate.Z);
            Vector2 after = new Vector2(after3.X, after3.Z);
            if (!crossChecker(before.X, before.Y, after.X, after.Y))
                MakeMove(ref human, direction);
           
        }


        private static void MakeMove(ref Human human, Vector3 direction)
        {
            human.model.Translate += direction * human.movementSpeed;
        }

        // if true to sie przecinaja
        static private bool crossChecker(double x1, double z1, double x2, double z2)
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
                //   Console.WriteLine("rownanie prostej sciany  to 0 =  {0} y + {1} x + {2}", As,Bs, Cs);

                crossing = true;
                if ((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs) > 0)
                {
                    // Console.WriteLine("Tutaj");
                    //Console.WriteLine((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs));
                    crossing = false;
                }
                if ((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch) > 0)
                {
                    // Console.WriteLine("Tutaj");
                    // Console.WriteLine((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch));
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
