"c:\Program Files\Git\usr\bin\date.exe" >> "%ATProSolutionPath%\gitlog.txt"
cd /d "%ATProSolutionPath%"
"C:\Program Files\Git\bin\git.exe" reset --hard
"C:\Program Files\Git\bin\git.exe" pull "https://cityindexqaminsk:cityindexqaminskuni1@github.com/cityindex/ATProAutomation.git" master
pause