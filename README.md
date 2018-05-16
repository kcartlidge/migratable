# Migratable

Simple, efficient, and tested .Net Core database migrations supporting multiple database technologies.

## Status

Not yet usable; undergoing initial development.

---

## For developers working on Migratable itself

If you only intend making use of *Migratable* in your own projects read no further.

### Generating a build and running the tests

There is no need to generate a build as *Migratable* is a class library not an application.
If you run the tests, a build is generated anyway automatically:

``` sh
cd Migratable.Tests
dotnet test
```

### Creating a new version for Nuget

The ```Migratable/Migratable.csproj``` file contains Nuget settings.
Within that file, update the version number then create the Nuget package:

``` sh
cd Migratable
dotnet pack
```

---

## MIT Licence

Copyright (c) 2018 K Cartlidge

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
