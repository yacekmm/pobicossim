using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    public interface IInstrEvents
    {
        void Instruction(IPobicosModel sender, InstructionsList instruction,String callID, String parameters);
        void Event(IPobicosView sender, EventsList evnt,String callID, String parameters);
        void EventReturn(IPobicosView sender,String callID, String value);
        void InstructionReturn(IPobicosModel sender,String callID, String value);
    }
}
