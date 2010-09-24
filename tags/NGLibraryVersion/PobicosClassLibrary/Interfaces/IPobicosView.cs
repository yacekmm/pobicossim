using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;

namespace PobicosLibrary
{
    /// <summary>
    /// Pobicos view interface
    /// </summary>
    public interface IPobicosView : IView
    {
        /// <summary>
        /// executes event return on class
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="returnValue"></param>
        void EventReturn(String callID, String returnValue);
        /// <summary>
        /// executes instruction on class
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="callID"></param>
        /// <param name="param"></param>
        void Instruction(String instruction,string callID, string param);
 

    }
}
