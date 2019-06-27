# Templated - Simple document templating

In small business you often have a group of documents for a task that you want copied.
This is simple templating which will take different template sets, and allow you to copy
template instances from a given set.

![tempalted](https://raw.githubusercontent.com/smacken/templated/master/docfx/term.PNG)

/templates

- should include variations of different templates
- selecting a template from the ui for a new instance will clone the template

Inside a destination folder you can get document templating with data merging.
- for each document you want replaced with data
- will only data merge if a data template exists, otherwise will be a simple copy
- you can also rebind data templates after creation if you want to change data but keep documents

## Getting Started

1. Run the app by entering the following command in the command shell:

   ```console
    dotnet run
   ```

### Prerequisites

Install the following:

- [.NET Core](https://dotnet.microsoft.com/download).

### Installing

Copy exe from /dist/templated.exe

Either include the exe in path or put the templator relative to the file system to create the templates.
i.e. Within the /templates/ folder

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

xUnit testing


### Break down into end to end tests

Testing templates being used

```
dotnet test
```

### And coding style tests

Editor.config

```
Editor.config
```

## Deployment

```
dotnet publish
```

## Built With

* [TerminalUI](https://github.com/migueldeicaza/gui.cs) - Terminal based ui

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Scott Mackenzie** - *Initial work* - [Smacktech](https://github.com/smacken)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* mmm

