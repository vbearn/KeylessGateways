# Keyless Gateways

A .NET application to open doors everywhere! Ditch the old physical keys and get digital. Remotely manage doors for your home, workplace and rental properties. Grant and revoke access to people you want in few seconds and be amazed on how fast and easy it is.

This project is built with scalability in mind, based on a microservices architecture and Docker containers.

## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) docker in your environment. Clone the repository in your local machine, open up your favorite terminal and then execute the below commands on the root repo directory:

```powershell
cd src
docker-compose build
docker-compose up
```

### Main Api Gateway

Main Api Gateway (Ocelot) is accesible from http://localhost:45000, and contains the URLs of internal microservices, which are exposed for showcase and development purposes.

### Postman

All Endpoints are exported as a Postman collection and can be accessed from `postman` directory. To execure APIs, the `base_address` variable in Postman should be set to `http://localhost:45000`

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide5.JPG)


## Architecture overview

This application is cross-platform, developed in .NET 6, and based on a microservice oriented architecture implementation with multiple autonomous microservices (each one owning its own data/DB).

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide1.JPG)

### Scalability

The reason for using microservice oriented architecture was the requirement for the scalability of the application. These architectural concerns in mind were:

- **Doors should be instantly opened upon user's request**. It frustrates the people if they command the door in front of them to be opened, only to get errors because of high loads on the backend servers.
  - A separate **Door Entrance microservice** is implemented, which should be Fast, Highly Available and Responsive, and should be scaled-out as needed, preferably in geolocations close to the end users.

- The **Management microservice** dedicated to Admins (to configure users, doors, access list, ...), has relatively lesser need for availablity and responsiveness, and can be scaled in a lower tier.

- Users are authenticated via a dedicated **Identity microservice**, with JWTs issued that are authorized accross all other services.
  - Currently an implementation based on **Microsoft Identity** server is used for the sake of simplicity of the application.

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide2.JPG)


### Contexts


![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide3.JPG)

### User Flows

The application has two types of user:

- **Admins:** can perform a wide range of actions including managing everything through the management service (define doors and users, authorize users to access doors), view entrance histories for all users, and open doors on behalf of other users (This policy is configurable via `IDoorAccessPolicy` interface implementations).
- **Non-admins**: can only open doors for which they are authorized, and can see **their own** entrance history.

![](https://raw.githubusercontent.com/vbearn/KeylessGateways/master/images/Slide4.JPG)

