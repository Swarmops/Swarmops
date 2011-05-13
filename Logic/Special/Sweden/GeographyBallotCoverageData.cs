using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Activizr.Logic.Structure;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Support;
using Activizr.Basic.Enums;
using ValsedlarDataset;
using ValsedlarDataset.ValSedlarTableAdapters;
using System.Web.Caching;


namespace Activizr.Logic.Special.Sweden
{
    [Serializable]
    public class GeographyBallotCoverageDataPoint : IComparable
    {
        public int GeographyId;
        public int AdvanceVotingStationsDistro;
        public int AdvanceVotingStationsTotal;
        public int VotingStationsTotal;
        public int VotingStationsDistroSingle;
        public int VotingStationsDistroDouble;
        public int VotingStationsComplete;
        public int WAdvanceVotingStationsDistro;
        public int WAdvanceVotingStationsTotal;
        public int WWAdvanceVotingStationsTotal;
        public int WWAdvanceVotingStationsDistro;
        public int WVotingStationsTotal;
        public int WVotingStationsDistroSingle;
        public int WVotingStationsDistroDouble;
        public int WVotingStationsComplete;
        public string BookingUrl;
        public int VoterCount;

        public double AdvanceVotingCoverage
        {
            get
            {
                return AdvanceVotingStationsDistro * 100.0 / AdvanceVotingStationsTotal;
            }

        }
        public double WAdvanceVotingCoverage
        {
            get
            {
                return WAdvanceVotingStationsDistro * 100.0 / WAdvanceVotingStationsTotal;
            }

        }
        public double WWAdvanceVotingCoverage
        {
            get
            {
                return WWAdvanceVotingStationsDistro * 100.0 / WWAdvanceVotingStationsTotal;
            }

        }

        public double VotingCoverage
        {
            get
            {
                return (VotingStationsDistroSingle) * 100.0 / VotingStationsTotal;
            }
        }

        

        public double WVotingCoverage
        {
            get
            {
                return (WVotingStationsDistroSingle) * 100.0 / WVotingStationsTotal;
            }

        }
        public double VotingDoubleCoverage
        {
            get
            {
                return (VotingStationsDistroDouble) * 100.0 / VotingStationsTotal;
            }
        }
        
        public double WVotingDoubleCoverage
        {
            get
            {
                return (WVotingStationsDistroDouble) * 100.0 / WVotingStationsTotal;
            }
        }

        public void CalculateWeighted ()
        {
            WAdvanceVotingStationsDistro = VoterCount * AdvanceVotingStationsDistro;
            WAdvanceVotingStationsTotal = VoterCount * AdvanceVotingStationsTotal;
            WVotingStationsTotal = VoterCount * VotingStationsTotal;
            WVotingStationsDistroSingle = VoterCount * VotingStationsDistroSingle;
            WVotingStationsDistroDouble = VoterCount * VotingStationsDistroDouble;
            WVotingStationsComplete = VoterCount * VotingStationsComplete;

        }
        #region IComparable Members

        public int CompareTo (object obj)
        {
            // This compares voting.

            GeographyBallotCoverageDataPoint otherPoint = (GeographyBallotCoverageDataPoint)obj;

            if (otherPoint.VotingCoverage > this.VotingCoverage)
            {
                return 1;
            }
            else if (otherPoint.VotingCoverage < this.VotingCoverage)
            {
                return -1;
            }

            return 0;
        }

        #endregion
    }


    [Serializable]
    public class GeographyBallotCoverageData : List<GeographyBallotCoverageDataPoint>
    {

        public static object cacheLocker = new object();

        public GeographyBallotCoverageLookup ToLookup ()
        {
            GeographyBallotCoverageLookup result = new GeographyBallotCoverageLookup();

            foreach (GeographyBallotCoverageDataPoint dataPoint in this)
            {
                result[dataPoint.GeographyId] = dataPoint;
            }

            return result;
        }

        public static GeographyBallotCoverageData UpdateBallotDistroData ()
        {

            ValSedlar ds = new ValSedlar();

            GeographyBallotCoverageData coverageData = new GeographyBallotCoverageData();

            Dictionary<int, GeographyBallotCoverageDataPoint> lookup = new Dictionary<int, GeographyBallotCoverageDataPoint>();

            bookingcountsTableAdapter countsTA = new bookingcountsTableAdapter();
            countsTA.Fill(ds.bookingcounts);

            GeographyBallotCoverageDataPoint SEdataPoint = new GeographyBallotCoverageDataPoint();
            SEdataPoint.GeographyId = Geography.SwedenId;
            SEdataPoint.BookingUrl = "http://data.piratpartiet.se/Valsedelbokning";

            coverageData.Add(SEdataPoint);
            
            lookup[SEdataPoint.GeographyId] = SEdataPoint;

            GeographyBallotCoverageDataPoint dataPoint = null;

            foreach (ValSedlar.bookingcountsRow line in ds.bookingcounts.Rows)
            {

                if (lookup.ContainsKey(line.GeographyId))
                {
                    dataPoint = lookup[line.GeographyId];
                }
                else
                {
                    dataPoint = new GeographyBallotCoverageDataPoint();
                    dataPoint.GeographyId = line.GeographyId;
                    dataPoint.AdvanceVotingStationsDistro = 0;
                    dataPoint.AdvanceVotingStationsTotal = 0;
                    dataPoint.WWAdvanceVotingStationsDistro = 0;
                    dataPoint.WWAdvanceVotingStationsTotal = 0;
                    dataPoint.BookingUrl = "http://data.piratpartiet.se/Valsedelbokning/main.aspx?kommun=" + line.GeographyId;
                    dataPoint.VotingStationsComplete = 0;
                    dataPoint.VotingStationsDistroDouble = 0;
                    dataPoint.VotingStationsDistroSingle = 0;
                    dataPoint.VotingStationsTotal = 0;
                    dataPoint.VoterCount = line.VoterCount;

                    SEdataPoint.VoterCount += line.VoterCount;

                    lookup[line.GeographyId] = dataPoint;

                    coverageData.Add(dataPoint);

                }
                if (line.Typ == "V")
                {

                    dataPoint.VotingStationsTotal = (int)line.CntLokal;
                    dataPoint.VotingStationsDistroSingle = (int)line.CntDist;
                    dataPoint.VotingStationsDistroDouble = (int)line.CntVard;
                    dataPoint.VotingStationsComplete = (int)line.CntFull;

                    dataPoint.CalculateWeighted();

                    SEdataPoint.VotingStationsComplete += dataPoint.VotingStationsComplete;
                    SEdataPoint.VotingStationsDistroDouble += dataPoint.VotingStationsDistroDouble;
                    SEdataPoint.VotingStationsDistroSingle += dataPoint.VotingStationsDistroSingle;
                    SEdataPoint.VotingStationsTotal += dataPoint.VotingStationsTotal;

                    SEdataPoint.WVotingStationsComplete += dataPoint.WVotingStationsComplete;
                    SEdataPoint.WVotingStationsDistroDouble += dataPoint.WVotingStationsDistroDouble;
                    SEdataPoint.WVotingStationsDistroSingle += dataPoint.WVotingStationsDistroSingle;
                    SEdataPoint.WVotingStationsTotal += dataPoint.WVotingStationsTotal;

                }
                else if (line.Typ == "F")
                {
                    dataPoint.AdvanceVotingStationsTotal = (int)line.CntLokal;
                    dataPoint.AdvanceVotingStationsDistro = (int)line.CntDist;
                    dataPoint.WWAdvanceVotingStationsTotal = (int)line.VoterCountCalcTot;
                    dataPoint.WWAdvanceVotingStationsDistro = (int)line.VoterCountCalcBooked;

                    dataPoint.CalculateWeighted();

                    SEdataPoint.AdvanceVotingStationsDistro += dataPoint.AdvanceVotingStationsDistro;
                    SEdataPoint.AdvanceVotingStationsTotal += dataPoint.AdvanceVotingStationsTotal;

                    SEdataPoint.WAdvanceVotingStationsDistro += dataPoint.WAdvanceVotingStationsDistro;
                    SEdataPoint.WAdvanceVotingStationsTotal += dataPoint.WAdvanceVotingStationsTotal;
                    SEdataPoint.WWAdvanceVotingStationsTotal += dataPoint.WWAdvanceVotingStationsTotal;
                    SEdataPoint.WWAdvanceVotingStationsDistro += dataPoint.WWAdvanceVotingStationsDistro;
                }

            }

            // This is not very optimized, but it runs only once per hour and
            // has easy-to-follow logic.

            // For each circuit, get the geography tree, and for every node that has coverage data,
            // aggreagate that data.

            Geographies circuits = Geographies.FromLevel(Country.FromCode("SE"), GeographyLevel.ElectoralCircuit);

            foreach (Geography circuit in circuits)
            {
                GeographyBallotCoverageDataPoint circuitDataPoint =
                    new GeographyBallotCoverageDataPoint { GeographyId = circuit.Identity };
                Geographies circuitTree = circuit.GetTree();

                foreach (Geography circuitNode in circuitTree)
                {
                    if (lookup.ContainsKey(circuitNode.Identity))
                    {
                        //BotLog.Write(4, "BallotDistroData", "Adding " + circuitNode.Name);

                        circuitDataPoint.AdvanceVotingStationsDistro +=
                            lookup[circuitNode.Identity].AdvanceVotingStationsDistro;
                        circuitDataPoint.AdvanceVotingStationsTotal +=
                            lookup[circuitNode.Identity].AdvanceVotingStationsTotal;
                        circuitDataPoint.VotingStationsDistroSingle +=
                            lookup[circuitNode.Identity].VotingStationsDistroSingle;
                        circuitDataPoint.VotingStationsDistroDouble +=
                            lookup[circuitNode.Identity].VotingStationsDistroDouble;
                        circuitDataPoint.VotingStationsComplete +=
                            lookup[circuitNode.Identity].VotingStationsComplete;
                        circuitDataPoint.VotingStationsTotal +=
                            lookup[circuitNode.Identity].VotingStationsTotal;

                        circuitDataPoint.WAdvanceVotingStationsDistro +=
                            lookup[circuitNode.Identity].WAdvanceVotingStationsDistro;
                        circuitDataPoint.WAdvanceVotingStationsTotal +=
                            lookup[circuitNode.Identity].WAdvanceVotingStationsTotal;
                        circuitDataPoint.WVotingStationsDistroSingle +=
                            lookup[circuitNode.Identity].WVotingStationsDistroSingle;
                        circuitDataPoint.WVotingStationsDistroDouble +=
                            lookup[circuitNode.Identity].WVotingStationsDistroDouble;
                        circuitDataPoint.WVotingStationsComplete +=
                            lookup[circuitNode.Identity].WVotingStationsComplete;
                        circuitDataPoint.WVotingStationsTotal +=
                            lookup[circuitNode.Identity].WVotingStationsTotal;

                        circuitDataPoint.WWAdvanceVotingStationsTotal += lookup[circuitNode.Identity].WWAdvanceVotingStationsTotal;
                        circuitDataPoint.WWAdvanceVotingStationsDistro += lookup[circuitNode.Identity].WWAdvanceVotingStationsDistro;


                        circuitDataPoint.VoterCount +=
                            lookup[circuitNode.Identity].VoterCount;
                    }
                }

                coverageData.Add(circuitDataPoint);
            }

            return coverageData; ;
        }
    }


    public class GeographyBallotCoverageLookup : Dictionary<int, GeographyBallotCoverageDataPoint>
    {

    }

}
