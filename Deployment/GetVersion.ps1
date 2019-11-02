param(
    [string]$url
)

add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@

$AllProtocols = [System.Net.SecurityProtocolType]'Ssl3,Tls,Tls11,Tls12'
[System.Net.ServicePointManager]::SecurityProtocol = $AllProtocols
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy



$version=(Invoke-WebRequest -Uri "https://$url/version.txt").Content.Split('.');
$version[3]=[int]$version[3]+1;
$NewVersion=[string]::Join(".",$version);
Write-Host -NoNewLine "$NewVersion";
