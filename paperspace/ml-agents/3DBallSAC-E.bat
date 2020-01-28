@ECHO OFF
SET variable=3DBall
SET /A "counter=1"
FOR /D %%i in (models\%variable%*) DO (SET /A "counter+=1"   )
ECHO New training set: %variable%%counter%
mlagents-learn ../config/sac_trainer_config.yaml --env=3DBall/3DBall --train --run-id=%variable%%counter%