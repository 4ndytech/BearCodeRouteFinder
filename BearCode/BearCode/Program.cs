﻿using System;
using System.Net;
using System.Xml;

namespace Directions
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Out.Write("Enter the origin address: ");
            string address1 = Console.ReadLine();
            Console.Out.Write("Enter the destination address: ");
            string address2 = Console.ReadLine();

            int minutes = GetDrivingDistance(address1, address2);

            Console.Out.WriteLine("This trip will take: " + minutes.ToString() + " minutes.");

            Console.ReadKey(); // this is like System("PAUSE");
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