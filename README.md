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

Use data template
- select this option if you want to data merge your templates

```yaml
key: value

list: 
- option1
- option2
- option3

images:
- ball.jpeg
- tree.png
```

This will create a data file for each template document which you can fill in to have your
desination data merged.
e.g. Customer.docx , Customer.yaml

Rebinding
- select rebind to data merge your template from your data file
If you would like to change your data input simply alter the data file and rebind the template to the data merge.

Combining
- include a data template in the template folder.
- combining will happen post template generation.
- each required combination should be in the form:

join.yaml
```yaml
output.docx:
- input1.docx
- input1.docx
```

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

Set the config path to the location of the file templates
```
config.json > templatePath: 
```

Run templated.exe:
- select the template to run.
- include the folder to output the templates

## Running the tests

xUnit testing

```bash
cd TemplateTests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```


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

See also the list of [contributors](https://github.com/smacken/templated/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* mmm

