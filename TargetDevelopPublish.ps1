$content = [IO.File]::ReadAllText(".\Window Walker\Window Walker\Window Walker.csproj");
$content = $content.Replace("<InstallUrl>https://download.windowwalker.com/</InstallUrl>", "<InstallUrl>https://develop.windowwalker.com/</InstallUrl>")
$content | Out-File -Encoding utf8 -FilePath ".\Window Walker\Window Walker\Window Walker.csproj"