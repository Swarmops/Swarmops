<%@ Page Language="C#" Description="dotnetCHARTING Component" %>
<%@ Register TagPrefix="dotnet"  Namespace="dotnetCHARTING" Assembly="dotnetCHARTING"%>
<%@ Import Namespace="System.Drawing" %>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>.netCHARTING Sample</title>
		<script runat="server">

        private void Page_Load (Object sender, EventArgs e)
        {
            Chart.Type = ChartType.Combo; //Horizontal;
            Chart.Width = 600;
            Chart.Height = 350;
            Chart.TempDirectory = "temp";
            Chart.Debug = true;
            Chart.DefaultSeries.DefaultElement.ShowValue = true;
            Chart.DefaultSeries.DefaultElement.SmartLabel.Alignment = LabelAlignment.Center;
            Chart.DefaultSeries.DefaultElement.SmartLabel.Text = "%YValue - %Name";
            Chart.LegendBox.Orientation = dotnetCHARTING.Orientation.Bottom;

            // This sample will demonstrate how to create a funnel chart.

            // Setup the title.
            Chart.Title = "Funnel Chart";
            Chart.TitleBox.Position = TitleBoxPosition.Full;
            Chart.TitleBox.Label.Alignment = StringAlignment.Center;

            // *DYNAMIC DATA NOTE* 
            // This sample uses random data to populate the chart. To populate 
            // a chart with database data see the following resources:
            // - Classic samples folder
            // - Help File > Data Tutorials
            // - Sample: features/DataEngine.aspx
            Series mySeries = getRandomData()[0];

            // First we sort the series 
            mySeries.Sort(ElementValue.YValue, "DESC");

            // Make it an area line and get rid of the element markers.
            mySeries.Type = SeriesType.AreaLine;
            mySeries.DefaultElement.Marker.Type = ElementMarkerType.None;

            // Create funnel data
            createFunnel(mySeries);

            // Five each element a new palette.	
            mySeries.PaletteName = Palette.Lavender;

            // Clear the background.
            Chart.YAxis.Clear();
            Chart.ChartArea.ClearColors();
            Chart.ChartArea.Background.Color = Color.FromArgb(220, 220, 220);
            Chart.YAxis.ZeroTick = null;
            Chart.XAxis.Clear();

            // Turn off all shadows.
            Chart.DefaultShadow.Color = Color.Empty;

            // Add the random data.
            Chart.SeriesCollection.Add(mySeries);
        }

        private void createFunnel (Series s)
        {
            // This loop will set the same but negated y value start of the area line as it's value;
            foreach (Element myElement in s.Elements)
            {
                // Because this will double the values we divide it by two first.
                myElement.YValue /= 2;
                myElement.YValueStart = -myElement.YValue;
            }
        }

        private SeriesCollection getRandomData()
        {
            SeriesCollection SC = new SeriesCollection();
            Random myR = new Random(1);
            for (int a = 0; a < 4; a++)
            {
                Series s = new Series();
                s.Name = "Series " + a;
                for (int b = 0; b < 4; b++)
                {
                    Element e = new Element();
                    e.Name = "Element " + b;
                    //e.YValue = -25 + myR.Next(50);
                    e.YValue = myR.Next(50);
                    s.Elements.Add(e);
                }
                SC.Add(s);
            }
            return SC;
        }

        </script>
	</head>
	<body>
		<div style="text-align:center">
			<dotnet:Chart id="Chart" runat="server" Width="568px" Height="344px">
			</dotnet:Chart>
		</div>
	</body>
</html>
