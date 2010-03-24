using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    public interface IController 
    {
        bool Running { get; set; }
        bool Connect();
        bool Disconnect();
        void RegisterModel(IModel model); 

    }
}
