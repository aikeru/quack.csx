::"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\wsdl.exe" "%1" /o:output.cs
::"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" /t:library /r:System.Web.Services.dll /r:System.Xml.dll /out:ServiceClient.dll output.cs

"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\svcutil.exe" "%1" /out:output.cs
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" /t:library /r:System.ServiceModel.dll /r:System.Xml.dll /out:ServiceClient.dll output.cs