@ECHO OFF
SET variable=PushBlock
SET /A "counter=1"
FOR /D %%i in (models\%variable%*) DO (SET /A "counter+=1"   )
ECHO New training set: %variable%%counter%
mlagents-learn config/trainer_config.yaml --train --run-id=%variable%%counter%