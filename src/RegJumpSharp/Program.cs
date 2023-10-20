// RegJumpSharp | A C# remake of RegJump by SysInternals | Copyright (c) 2023 Vurdalakov | MIT License | https://github.com/vurdalakov/regjumpsharp

namespace Vurdalakov.RegJumpSharp
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    internal class Program
    {
        static void Main(String[] args)
        {
            try
            {
                // check command line

                if (0 == args.Length)
                {
                    var version = Assembly.GetEntryAssembly().GetName().Version.ToString(2);
                    Console.WriteLine($"RegJump# v{version} | A C# remake of RegJump by SysInternals | https://github.com/vurdalakov/regjumpsharp");
                    Console.WriteLine("Usage: RegJump <path|-c>");
                    Console.WriteLine("       -c to copy path from Clipboard");
                    Halt(1, "e.g.: RegJump HKLM\\Software\\Microsoft\\Windows");
                }

                // get registry path

                var registryPath = "";

                if ((1 == args.Length) && (2 == args[0].Length) && ('-' == args[0][0]) && ('c' == Char.ToLowerInvariant(args[0][1])))
                {
                    if (!NativeHelpers.TryGetClipboardText(out registryPath))
                    {
                        Halt(2, "Clipboard does not contain text.");
                    }
                }
                else
                {
                    registryPath = String.Join(" ", args);
                }

                // verify path

                registryPath = registryPath.Trim();

                registryPath = registryPath.ToUpperInvariant();

                if (('"' == registryPath[0]) && ('"' == registryPath[registryPath.Length - 1]))
                {
                    registryPath = registryPath.Substring(1, registryPath.Length - 2);
                }

                const String Computer = @"COMPUTER\";

                if (registryPath.StartsWith(Computer))
                {
                    registryPath = registryPath.Substring(Computer.Length);
                }

                var registryHive = "";

                const String HKCR = @"HKEY_CLASSES_ROOT\";
                const String HKCU = @"HKEY_CURRENT_USER\";
                const String HKLM = @"HKEY_LOCAL_MACHINE\";
                const String HKU = @"HKEY_USERS\";
                const String HKCC = @"HKEY_CURRENT_CONFIG\";

                if (registryPath.StartsWith(@"HKCR\") || registryPath.StartsWith(HKCR))
                {
                    registryHive = HKCR;
                }
                else if (registryPath.StartsWith(@"HKCU\") || registryPath.StartsWith(HKCU))
                {
                    registryHive = HKCU;
                }
                else if (registryPath.StartsWith(@"HKLM\") || registryPath.StartsWith(HKLM))
                {
                    registryHive = HKLM;
                }
                else if (registryPath.StartsWith(@"HKU\") || registryPath.StartsWith(HKU))
                {
                    registryHive = HKU;
                }
                else if (registryPath.StartsWith(@"HKCC\") || registryPath.StartsWith(HKCC))
                {
                    registryHive = HKCC;
                }
                else
                {
                    Halt(3, $"Not a valid Registry path: '{registryPath}'");
                }

                var backslash = registryPath.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
                registryPath = registryPath.Substring(backslash + 1);

                registryPath = $"{Computer}{registryHive}{registryPath}";

                // verify that program is running with eleveted rights

                if (!NativeHelpers.IsAdministrator())
                {
                    Halt(4, "Administrative privileges are required to open the path.");
                }

                // start RegEdit if it is not running

                const String ProcessName = "regedit";

                var processes = Process.GetProcessesByName(ProcessName);
                if (processes.Length == 0)
                {
                    try
                    {
                        using (var process = new Process())
                        {
                            process.StartInfo.FileName = ProcessName;
                            process.Start();

                            process.WaitForInputIdle();
                        }
                    }
                    catch (Exception ex)
                    {
                        Halt(5, $"Cannot start RegEdit: {ex.Message}");
                    }
                }

                // find address bar edit control

                var mainWindowHandle = NativeMethods.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "RegEdit_RegEdit", null);
                if (IntPtr.Zero == mainWindowHandle)
                {
                    Halt(6, "Cannot get RegEdit main window handle");
                }

                var editWindowHandle = NativeMethods.FindWindowEx(mainWindowHandle, IntPtr.Zero, "Edit", null);
                if (IntPtr.Zero == editWindowHandle)
                {
                    Halt(7, "Cannot get RegEdit edit control window handle");
                }

                // activate RegEdit window

                NativeHelpers.ActivateWindow(mainWindowHandle);

                // type path to address bar and press ENTER

                NativeMethods.SendMessageString(editWindowHandle, NativeMethods.WM_SETTEXT, 0, registryPath);

                NativeMethods.SendMessageInt32(editWindowHandle, NativeMethods.WM_SETFOCUS, 0, 0);
                Thread.Sleep(50);

                NativeMethods.SendMessageInt32(editWindowHandle, NativeMethods.WM_KEYDOWN, NativeMethods.VK_RETURN, 0);
                Thread.Sleep(50);
                NativeMethods.SendMessageInt32(editWindowHandle, NativeMethods.WM_KEYUP, NativeMethods.VK_RETURN, 0);

                // finally

                Console.WriteLine($"Regedit jump to '{registryPath}' complete. ");
            }
            catch (Exception ex) 
            {
                Halt(9, ex.Message);
            }
        }

        static void Halt(Int32 errorCode, String errorMessage)
        {
            Console.WriteLine(errorMessage);
            Environment.Exit(errorCode);
        }
    }
}
