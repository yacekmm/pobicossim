using System;
using System.Data;
using PobicosLibrary;
using System.IO;

namespace PobicosLibrary
{
    /// <summary>
    /// Pobicos model interface
    /// </summary>
    public interface IPobicosModel : IModel
    {
        /// <summary>
        /// returns list of intructions and methods
        /// </summary>
        String[] ResourceDescripton { get; }

        /// <summary>
        /// executes Instruction in model
        /// </summary>
        /// <param name="instructionLabel"></param>
        /// <param name="callID"></param>
        /// <param name="param"></param>   
        void Instruction(String instructionLabel, String callID, String param);
        /// <summary>
        /// executes event return in model
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="returnValue"></param>
        void EventReturn(String callID, String returnValue);
        /// <summary>
        /// executes instruction return in model
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="returnValue"></param>
        void InstructionReturn(String callID, String returnValue);

    }
}
