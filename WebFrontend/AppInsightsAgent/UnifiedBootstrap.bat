REM /*********************************************************
REM *                                                        *
REM *    Copyright (C) Microsoft. All rights reserved.       *
REM *                                                        *
REM *********************************************************/
@echo off

:: Set appropriate execution policy on the host machine
set ExecutionPolicyLevel=RemoteSigned
for /F "usebackq" %%i in (`powershell -noprofile -command "Get-ExecutionPolicy"`) do (
    set ExecutionPolicy=%%i
    if /I "%%i"=="Unrestricted" goto :AllIsWell
    if /I "%%i"=="RemoteSigned" goto :AllIsWell
    Powershell.exe -NoProfile -Command "Set-ExecutionPolicy RemoteSigned" < NUL >> NUL 2>> NUL
)

:AllIsWell

Powershell.exe -NoProfile -Command "& '%~dp0UnifiedBootstrap.ps1'" < NUL  >> NUL 2>> NUL
echo "done" >"%ROLEROOT%\startup.task.done.sem"

exit 0