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
- **User Friendly**: Error message can include as much information as desired, so that the user (or application) can figure out what actually went wrong.
- **Permissive license**: All code in this repository is covered by the [MIT license](http://opensource.org/licenses/MIT "The MIT License"), which is a permissive license. In summary, anyone can do anything at all with this code with proper attribution and with no warranty. Commercial use is specifically allowed.

### How do I get set up? ###

The project is configured for Netduino Plus 2 hardware and .net MicroFramework 4.3 but should work on any .Net Micro Framework device with a network port.

## Making it Your Own ##

The two key interfaces you'll need to be concerned with are:
 - `ICommandTarget` represents an addressable device; something that can accept commands. 
 - `ICommandProcessor` instances are responsible for processing commands, each instance handles a specific command verb against a specific `CommandTarget`. 

### `ICommandTarget`  ###
Implementations of this interface typically correspond one to one with physical bits of hardware, such as a motor or a temperature probe, but that's not a requirement. You can have instances that represent virtual devices or collections of devices. Do whatever makes sense for your application. We provide a sample implementation `TemperatureProbe` that simulates a temperature probe. It exposes a property `internal double Temperature {get;}` by which the temperature value can be read (in the simulation is just returns a constant value). An example of a 'virtual device' might represent the controller itself and have commands that affect global configuration or return properties such as the firmware version.

    internal class TemperatureProbe : ICommandTarget
        {
        readonly string deviceAddress;

        public TemperatureProbe(string deviceAddress)
            {
            this.deviceAddress = deviceAddress;
            }

        internal double Temperature { get { return 12.5; } }

        public ICommandProcessor[] GetCommandProcessors()
            {
            var temperature = new Temperature(deviceAddress, this);
            var processors = new ICommandProcessor[] {temperature};
            return processors;
            }
        }


Each implementation of `ICommandTarget` usually 'owns' any resources that command processors may need to operate on. We've found that it works best if you create any dependencies/resources before creating your command targets and pass them in as constructor parameters (dependency inversion principle; single responsibility principle) but you could also use public static (global) variables or have the command targets create their own resources. 

Command targets must be able to return a list of their own command processors, each of which must implement `ICommandProcessor`. We've adopted the practice of having each command target new up its own command processors.

### `ICommandProcessor` ###

Each command verb requires its own implementation of `ICommandProcessor`. Instances are typically created by a command target and passed a reference to the command target instance itself, plus a reference to any resources that the command might need to do its job.

A command processor is responsible for fully executing a single command verb. By way of example, we have provided a command processor that handles the "Temperature" command by reading the current temperature value from the TemperatureProbe command target, and returning the value in Kelvin, °C and °F. Note the flexibility to return multiple values in a single response.

    internal class Temperature : ICommandProcessor
        {
        readonly string deviceAddress;
        readonly TemperatureProbe temperatureProbe;

        public Temperature(string deviceAddress, TemperatureProbe temperatureProbe)
            {
            this.deviceAddress = deviceAddress;
            this.temperatureProbe = temperatureProbe;
            }

        public string DeviceAddress { get { return deviceAddress; } }
        public string Verb { get { return "Temperature"; } }

        public Response Execute(Command command)
            {
            var temperatureC = temperatureProbe.Temperature;
            var temperatureF = 1.8*temperatureC + 32;
            var temperatureK = temperatureC + 273;
            var builder = new ResponseBuilder(command);
            builder.AddPayloadItem("Celsius", temperatureC.ToString());
            builder.AddPayloadItem("Farenheit", temperatureF.ToString());
            builder.AddPayloadItem("Kelvin", temperatureK.ToString());
            return builder.ToResponse();
            }
 
While each command processor can handle a single command verb, note that it can be associated with multiple command targets. Consider for example some application that controls two motors, "M1" and "M2". There may be two instances of the MotorCommandTarget class, having device addresses "M1" and "M2" respectively. Each target may create an instance of a MotorStartCommandProcessor. Thus there may be two instances of the `MotorStartCommandProcessor` class, each bound to a different command target. This is why we suggest that command processors should hold a reference to their command target.

### Composition ###

To bring all this together, something in your application needs to create the command targets and start the network server. One way to do so is as follows:

    public class Program
        {
        const int RxTxBufferSize = 1024;

        public static void Main()
            {
            ConfigureCommandTargets();
            while (true)
                {
                try
                    {
                    Server.ListenForConnections(); // should never return.
                    }
                catch (Exception ex)
                    {
                    Debug.Print("Exception caught in Main loop (attempting to continue):");
                    Debug.Print(ex.ToString());
                    }
                }
            }

        static void ConfigureCommandTargets()
            {
            // Add your command targets here.

            // Temperature probe (example)
            var probe = new TemperatureProbe("T1");
            CommandDispatcher.RegisterCommandTarget(probe);
            }
        }

We adopted the convention of having a method called `ConfigureCopmmandTargets()` that is called from the `Main()` method or similar.

We then call `Server.ListenForConnections()` which should never return unless an error has occurred. This then effectively becomes the main application loop. As in our example code above, it may be sensible to put error recovery around this call, or to simply reboot and let the application restart.

## Command Life Cycle ##

At runtime, `Server` listens for TCP connections on a given port number. When a connection request is received, it is handed off to `ConnectionHandler`, which reads the incoming data stream into a string.

Once a request string has been received, it is passed to `CommandParser` which uses regular expressions to vlidate the request and break it up into its various constituent parts, returning a `Command` instance.

Once a valid command packet has been received, it is handed to `CommandDispatcher.Dispatch` which identifies the correct `ICommandProcessor` based on the device address and command verb contained within the command. It then calls `ICommandProcessor.Execute` on the identified command processor, passing in the `Command` object.

The command processor is expected to fully execute the command verb and return a `Response` object containing the results of the command (which may be an error code). The response is passed back up to the network server code, where `ConnectionHandler.SendResponse` formats the response correctly and transmits it back to the client.

This is a relatively simple scheme and the main advantage is that all aspects of the network protocol are fully specified and handled. Thus the developer can concentrate on what to do with the commands once they are received and does not have to worry too much about parsing, formatting and networking. We feel that we've done a reasonable job of separating out responsibilities so that, for example, device handling code is not having to worry about networking or command formats.

### Contribution guidelines ###

We invite and encourage pull requests. Each request will undergo code review and must build successfully on our build server before being merged. We use [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/ "a successful Git branching model"), which reserves the *master* branch for published releases. Therefore, please push your code to `develop` or a feature branch under `feature/`. [Atlassian SourceTree](http://www.sourcetreeapp.com/ "Free Git and Mercurial client for Windows and Mac") has built-in support for GitFlow and makes it all very simple. 

When you push code or submit a pull request to this repository, you are agreeing that your code is irrevocably donated to the project under the project's [MIT License](http://opensource.org/licenses/MIT "The MIT License"). Note that the license allows for commercial use of the code. Please don't submit code if you are not comfortable with that.

### Who do I talk to? ###

* Repo owner/admin: Tim Long ([Tigra Astronomy](http://tigra-astronomy.com/ "Tigra Astronomy"))

-----
This project is licensed under the [MIT license](http://opensource.org/licenses/MIT "The MIT License")