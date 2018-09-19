# MiniTerm

Experimental terminal using the new PTY APIs from Microsoft. Written in C#, and heavily based on the native code examples.

**EDIT: All development has been moved to [my fork of microsoft/console](https://github.com/waf/console/tree/add-csharp-conpty-sample). It will soon result in a PR to that repo.** 

## Status

Buggy proof-of-concept. Demonstrates the basic API calls required.

## Resources

- [Introductory blog post from Microsoft](https://blogs.msdn.microsoft.com/commandline/2018/08/02/windows-command-line-introducing-the-windows-pseudo-console-conpty/)
- [MSDN Documentation](https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session)

## System Requirements

See the Introductory blog post from Microsoft for the full setup instructions. This project has been tested with:

- OS: Windows 10 Pro Build 17744
- SDK: Windows_InsiderPreview_SDK_en-us_17749
