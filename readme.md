docker build -t gabrieldeoliveiraest/projetofinal2_web:latest -f Dockerfile .
docker login -u "gabrieldeoliveiraest" -p "password" docker.io
docker push gabrieldeoliveiraest/projetofinal2_web:latest


docker build -t sant77/mqtt-touch-lamp-conection-user-back:0.0.2 .
docker build -t sant77/mqtt-touch-lamp-conection-page:0.0.3 .
docker compose up -d