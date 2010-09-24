using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    /// <summary>
    /// MVC View interface
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// updates view according to model
        /// </summary>
        /// <param name="model"></param>
        void Update(IModel model);
        /// <summary>
        /// holds model for this MVC view
        /// </summary>
        IModel Model { get; set; }

    }
}
