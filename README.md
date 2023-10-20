# RegJump#

[RegJump#](https://github.com/vurdalakov/regjumpsharp) is a C# remake of [RegJump](https://learn.microsoft.com/en-us/sysinternals/downloads/regjump) by [SysInternals](https://learn.microsoft.com/en-us/sysinternals/).

This command-line tool takes a Registry path and makes RegEdit open to that path.
* Starts RegEdit if it is not yet open and activates it if it is already open.
* Administrative privileges are required to open the Registry path.
* Accepts root keys in standard (e.g. `HKEY_LOCAL_MACHINE`) and abbreviated form (e.g. `HKLM`).
* Registry path can start with `COMPUTER` prefix.

Usage: `RegJump <path|-c>`

Use `-c` parameter to copy path from Clipboard.

Example 1: `RegJump HKLM\Software\Microsoft\Windows`

Example 2: `RegJump -c`

## License

`RegJumpSharp` program is distributed under the terms of the [MIT license](https://opensource.org/licenses/MIT).
