# 🚀 InfraPilot

InfraPilot is an AI-powered Infrastructure Automation Platform that enables users to interact with infrastructure systems using natural language.

Instead of manually writing Kubernetes manifests, inspecting cluster resources, debugging workloads, or interacting with container runtimes, users can describe their intent in plain English and InfraPilot orchestrates specialized AI agents to perform the required actions.

The platform combines Multi-Agent AI, Tool Calling, Human-in-the-Loop approvals, and Durable Workflow Execution to safely automate infrastructure operations.

## What Can InfraPilot Do?

Examples:

* Generate Kubernetes manifests
* Debug failing Kubernetes workloads
* Investigate pod crashes
* Analyze deployment issues
* Inspect cluster resources
* Explain infrastructure failures
* Review cluster health
* Interact with Docker containers
* Inspect container runtime state
* Perform operational diagnostics
* Generate infrastructure recommendations

## Why Kubernetes and Docker Access?

InfraPilot agents do not rely solely on LLM knowledge.
To provide accurate infrastructure insights, agents must interact with live environments.

For example:

1. Kubernetes Integration
2. Container Runtime Integration

Supported runtimes:

* Docker
* Podman

You can check all the exposed tools from here: https://github.com/puneet-goel/InfraPilot/tree/main/src/Servers

## Key Capabilities

* Multi-Agent Orchestration
* Human-in-the-Loop Approvals
* Tool Calling
* Durable Workflow Execution
* Real-Time Event Streaming
* Agent Memory Persistence
* Workflow Resumption
* Infrastructure Diagnostics
* Kubernetes Integration
* Container Runtime Integration
* Execution Audit Trail

InfraPilot is designed to act as an AI-powered infrastructure copilot while maintaining visibility, auditability, and human control over sensitive operations.

## Running InfraPilot with Docker

### Environment Variables

InfraPilot is configured entirely through environment variables.

| Variable                 | Description                                                                                                                                        |
| ------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ASPNETCORE_URLS`        | URL binding used by ASP.NET Core inside the container. `http://+:8080` exposes the application on port `8080`.                                     |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core runtime environment. Typical values are `Development`, and `Production`.                                                   |
| `LLM_CRED`               | API key used to authenticate with the configured Large Language Model provider.                                                                    |
| `LLM_MODEL`              | Model used by agents for planning, reasoning, and tool calling. Example: `gemini-2.5-flash`.                                                       |
| `LLM_BASE_URL`           | Base URL for the OpenAI-compatible API endpoint used by the application. Allows switching between OpenAI, Gemini, Azure OpenAI, local models, etc. |
| `POSTGRES`               | PostgreSQL connection string used for workflow persistence, agent memory, execution history, approvals, and audit logs.                            |
| `KUBE_HOST`              | Kubernetes API Server endpoint used by infrastructure agents. Example: Kind, Minikube, AKS, EKS, or GKE API server URL.                            |
| `KUBE_TOKEN`             | Service Account bearer token used by InfraPilot to authenticate against the Kubernetes API.                                                        |
| `CONTAINER_RUNTIME`      | Container runtime used by infrastructure agents. Supported values: `docker`, `podman`.                                                             |
| `CONTAINER_SOCKET`       | Unix socket used to communicate with the container runtime.                                                                                        |

InfraPilot does not require both Kubernetes and Container Runtime integrations to be configured.

The runtime environment is determined dynamically during workflow execution based on the task being performed.

You may configue Kubernetes only or Container Runtime only or Both.

---

### Container Runtime Socket

Docker:

```yaml
CONTAINER_RUNTIME: docker
CONTAINER_SOCKET: unix:///var/run/docker.sock
```

Podman:

```yaml
CONTAINER_RUNTIME: podman
CONTAINER_SOCKET: unix:///run/user/1000/podman/podman.sock
```

---

### Volume Mounts

Docker socket access:

```yaml
volumes:
  - /var/run/docker.sock:/var/run/docker.sock
```

Podman socket access:

```yaml
volumes:
  - /run/user/1000/podman/podman.sock:/run/user/1000/podman/podman.sock
```

InfraPilot uses these sockets to allow infrastructure agents to inspect and manage containers through the selected runtime.

---

### Example Configuration

```yaml
environment:
  ASPNETCORE_URLS: http://+:8080
  ASPNETCORE_ENVIRONMENT: Production

  LLM_CRED: YOUR_API_KEY
  LLM_MODEL: gemini-2.5-flash
  LLM_BASE_URL: https://generativelanguage.googleapis.com/v1beta/openai/

  POSTGRES: Host=postgres;Port=5432;Database=infrapilot;Username=postgres;Password=postgres123

  KUBE_HOST: https://host.docker.internal:63408
  KUBE_TOKEN: YOUR_KUBERNETES_TOKEN

  CONTAINER_RUNTIME: docker
  CONTAINER_SOCKET: unix:///var/run/docker.sock
```


### Create Kubernetes Service Account

InfraPilot interacts with Kubernetes clusters through AI agents. To allow the platform to inspect and manage cluster resources, a Service Account with the required permissions must be created.

Apply the following manifest: https://github.com/puneet-goel/InfraPilot/blob/main/deployments/service-account.yaml

```bash
kubectl apply -f service-accpunt.yaml
```

---

### Generate Access Token

Generate a token that InfraPilot will use to authenticate with the Kubernetes API server.

```bash
kubectl create token infrapilot-admin -n kube-system
```

Copy the generated token and configure it in the application environment variables.

---

### Verify Permissions

Verify that the service account has access to the cluster:

```bash
kubectl auth can-i list pods \
  --as=system:serviceaccount:kube-system:infrapilot-admin
```

Expected output:

```text
yes
```

---

### Start InfraPilot

```bash
docker compose up -d
```

Access:

Frontend:

```text
http://localhost:8080/InfraPilot/
```

Backend:

```text
http://localhost:8080/swagger/index.html
```

---

## Why is the Kubernetes Service Account Required?

InfraPilot contains infrastructure-focused AI agents capable of:

* Inspecting cluster resources
* Listing pods
* Reading deployments
* Investigating failures
* Executing infrastructure operations
* Performing cluster diagnostics

The Kubernetes API requires authentication for all operations.

Instead of relying on a user's local kubeconfig, InfraPilot uses a dedicated Service Account and Bearer Token. This allows the platform to:

* Run inside Docker containers
* Execute in CI/CD environments
* Connect to remote clusters
* Operate consistently across environments

The Service Account acts as the identity used by InfraPilot when communicating with Kubernetes.


## AI Architecture

InfraPilot implements the following AI concepts:

### Agentic AI

Autonomous agents capable of planning and tool execution.

### Multi-Agent Systems

Multiple specialized agents collaborate to complete a workflow.

### Tool Calling

Agents invoke external tools through structured function calling.

### Human-in-the-Loop (HITL)

Sensitive operations require human approval before execution.

### Workflow Planning

Natural language requests are converted into executable workflow plans.

### Agent Memory

Conversation history is persisted and restored across executions.

### Durable Execution

Workflows can pause and resume without losing context.

### Event Driven Architecture

Workflow events are streamed in real-time to the UI.

### Retrieval-Augmented Decision Making

Agents can leverage external systems and infrastructure state before taking actions.

### AI Safety Controls

Approval gates prevent destructive operations from executing automatically.

## Tech Stack

### Frontend

- React
- TypeScript
- Material UI
- MUI Data Grid
- TanStack Query
- Vite

### Backend

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Hangfire

### AI

- Microsoft Agent Framework
- OpenAI Compatible Models
- Tool Calling
- Structured Outputs
- Model Control Protocol
- Agent To Agent (A2A)

### Infrastructure

- Docker
- Podman
- Kubernetes

## Protocols & Standards

- MCP (Model Context Protocol)
- Function Calling
- Server Sent Events (SSE)
- REST APIs
- OpenTelemetry Ready Architecture
- JSON Schema Based Tool Definitions

## Workflow Lifecycle

1. User submits request
2. Planner generates execution plan
3. Orchestrator dispatches agents
4. Agents invoke tools
5. Approval gates pause execution if required
6. User approves/rejects actions **(Human in the Loop)**
7. Workflow resumes
8. Results are streamed to UI
9. Execution history is persisted
