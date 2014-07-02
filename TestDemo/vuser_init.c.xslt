<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:cs="urn:cs" exclude-result-prefixes="msxsl">

  <xsl:output method="text" indent="yes" encoding="us-ascii"/>
  <xsl:param name="p_model" select="'XMLFile1.xml'" />
  <xsl:param name="p_io" select="'XMLFile2.xml'" />
  <xsl:variable name="model" select="document($p_model)" />
  <xsl:variable name="io" select="document($p_io)" />
  <msxsl:script language="C#" implements-prefix="cs">
    <![CDATA[
     public string FormatVariable(XPathNavigator v)
     {
        return(DateTime.Now.ToString("yyyy'-'MM'-'dd' T'HH':'mm':'ss'Z'"));
     }
     ]]>
  </msxsl:script>
  
  <xsl:template match="/">
;WSRData 2 1

    <xsl:for-each select="/TestCase/Operations/TsNode/Operation/Steps/TsStep">
      <xsl:variable name="stID" select="current()/Id"/>
      <xsl:for-each select="Values/TsStepValue/Variable" >
        <xsl:variable name="vName" select="$model/LinkedModel/Links/Link/Variables/Variable[Key=current()/VariableKey]/Name"/>
        <xsl:variable name="vType" select="$model/LinkedModel/Links/Link[Variables/Variable/Key=current()/VariableKey]/Type"/>
        <xsl:variable name="vPort" select="$model/LinkedModel/Links/Link/Variables/Variable[Key=current()/VariableKey]/Protocol/ChannelId"/>
        <xsl:variable name="vLength" select="$model/LinkedModel/Links/Link/Variables/Variable[Key=current()/VariableKey]/Length"/>
        <xsl:choose>
          <xsl:when test="$vType='RS422' or $vType='RS232' or $vType='RS485' ">
            <xsl:variable name="ioConfigIP" select="$io/ConfigRoot/IOTranConfig/SerialConfigs/SerialConfig[@IOPort=$vPort]/@ConfigIP"/>
            <xsl:variable name="ioDataPort" select="$io/ConfigRoot/IOTranConfig/SerialConfigs/SerialConfig[@IOPort=$vPort]/@DataPort"/>
            <xsl:if test="not(Source=/TestCase/UUT/Name) and (Target=/TestCase/UUT/Name)">
send buf_<xsl:value-of select="$vName"/> <xsl:value-of select="$vLength"/>
            </xsl:if>
            <xsl:if test="not(Target=/TestCase/UUT/Name) and (Source=/TestCase/UUT/Name)">
recv buf_<xsl:value-of select="$vName"/> <xsl:value-of select="$vLength"/>
            </xsl:if>
          </xsl:when>

          <xsl:when test="vType='CAN'">
            <xsl:variable name="ioConfigIP" select="$io/ConfigRoot/IOTranConfig/CANConfigs/CANConfig[@IOPort=$vPort]/@ConfigIP"/>
            <xsl:variable name="ioDataPort" select="$io/ConfigRoot/IOTranConfig/CANConfigs/CANConfig[@IOPort=$vPort]/@DataPort"/>
            <xsl:if test="not(Source=/TestCase/UUT/Name) and (Target=/TestCase/UUT/Name)">
              lr_send("s_<xsl:value-of select="$vName"/>","buf_<xsl:value-of select="$stID"/>", "target=<xsl:value-of select="$ioConfigIP"/>:<xsl:value-of select="$ioDataPort"/> ", <xsl:value-of select="$vLength"/>);
            </xsl:if>
            <xsl:if test="not(Target=/TestCase/UUT/Name) and (Source=/TestCase/UUT/Name)">
              lr_receive("s_<xsl:value-of select="$vName"/>","buf_<xsl:value-of select="$stID"/>", "target=<xsl:value-of select="$ioConfigIP"/>:<xsl:value-of select="$ioDataPort"/> ", <xsl:value-of select="$vLength"/>);
            </xsl:if>
          </xsl:when>

          <xsl:when test="vType='Ethernet'">
            <xsl:variable name="ioConfigIP" select="$io/ConfigRoot/IOTranConfig/EtherConfigs/EtherConfig[@IOPort=$vPort]/@ConfigIP"/>
            <xsl:variable name="ioDataPort" select="$io/ConfigRoot/IOTranConfig/EtherConfigs/EtherConfig[@IOPort=$vPort]/@DataPort"/>
            <xsl:if test="not(Source=/TestCase/UUT/Name) and (Target=/TestCase/UUT/Name)">
              lr_send("s_<xsl:value-of select="$vName"/>","buf_<xsl:value-of select="$stID"/>", "target=<xsl:value-of select="$ioConfigIP"/>:<xsl:value-of select="$ioDataPort"/> ", <xsl:value-of select="$vLength"/>);
            </xsl:if>
            <xsl:if test="not(Target=/TestCase/UUT/Name) and (Source=/TestCase/UUT/Name)">
              lr_receive("s_<xsl:value-of select="$vName"/>","buf_<xsl:value-of select="$stID"/>", "target=<xsl:value-of select="$ioConfigIP"/>:<xsl:value-of select="$ioDataPort"/> ", <xsl:value-of select="$vLength"/>);
            </xsl:if>
          </xsl:when>
        </xsl:choose>

      </xsl:for-each>
    </xsl:for-each>
    
-1
  </xsl:template>

</xsl:stylesheet>
