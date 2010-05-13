using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PobicosLibrary;

namespace PobicosNGNConsole
{
    class Main
    {

        static  private bool crossChecker(double x1,double z1,double x2,double z2)
        {
            double Bh, Ch,Ah=1, As, Bs, Cs;
            bool crossing = false;
            if (x1 == x2)
            {
                Ah = 0;
                Bh = -1;
                Ch = x1;

            }
            else
            {
                Bh = -(z2 - z1) / (x2 - x1);
                Ch = -z1 - Bh * x1;
            }            
          //  Console.WriteLine("rownanie prostej ludka to 0 =  {0} y +  {1} x + {2}", Ah, Bh, Ch);
            Wall wall = new Wall(-5, -8, 6, 2);
            if (wall.x1 == wall.x2)
            {
                Cs = wall.x1;
                Bs = -1;
                As = 0;
            }
            else
            {
                Bs = 0;
                Cs = - wall.z1;
                As = 1;
            }
         //   Console.WriteLine("rownanie prostej sciany  to 0 =  {0} y + {1} x + {2}", As,Bs, Cs);

            crossing = true;
            if ((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs) > 0)
            {
               // Console.WriteLine("Tutaj");
                //Console.WriteLine((Bs * x1 + As * z1 + Cs) * (Bs * x2 + As * z2 + Cs));
                crossing = false;
            }
            if ((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch) > 0)
            {
               // Console.WriteLine("Tutaj");
               // Console.WriteLine((Bh * wall.x1 + Ah * wall.z1 + Ch) * (Bh * wall.x2 + Ah * wall.z2 + Ch));
                crossing = false;
            }
            return crossing;
        }

        public Main()
        {

       //     Pozycja -1.370442;0,3;-7,842413 
//Pozycja -1,370442;0;-8,022414

            double x1 = -1.370442, z1 = -7.842413, x2 = -1.370442, z2 = -8.022414;
            if (crossChecker(x1, z1, x2, z2))
                Console.WriteLine("Przecinaja sie");
            else
                Console.WriteLine("Nie przecinaja sie");
           /* double a, b,csciany, asciany, bsciany;
           
            bool crossing = false;
            if (x1 == x2)
            {
                a = -0;
                b = -1;
                
            }
            else
            {
                a = -(z2 - z1) / (x2 - x1);
                b = -z1 - a * x1;
            }
            Console.WriteLine("rownanie prostej to 0 =  y +  {0} x + {1}", a, b);
            Wall wall = new Wall(6, -10, 6, 10);
            if (wall.x1 == wall.x2)
            {
                bsciany = wall.x1;
                asciany = -1;
                csciany = 0;
            }
            else
            {
                asciany = -1;
                bsciany = -0;
                csciany = 1;
            }
            Console.WriteLine("rownanie prostej sciany  to 0 =  {2} y + {0} x + {1}", asciany, bsciany,csciany);

            crossing = true;
            if ((asciany * x1 + csciany * z1 + bsciany) * (asciany * x2 + csciany * z2 + bsciany) > 0)
            {
                Console.WriteLine("Tutaj");
                Console.WriteLine((asciany * x1 + 1 * z1 + bsciany) * (asciany * x2 + 1 * z2 + bsciany));
                crossing = false;
            }
            if ((a * wall.x1 + 1 * wall.z1 + b) * (a * wall.x2 + 1 * wall.z2 + b) > 0)
            {
                Console.WriteLine("Tutaj");
                Console.WriteLine((a * wall.x1 + 1 * wall.z1 + b) * (a * wall.x2 + 1 * wall.z2 + b));
                crossing = false;
            }
            if (crossing) Console.WriteLine(" Przecinają sie");
            */
            Console.ReadLine();


















            // PobicosClassLibrary.Env env = AdminTools.deserialize("list.xml");
            //  Client client = new Client();
            //  Console.WriteLine(Environment.CurrentDirectory);

            //client.CommandReceived += new Client.CommandReceivedEventHandler(client_commandReceived);
            //  client.Connect();



        }

        void client_commandReceived(object sender, CommandArgs args)
        {
            AdminTools.eventLog.WriteEntry("Otrzymano komendę: " + args.Command, EventLogEntryType.Information);
        }



        void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            Console.WriteLine(e.Entry.EntryType.ToString() + " : " + e.Entry.Message);
        }


    }
}
