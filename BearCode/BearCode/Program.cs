using System;
using System.Net;
using System.Xml;

namespace Directions
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Out.Write("How many addresses: "); //same as cout
            //int count = int.Parse(Console.ReadLine()); // cin << count

            Console.Out.Write("Three addresses route system :"); //I'm forcing three routes until algorithum is solved
            Console.Out.WriteLine(); // endl;
            int count = 3; // Force 3 Adresses



            string[] add1 = new string[(count * (count - 1))]; //Track origin
            string[] add2 = new string[(count * (count - 1))]; //Track destination
            int[] min = new int[(count * (count - 1))];        //Track travel time minutes

            string[] addressList = new string[200];

            for (int i = 0; i < count; i++) // loop for address
            {
                Console.Out.Write("Enter the origin address: ");
                addressList[i] = Console.ReadLine();
            }

            int y = 0; // declare y because I have no idea what I'm doing

            for (int i = 0; i < count - 1; i++)
            {
                for (int x = i + 1; x < count; x++)
                {
                    string a1 = addressList[i];
                    string a2 = addressList[x];
                    int minutes = GetDrivingDistance(a1, a2);
                    //Console.Out.WriteLine("The trip between " + a1 + " and " + a2 + " will take: " + minutes.ToString() + " minutes.");

                    add1[y] = a1; //record origin
                    add2[y] = a2; //record destination
                    min[y] = minutes; // record amount of time it takes to get there

                    y++;

                    minutes = GetDrivingDistance(a2, a1);
                    //Console.Out.WriteLine("The trip between " + a2 + " and " + a1 + " will take: " + minutes.ToString() + " minutes.");

                    add1[y] = a2; //record origin
                    add2[y] = a1; //record destination
                    min[y] = minutes; // record amount of time it takes to get there

                    y++;
                }
            }

            for (int i = 0; i < (count * (count - 1)); i++)
            {
                Console.Out.WriteLine("  The trip between " + add1[i] + " and " + add2[i] + " will take: " + min[i].ToString() + " minutes.");
            }

            int[] route = new int[6];

            // We need to find out how to create this as a loop.

            for (int i = 0; i < (count * (count - 1)); i++) // match shortest route time to Route
            {
                int x = FindNextRoute(add1, add2, i, count);
                route[i] = min[i] + min[x];
                Console.Out.WriteLine("-Route " + i + ": " + add1[i] + " to " + add2[i] + " to " + add2[x] + " takes " + route[i] + " minutes.");
            }

            int minroute = MinTime(route[0], route[1]); // Compares all the route times and returns lowest value
            for (int i = 0; i < (count * (count - 1)); i++) // match shortest route time to Route
            {
                minroute = MinTime(minroute, route[i]);
            }

            // Find which route matches the shortest time
            for (int i = 0; i < (count * (count - 1)); i++) // match shortest route time to Route
            {
                if (minroute == route[i])
                    Console.Out.WriteLine("Route " + i + " is the fastest.");
            }

            Console.ReadKey(); // this is like System("PAUSE");
        }
        static int FindNextRoute(string[] add1, string[] add2, int i, int count)
        {
            int foundnum = 1000; // set to 1000, crashes program if foundnum doesn't work
            for (int n = 0; n < (count * (count - 1)); n++)
            {
                if ((add2[i] == add1[n]) && (add2[n] != add1[i]))
                {
                    foundnum = n;
                }
            }
            return foundnum;
        }
        static int MinTime(int r1, int r2) // Takes two routes and returns the shortest time.
        {
            if (r1 < r2)
                return r1;
            else
                return r2;
        }
        // this function takes an adress for origin and an address destination.
        // it returns the diriving time between the two addresses.        
        static int GetDrivingDistance(string origin, string destination)
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
            return durationInSeconds / 60;
        }

    }
}
