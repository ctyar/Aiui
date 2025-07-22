dotnet publish src\Web\Web.csproj --configuration Release -o artifacts -tl

Compress-Archive -Path artifacts\* -DestinationPath artifacts\deploy.zip -Force

$deployFile = (Get-Item .\artifacts\deploy.zip).FullName

az webapp deploy --resource-group AiuiResourceGroup --name aiui --src-path $deployFile --type zip --async true