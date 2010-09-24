using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Net.Sockets;

namespace PobicosLibrary
{
    /// <summary>
    /// MVC model interface
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// registers the view in model
        /// </summary>
        /// <param name="view"></param>
        void RegisterObserver(IView view);
        /// <summary>
        /// unregisters the view in model
        /// </summary>
        /// <param name="view"></param>
        void RemoveObserver(IView view);
        /// <summary>
        /// allows to get model property by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        object GetProperty(string Name);
        /// <summary>
        /// allows to set property by value
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="value"></param>
        void SetProperty(string Name, object value);                
        /// <summary>
        /// holds definition of object
        /// </summary>
        DataSet Definition { get; set; }
        /// <summary>
        /// returns the name of the object
        /// </summary>
        String Name { get; }
        /// <summary>
        /// returns the clientID
        /// </summary>
        String ID { get; }
        /// <summary>
        /// lets to disable object
        /// </summary>
        Boolean Enabled { get; set; }
        /// <summary>
        /// link status
        /// </summary>
        LinkStatus LinkStat { get; set; }
        /// <summary>
        /// returns the Socket model is assigned to 
        /// </summary>
        Socket Socket { get; set; }


    }
}
