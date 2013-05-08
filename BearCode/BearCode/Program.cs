using System;
using System.Net;
using System.Xml;

namespace Directions
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Out.Write("How many addresses: "); //same as cout
            int count = int.Parse(Console.ReadLine()); // cin << count

            string[] add1 = new string[6];
            string[] add2 = new string[6];
            int[] min = new int[6];

            string[] addressList = new string[200]; // initalize 200 address list

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
                Console.Out.WriteLine("= The trip between " + add1[i] + " and " + add2[i] + " will take: " + min[i].ToString() + " minutes.");
            }

            //DEBUG SECTION - Find out which origin adress is in which array.

            //for (int i = 0; i < (count * (count - 1)); i++) // locate first address
            //{
            //    if (addressList[0] == add1[i])
            //        Console.Out.WriteLine(addressList[0] + " located in in array " + i);
            //}

            //for (int i = 0; i < (count * (count - 1)); i++) // locate second address
            //{
            //    if (addressList[1] == add1[i])
            //        Console.Out.WriteLine(addressList[1] + " located in array " + i);
            //}

            //for (int i = 0; i < (count * (count - 1)); i++) // locate third address
            //{
            //    if (addressList[2] == add1[i])
            //        Console.Out.WriteLine(addressList[2] + " located in array " + i);
            //}

            int[] route = new int[6];

            route[0] = min[0] + min[4];
            Console.Out.WriteLine("Route 0 : " + add1[0] + " to " + add2[0] + " to " + add2[4] + " takes " + route[0] + " minutes.");

            route[1] = min[1] + min[2];
            Console.Out.WriteLine("Route 1 : " + add1[1] + " to " + add2[1] + " to " + add2[2] + " takes " + route[1] + " minutes.");

            route[2] = min[2] + min[5];
            Console.Out.WriteLine("Route 2 : " + add1[2] + " to " + add2[2] + " to " + add2[5] + " takes " + route[2] + " minutes.");

            route[3] = min[3] + min[0];
            Console.Out.WriteLine("Route 3 : " + add1[3] + " to " + add2[3] + " to " + add2[0] + " takes " + route[3] + " minutes.");

            route[4] = min[4] + min[3];
            Console.Out.WriteLine("Route 4 : " + add1[4] + " to " + add2[4] + " to " + add2[3] + " takes " + route[4] + " minutes.");

            route[5] = min[5] + min[1];
            Console.Out.WriteLine("Route 5 : " + add1[5] + " to " + add2[5] + " to " + add2[1] + " takes " + route[5] + " minutes.");

            int minroute = MinTime(route[0], route[1]);
            minroute = MinTime(minroute, route[2]);
            minroute = MinTime(minroute, route[3]);
            minroute = MinTime(minroute, route[4]);
            minroute = MinTime(minroute, route[5]);

            for (int i = 0; i < (count * (count - 1)); i++) // match shortest route time to Route
            {
                if (minroute == route[i])
                    Console.Out.WriteLine("Route " + i + " is the fastest.");
            }

            Console.ReadKey(); // this is like System("PAUSE");
        }


        // this function takes an adress for origin and an address destination.
        // it returns the diriving time between the two addresses.

        static int MinTime(int r1, int r2) // Takes two routes and returns the shortest time.
        {
            if (r1 < r2)
                return r1;
            else
                return r2;
        }

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
