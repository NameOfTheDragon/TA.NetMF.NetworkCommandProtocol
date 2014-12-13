# General Purpose Network Command Protocol and Server for .Net Micro Framework #

This project implements a general purpose network-based command/response protocol which can serve as the front end for a variety of device control applications. The primary use-case we had in mind when designing this was for astronomical instrumentation that can be controlled by an [ASCOM](http://ascom-standards.org "Astronomy Common Object Model") driver.

The code is structured using object oriented design principles, so that the command protocol can be very easily re-purposed for different projects. The protocol is designed to be human-readable as well as machine-readable.

### What's it For? ###

Implementing a command protocol for a network device is something that a lot of developers do from scratch. We thought it was time for a ready-made solution that can serve as the basis for such a development. The protocol format is defined and implemented, but it is done in such a way that it is quite flexible and easy to extend and re-purpose for whatever application is being developed.

There is a lot of micro-framework code out there that looks like it was written in Fortran in the 1960s. We have tried to take a more object oriented approach to this library.

### Some Features and Aims of This Project ###

- **Object oriented**: we strive to give each class a single responsibility and basically follow the SOLID best practices.
- **Clean code**: we strive to maintain 'clean code' that is well documented, readable by human beings and easy to maintain.
- **General purpose**: The protocol can be very easily adapted to whatever device is being controlled, by adding *command targets* and *command processors* to the system. We have included a trivial example of a simulated temperature probe, which always reads the same temperature.
- **Transactional**: Each command includes a Transaction ID that is echoed in the response, but is otherwise ignored. This can be very useful in the client application for matching commands to responses, especially where multiple commands can be in flight at the same time.
- **Command/Response Protocol**: The protocol uses a Command/Response paradigm where each command elicits a single response.
	- Commands are in a single-line format with start and end terminators. Each command is in the format:
	`'<' DeviceAddress, Transaction ID, Command Verb { = Payload } '>'`  
For example:  
`<T1,23,Reset>`  
`<T1,24,ZeroPoint=23>`  
These examples address a device at address `T1` (temperature probe 1). They have transaction IDs of `23` and `24` respectively. The first has a command verb of `Reset` and the second has a command verb of `ZeroPoint` with a payload (parameter) of `23`. Every command has this format, but new devices and command verbs can be easily added.

	- Responses include the transaction ID, followed by zero or more Key-Value pairs containing any data that needs to be returned to the application, followed by the literal text `END`.  
	Key-Value pair collections are unbounded, so the device can return as much data as needed. They are simple to parse and address in the client application.
- **Community involvement welcome**: We would love to get some pull requests! Please help us improve the design.
- **Orthogonal**: All commands and responses have the exact same format, which greatly simplifies parsing and creating commmand and response packets.
- User Friendly: Error message can include as much information as desired, so that the user (or application) can figure out what actually went wrong.
- **Permissive license**: All code in this repository is covered by the [MIT license](http://opensource.org/licenses/MIT "The MIT License"), which is a permissive license. In summary, anyone can do anything at all with this code with proper attribution and with no warranty. Commercial use is specifically allowed.

### How do I get set up? ###

The project is configured for Netduino Plus 2 hardware and .net MicroFramework 4.3 but should work on any .Net Micro Framework device with a network port.

### Contribution guidelines ###

We invite and encourage pull requests. Each request will undergo code review before being merged. We use [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/ "a successful Git branching model"), which reserves the *master* branch for published releases. Therefore, please push your code to `develop` or a feature branch under `feature/`. [Atlassian SourceTree](http://www.sourcetreeapp.com/ "Free Git and Mercurial client for Windows and Mac") has built-in support for GitFlow and makes it all very simple. 

When you push code or submit a pull request to this repository, you are agreeing that your code is irrevocably donated to the project under the project's [MIT License](http://opensource.org/licenses/MIT "The MIT License"). Note that the license allows for commercial use of the code. Please don't submit code if you are not comfortable with that.

### Who do I talk to? ###

* Repo owner/admin: Tim Long ([Tigra Astronomy](http://tigra-astronomy.com/ "Tigra Astronomy"))

-----
This project is licensed under the [MIT license](http://opensource.org/licenses/MIT "The MIT License")