Param(
  [Parameter(Mandatory=$True)]
   [string]$folderToAnalyse,
   [string]$filter="*.*",
   [ValidateSet('UTF8_BOM', 'UTF8_NOBOM', 'ANSI', 'UTF16LE', 'UTF16BE', 'UTF16LE_NOBOM', 'UTF16BE_NOBOM', 'ASCII')]
   [string]$convertTo,
   [ValidateSet('MACOS', 'UNIX', 'DOS')]
   [string]$endOfLineTo,
   [string]$outputPattern,
   [int]$silenceLevel=1,
   [switch]$replaceOriginalFile,
   [switch]$recurse

)

$files=@();
if ($recurse) {
    $files=@(Get-ChildItem -Path $folderToAnalyse -filter $filter -recurse);
} else {
    $files=@(Get-ChildItem -Path $folderToAnalyse -filter $filter);
}

$arguments="-s "+$silenceLevel+" ";
if ($convertTo.Length -gt 0) {
    $arguments += "-c "+$convertTo +" ";
}
if ($endOfLineTo.Length -gt 0) {
    $arguments += "-e "+$endOfLineTo +" ";
}

if ($replaceOriginalFile) {
    $arguments += "-o `"SAME_AS_INPUT`" ";
}

if ($outputPattern.Length -gt 0) {
    $arguments += "-p "+$outputPattern +" ";
}

Start-Process -FilePath ".\DetectEncoding.exe" -NoNewWindow -Wait -ArgumentList "-a";

foreach($file in $files) {
    $argumentsForFile = $arguments+"-f `""+$file.fullname+"`""; 
            

    Start-Process -FilePath ".\DetectEncoding.exe" -NoNewWindow -Wait -ArgumentList $argumentsForFile;
    write-host "";
}
