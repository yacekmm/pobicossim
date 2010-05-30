using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Net.Sockets;

namespace PobicosLibrary
{
    public interface IModel
    {
        object GetProperty(string Name);
        void SetProperty(string Name, object value);
        void AddObserver(IView view);
        void RemoveObserver(IView view);
        DataSet Definition { get; set; }
        String Name { get; }
        String ClientID { get; }
        Socket Socket { get; set; }
        LinkStatus LinkStat { get; set; }
        String[] ResourceDescripton { get; }
        Boolean Enabled { get; set; }

    }
}
