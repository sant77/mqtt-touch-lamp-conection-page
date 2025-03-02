from fastapi import APIRouter
from dotenv import load_dotenv
import paho.mqtt.publish as publish
import os

load_dotenv()
MQTT_BROKER = os.environ.get("MQTT_BROKER") 
MQTT_PORT = os.environ.get("MQTT_PORT")
MQTT_CLIENT_ID = os.environ.get("MQTT_CLIENT_ID")
USER_NAME = os.environ.get("USER_NAME")
router = APIRouter()


@router.get("/send_mqtt_message")
async def send_mqtt_message(mqtt_topic: str, mqtt_message:str):
    
        # Publicar el mensaje MQTT
    publish.single(
        topic=mqtt_topic,
        payload=mqtt_message,
        hostname=MQTT_BROKER,
        port= int(MQTT_PORT),
        auth={"username":USER_NAME}
    )
    return {"status": "success", "message": f"Mensaje enviado al tópico {mqtt_topic}"}

 

