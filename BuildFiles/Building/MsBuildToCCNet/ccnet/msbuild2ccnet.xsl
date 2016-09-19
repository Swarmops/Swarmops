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
      <b><xsl:value-of select="@solution_name"/></b> in <xsl:value-of select="@solution_dir"/>: 
			<xsl:choose>
				<xsl:when test="@error_count > 0">
				    Build failed with <xsl:value-of select="@error_count"/> errors
				</xsl:when>
				<xsl:when test="@warning_count > 0">
					<xsl:value-of select="@project_count"/> Projects built with <xsl:value-of select="@warning_count"/> warnings
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@project_count"/> Projects built with no warnings at all :-) 
				</xsl:otherwise>
			</xsl:choose>
		</td></tr>
			<xsl:if test="@error_count = 0 and @warning_count = 0"> <tr><td><img src="/ccnet/your_happy_image.jpg" alt="Happy Image :-)" /> Juchuu !!!</td></tr></xsl:if>
			<xsl:apply-templates select="project[error|warning]" />
		</table>
	</xsl:template>

	<xsl:template match="project" >
		<tr>
			<td class="section-project">
				<b><xsl:value-of select="@name"/></b> in <xsl:value-of select="@dir"/>:
			</td>
		</tr>
		<tr>
			<xsl:element name="td">
			    <xsl:attribute name="class">
					<xsl:if test="../@error_count > 0">section-error</xsl:if>
					<xsl:if test="../@error_count &lt;= 0">section-warning</xsl:if>
			    </xsl:attribute> 
				<xsl:apply-templates/>
			</xsl:element>
		</tr>
	</xsl:template>
	
	<xsl:template match="error|warning">
		<xsl:value-of select="@code" />: <xsl:value-of select="@message" /> in <xsl:value-of select="@name" /><xsl:value-of select="@pos"/><br/>
	</xsl:template>

</xsl:stylesheet>
