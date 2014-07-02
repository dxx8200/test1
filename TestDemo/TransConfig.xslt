<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

	<xsl:output method="xml" indent="yes"/>
	<xsl:variable name="model" select="document('XMLFile1.xml')" />

	<xsl:template match="/ConfigRoot/IOTranConfig">
		<ConfigRoot>
			<CANConfigs>
				<xsl:for-each select="CANConfigs/CANConfig">
					<CANConfig>
					<xsl:copy-of select="current()/@*"/>
					<xsl:attribute name="IOPort">
						<xsl:text>a</xsl:text>
					</xsl:attribute>
					</CANConfig>
				</xsl:for-each>
			</CANConfigs>
			<SerialConfigs>
				<xsl:for-each select="SerialConfigs/SerialConfig">
					<xsl:copy-of select="current()"/>
				</xsl:for-each>
			</SerialConfigs>
			<EtherConfigs>
				<xsl:for-each select="EtherConfigs/EtherConfig">
					<xsl:copy-of select="current()"/>
				</xsl:for-each>
			</EtherConfigs>
		</ConfigRoot>
	</xsl:template>

</xsl:stylesheet>
