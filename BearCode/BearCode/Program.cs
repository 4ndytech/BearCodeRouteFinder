using System;
using System.Net;
using System.Xml;

namespace Directions
{
    class Program
    {
        static void Main(string[] args)
        {
            // By default this program calculates three address routes.
            // It cannot calculate more until this program is improved.

            int debug = 0; // Defualt 0 - Debug off

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine("BEARCODE Fastest Route Finder - CIS 3100 GROUP PROJECT (SPRING 2013)");
            Console.ResetColor();
            Console.Out.WriteLine(); // endl; 

            int count = 3; // Force 3 Adresses

            // Following code allows user to define number of addresses in route - PROGRAM WILL NOT WORK
            //Console.Out.Write("How many addresses: "); //same as cout
            //int count = int.Parse(Console.ReadLine()); // cin << count

            int count2 = (count * (count - 1)); // n*(n-1) is used alot, so we declare a variable for it

            string[] addressList = new string[count2];

            for (int i = 0; i < count; i++) // loop for address input
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Out.Write("Enter Address " + (i + 1) + " of 3 : ");
                Console.ResetColor();
                addressList[i] = Console.ReadLine();
            }

            string[] add1 = new string[count2]; //Record Address 1
            string[] add2 = new string[count2]; //Record Address 2
            int[] min = new int[count2];        //Record travel time minutes
            double[] miles = new double[count2];

            int y = 0; // Y is counter for array number to store information
            double distance = 0;
            int minutes = 0;

            Console.Out.WriteLine(); // endl
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Out.WriteLine("Calculating all possible routes and travel times...");
            Console.Out.WriteLine();
            Console.ResetColor();

            for (int i = 0; i < count - 1; i++)
            {
                for (int x = i + 1; x < count; x++) // Asks google directions from A to B
                {
                    string a1 = addressList[i];
                    string a2 = addressList[x];

                    minutes = GetDrivingDistance(a1, a2, ref distance); // Google Directions From A to B                   
                    RecordList(a1, a2, minutes, distance, ref y, add1, add2, min, miles); // Record in parallel arrays

                    minutes = GetDrivingDistance(a2, a1, ref distance); // Google Directions from B to A
                    RecordList(a2, a1, minutes, distance, ref y, add1, add2, min, miles); // Record in parallel arrays
                }
            }

            if (debug == 1) // Default off, shows Route times for all possible two destination points
            {
                for (int i = 0; i < count2; i++)
                {
                    Console.Out.WriteLine("The trip between " + add1[i] + " and " + add2[i] + " will take: " + min[i].ToString() + " minutes." + miles[i].ToString("#.##"));
                }
            }

            int[] routetime = new int[count2];        //Route = Adddress 1&2 time + Address 2&3 time
            double[] routemiles = new double[count2]; //Route = Adddress 1&2 time + Address 2&3 time
            string[] add3 = new string[count2];       //Track route Address 3

            for (int i = 0; i < count2; i++) // match shortest route time to Route
            {
                int x = FindNextRoute(add1, add2, i, count2);
                routetime[i] = min[i] + min[x]; // adds two two destination route times together to create a three rotue time
                routemiles[i] = miles[i] + miles[x];
                add3[i] = add2[x];              // Assigns Address 3 the address located in address 2[x]
            }

            int minroute = FindMinTime(routetime[0], routetime[1]); // Compares all the route times and returns lowest value

            for (int i = 0; i < count2; i++) // match shortest route time to Route
            {
                minroute = FindMinTime(minroute, routetime[i]);
            }

            for (int i = 0; i < count2; i++) // Console output all Routes to User
            {
                if (minroute == routetime[i]) // If shortest route, Make it Green
                    Console.ForegroundColor = ConsoleColor.Green;
                else // Make all the other routes Dark Green
                    Console.ForegroundColor = ConsoleColor.DarkGreen;

                Console.Out.WriteLine("  " + (i + 1) + ": " + add1[i] + " to " + add2[i] + " to " + add3[i]);
                Console.Out.Write("     Travel Time : " + routetime[i] + " minutes");
                Console.Out.Write("  Travel Distance : " + routemiles[i].ToString("#.##") + " miles." + "\n\n");
                Console.ResetColor();
            }

            // Outputs which routes matches the shortest time
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Out.WriteLine("The Fastest Route is: ");
            Console.Out.WriteLine();
            Console.ResetColor();
            for (int i = 0; i < count2; i++)
            {
                if (minroute == routetime[i])
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Out.WriteLine("Route " + (i + 1) + " " + add1[i] + " to " + add2[i] + " to " + add3[i]);
                    Console.Out.Write("     Travel Time : " + routetime[i] + " minutes");
                    Console.Out.Write("  Travel Distance : " + routemiles[i].ToString("#.##") + " miles." + "\n");
                    Console.ResetColor();
                }
            }

            Console.ReadKey(); // this is like System("PAUSE");
        }
        static void RecordList(string a1, string a2, int minutes, double distance, ref int y, string[] add1, string[] add2, int[] min, double[] miles)
        {
            add1[y] = a1; //record Address 1 to array
            add2[y] = a2; //record Address 2 to array
            min[y] = minutes; // record travel time of Address 1 to Address 2 route to array
            miles[y] = distance; // Record distance
            y++; // Increment Array counter
        }
        static int FindNextRoute(string[] add1, string[] add2, int i, int count2)
        {
            int foundnum = 0; // 
            for (int n = 0; n < count2; n++)
            {
                if ((add2[i] == add1[n]) && (add2[n] != add1[i]))
                {
                    foundnum = n;
                }
            }
            return foundnum;
        }
        static int FindMinTime(int r1, int r2) // Takes two routes and returns the shortest time.
        {
            if (r1 < r2)
                return r1;
            else
                return r2;
        }
        // this function takes an adress for origin and an address destination.
        // it returns the diriving time between the two addresses.        
        static int GetDrivingDistance(string origin, string destination, ref double distance)
        {
            // we need to call google with the two address provided
            WebClient client = new WebClient();
            string response = client.DownloadString("http://maps.googleapis.com/maps/api/directions/xml?origin=" + origin + "&destination=" + destination + "&sensor=false");

            // google returns an xml file containing all the way points from origin to destination
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            // get the root element of the xml document
            XmlNode root = xml.DocumentElement;

            // let's make sure that the xml document status is OK, otherewise there is an error. 
            if (root.SelectSingleNode("status").InnerText != "OK")
            {
                Console.Out.Write("Error: " + root.SelectSingleNode("status").InnerText);
                return 0;
            }
            // Get distance - Andy
            double meters = 0;
            foreach (XmlNode step in root.SelectNodes("/DirectionsResponse/route/leg/step"))
            {
                // add each step to distance
                meters += double.Parse(step.SelectSingleNode("distance/value").InnerText);
            }
            distance = (meters * 0.000621371192); // convert Kilo to Miles

            // google returns the step by step directions.
            // if I want the total time needed, I have to add up the duration of each step. 
            // duration is provided in second. 
            int durationInSeconds = 0;

            // here is the for loop to go through each step
            foreach (XmlNode step in root.SelectNodes("/DirectionsResponse/route/leg/step"))
            {
                // take the duration of this step and accumulate in durationInSeconds variable
                durationInSeconds += int.Parse(step.SelectSingleNode("duration/value").InnerText);
            }

            // let's return the results in minutes, so divide by 60.
            return (durationInSeconds / 60);
        }

    }
}
