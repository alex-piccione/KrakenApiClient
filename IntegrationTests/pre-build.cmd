:: copy the secret  API key (not part of the repository) into the build folder
set ProjectDir=%~1
set TargetDir=%~2

IF EXIST "%ProjectDir%api keys.secret.yaml" (copy "%ProjectDir%api keys.secret.yaml" "%TargetDir%")