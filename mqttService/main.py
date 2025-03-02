from fastapi import Depends, FastAPI

from controller import mqtt_user

app = FastAPI()


app.include_router(mqtt_user.router)
