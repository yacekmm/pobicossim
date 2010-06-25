using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PobicosLibrary
{
    /// <summary>
    /// Interface with event and instruction methods
    /// </summary>
    public interface IInstrEvents
    {
        /// <summary>
        /// implements instruction on class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="instruction"></param>
        /// <param name="callID"></param>
        /// <param name="parameters"></param>
        void Instruction(IPobicosModel sender, InstructionsList instruction,String callID, String parameters);
        /// <summary>
        /// implements event on class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="evnt"></param>
        /// <param name="callID"></param>
        /// <param name="parameters"></param>
        void Event(IPobicosView sender, EventsList evnt,String callID, String parameters);
        /// <summary>
        /// implements event return on class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="callID"></param>
        /// <param name="value"></param>
        void EventReturn(IPobicosView sender,String callID, String value);
        /// <summary>
        /// implements  instruction return on class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="callID"></param>
        /// <param name="value"></param>
        void InstructionReturn(IPobicosModel sender,String callID, String value);
    }
}
