using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

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
        StreamWriter streamWriter { get; set; }
        StreamReader streamReader { get; set; }
        String[] ResourceDescripton { get; }

    }
}
