namespace Swarmops.Utility.BotCode
{
    public class UpdateStatsCache
    {
        static public void Run()
        {
            BotLog.Write(1, "UpdateStatsCache", "Entering (obsoleted does nothing");

            UpdateBallotDistroData();

            BotLog.Write(1, "UpdateStatsCache", "Exiting");
        }

        private static void UpdateBallotDistroData ()
        {
            //Empty method now this is done from the web app.
        }

        private static void ObsoletedUpdateBallotDistroData ()
        {
            #region Obsoleted commented code
            //BotLog.Write(2, "BallotDistroData", "Entering");
            //BotLog.Write(2, "BallotDistroData", "Getting geographic voter counts...");

            //Dictionary<int, int> voterCount = Optimizations.GetGeographyVoterCounts();

            //BotLog.Write(2, "BallotDistroData", "...done. Getting advance voting data...");

            //WebRequest myWebRequest = WebRequest.Create("http://valsedlar.piratpartiet.se/export/municipalities_book_data/");

            //// Send the 'WebRequest' and wait for response.
            //WebResponse myWebResponse = myWebRequest.GetResponse();

            //BotLog.Write(2, "BallotDistroData", "...reading response...");
            //string data = null;

            //// Obtain a 'Stream' object associated with the response object.
            //using (Stream receiveStream = myWebResponse.GetResponseStream())
            //{

            //    Encoding encode = Encoding.UTF8;

            //    // Pipe the stream to a higher level stream reader with the required encoding format.
            //    using (StreamReader readStream = new StreamReader(receiveStream, encode))
            //    {
            //        data = readStream.ReadToEnd();
            //    }
            //}

            //BotLog.Write(2, "BallotDistroData", "...done. Decoding data per city.");

            //string[] lines = data.Split('\n');

            //int advanceStationCount = 0;
            //int advanceStationDistroCount = 0;
            //int stationCount = 0;
            //int stationSingleDistroCount = 0;
            //int stationDoubleDistroCount = 0;
            //int stationFullCoverage = 0;

            //GeographyBallotCoverageData coverageData = new GeographyBallotCoverageData();
            //Dictionary<int, GeographyBallotCoverageDataPoint> lookup = new Dictionary<int, GeographyBallotCoverageDataPoint>();

            //foreach (string line in lines)
            //{
            //    if (line.Contains(";") && !line.StartsWith("#"))
            //    {
            //        string[] parts = line.Split(';');

            //        string cityName = parts[0];

            //        if (cityName.StartsWith("Göteborg") ||
            //            cityName.StartsWith("Gotland") ||
            //            cityName.StartsWith("Malmö"))
            //        {
            //            cityName = cityName.Replace("kommun", "valkrets");
            //        }
            //        Geography city = Geography.FromName(cityName);

            //        int booked = Int32.Parse(parts[2]);
            //        int total = Int32.Parse(parts[1]);
            //        string bookingUrl = parts[8];

            //        advanceStationCount += total * voterCount[city.Identity];
            //        advanceStationDistroCount += booked * voterCount[city.Identity];

            //        GeographyBallotCoverageDataPoint dataPoint =
            //            new GeographyBallotCoverageDataPoint
            //            {
            //                GeographyId = city.Identity,
            //                AdvanceVotingStationsDistro = booked * voterCount[city.Identity],
            //                AdvanceVotingStationsTotal = total * voterCount[city.Identity],
            //                BookingUrl = bookingUrl
            //            };

            //        lookup[city.Identity] = dataPoint;

            //        BotLog.Write(3, "BallotDistroData", city.Name + " Advance: " + booked.ToString() + "/" + total.ToString());
            //    }
            //}

            //BotLog.Write(2, "BallotDistroData", "Done collecting advance data. Collecting election day data...");

            //myWebRequest = WebRequest.Create("http://valsedlar.piratpartiet.se/export/votingstation_coverage/");

            //// Send the 'WebRequest' and wait for response.
            //myWebResponse = myWebRequest.GetResponse();

            //BotLog.Write(2, "BallotDistroData", "...reading response...");
            //data = null;

            //// Obtain a 'Stream' object associated with the response object.
            //using (Stream receiveStream = myWebResponse.GetResponseStream())
            //{

            //    Encoding encode = Encoding.UTF8;

            //    // Pipe the stream to a higher level stream reader with the required encoding format.
            //    using (StreamReader readStream = new StreamReader(receiveStream, encode))
            //    {
            //        data = readStream.ReadToEnd();
            //    }
            //}

            //BotLog.Write(2, "BallotDistroData", "...done. Decoding election day data per city.");

            //lines = data.Split('\n');

            //foreach (string line in lines)
            //{
            //    if (line.Contains(";") && !line.StartsWith("#") && !line.StartsWith(";"))
            //    {
            //        string[] parts = line.Split(';');

            //        string cityName = parts[1];

            //        if (cityName.StartsWith("Göteborg") ||
            //            cityName.StartsWith("Gotland") ||
            //            cityName.StartsWith("Malmö"))
            //        {
            //            cityName = cityName.Replace("kommun", "valkrets");
            //        }
            //        Geography city = Geography.FromName(cityName);

            //        int total = Int32.Parse(parts[2]);
            //        int singleDistro = Int32.Parse(parts[4]);
            //        int doubleDistro = Int32.Parse(parts[5]);
            //        int fullCoverage = Int32.Parse(parts[6]);

            //        stationCount += total;
            //        stationSingleDistroCount += singleDistro;
            //        stationDoubleDistroCount += doubleDistro;
            //        stationFullCoverage += fullCoverage;

            //        lookup[city.Identity].VotingStationsTotal = total;
            //        lookup[city.Identity].VotingStationsDistroSingle = singleDistro;
            //        lookup[city.Identity].VotingStationsDistroDouble = doubleDistro;
            //        lookup[city.Identity].VotingStationsComplete = fullCoverage;

            //        BotLog.Write(3, "BallotDistroData", city.Name + " ElectionDay: " + (singleDistro + doubleDistro + fullCoverage).ToString() + "/" + total.ToString());
            //    }
            //}

            //coverageData.Add(new GeographyBallotCoverageDataPoint
            //                     {
            //                         GeographyId = Geography.FromName("Sverige").Identity,
            //                         AdvanceVotingStationsDistro = advanceStationDistroCount,
            //                         AdvanceVotingStationsTotal = advanceStationCount,
            //                         VotingStationsComplete = stationFullCoverage,
            //                         VotingStationsDistroDouble = stationDoubleDistroCount,
            //                         VotingStationsDistroSingle = stationSingleDistroCount,
            //                         VotingStationsTotal = stationCount
            //                     });

            //BotLog.Write(2, "BallotDistroData", "Done collecting election day data.");




            //BotLog.Write(2, "BallotDistroData", "Aggregating per electoral circuit.");



            //// This is not very optimized, but it runs only once per hour and
            //// has easy-to-follow logic.

            //// For each circuit, get the geography tree, and for every node that has coverage data,
            //// aggreagate that data.

            //Geographies circuits = Geographies.FromLevel(Country.FromCode("SE"), GeographyLevel.ElectoralCircuit);

            //foreach (Geography circuit in circuits)
            //{
            //    BotLog.Write(3, "BallotDistroData", circuit.Name + " --");
            //    GeographyBallotCoverageDataPoint circuitDataPoint =
            //        new GeographyBallotCoverageDataPoint { GeographyId = circuit.Identity };
            //    Geographies circuitTree = circuit.GetTree();

            //    foreach (Geography circuitNode in circuitTree)
            //    {
            //        if (lookup.ContainsKey(circuitNode.Identity))
            //        {
            //            BotLog.Write(4, "BallotDistroData", "Adding " + circuitNode.Name);

            //            circuitDataPoint.AdvanceVotingStationsDistro +=
            //                lookup[circuitNode.Identity].AdvanceVotingStationsDistro;
            //            circuitDataPoint.AdvanceVotingStationsTotal +=
            //                lookup[circuitNode.Identity].AdvanceVotingStationsTotal;
            //            circuitDataPoint.VotingStationsDistroSingle +=
            //                lookup[circuitNode.Identity].VotingStationsDistroSingle;
            //            circuitDataPoint.VotingStationsDistroDouble +=
            //                lookup[circuitNode.Identity].VotingStationsDistroDouble;
            //            circuitDataPoint.VotingStationsComplete +=
            //                lookup[circuitNode.Identity].VotingStationsComplete;
            //            circuitDataPoint.VotingStationsTotal +=
            //                lookup[circuitNode.Identity].VotingStationsTotal;
            //        }
            //    }

            //    BotLog.Write(3, "BallotDistroData",
            //                 "...done. Total for " + circuit.Name + ": " + circuitDataPoint.AdvanceVotingStationsDistro +
            //                 "/" + circuitDataPoint.AdvanceVotingStationsTotal + ".");

            //    coverageData.Add(circuitDataPoint);
            //}

            //foreach (int key in lookup.Keys)
            //{
            //    coverageData.Add(lookup[key]);
            //}

            //BotLog.Write(2, "BallotDistroData", "Writing to db");

            //Persistence.Key["BallotCoverage-SE"] = coverageData.ToXml();

            //BotLog.Write(2, "BallotDistroData", "Exiting");
            #endregion
        }
    }
}
