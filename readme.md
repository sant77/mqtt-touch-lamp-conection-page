# Mqtt device conection

This proyect included the integration of all mqtt devices through a web page.
The page allow to admin the different mqtt devices.

The main device and first is a mqtt lamp. With this lamp you can controller with a dasboard, also included a
function that give the possibility to conect with other lamps to have a near experience with your special.


# Folder 

- *docker-compose*: instrution to deploy the server
- *frontend*: Interfaz on Angular 
- *mqttService* : Service on Python which controll the conection with mqtt server
- *userService* : Service on .net which controll the user and devices 

#  
```bash
docker build -t sant77/mqtt-touch-lamp-conection-user-back:0.0.2 .
```
```bash
docker build -t sant77/mqtt-touch-lamp-conection-page:0.0.3 .
```
```bash
docker build -t sant77/mqtt-touch-lamp-conection-mqtt-back:0.0.1 .
```
```bash
docker login -u "user" -p "password" docker.io
```
```bash
docker push sant77/mqtt-touch-lamp-conection-user-back:0.0.2
```
```bash
docker push sant77/mqtt-touch-lamp-conection-page:0.0.3
```
```bash
docker compose build
```
```bash
docker compose up -d
```

https://csscrafter.com/css-particle-effects/
