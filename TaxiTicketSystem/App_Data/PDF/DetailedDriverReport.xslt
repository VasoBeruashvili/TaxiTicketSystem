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
        <table style="border: 1px solid #777;">
          <thead>
            <tr style="background-color: #9BCDFF; font-size: 12px; color: #555;">
              <th style="padding: 2px 0 5px 0;">კომპანიის ID</th>
              <th style="padding: 2px 0 5px 0;">მძღოლი</th>
              <th style="padding: 2px 0 5px 0;">მგზავრობის თარიღი</th>
              <th style="padding: 2px 0 5px 0;">კონტრაგენტი</th>
              <th style="padding: 2px 0 5px 0;">ავტომობილის N</th>
              <th style="padding: 2px 0 5px 0;">გავლილი მანძილი</th>
              <th style="padding: 2px 0 5px 0;">ღირებულება</th>
              <th style="padding: 2px 0 5px 0;">სადგომის ხარჯი</th>
              <th style="padding: 2px 0 5px 0;">საბაჟოს მოსაკრებელი</th>
              <th style="padding: 2px 0 5px 0;">დამატებითი ხარჯი</th>
              <th style="padding: 2px 0 5px 0;">სულ ჯამი</th>
              <th style="padding: 2px 0 5px 0;">სულ ჯამი + დღგ</th>
              <th style="padding: 2px 0 5px 0;">სულ ჯამი - დღგ</th>
              <th style="padding: 2px 0 5px 0;">პრინტერის გარეშე</th>
              <th style="padding: 2px 0 5px 0;">კომენტარი</th>
            </tr>
          </thead>
          <tbody>
            <xsl:for-each select="*/items/item">
              <tr>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="contragentFakeID" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="staffName" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="dateNow" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="companyName" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="carNumber" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="traveledDistance" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="amountPrice" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="parkingCosts" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="customsFees" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="additionalCosts" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="subTotalSum" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="TSWV1" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="TSWV2" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="withoutPrint" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px;">
                  <xsl:value-of select="commentOut" />
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
        <span style="float: right; margin: 0 0 5px 0; padding: 2px 2px 5px 2px; border: 1px solid #777; font-size: 12px; border-top: none; background-color: #FFFF00;">
          ჯამი: <xsl:value-of select="*/totalSum" />
        </span>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>