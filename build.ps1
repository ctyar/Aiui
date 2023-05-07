Get-ChildItem -Path '.\artifacts' | Remove-Item -Force -Recurse

dotnet pack src\Aiui\Aiui.csproj -o artifacts