# vstemplates
A collection of Visual Studio project templates.

## MVC5
Based on the default MVC5 Web Application template from Visual Studio 2015.

Changes:
* Updated all NuGet packages.
* Updated target framework.
* Organized classes (mostly related to Account / Manage) into separate folders.

## MVC5_R
A heavily opinionated modification of the default MVC5 Web Application template from Visual Studio 2017.

This template uses:
* [MediatR] - for supporting a vertical slices architecture
* [FluentValidation] - to declutter MediatR classes and enable custom validation
* [SimpleInjector] - for dependency injection
* [SendGrid] - for sending email
* [Bower] - for managing front-end packages.

There is no service layer and no repository layer. Most of the viewmodels have been replaced with MediatR request and response classes. MediatR handlers implement business logic.

Scripts, stlyles, and front-end libraries have been moved into the wwwroot folder.

[MediatR]: https://github.com/jbogard/MediatR
[FluentValidation]: https://github.com/JeremySkinner/FluentValidation
[SimpleInjector]: https://github.com/simpleinjector/SimpleInjector
[SendGrid]: https://docs.microsoft.com/en-us/azure/app-service-web/sendgrid-dotnet-how-to-send-email
[Bower]: https://github.com/bower/bower
