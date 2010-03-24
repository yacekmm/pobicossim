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
            AdminTools.prepareLog();
            Main main = new Main();
            Console.WriteLine(InstructionsList.pongiAlert);
            Console.ReadLine();
        }
    }
}
