#/opt/mssql-tools/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -Q "Select 1"
/opt/mssql-tools18/bin/sqlcmd -U sa -P YourStrong@Passw0rd -C -Q "SELECT 1" -b -o /dev/null