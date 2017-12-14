using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFPX2CORoute
{
    class Program
    {
        static private string jar320Location;
        static private string ff757Location;
        static private string ff767Location;
        static private string ixeg737Location;
        static private string ff320Location;
        static private ArrayList files = new ArrayList();
        static private string file;
        static private string output;
        static private string ffoutput;
        static private string dep;
        static private string arr;

        static private void FindFiles(string dir) {
            DirectoryInfo d = new DirectoryInfo(dir);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.flp"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                files.Add(file.Name);
            }
        }

        static private void SelectFile(){
            Console.WriteLine("Select an option:");
            int i = 1;
            foreach (string s in files)
            {
                Console.WriteLine(i + ". " + s);
                i++;
            }
            int selection = Int32.Parse(Console.ReadLine());
            file = (string)files[selection-1];
        }

        static void JarConvert(string dir){
            string[] lines = System.IO.File.ReadAllLines(dir + @"\" + file);
            dep = lines[1].Substring(lines[1].Length - 4);
            arr = lines[2].Substring(lines[2].Length - 4);

            StringComparison comp = StringComparison.Ordinal;

            string From = "";
            string To = "";
            string Airway = "";

            output = dep + " SID";

            int i = 1;
            foreach (string s in lines)
            {
                if (s.IndexOf("Airway" + i, comp) != -1)
                {

                    if (s.IndexOf("FROM", comp) != -1)
                    {
                        From = s.Substring(s.IndexOf("=", comp) + 1);
                    }
                    else if (s.IndexOf("TO", comp) != -1)
                    {
                        To = s.Substring(s.IndexOf("=", comp) + 1);
                    }
                    else
                    {
                        Airway = s.Substring(s.IndexOf("=", comp) + 1);
                    }

                    if (From != "" && To != "" && Airway != "") {
                        if (i == 1)
                            output = output + " " + From + " " + Airway + " " + To;
                        else
                            output = output + " " + Airway + " " + To;

                        From =""; To=""; Airway = "";
                        i++;
                    }
                }
                if (s.IndexOf("DctWpt" + i + "=", comp) != -1)
                {
                    if (i == 1)
                        output = output + s.Substring(s.IndexOf("=", comp) + 1);
                    else
                        output = output + " DCT " + s.Substring(s.IndexOf("=", comp) + 1);
                    i++;
                }

            }

            output = output + " STAR " + arr;
            Console.WriteLine("----------------------------");
            Console.WriteLine("Route:");
            Console.WriteLine(output);
        }

        static void FF320Convert(string dir)
        {
            string[] lines = System.IO.File.ReadAllLines(dir + @"\" + file);
            dep = lines[1].Substring(lines[1].Length - 4);
            arr = lines[2].Substring(lines[2].Length - 4);

            StringComparison comp = StringComparison.Ordinal;

            string From = "";
            string To = "";
            string Airway = "";

            ffoutput = "RTE " + dep + arr + " " + dep;

            Console.WriteLine("\n" + ffoutput);

            int i = 1;
            foreach (string s in lines)
            {
                if (s.IndexOf("Airway" + i, comp) != -1)
                {

                    if (s.IndexOf("FROM", comp) != -1)
                    {
                        From = s.Substring(s.IndexOf("=", comp) + 1);
                    }
                    else if (s.IndexOf("TO", comp) != -1)
                    {
                        To = s.Substring(s.IndexOf("=", comp) + 1);
                    }
                    else
                    {
                        Airway = s.Substring(s.IndexOf("=", comp) + 1);
                    }

                    if (From != "" && To != "" && Airway != "")
                    {
                        if (i == 1)
                            ffoutput = ffoutput + " " + From + " " + Airway + " " + To;
                        else
                            ffoutput = ffoutput + " " + Airway + " " + To;

                        From = ""; To = ""; Airway = "";
                        i++;
                    }
                }
                if (s.IndexOf("DctWpt" + i + "=", comp) != -1)
                {
                    if (i == 1)
                        ffoutput = ffoutput + s.Substring(s.IndexOf("=", comp) + 1);
                    else
                        ffoutput = ffoutput + " DCT " + s.Substring(s.IndexOf("=", comp) + 1);
                    i++;
                }

            }

            Console.WriteLine("\n" + ffoutput);

            ffoutput = ffoutput + " " + arr + " " + "CI25";
        }

        static void CFGLoad() {
            string[] cfg = System.IO.File.ReadAllLines(@"settings.cfg");
            ff757Location = cfg[0];
            ff767Location = cfg[1];
            jar320Location = cfg[2];
            ixeg737Location = cfg[3];
            ff320Location = cfg[4];
            Console.WriteLine("Config loaded!");
            Console.WriteLine("-----------------------------------------");
        }

        static void Main(string[] args){
            CFGLoad();

            FindFiles(@"routes");

            SelectFile();

            JarConvert(@"routes");
            FF320Convert(@"routes");

            System.IO.File.WriteAllText(jar320Location + @"\" + dep+arr+"01.txt", output);
            File.Copy(@"routes\" + file, ff757Location + @"\" + file, true);
            File.Copy(@"routes\" + file, ff767Location + @"\" + file, true);
            System.IO.File.WriteAllText(ixeg737Location + @"\" + dep + arr + "01.fpl", output);
            System.IO.File.AppendAllText(ff320Location + @"\corte.in", "\n" + ffoutput);

            Console.WriteLine("\nDone!... Have a good flight!");
            Console.ReadKey();
        }
    }
}
