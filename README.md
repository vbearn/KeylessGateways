# Keyless Gateways

A .NET 5 application to open doors everywhere! Ditch the old physical keys and get digital. Remotely manage doors for your home, workplace and rental properties. Grant and revoke access to people you want in few seconds and be amazed on how fast and easy it is.

This project is built with scalability in mind, based on a microservices architecture and Docker containers.

## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) docker in your environment. Clone the repository in your local machine, open up your favorite terminal and then execute the below commands on the root repo directory:

```powershell
cd src
docker-compose build
docker-compose up
```

You should be able to browse different components of the application by using the below URLs :

- Main ApiGateway which contains all REST endpoints: http://localhost:45000
- Identity Microservice Swagger: http://localhost:45010
- Management Microservice Swagger: http://localhost:45011
- Door Entrance Microservice Swagger: http://localhost:45012

## Architecture overview

This application is cross-platform, developed in .NET 5, and based on a microservice oriented architecture implementation with multiple autonomous microservices (each one owning its own data/db).

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide1.JPG)

### Scalability

The reason for using microservice oriented architecture was the requirement for the scalability of the application. These architectural concerns in mind were:

- **The user should NOT wait for more than a few seconds for a door to open**. It frustrates the people, and cause numerous problems, if commanding a door in front of a person to be opened, only to get errors because of high loads on the backend servers. To address this issue:
  - We used a dedicated **Door Entrance microservice**, which should be Highly Available and Responsive, and should be scaled-out as needed, preferably in geolocations close to the end users.
- The admins (the ones who manage users, doors, and authorize users to open doors) have relatively higher tolerance for service outages and resource congestions. It is not so much terrible if the management panel opens with higher latency, as it is accessed much less than the entrance service.
  - We used a dedicated **Management microservice**, which can be scaled-out as needed, but is **not as sensitive as the entrance service**. In other words, in critical resource shortage situations, **the resource priority is given to the entrance service rather than this management service**
- All users should authenticated using an Identity server, with JWTs issued that are accepted through all microservices.
  - We used a dedicated **Identity microservice**, to off-load the resource-consuming task of user management and authentication from our other operational services.
  - We have currently used an implementation based on **Microsoft Identity** server for the sake of simplicity of the application, but later it can later be out-sourced to use Azure Active Directory in a fully cloud-native approach.

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide2.JPG)

Each microservice implements a data-driven, CRUD approach, with traditional layers (Controller, Services, Database). We have an Api Gateway built using Ocelot which exposes the services to the outside.

DDD/CQRS pattern was not used globally, due to simplicity of the application (as of now) and the overhead it brought. But CQRS is still used in parts of the application (inter-service communications).

### Contexts and Communications

The application uses these communication protocols:

- **Http protocol** for communication of the client to the Api Gateway and then to (re-routing) to the microservices. The communication between Api Gateway and microservices are **planned to be upgraded** to gRPC, using [Envoy](https://www.envoyproxy.io/) as recommended by Microsoft.
- **RabbitMQ** as an Event Bus (implemented via [MassTransit](https://masstransit-project.com/)) for asynchronous communication for data updates propagation across microservices

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide3.JPG)

### User Flows

The application has two types of user:

- **Admins:** can perform a wide range of actions including managing everything through the management service (define doors and users, authorize users to access doors), view entrance histories for all users, and open doors on behalf of other users (This policy is easily configurable via `IDoorAccessPolicy` interface implementations).
- **Non-admins**: can open doors for which they are authorized, and can see **their own** entrance history.

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide4.JPG)

### Postman

Some of the most important endpoints are exported with Postman and can be accessed from `postman` directory

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide5.JPG)
