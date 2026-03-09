[![](https://img.shields.io/nuget/v/soenneker.utils.case.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.case/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.case/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.case/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.case.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.case/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.case/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.case/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Case
### High performance case transformation utility methods

## Installation

```
dotnet add package Soenneker.Utils.Case
```

## Usage

```csharp
using Soenneker.Utils.Case;
```

All conversions use a single tokenizer internally, then format into the target case.

### Core methods

- `ToKebab(ReadOnlySpan<char>)`
- `ToSnake(ReadOnlySpan<char>)`
- `ToUpperSnake(ReadOnlySpan<char>)`
- `ToDot(ReadOnlySpan<char>)`
- `ToFlat(ReadOnlySpan<char>)`
- `ToPath(ReadOnlySpan<char>)`
- `ToSpace(ReadOnlySpan<char>)`
- `ToTrain(ReadOnlySpan<char>)`
- `ToPascal(ReadOnlySpan<char>)`
- `ToCamel(ReadOnlySpan<char>)`
- `ToTitle(ReadOnlySpan<char>, CultureInfo? culture = null)`
- `ToTitle(string? value, CultureInfo? culture = null)`
- `NormalizeKebab(ReadOnlySpan<char>)`

### Example

```csharp
const string input = "HTTPServer_v2 parser";

CaseUtil.ToKebab(input);      // "http-server-v2-parser"
CaseUtil.ToSnake(input);      // "http_server_v2_parser"
CaseUtil.ToUpperSnake(input); // "HTTP_SERVER_V2_PARSER"
CaseUtil.ToDot(input);        // "http.server.v2.parser"
CaseUtil.ToFlat(input);       // "httpserverv2parser"
CaseUtil.ToPath(input);       // "http/server/v2/parser"
CaseUtil.ToSpace(input);      // "http server v2 parser"
CaseUtil.ToTrain(input);      // "HTTP-Server-V2-Parser"
CaseUtil.ToPascal(input);     // "HTTPServerV2Parser"
CaseUtil.ToCamel(input);      // "httpServerV2Parser"
CaseUtil.ToTitle(input);      // "HTTP Server V2 Parser"
```
