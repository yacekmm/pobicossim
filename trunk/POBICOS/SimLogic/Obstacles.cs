using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.IO;

namespace POBICOS.SimLogic
{
    [XmlRoot("Obstacles")]
    public class Obstacles
    {
        [XmlIgnore]
        public ArrayList Walls { get; private set; }

        private static Obstacles _instance;
        [XmlIgnore]
        public static Obstacles Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Deserialize();
                return _instance;
            }
            private set { _instance = value; }

        }

        ~Obstacles()
        {
            Serialize(this);
        }


        private Obstacles()
        {
            Walls = new ArrayList();
        }

        [XmlElement("wall")]
        public Wall[] Items
        {
            get
            {
                Wall[] items = new Wall[Walls.Count];
                Walls.CopyTo(items);
                return items;
            }
            set
            {
                if (value == null) return;
                Wall[] items = (Wall[])value;
                Walls.Clear();
                foreach (Wall item in items)
                    Walls.Add(item);
            }
        }

        public int AddWall(Wall item)
        {
            return Walls.Add(item);
        }

        private static void Serialize(Obstacles obst)
        {
            // Serialization
            XmlSerializer s = new XmlSerializer(typeof(Obstacles));
            TextWriter w = new StreamWriter(@"walls.xml");
            s.Serialize(w, obst);
            w.Close();
        }
        private  static Obstacles Deserialize()
        {
            // Deserialization
            Obstacles newList;
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(Obstacles));
                TextReader r = new StreamReader("walls.xml");
                newList = (Obstacles)s.Deserialize(r);
                r.Close();
            }
            catch (Exception)
            {
                return new Obstacles();
            }
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
