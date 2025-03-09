from fastapi import Depends, FastAPI
from fastapi.middleware.cors import CORSMiddleware
from controller import mqtt_user

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Permite todos los orígenes
    allow_credentials=True,
    allow_methods=["*"],  # Permite todos los métodos
    allow_headers=["*"],  # Permite todos los headers
)


app.include_router(mqtt_user.router)
