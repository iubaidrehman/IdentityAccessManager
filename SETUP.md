# Identity & Access Management Microservice - Setup Guide

## 🚀 Quick Start

This guide will help you set up and run the complete Identity & Access Management microservice solution locally.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **Docker & Docker Compose** - [Download here](https://www.docker.com/products/docker-desktop/)
- **SQL Server** (optional - Docker will provide this)

## 🏗️ Project Structure

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

## 🔧 Setup Instructions

### Option 1: Docker Compose (Recommended)

1. **Clone and Navigate**
   ```bash
   cd IdentityAccessManager
   ```

2. **Start All Services**
   ```bash
   cd infrastructure
   docker-compose up -d
   ```

3. **Access the Application**
   - **Frontend**: http://localhost:3000
   - **API Gateway**: http://localhost:5000
   - **Identity Service**: https://localhost:5001
   - **Users Service**: https://localhost:5002
   - **Seq Logging**: http://localhost:5341

### Option 2: Local Development

#### Backend Setup

1. **Restore Dependencies**
   ```bash
   cd backend
   dotnet restore
   ```

2. **Update Database Connection**
   Edit the connection strings in:
   - `src/IdentityAccessManager.Identity/appsettings.json`
   - `src/IdentityAccessManager.Users/appsettings.json`

3. **Run Database Migrations**
   ```bash
   # Identity Service
   cd src/IdentityAccessManager.Identity
   dotnet ef database update

   # Users Service
   cd ../IdentityAccessManager.Users
   dotnet ef database update
   ```

4. **Start Services**
   ```bash
   # Terminal 1 - Identity Service
   cd backend/src/IdentityAccessManager.Identity
   dotnet run

   # Terminal 2 - Users Service
   cd backend/src/IdentityAccessManager.Users
   dotnet run

   # Terminal 3 - API Gateway
   cd backend/src/IdentityAccessManager.Gateway
   dotnet run
   ```

#### Frontend Setup

1. **Install Dependencies**
   ```bash
   cd frontend
   npm install
   ```

2. **Start Development Server**
   ```bash
   npm run dev
   ```

## 🔐 Default Credentials

The system comes with pre-seeded users:

- **Admin User**
  - Email: `admin@example.com`
  - Password: `Admin123!`
  - Role: `Admin`

- **Regular User**
  - Email: `user@example.com`
  - Password: `User123!`
  - Role: `User`

## 🌐 API Endpoints

### Identity Service (OAuth2/OIDC)
- **Discovery**: `https://localhost:5001/.well-known/openid_configuration`
- **Token**: `https://localhost:5001/connect/token`
- **User Info**: `https://localhost:5001/connect/userinfo`

### Users Service (Protected)
- **Current User**: `GET /api/users/me`
- **All Users**: `GET /api/users` (Admin only)
- **Update User**: `PUT /api/users/me`
- **Delete User**: `DELETE /api/users/{id}` (Admin only)

### API Gateway
- **All routes**: `http://localhost:5000/api/*`

## 🔧 Configuration

### Environment Variables

#### Backend Services
```bash
# Database
ConnectionStrings__DefaultConnection=Server=localhost;Database=IdentityAccessManager_Identity;Trusted_Connection=true;TrustServerCertificate=true

# JWT Settings
Jwt__Authority=https://localhost:5001
Jwt__Audience=api1
Jwt__Issuer=https://localhost:5001

# Google OAuth (Optional)
Authentication__Google__ClientId=your_google_client_id
Authentication__Google__ClientSecret=your_google_client_secret
```

#### Frontend
```bash
# API Configuration
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_AUTH_URL=https://localhost:5001
```

## 🧪 Testing

### Backend Tests
```bash
cd backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
npm test
```

## 🐳 Docker Commands

### Build Images
```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build identity
```

### View Logs
```bash
# All services
docker-compose logs

# Specific service
docker-compose logs identity
```

### Stop Services
```bash
docker-compose down
```

## 🔍 Troubleshooting

### Common Issues

1. **Port Conflicts**
   - Ensure ports 3000, 5000, 5001, 5002, 1433, 5341 are available
   - Change ports in `docker-compose.yml` if needed

2. **Database Connection**
   - Verify SQL Server is running
   - Check connection strings in `appsettings.json`
   - Ensure database exists

3. **SSL Certificate Issues**
   - For development, the services use HTTP
   - In production, configure proper SSL certificates

4. **Frontend Build Issues**
   - Clear node_modules: `rm -rf node_modules && npm install`
   - Clear Next.js cache: `rm -rf .next`

### Logs and Monitoring

- **Seq Logging**: http://localhost:5341
- **Application Logs**: Check console output or Docker logs
- **Database**: Use SQL Server Management Studio or Azure Data Studio

## 🚀 Production Deployment

### Kubernetes
```bash
cd infrastructure/kubernetes
kubectl apply -f .
```

### Azure/AWS
- Use the provided ARM templates or CloudFormation
- Configure proper SSL certificates
- Set up monitoring and logging

## 📚 Next Steps

1. **Customize Authentication**
   - Add social login providers
   - Implement MFA
   - Configure password policies

2. **Extend Functionality**
   - Add more microservices
   - Implement event sourcing
   - Add real-time notifications

3. **Security Hardening**
   - Configure proper SSL/TLS
   - Implement rate limiting
   - Add security headers

4. **Monitoring & Observability**
   - Set up Prometheus + Grafana
   - Configure health checks
   - Implement distributed tracing

## 🤝 Support

For issues and questions:
1. Check the troubleshooting section
2. Review the logs
3. Create an issue in the repository

## 📄 License

MIT License - see [LICENSE](LICENSE) for details. 