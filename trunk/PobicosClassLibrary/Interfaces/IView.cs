using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    public interface IView
    {
        void Update(IModel model);
        IModel Model { get; set; }

    }
}
