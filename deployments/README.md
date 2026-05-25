# InfraPilot Docker Commands Reference

## Build Docker Image

```bash
# Run from repository root
# -f => Dockerfile location
# .  => build context (entire repo)
docker build -f src/Api/Dockerfile -t infrapilot .
```

---

## Run Docker Container Locally

```bash
# Run container and expose port 8080
# Pass environment variables to ASP.NET app

docker run -p 8080:8080 \
  -e LLM_CRED="YOUR_API_KEY" \
  -e LLM_MODEL="gemini-2.5-flash" \
  -e LLM_BASE_URL="https://generativelanguage.googleapis.com/v1beta/openai/" \
  -e POSTGRES="Host=YOUR_HOST;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=VerifyFull;Channel Binding=Require;" \
  infrapilot
```

---

## Login to Docker Hub

```bash
# Login using Docker Hub username and access token
docker login
```

---

## Tag Docker Image

```bash
# Tag local image with Docker Hub namespace
# docker.io/library/infrapilot
# library/ means Docker thinks this is an official public image namespace, not your account.
# That’s the problem.

# You must push using your Docker Hub username.
docker tag infrapilot puneetgoel16/infrapilot:latest
```

---

## Push Docker Image

```bash
# Push image to Docker Hub
docker push puneetgoel16/infrapilot:latest
```