# Identity & Access Management Microservice

A modern, secure, scalable identity and access management (IAM) solution built with .NET 8 microservices, Next.js frontend, and YARP API Gateway.

## 🏗️ Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Next.js FE    │    │   YARP Gateway  │    │  .NET Services  │
│   (OAuth PKCE)  │◄──►│   (Routing)     │◄──►│  (Microservices)│
└─────────────────┘    └─────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌─────────────────┐
                       │   SQL Server    │
                       │   (Persistence) │
                       └─────────────────┘
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server (or Docker)
- Node.js 18+
- Docker & Docker Compose

### Running the Solution

1. **Backend Services**
   ```bash
   cd backend
   dotnet restore
   dotnet run --project src/IdentityAccessManager.Gateway
   ```

2. **Frontend**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

3. **Docker Compose (All Services)**
   ```bash
   docker-compose up -d
   ```

## 📁 Project Structure

```
IdentityAccessManager/
├── backend/                          # .NET Backend Services
│   ├── src/
│   │   ├── IdentityAccessManager.Gateway/     # YARP API Gateway
│   │   ├── IdentityAccessManager.Identity/    # Identity Service (OAuth2/OIDC)
│   │   ├── IdentityAccessManager.Users/       # User Profile Service
│   │   └── IdentityAccessManager.Shared/      # Shared Contracts & Models
│   ├── tests/                        # Backend Tests
│   └── docker/                       # Docker Configurations
├── frontend/                         # Next.js Frontend
│   ├── src/
│   │   ├── app/                      # Next.js App Router
│   │   ├── components/               # React Components
│   │   └── lib/                      # Utilities & Config
│   └── public/                       # Static Assets
├── infrastructure/                   # Infrastructure & DevOps
│   ├── docker-compose.yml           # Local Development
│   ├── kubernetes/                   # K8s Manifests
│   └── scripts/                      # Deployment Scripts
└── docs/                            # Documentation
```

## 🔐 Security Features

- **OAuth2/OIDC** compliant authentication
- **PKCE** flow for frontend security
- **JWT** access & refresh tokens
- **Role-based** and **policy-based** authorization
- **API Gateway** with token validation
- **Multi-tenant** support ready

## 🧪 Testing Strategy

- **Unit Tests**: xUnit + Moq
- **Integration Tests**: Testcontainers
- **E2E Tests**: Playwright (planned)

## 🚀 Deployment

- **Local**: Docker Compose
- **Production**: Kubernetes (k3s/EKS)
- **CI/CD**: GitHub Actions / Azure DevOps

## 📚 Documentation

- [API Documentation](./docs/api.md)
- [Architecture Guide](./docs/architecture.md)
- [Security Guide](./docs/security.md)
- [Deployment Guide](./docs/deployment.md)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

MIT License - see [LICENSE](LICENSE) for details. 