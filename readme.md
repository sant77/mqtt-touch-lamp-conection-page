docker build -t gabrieldeoliveiraest/projetofinal2_web:latest -f Dockerfile .
docker login -u "gabrieldeoliveiraest" -p "password" docker.io
docker push gabrieldeoliveiraest/projetofinal2_web:latest