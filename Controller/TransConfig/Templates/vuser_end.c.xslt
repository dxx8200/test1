<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:cs="urn:cs" exclude-result-prefixes="msxsl">

  <xsl:output method="text" indent="yes" encoding="us-ascii"/>
  <xsl:param name="p_model" />
  <xsl:param name="p_io" />
  <xsl:variable name="model" select="document($p_model)" />
  <xsl:variable name="io" select="document($p_io)" />
  <msxsl:script language="C#" implements-prefix="cs">
    <![CDATA[
     public string datenow()
     {
        return(DateTime.Now.ToString("yyyy'-'MM'-'dd' T'HH':'mm':'ss'Z'"));
     }
     ]]>
  </msxsl:script>

  <xsl:template match="/">
    /*********************************************************************
    * Auto Created by Test Case Generator
    * Created on: <xsl:value-of select="cs:datenow()"/>
    *
    * Test Case Information:
    * Author:<xsl:value-of select="/TestCase/Infor/Author"/>
    * Created on:<xsl:value-of select="/TestCase/Infor/Date"/>
    *********************************************************************/

    #include "lrs.h"

    vuser_end()
    {
    <xsl:for-each select="$io/ConfigRoot/IOTranConfig/SerialConfigs/SerialConfig">
      <xsl:variable name="vName" select="$model/LinkedModel/Links/Link[Type='RS422' or Type='RS232' or Type='RS485']/Variables/Variable[Protocol/ChannelId=current()/@IOPort]/Name"/>
      <xsl:if test="count($vName) > 0">
        lrs_close_socket("s_<xsl:value-of select="$vName"/>");
      </xsl:if>
    </xsl:for-each>

    <xsl:for-each select="$io/ConfigRoot/IOTranConfig/CANConfigs/CANConfig">
      <xsl:variable name="vName" select="$model/LinkedModel/Links/Link[Type='CAN']/Variables/Variable[Protocol/ChannelId=current()/@IOPort]/Name"/>
      <xsl:if test="count($vName) > 0">
        lrs_close_socket("s_<xsl:value-of select="$vName"/>");
      </xsl:if>
    </xsl:for-each>

    <xsl:for-each select="$io/ConfigRoot/IOTranConfig/EtherConfigs/EtherConfig">
      <xsl:variable name="vName" select="$model/LinkedModel/Links/Link[Type='Ethernet']/Variables/Variable[Protocol/ChannelId=current()/@IOPort]/Name"/>
      <xsl:if test="count($vName) > 0">
        lrs_close_socket("s_<xsl:value-of select="$vName"/>");
      </xsl:if>
    </xsl:for-each>
    lrs_cleanup();

    return 0;
    }
  </xsl:template>

</xsl:stylesheet>
