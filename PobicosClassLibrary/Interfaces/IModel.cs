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
    /// Model interface in MVC pattern
    /// </summary>
    public interface IModel
    {

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
        /// lets to disable object
        /// </summary>
        Boolean Enabled { get; set; }
        /// <summary>
        /// link status
        /// </summary>
        LinkStatus LinkStat { get; set; }
        /// <summary>
        /// gets list of instructions and events
        /// </summary>
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
        String ClientID { get; }
        /// <summary>
        /// returns the Socket model is assigned to 
        /// </summary>
        Socket Socket { get; set; }


    }
}
