rem xcopy /Y "node_modules\mermaid\dist\mermaid.min.js" "docs\plugins\"
docfx metadata
docfx build --serve
pause
