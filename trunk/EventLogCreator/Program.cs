using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;

namespace EventLogCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Preparing log for application");
                AdminTools.prepareLog();
                Console.WriteLine("Operation suceeded");
                Console.ReadLine();
            }
            catch (Exception)
            {
                Console.WriteLine("This program needs to be launched with admin privileges to prepare log");
                Console.ReadLine();
            }
        }
    }
}
