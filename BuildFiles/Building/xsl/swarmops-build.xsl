<?xml version="1.0" encoding="UTF-8"?>
<!-- Copyright © 2006 by Christian Rodemeyer (mailto:christian@atombrenner.de) -->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/02/xpath-functions" xmlns:xdt="http://www.w3.org/2005/02/xpath-datatypes">
  <xsl:output method="html" version="1.0" encoding="UTF-8" indent="no"/>
  <xsl:template match="/">
    <xsl:apply-templates select="//xbuild" />
  </xsl:template>

  <xsl:template match="xbuild">
    <hr size="1" width="98%" align="left" color="#888888"/>
		<table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
		<tr><td class="sectionheader">
      <span style="font-weight:bold;font-size:125%">BUILD NOTES 
      <xsl:choose>
				<xsl:when test="@error_count > 0">
				    (<span style="color:#800"><xsl:value-of select="@error_count"/> ERRORS</span>)
				</xsl:when>
				<xsl:when test="@code_warning_count > 0">
				    (<span style="color:#750"><xsl:value-of select="@code_warning_count"/> CODE WARNINGS</span>)
				</xsl:when>
                                <xsl:when test="@environment_warning_count > 0">
                                    (<span style="color:#660"><xsl:value-of select="@environment_warning_count"/> ENVIRONMENT WARNINGS</span>)
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
				<b><xsl:value-of select="@name"/></b>:
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
          <xsl:text>&#xa;</xsl:text>
          <xsl:choose>
            <xsl:when test="@code!=''">
              <xsl:value-of select="@code" />: <span style="color:#444"><xsl:value-of select="@message" /></span> (<xsl:value-of select="@name" />, <xsl:value-of select="@line"/>)
            </xsl:when>
            <xsl:otherwise>
              <span style="color:#444"><xsl:value-of select="@message" /></span>
            </xsl:otherwise>
          </xsl:choose>
          <br/>
	</xsl:template>
  <xsl:template match="message"></xsl:template>

</xsl:stylesheet>
