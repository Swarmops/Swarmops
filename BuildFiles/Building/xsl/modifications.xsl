<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>

    <xsl:variable name="modification.list" select="/cruisecontrol/modifications/modification"/>

    <xsl:template match="/">
        <table class="section-table" cellpadding="2" cellspacing="0" border="0" width="98%">
            <!-- Modifications -->
            <tr>
                <td class="sectionheader" colspan="5">
                    <br/><span style="font-size:125%"><b>MODIFICATIONS SINCE LAST BUILD</b> (<xsl:value-of select="count($modification.list)"/>)</span>
                </td>
            </tr>

            <xsl:apply-templates select="$modification.list">
                <xsl:sort select="date" order="descending" data-type="text" />
            </xsl:apply-templates>

            <xsl:if test="count($modification.list) = 0">
                <tr><td>None since last build.</td></tr>
            </xsl:if>
            
        </table>
    </xsl:template>

    <!-- Modifications template -->
    <xsl:template match="modification">
        <tr>
            <xsl:if test="position() mod 2=0">
                <xsl:attribute name="class">section-oddrow</xsl:attribute>
            </xsl:if>
            <xsl:if test="position() mod 2!=0">
                <xsl:attribute name="class">section-evenrow</xsl:attribute>
            </xsl:if>

            <td class="section-data" valign="top"><xsl:value-of select="@type"/></td>
            <td class="section-data" valign="top"><xsl:value-of select="user"/></td>
            <td class="section-data" valign="top">
            	<xsl:choose>
            		<xsl:when test="count(url) = 1 ">
						<a>
							<xsl:attribute name="href">
								<xsl:value-of select="url" />
							</xsl:attribute>
							<xsl:if test="project != ''"><xsl:value-of select="project"/>/</xsl:if>
							<xsl:value-of select="filename"/>
						</a>
					</xsl:when>
            		<xsl:otherwise>
						<xsl:if test="project != ''"><xsl:value-of select="project"/>/</xsl:if>
						<xsl:value-of select="filename"/>
		      		</xsl:otherwise>
            	</xsl:choose>
			</td>
            <td class="section-data" valign="top"><xsl:value-of select="comment"/></td>
            <td class="section-data" valign="top"><xsl:value-of select="date"/></td>
        </tr>
    </xsl:template>

</xsl:stylesheet>
