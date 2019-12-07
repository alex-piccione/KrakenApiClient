:: copy the secret  API key (not part of the repository) into the build folder
IF EXIST "$(ProjectDir)api keys.secret.yaml" "$(TargetDir)" (copy "$(ProjectDir)api keys.secret.yaml" "$(TargetDir)")