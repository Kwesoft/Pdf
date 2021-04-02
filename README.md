[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Kwesoft/Pdf/blob/master/LICENSE.md)
[![NuGet](https://buildstats.info/nuget/Kwesoft.Pdf)](https://www.nuget.org/packages/Kwesoft.Pdf/)

# Kwesoft.Pdf
A native .NET Standard 2.0 library for PDF editing & parsing.

## License
Kwesoft.Pdf is licensed under the MIT license.  See [LICENSE.md](https://github.com/Kwesoft/Pdf/blob/master/LICENSE.md).

## Dependencies
* [.Net Standard 2.0](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md) (.Net Core 2.0/.NetFramework 4.6.1 / see docs for other platforms)
* [System.Text.Encoding.CodePages](https://www.nuget.org/packages/System.Text.Encoding.CodePages/)

## Features
* Read PDF from byte array
* Traverse PDF structure
* Add/remove objects to/from PDF structure 
* Edit text and numeric data
* Edit metadata

## Coming Soon
* Concatenation of PDFs
* Support for more recent PDF features
* Stream parsing

## Usage
```cs
using Kwesoft.Pdf;
using System.IO;
```

```cs
byte[] data = await File.ReadAllBytesAsync("C:\rtfm.pdf");
var pdf = new InMemoryPdf(data);
```

## Contributing
We welcome new features/fixes from other contributors.

If you find a bug please [raise an issue](https://github.com/Kwesoft/Pdf/issues/new).

If you would like to contribute more directly then please feel free to clone this repository, make your changes, and [raise a pull request](https://help.github.com/articles/about-pull-requests/). New features should be accompanied by appropriate testing and should follow the style of the existing codebase. Please follow SOLID, KISS and DRY principles.