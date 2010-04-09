using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;

namespace DawnLampMW
{
    class Program
    {
        static MW mw = new MW();
        static void Main(string[] args)
        {
           if (mw.load() && mw.connect())
            {
                mw.startWorking();
            }
            Console.WriteLine("Naciśnij x aby zakonczyć pracę MW");
            while (true)
            {
                if (Console.ReadLine().Equals("x"))
                {
                    mw.stopWorking();
                    break;
                }
            }
            Console.WriteLine("Zakończono pracę MW, naciśnij dowolny klawisz, aby wyjść");
            Console.ReadLine();
            

            
        }


            }
}
