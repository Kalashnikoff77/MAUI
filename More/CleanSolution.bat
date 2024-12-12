@echo off
for /d /r .. %%d in (bin,obj,pkg,pkgobj) do @if exist "%%d" rd /s/q "%%d"