# Mqtt device conection 💡

This project involves the integration of multiple MQTT devices through a web interface. The platform allows administrators to manage different MQTT-connected devices efficiently.

The primary device—and the first implemented—is an MQTT-controlled lamp. Through an interactive dashboard, users can control the lamp's functionality. Additionally, the system includes a feature to connect multiple lamps, enabling a synchronized lighting experience for special occasions.


# Folder 📂

- *docker-compose*:  Deployment instructions for the server.
- *frontend*: User interface built with Angular.
- *mqttService* : Python service handling MQTT server connections
- *userService* : .NET service managing users and devices.

#  Run proyect 🏃

You can run the project locally or configure GitHub secrets to enable CI/CD pipeline deployment.

## Git hub actions :octocat:

The project includes a CI/CD pipeline that automates deployment to an AWS server. The workflow consists of two main stages:

1- Build and Push: Creates Docker images and uploads them to Docker Hub.

2- Deploy: Copies the necessary files to the server and executes docker-compose commands.


## Docker compose 🐳

To run the project locally, execute the following commands:

```bash
docker compose build
```
```bash
docker compose up -d
```
Note: Ensure the image versions in docker-compose.yml are updated.

# Sources & Credits
- CSS Animations: [CSS Crafter](https://csscrafter.com/css-particle-effects/)
- SVG Icons: [Font Awesome](https://fontawesome.com/)