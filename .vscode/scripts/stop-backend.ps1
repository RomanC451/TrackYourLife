$ErrorActionPreference = 'SilentlyContinue'

Get-Process -Name 'TrackYourLife.App' | Stop-Process -Force

foreach ($port in @(5244, 7196)) {
    Get-NetTCPConnection -LocalPort $port -State Listen |
        ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
}

exit 0
