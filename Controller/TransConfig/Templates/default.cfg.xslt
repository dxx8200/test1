<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="text" indent="yes" encoding="us-ascii"/>
  <xsl:template match="/">
    <xsl:apply-templates select="/TestCase/Operations"/>
  </xsl:template>
  
  <xsl:template match="/TestCase/Operations">
    <xsl:text>
[General]
XlBridgeTimeout=120
DefaultRunLogic=default.usp
AutomaticTransactions=1
[ThinkTime]
Options=NOTHINK
Factor=1
LimitFlag=0
Limit=1

[Iterations]
NumOfIterations=1
IterationPace=IterationASAP
StartEvery=60
RandomMin=60
RandomMax=90

[Log]
LogOptions=LogBrief
MsgClassData=0
MsgClassParameters=0
MsgClassFull=0

</xsl:text>
  </xsl:template>

</xsl:stylesheet>
