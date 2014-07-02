<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="text" indent="yes" encoding="us-ascii"/>
  <xsl:template match="/">
    <xsl:apply-templates select="/TestCase/Operations"/>
  </xsl:template>
  
  <xsl:template match="/TestCase/Operations">
    <xsl:text>
[RunLogicEndRoot]
Name="End"
MercIniTreeSectionName="RunLogicEndRoot"
RunLogicNumOfIterations="1"
RunLogicObjectKind="Group"
RunLogicActionType="VuserEnd"
MercIniTreeFather=""
RunLogicRunMode="Sequential"
RunLogicActionOrder="vuser_end"
MercIniTreeSons="vuser_end"
[RunLogicInitRoot:vuser_init]
Name="vuser_init"
MercIniTreeSectionName="vuser_init"
RunLogicObjectKind="Action"
RunLogicActionType="VuserInit"
MercIniTreeFather="RunLogicInitRoot"
[RunLogicEndRoot:vuser_end]
Name="vuser_end"
MercIniTreeSectionName="vuser_end"
RunLogicObjectKind="Action"
RunLogicActionType="VuserEnd"
MercIniTreeFather="RunLogicEndRoot"
[RunLogicRunRoot:Action]
Name="Action"
MercIniTreeSectionName="Action"
RunLogicObjectKind="Action"
RunLogicActionType="VuserRun"
MercIniTreeFather="RunLogicRunRoot"
[RunLogicRunRoot]
Name="Run"
MercIniTreeSectionName="RunLogicRunRoot"
RunLogicNumOfIterations="1"
RunLogicObjectKind="Group"
RunLogicActionType="VuserRun"
MercIniTreeFather=""
RunLogicRunMode="Sequential"
RunLogicActionOrder="Action"
MercIniTreeSons="Action"
[RunLogicInitRoot]
Name="Init"
MercIniTreeSectionName="RunLogicInitRoot"
RunLogicNumOfIterations="1"
RunLogicObjectKind="Group"
RunLogicActionType="VuserInit"
MercIniTreeFather=""
RunLogicRunMode="Sequential"
RunLogicActionOrder="vuser_init"
MercIniTreeSons="vuser_init"
[Profile Actions]
Profile Actions name=vuser_init,Action,vuser_end
[RunLogicErrorHandlerRoot]
MercIniTreeSectionName="RunLogicErrorHandlerRoot"
RunLogicNumOfIterations="1"
RunLogicActionOrder="vuser_errorhandler"
RunLogicObjectKind="Group"
Name="ErrorHandler"
RunLogicRunMode="Sequential"
RunLogicActionType="VuserErrorHandler"
MercIniTreeSons="vuser_errorhandler"
MercIniTreeFather=""
[RunLogicErrorHandlerRoot:vuser_errorhandler]
MercIniTreeSectionName="vuser_errorhandler"
RunLogicObjectKind="Action"
Name="vuser_errorhandler"
RunLogicActionType="VuserErrorHandler"
MercIniTreeFather="RunLogicErrorHandlerRoot"


</xsl:text>
  </xsl:template>

</xsl:stylesheet>
