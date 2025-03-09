from fastapi import APIRouter, Depends, Security, HTTPException, status
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
from dotenv import load_dotenv
from jose import jwt, JWTError
import paho.mqtt.publish as publish
import os

load_dotenv()

# Configuración MQTT
MQTT_BROKER = os.getenv("MQTT_BROKER")
MQTT_PORT = int(os.getenv("MQTT_PORT", 1883))
MQTT_CLIENT_ID = os.getenv("MQTT_CLIENT_ID")
USER_NAME = os.getenv("USER_NAME")

# Configuración JWT
SECRET_KEY = "TuClaveSecretaSuperSeguraDe32CaracteresOmas"
ALGORITHM = "HS256"

# Crear el router
router = APIRouter()

# Configurar autenticación con FastAPI
security = HTTPBearer()

# Función para obtener el usuario a partir del JWT
async def get_current_user(
    credentials: HTTPAuthorizationCredentials = Security(security)
):
    token = credentials.credentials  # Extrae el token sin "Bearer "

    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="Could not validate credentials",
        headers={"WWW-Authenticate": "Bearer"},
    )
    
    try:
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM], options={"verify_aud": False})
       
        user_id = payload.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
        user_name = payload.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
        user_email = payload.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
        
        if user_id is None or user_name is None or user_email is None:
            raise credentials_exception

        return user_id, user_email, user_name

    except JWTError as e:
        print(f"Error de JWT: {e}")
        raise credentials_exception

# Ruta protegida que envía mensajes MQTT
@router.get("/send_mqtt_message")
async def send_mqtt_message(
    mqtt_topic: str,
    mqtt_message: str,
    user: tuple = Depends(get_current_user)  # Obtener usuario autenticado
):
    _, _, user_name = user

    # Publicar el mensaje MQTT
    publish.single(
        topic=f"{user_name}/{mqtt_topic}",
        payload=mqtt_message,
        hostname=MQTT_BROKER,
        port=MQTT_PORT,
        auth={"username": USER_NAME}
    )

    return {"status": "success", "message": f"Mensaje enviado al tópico {mqtt_topic}"}

