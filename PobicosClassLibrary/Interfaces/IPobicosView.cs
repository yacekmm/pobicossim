using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;

namespace PobicosLibrary
{
    public interface IPobicosView : IView
    {
        void EventReturn(String callID, String returnValue);
        void Instruction(InstructionsList instruction,string callID, string param);
 

    }
}
