
# Spyware

Prototype for an online classroom platform, where teachers are able to turn off the students monitors.

## Description

Spyware is a backend, frontend, and client for a classroom digital platform made in C# with ASP.NET, Blazor, and WinForms as a school project.

## Installation

Client only supports `windows`! 

1. Clone the repository
2. Install [dotnet](https://dotnet.microsoft.com/en-us/)
3. Run `dotnet build` in root

## Usage

Run `dotnet run` in `src/Server`, `src/Client/Web`, and `src/Client/Windows`.
On the [web dashboard](http://localhost:5020), login using the username and password `eve`. This is your teacher account. Go to the [teacher dashboard](http://localhost:5020/teacher-dashboard).

In the client winforms, login using username and password `charlie`. Then press the `Raise Hand` button. Now in the teacher dashboard confirm that the student name Charlie, has raised their hand. Try press the `Turn On/Off Screens` on the dashboard a few times. Your screen should turn off.

## Contributing

This is for a school project and is not open for contributions.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
