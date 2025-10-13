# GTrack (Ground Tracking Network)

A distributed network of ground stations for satellite communication and control, consisting of tracking nodes (`GTrack-Node`), ground stations (`GTrack-Station`), and mission control interfaces (`MCC-Gateway`, `GTrack-Control`).

## Architecture Overview

GTrack is a hierarchical network for reliable satellite communication and automated antenna control.

### Structural Diagram

![Structural Diagram](assets/structural-diagram.png)

## System Components

### MCC Gateway
Server for automated flight control. Connects to multiple `GTrack-Node` instances.

### GTrack Control
Desktop app for manual control if the automated server is unavailable.

### GTrack Node
Software deployed at each control point. Acts as a gateway between the Mission Control Center and stations.

**Features:**
- Authentication of connections
- Session management
- Data routing
- TLE processing (Gpredict)
- Telemetry storage

### GTrack Station
Software for managing station hardware (antennas, radio modules). Connects to one Node.

**Features:**
- Hardware control (GNU Radio, rotators)
- Execute Az/El/Doppler commands
- Telemetry processing
- Equipment configuration

## Protocols and Data

- Inter-component: gRPC, TCP sockets  
- Hardware: device-specific protocols  

Data types: telecommands, telemetry, equipment control commands.
