# Triportunity

Welcome to **Triportunity** — your go-to solution for efficient and organized carpooling. Whether you're a Triportunity Administrator, Driver, or Passenger, Triportunity is designed to enhance your ride-sharing experience, simplify coordination, and maximize efficiency.

## 🚀 Key Features

### 📋 Server Functionalities
- **Event Reception**: Receive and store travel events from the main server.
- **Travel Filtering**: Filter and sort travel events based on multiple criteria such as price and destination. Supports at least three criteria with combinable options.
- **User Event Tracking**: Track user login events and generate statistical reports.
- **Report Generation**: Request, check status, and view reports summarizing travel data.

### 📊 Administrative Server
- **Travel Management**: Add, remove, or modify travel entries with existing constraints.
- **Travel Ratings**: Query ratings and feedback for specific travels.
- **Upcoming Travel Display**: Print details of upcoming travels in real-time as they are generated.
- **Multi-User Console**: Supports multiple administrative users simultaneously via a console interface.
  - **Note**: The administrative console utilizes **gRPC** for seamless and efficient communication with the main server, ensuring real-time updates and robust interaction.

### 🔢 Statistics Server
- **Event Reception**: Receives and stores travel events from the main server.
- **Event Filtering**: Provides filtering of travel events based on user-defined criteria.
- **User Event Tracking**: Records and tracks user login events for statistical analysis.
- **Report Generation**: Generates and manages statistical reports based on travel data.
  - **Note**: The Statistics Server employs **RabbitMQ** for handling event messages and processing, ensuring reliable and scalable event management.

### 🔗 Communication and Integration
- **Remote Access**: Expose server functionalities through REST API, allowing remote access and management.
- **Parallel Processing**: Utilize parallel processing for calculating travel statistics and generating reports.

## 🛠️ Technologies Used
- **gRPC**: Used for efficient remote procedure calls and inter-service communication between the administrative console and the main server.
- **RabbitMQ**: Utilized by the Statistics Server for message brokering and handling events, ensuring reliable and scalable event processing.
- **REST API**: For remote service access and integration.

## 📈 Getting Started

### Setting Up the Server
1. **Install Dependencies**: Ensure all necessary dependencies for gRPC, RabbitMQ, and REST API are installed.
2. **Configure Services**: Set up the configurations for the server, including gRPC endpoints and RabbitMQ settings.
3. **Run the Server**: Start the server and ensure it is running correctly by testing its endpoints.

### Running the Client
1. **Connect to Server**: Configure the client application to connect to the server using the provided gRPC and REST endpoints.
2. **Test Functionalities**: Verify that the client can interact with all server functionalities, including event reception and travel management.

### Using the Administrative Console
1. **Log In**: Access the console using administrative credentials.
2. **Manage Travels**: Add, update, or delete travel records as needed.
3. **View Upcoming Travels**: Monitor and display upcoming travels in real-time.

### Using the Statistics Server
1. **Connect to RabbitMQ**: Ensure the Statistics Server is properly connected to RabbitMQ for event processing.
2. **Track Events**: Monitor and filter travel events based on defined criteria.
3. **Generate Reports**: Request and manage statistical reports on travel data.

### Testing and Documentation
- **Postman Collection**: A collection of Postman requests is included for testing server endpoints. Refer to the provided documentation for details on using these requests.
- **Documentation**: Comprehensive documentation is available, detailing the architecture, design decisions, and setup instructions.

## 🐣 Getting Started
1. **Clone the Repository**: `git clone <repository-url>`
2. **Install Dependencies**: Follow the setup instructions in the `README.md` and configuration files.
3. **Run the Application**: Start the server and client applications using the provided scripts and configurations.

## 📅 Release Information
The code and executables are available in the release section of this repository. Make sure to check the latest release for updates and new features. For any questions or issues, please refer to the [issues page](path/to/issues) or contact the development team.

Happy managing with Triportunity!

