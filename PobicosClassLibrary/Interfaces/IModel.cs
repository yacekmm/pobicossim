﻿using System;
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
        void AddObserver(IPobicosView view);
        void RemoveObserver(IPobicosView view);
        DataSet Definition { get; set; }
        String Name { get; }
        String Id { get; }
        StreamWriter streamWriter { get; set; }
        StreamReader streamReader { get; set; }
        String[] resourceDescripton { get; }

    }
}
