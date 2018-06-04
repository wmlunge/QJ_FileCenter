TASKKILL /F /IM QJ_FileCenter.exe /T 
NET STOP QJ_FileCenterService
%Windir%\Microsoft.NET\Framework\v4.0.30319\installutil /u "%~dp0QJ_FileCenter.exe"
