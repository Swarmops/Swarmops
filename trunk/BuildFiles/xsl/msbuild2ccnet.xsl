<?xml version="1.0" encoding="UTF-8"?>
<!-- Copyright © 2006 by Christian Rodemeyer (mailto:christian@atombrenner.de) -->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/02/xpath-functions" xmlns:xdt="http://www.w3.org/2005/02/xpath-datatypes">
	<xsl:output method="html" version="1.0" encoding="UTF-8" indent="no"/>
	<xsl:template match="/">
		<xsl:apply-templates select="//msbuild" />
	</xsl:template>

	<xsl:template match="msbuild">
		<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
		<tr><td class="sectionheader">
      <br/><span style="font-weight:bold;font-size:125%">BUILD NOTES 
      <xsl:choose>
				<xsl:when test="@error_count > 0">
				    (<span style="color:#800"><xsl:value-of select="@error_count"/> ERRORS</span>)
				</xsl:when>
				<xsl:when test="@warning_count > 0">
				    (<span style="color:#750"><xsl:value-of select="@warning_count"/> WARNINGS</span>)
				</xsl:when>
				<xsl:otherwise>
				    (<span style="color:#080">CLEAN BUILD</span>)
				</xsl:otherwise>
			</xsl:choose>
		</span></td></tr>
			<xsl:if test="@error_count = 0 and @warning_count = 0"> <tr><td><!--<img src="/ccnet/your_happy_image.jpg" alt="Happy Image :-)" />--> Juchuu !!!</td></tr></xsl:if>
			<xsl:apply-templates select="project[error|warning]" />
		</table>
	</xsl:template>

	<xsl:template match="project" >
		<tr>
			<td class="section-project">
				<b>Project <xsl:value-of select="@name"/></b>:
			</td>
		</tr>
		<tr>
			<xsl:element name="td">
			    <xsl:attribute name="class">
					<xsl:if test="../@error_count > 0">error</xsl:if>
					<xsl:if test="../@error_count &lt;= 0">warning</xsl:if>
			    </xsl:attribute> 
				<xsl:apply-templates/>
			</xsl:element>
		</tr>
	</xsl:template>
	
	<xsl:template match="error|warning">
    <xsl:if test="@code!=''"><xsl:value-of select="@code" />: </xsl:if>
		<xsl:value-of select="@message" /> in <xsl:value-of select="@name" /> <xsl:value-of select="@pos"/><br/>
	</xsl:template>
  <xsl:template match="message"></xsl:template>

</xsl:stylesheet>
