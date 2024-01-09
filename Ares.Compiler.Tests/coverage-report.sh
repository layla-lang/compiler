#!/bin/bash
rm -rf ./coverage
dotnet test --no-restore --collect:"XPlat Code Coverage" --results-directory="./.coverage"
reportgenerator \
    "-reports:.coverage/**/*.cobertura.xml" \
    -targetdir:".coveragereport" \
    -reporttypes:Html