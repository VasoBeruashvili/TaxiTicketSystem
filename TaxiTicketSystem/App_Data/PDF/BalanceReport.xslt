<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template match="/">
    <xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html></xsl:text>
    <html>
      <head>
        <meta content="text/html; charset=utf-8" http-equiv="content-type" />
        <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE10" />
        <title></title>        
      </head>
      <body style="padding: 5px 5px 20px 5px;">
        <table style="border: 1px solid #777; width: 100%; margin-top: 2px;">
          <thead>
            <tr style="background-color: #9BCDFF; font-size: 12px; color: #555;">
              <th style="padding: 2px 0 5px 0;">ID</th>
              <th style="padding: 2px 0 5px 0;">კომპანია</th>
              <th style="padding: 2px 0 5px 0;">წინა პერიოდი</th>
              <th style="padding: 2px 0 5px 0;">მიმდინარე</th>
              <th style="padding: 2px 0 5px 0;">ჯამი</th>
            </tr>
          </thead>
          <tbody>
            <xsl:for-each select="*/items/item">
              <tr>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="fakeID" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="companyName" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="prevPeriod" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="currentPeriod" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="totalPeriod" />
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>        
        <span style="float: right; margin: 0 0 5px 0; padding: 2px 2px 5px 2px; border: 1px solid #777; font-size: 12px; border-top: none; background-color: #FFFF00;">
          სულ ჯამი: <xsl:value-of select="*/totalSum" />
        </span>
        <span style="float: right; margin: 0 5px 5px 0; padding: 2px 2px 5px 2px; border: 1px solid #777; font-size: 12px; border-top: none; background-color: #FFFF00;">
          მიმდინარეს ჯამი: <xsl:value-of select="*/currentSum" />
        </span>
        <span style="float: right; margin: 0 5px 5px 0; padding: 2px 2px 5px 2px; border: 1px solid #777; font-size: 12px; border-top: none; background-color: #FFFF00;">
          წინა პერიოდის ჯამი: <xsl:value-of select="*/prevSum" />
        </span>
        <span style="float: left; margin: 0 0 5px 0; padding: 2px 2px 5px 2px; border: 1px solid #777; font-size: 12px; border-top: none; background-color: #FFFF00;">
          მიმდინარე თვის მეტობა: <xsl:value-of select="*/cur" />
        </span>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>