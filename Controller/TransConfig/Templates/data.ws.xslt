<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

  <xsl:output method="text" indent="yes" encoding="us-ascii"/>
  <xsl:template match="/">
    <xsl:apply-templates select="/TestCase/Operations"/>
  </xsl:template>
  
  <xsl:template match="/TestCase/Operations">
    <xsl:text>

</xsl:text>
  </xsl:template>

</xsl:stylesheet>
