using System;
using System.Data;
using PobicosLibrary;
using System.IO;

namespace PobicosLibrary
{
    public interface IPobicosModel: IModel
    {
        DataTable ResultTable { get; }
        DataTable EventTable { get; }
        
        void Instruction(String instructionLabel,String callID, String param);
        void EventReturn(String callID, String returnValue);
        void InstructionReturn(String callID,String returnValue);

    }
}
