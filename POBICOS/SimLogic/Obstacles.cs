using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.IO;

namespace POBICOS.SimLogic
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("Obstacles")]
    public class Obstacles
    {
        /// <summary>
        /// Set of all obstacles in 3D world
        /// </summary>
        [XmlIgnore]
        public ArrayList Walls { get; private set; }

        private static Obstacles _instance;
        /// <summary>
        /// Helper object. Implements singleton pattern
        /// </summary>
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



        private Obstacles()
        {
            Walls = new ArrayList();
        }
        /// <summary>
        /// Helper Array. Helps with reading from and writing to file informations about wall localizations
        /// </summary>
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
        /// <summary>
        /// Adds wall to the set of obstacles in 3D world
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int AddWall(Wall item)
        {
            return Walls.Add(item);
        }

        private static void Serialize(Obstacles obst)
        {
            // Serialization
            XmlSerializer s = new XmlSerializer(typeof(Obstacles));
            TextWriter w = new StreamWriter(@"Configurations\walls.xml");
            s.Serialize(w, obst);
            w.Close();
        }
        private static Obstacles Deserialize()
        {
            // Deserialization
            Obstacles newList;
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(Obstacles));
                TextReader r = new StreamReader(@"Configurations\walls.xml");
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

/// <summary>
/// Class holds info about single wall in 3D world
/// </summary>
public class Wall
{
    /// <summary>
    /// Wall parameter
    /// </summary>
    [XmlAttribute("x1")]
    public double x1;
    /// <summary>
    /// Wall parameter
    /// </summary>
    [XmlAttribute("z1")]
    public double z1;
 
    /// <summary>
    /// Wall parameter
    /// </summary>
    [XmlAttribute("x2")]
    public double x2;
    /// <summary>
    /// Wall parameter
    /// </summary>
    [XmlAttribute("z2")]
    public double z2;
    /// <summary>
    /// Default constructor
    /// </summary>
    public Wall()
    {
    }
    /// <summary>
    /// Constructor taking coordintes of the beginning and the end of the wall
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="z1"></param>
    /// <param name="x2"></param>
    /// <param name="z2"></param>
    public Wall(double x1, double z1, double x2, double z2)
    {
        this.x1 = x1;
        this.x2 = x2;
        this.z1 = z1;
        this.z2 = z2;

    }
}
