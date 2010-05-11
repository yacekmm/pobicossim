using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.IO;

namespace PobicosNGNConsole
{
    [XmlRoot("Obstacles")]
    public class Obstacles
    {
        private ArrayList walls;

        public Obstacles()
        {
            walls = new ArrayList();
        }

        [XmlElement("wall")]
        public Wall[] Items
        {
            get
            {
                Wall[] items = new Wall[walls.Count];
                walls.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                Wall[] items = (Wall[])value;
                walls.Clear();
                foreach (Wall item in items)
                    walls.Add(item);
            }
        }

        public int AddWall(Wall item)
        {
            return walls.Add(item);
        }

        public static void serialize(Obstacles obst)
        {
            // Serialization
            XmlSerializer s = new XmlSerializer(typeof(Obstacles));
            TextWriter w = new StreamWriter(@"walls.xml");
            s.Serialize(w, obst);
            w.Close();
        }
        public static Obstacles deserialize()
        {
            // Deserialization
            Obstacles newList;
            XmlSerializer s = new XmlSerializer(typeof(Obstacles));
            TextReader r = new StreamReader("walls.xml");
            newList = (Obstacles)s.Deserialize(r);
            r.Close();
            return newList;
        }
    }
}

// Walls
public class Wall
{
    [XmlAttribute("x1")]
    public double x1;
    [XmlAttribute("z1")]
    public double z1;
    [XmlAttribute("x2")]
    public double x2;
    [XmlAttribute("z2")]
    public double z2;

    public Wall()
    {
    }

    public Wall(double x1, double z1, double x2, double z2)
    {
        this.x1 = x1;
        this.x2 = x2;
        this.z1 = z1;
        this.z2 = z2;

    }
}
