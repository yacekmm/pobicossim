using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PobicosLibrary;

namespace PobicosNGNConsole
{
    class Program
    {   
        static void Main(string[] args)
        {
           // AdminTools.prepareLog();
            Main main = new Main();
            Console.WriteLine(InstructionsList.Alert);
            ProcessStartInfo pss = new ProcessStartInfo();

            foreach (string s in pss.Verbs)
                Console.WriteLine(s);
            Console.ReadLine();
        }
    }
}
