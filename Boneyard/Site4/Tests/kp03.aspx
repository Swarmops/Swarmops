<%@ Page Language="C#" Description="dotnetCHARTING Component" %>
<%@ Register TagPrefix="dotnet"  Namespace="dotnetCHARTING" Assembly="dotnetCHARTING"%>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<script runat="server">
void Page_Load(Object sender,EventArgs e)
{
	Chart.TempDirectory = "temp";
	Chart.Debug = true;
	Chart.Palette = new Color[]{Color.FromArgb(49,255,49),Color.FromArgb(255,255,0),Color.FromArgb(255,99,49),Color.FromArgb(0,156,255)};
	
	Chart.Type = ChartType.MultipleGrouped;
	Chart.Size = "600x350";
	Chart.Title = ".netCHARTING Sample";
	
	Chart.Use3D = true;
	Chart.DefaultSeries.DefaultElement.Transparency = 20;
	Chart.DefaultSeries.DefaultElement.ShowValue = true;
	Chart.DefaultSeries.Type = SeriesTypeMultiple.Pyramid;
	
	// This sample demonstrates the MultipleGrouped chart type.
	
	// *DYNAMIC DATA NOTE* 
	// This sample uses random data to populate the chart. To populate 
	// a chart with database data see the following resources:
	// - Help File > Getting Started > Data Tutorials
	// - DataEngine Class in the help file	
	// - Sample: features/DataEngine.aspx
	
	SeriesCollection mySC = getRandomData();

	// Add the random data.
	Chart.SeriesCollection.Add(mySC);
        
}

SeriesCollection getRandomData()
{
	Random myR = new Random(1);
	SeriesCollection SC = new SeriesCollection();
	for(int a = 1; a < 5; a++)
	{
		Series s = new Series("Series " + a.ToString());
		for(int b = 1; b < 5; b++)
		{
			Element e = new Element("Element " + b.ToString());
            e.YValue = 1; // myR.Next(50);
			s.Elements.Add(e);
		}
		SC.Add(s);
	}
	return SC;
}
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>.netCHARTING Sample</title>	</head>
	<body>
		<div align="center">
			<dotnet:Chart id="Chart" runat="server"/>
		</div>
	</body>
</html>
