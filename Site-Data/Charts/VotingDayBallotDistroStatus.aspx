<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VotingDayBallotDistroStatus.aspx.cs"
    Inherits="Charts_VotingDayBallotDistroStatus" %>

<%@ Register TagPrefix="dotnet" Namespace="dotnetCHARTING" Assembly="dotnetCHARTING" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Ballot Distribution Status</title>
    <style>
    .bodyText
    {
            font-family: Arial, Helvetica, sans-serif;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <br />
    <asp:Label ID="Label1" runat="server" CssClass="bodyText">Antal väljare som saknar valsedlar i sin vallokal.</asp:Label>
    <br />
    <asp:Label ID="Label2" runat="server" CssClass="bodyText"></asp:Label>
    <br />
    <dotnet:Chart ID="Chart" runat="server" Type="ComboHorizontal">
        <DefaultTitleBox>
            <HeaderLabel GlowColor="" Type="UseFont">
            </HeaderLabel>
            <HeaderBackground ShadingEffectMode="None"></HeaderBackground>
            <Background ShadingEffectMode="None"></Background>
            <Label GlowColor="" Type="UseFont">
            </Label>
        </DefaultTitleBox>
        <SmartForecast Start=""></SmartForecast>
        <Background ShadingEffectMode="None"></Background>
        <DefaultLegendBox Padding="4" CornerBottomRight="Cut">
            <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
            <DefaultEntry ShapeType="None">
                <Background ShadingEffectMode="None"></Background>
                <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
            </DefaultEntry>
            <HeaderEntry ShapeType="None" Visible="False">
                <Background ShadingEffectMode="None"></Background>
                <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
            </HeaderEntry>
            <HeaderLabel GlowColor="" Type="UseFont">
            </HeaderLabel>
            <HeaderBackground ShadingEffectMode="None"></HeaderBackground>
            <Background ShadingEffectMode="None"></Background>
        </DefaultLegendBox>
        <ChartArea CornerTopLeft="Square" StartDateOfYear="">
            <DefaultElement ShapeType="None">
                <DefaultSubValue Name="">
                    <Line Length="4" />
                </DefaultSubValue>
                <SmartLabel GlowColor="" Type="UseFont">
                </SmartLabel>
                <LegendEntry ShapeType="None">
                    <Background ShadingEffectMode="None"></Background>
                    <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
                </LegendEntry>
            </DefaultElement>
            <Label GlowColor="" Type="UseFont" Font="Tahoma, 8pt">
            </Label>
            <YAxis GaugeNeedleType="One" GaugeLabelMode="Default" SmartScaleBreakLimit="2">
                <ScaleBreakLine Color="Gray"></ScaleBreakLine>
                <TimeScaleLabels MaximumRangeRows="4">
                </TimeScaleLabels>
                <MinorTimeIntervalAdvanced Start=""></MinorTimeIntervalAdvanced>
                <ZeroTick>
                    <Line Length="3"></Line>
                    <Label GlowColor="" Type="UseFont">
                    </Label>
                </ZeroTick>
                <DefaultTick>
                    <Line Length="3"></Line>
                    <Label GlowColor="" Type="UseFont" Text="%Value">
                    </Label>
                </DefaultTick>
                <TimeIntervalAdvanced Start=""></TimeIntervalAdvanced>
                <AlternateGridBackground ShadingEffectMode="None"></AlternateGridBackground>
                <Label GlowColor="" Type="UseFont" Alignment="Center" LineAlignment="Center" Font="Arial, 9pt, style=Bold">
                </Label>
            </YAxis>
            <XAxis GaugeNeedleType="One" GaugeLabelMode="Default" SmartScaleBreakLimit="2">
                <ScaleBreakLine Color="Gray"></ScaleBreakLine>
                <TimeScaleLabels MaximumRangeRows="4">
                </TimeScaleLabels>
                <MinorTimeIntervalAdvanced Start=""></MinorTimeIntervalAdvanced>
                <ZeroTick>
                    <Line Length="3"></Line>
                    <Label GlowColor="" Type="UseFont">
                    </Label>
                </ZeroTick>
                <DefaultTick>
                    <Line Length="3"></Line>
                    <Label GlowColor="" Type="UseFont" Text="%Value">
                    </Label>
                </DefaultTick>
                <TimeIntervalAdvanced Start=""></TimeIntervalAdvanced>
                <AlternateGridBackground ShadingEffectMode="None"></AlternateGridBackground>
                <Label GlowColor="" Type="UseFont" Alignment="Center" LineAlignment="Center" Font="Arial, 9pt, style=Bold">
                </Label>
            </XAxis>
            <Background ShadingEffectMode="None"></Background>
            <TitleBox Position="Left">
                <HeaderLabel GlowColor="" Type="UseFont">
                </HeaderLabel>
                <HeaderBackground ShadingEffectMode="None"></HeaderBackground>
                <Background ShadingEffectMode="None"></Background>
                <Label GlowColor="" Type="UseFont" Color="Black">
                </Label>
            </TitleBox>
            <LegendBox Padding="4" Position="Top" CornerBottomRight="Cut">
                <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
                <DefaultEntry ShapeType="None">
                    <Background ShadingEffectMode="None"></Background>
                    <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
                </DefaultEntry>
                <HeaderEntry ShapeType="None" Name="Name" Value="Value" Visible="False" SortOrder="-1">
                    <Background ShadingEffectMode="None"></Background>
                    <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
                </HeaderEntry>
                <HeaderLabel GlowColor="" Type="UseFont">
                </HeaderLabel>
                <HeaderBackground ShadingEffectMode="None"></HeaderBackground>
                <Background ShadingEffectMode="None"></Background>
            </LegendBox>
        </ChartArea>
        <DefaultElement ShapeType="None">
            <DefaultSubValue Name="">
            </DefaultSubValue>
            <SmartLabel GlowColor="" Type="UseFont">
            </SmartLabel>
            <LegendEntry ShapeType="None">
                <Background ShadingEffectMode="None"></Background>
                <LabelStyle GlowColor="" Type="UseFont"></LabelStyle>
            </LegendEntry>
        </DefaultElement>
        <NoDataLabel GlowColor="" Type="UseFont">
        </NoDataLabel>
        <TitleBox Position="Left">
            <HeaderLabel GlowColor="" Type="UseFont">
            </HeaderLabel>
            <HeaderBackground ShadingEffectMode="None"></HeaderBackground>
            <Background ShadingEffectMode="None"></Background>
            <Label GlowColor="" Type="UseFont" Color="Black">
            </Label>
        </TitleBox>
    </dotnet:Chart>
    <asp:Literal ID="LiteralXml" runat="server" />
    </form>
</body>
</html>
