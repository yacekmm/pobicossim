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
        //  if move is possible, human will do it
        public static void Move(ref Human human, Vector3 direction)
        {
            
            human.model.Translate+= direction * human.movementSpeed;
           
        }
    }
}
