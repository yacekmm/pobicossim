using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    /// <summary>
    ///  MVC controller Interface 
    /// </summary>
    public interface IController 
    {
        /// <summary>
        /// is running
        /// </summary>
        bool Running { get;  }
        /// <summary>
        /// connect method
        /// </summary>
        /// <returns></returns>
        bool Connect();
        /// <summary>
        /// disconnect method
        /// </summary>
        /// <returns></returns>
		bool Disconnect();
        /// <summary>
        /// registers model in controller
        /// </summary>
        /// <param name="model"></param>
        void RegisterModel(IModel model); 
    }
}
