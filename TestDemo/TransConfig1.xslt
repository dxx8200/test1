<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

	<xsl:output method="xml" indent="yes"/>
	<xsl:param name="p_model" select="'XMLFile1.xml'" />
	<xsl:variable name="model" select="document($p_model)"/>

	<xsl:template match="/">
		<ConfigRoot>
			<CANConfigs>
				<xsl:for-each select="ConfigRoot/IOTranConfig/CANConfigs/CANConfig">
					<CANConfig>
						<xsl:copy-of select="current()/@*"/>
						<xsl:variable name="cofcan" select="$model/LinkedModel/Links/Link[Type='CAN']/Variables/Variable/Protocol[ChannelId=current()/@IOPort]"/>
						<xsl:if test="$cofcan">
							<xsl:attribute name="BaudRate">
								<xsl:value-of select="$cofcan/BaudRate"/>
							</xsl:attribute>
							<xsl:attribute name="WorkMode">
								<xsl:value-of select="$cofcan/WorkMode"/>
							</xsl:attribute>
							<xsl:attribute name="TransferProtocol">
								<xsl:value-of select="$cofcan/TransferProtocol"/>
							</xsl:attribute>
							<xsl:attribute name="FrameFormat">
								<xsl:value-of select="$cofcan/FrameFormat"/>
							</xsl:attribute>
						</xsl:if>
					</CANConfig>
				</xsl:for-each>
			</CANConfigs>
			<SerialConfigs>
				<xsl:for-each select="ConfigRoot/IOTranConfig/SerialConfigs/SerialConfig">
					<SerialConfig>
						<xsl:copy-of select="current()/@*"/>
						<xsl:variable name="cofserial" select="$model/LinkedModel/Links/Link[Type='RS422' or Type='RS232' or Type='RS485']/Variables/Variable/Protocol[ChannelId=current()/@IOPort]"/>
						<xsl:if test="$cofserial">
							<xsl:attribute name="BaudRate">
								<xsl:value-of select="$cofserial/BaudRate"/>
							</xsl:attribute>
							<xsl:attribute name="DataBits">
								<xsl:value-of select="$cofserial/DataBits"/>
							</xsl:attribute>
							<xsl:attribute name="Parity">
								<xsl:value-of select="$cofserial/Parity"/>
							</xsl:attribute>
							<xsl:attribute name="StopBits">
								<xsl:value-of select="$cofserial/StopBits"/>
							</xsl:attribute>
						</xsl:if>
					</SerialConfig>
				</xsl:for-each>
			</SerialConfigs>
			<EtherConfigs>
				<xsl:for-each select="ConfigRoot/IOTranConfig/EtherConfigs/EtherConfig">
					<EtherConfig>
						<xsl:copy-of select="current()/@*"/>
						<xsl:variable name="cofether" select="$model/LinkedModel/Links/Link[Type='Ethernet']/Variables/Variable/Protocol[ChannelId=current()/@IOPort]"/>
						<xsl:if test="$cofether">

						</xsl:if>
					</EtherConfig>
				</xsl:for-each>
			</EtherConfigs>

		</ConfigRoot>
	</xsl:template>

</xsl:stylesheet>
