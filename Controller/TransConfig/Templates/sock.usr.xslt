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
Type=WinSock
RecordedProtocol=
DefaultCfg=default.cfg
AppName=
BuildTarget=
ParamRightBrace=&gt;
ParamLeftBrace=&lt;
NewFunctionHeader=0
LastActiveAction=
CorrInfoReportDir=
LastResultDir=
DevelopTool=Vugen
MajorVersion=8
MinorVersion=1
ParameterFile=
GlobalParameterFile=
LastModifyVer=8.1
[ExtraFiles]
data.ws=
[TransactionsOrder]
Order=""
[Actions]
vuser_init=vuser_init.c
Action=Action.c
vuser_end=vuser_end.c
[Recorded Actions]
vuser_init=0
Action=0
vuser_end=0
data.ws=0
[Replayed Actions]
vuser_init=0
Action=0
vuser_end=0
data.ws=0
[RunLogicFiles]
Default Profile=default.usp
[StateManagement]
1=1
5=0
6=0
7=0
8=0
9=0
10=0
12=0
13=0
14=0
15=0
CurrentState=1
VuserStateHistory= 0
LastReplayStatus=0
</xsl:text>
  </xsl:template>

</xsl:stylesheet>
