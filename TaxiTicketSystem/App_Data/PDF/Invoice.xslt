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
      <img src="http://localhost:19762/Content/Resources/1.png" />
        
      <h2 align="center" style="font-weight: normal;">ი ნ ვ ო ი ს ი</h2>
      <br />
        
      <div style="width: 50%; height: 50%; float:left;">
        <span style="color: #e46c0a; font-size: 12px;">დამკვეთი: </span> <span style="font-size: 12px;"><xsl:value-of select="*/companyName" /></span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">ს/კ: </span> <span style="font-size: 12px;"><xsl:value-of select="*/companyCode" /></span>
      </div>
        
      <div style="width: 50%; height: 50%; float:right;">
        <span style="color: #e46c0a; font-size: 12px;">თარიღი: </span> <span style="font-size: 12px;"><xsl:value-of select="*/dateNow" /></span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">გადახდის ვადა: </span> <span style="font-size: 12px;"><xsl:value-of select="*/servicePaymentDate" /></span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">მიმღები: </span> <span style="font-size: 12px;"><xsl:value-of select="*/ownerCompanyName" /></span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">ს/კ: </span> <span style="font-size: 12px;"><xsl:value-of select="*/ownerCompanyCode" /></span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">მისამართი: </span> <span style="font-size: 12px;"><xsl:value-of select="*/ownerCompanyAddress" /></span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">მომსახურე ბანკი: </span> <span style="font-size: 12px;">JSC "TeraBank"</span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">SWIFT сode: </span> <span style="font-size: 12px;">TEBAGE22</span>
        <br /><br />
        <span style="color: #e46c0a; font-size: 12px;">IBAN: </span> <span style="font-size: 12px;">GE91 KS00 0000 0360 2003 37</span>
        <!--<br /><br />-->
        <!--<span style="color: #e46c0a; font-size: 12px;">დირექტორი: </span> <span style="font-size: 12px;"><xsl:value-of select="*/ownerCompanyChief" /></span>-->
        <br /><br /><br /><br />
      </div>
        
        <table style="border: 1px solid #777; width: 100%; margin-top: 2px;">
          <thead>
            <tr style="background-color: #f58f09; height: 50px; font-size: 12px; color: #fff;">
              <th style="padding: 2px 0 5px 0;">გაწეული მომსახურების დასახელება</th>
              <th style="padding: 2px 0 5px 0;">თანხა დღგ-ს გარეშე</th>
              <th style="padding: 2px 0 5px 0;">დღგ</th>
              <th style="padding: 2px 0 5px 0;">თანხა დღგ-ს ჩათვლით</th>
            </tr>
          </thead>
          <tbody>
              <tr>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/generalDocPurpose" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/generalDocWithoutVat" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/generalDocVat" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/generalDocWithVat" />
                </td>
              </tr>
          
              <tr>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/startBalancePurpose" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/startBalanceWithoutVat" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/startBalanceVat" />
                </td>
                <td style="background-color: #f7f7f7; font-size: 12px; height: 30px; line-height: 20px;">
                  <xsl:value-of select="*/startBalanceWithVat" />
                </td>
              </tr>
          </tbody>
        </table>
        <span style="float: right; margin: 0 0 5px 0; padding: 5px; border: 1px solid #777; font-size: 12px; border-top: none; background-color: #f7f7f7;">
          <xsl:value-of select="*/totalSum" />
        </span>
        <span style="float: right; margin: 0 0 5px 0; padding: 5px; font-size: 12px;">მთლიანი თანხა ლარებში </span>

        <img style="margin-top: 50px; position:fixed;" src="http://192.168.88.121/Content/Resources/2.png" />
        <img style="margin-top: 220px; position:fixed;" src="http://192.168.88.121/Content/Resources/3.png" />
        <table style="margin-top: 300px; position:fixed; width: 100%;">
        <tr>
          <td><img src="http://192.168.88.121/Content/Resources/4.png" /></td>
          <td>
            <p style="font-weight: bold; font-size: 14px;">
              WEST SIDE GROUP Ltd <br />
              Code fiscal: 437058158 <br />
              BANK: JSC "TeraBank" <br />
              SWIFT code: TEBAGE22 <br />
              GE91KS0000000360200337
            </p>            
          </td>
          <td>
            <p style="font-weight: bold; font-size: 14px;">
              1, Heroes'Square <br />
              0171 Tbilisi, Georgia <br />
              Tel.: +995 32 230 60 60<br />
              E.mail: info@renotaxi.ge <br />
              Web: www.renotaxi.ge
            </p>
          </td>
        </tr>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>