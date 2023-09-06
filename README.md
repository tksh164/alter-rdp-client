# Alter

Alter is an remote desktop client application.

## ✨ Features

- High DPI support - Reflect to the local DPI to the remote desktop session.
- Update the resolution on the window resize.

## 📥 Install

1. Download [the app's latest release zip file](https://github.com/tksh164/alter-rdp-client/releases/latest).

2. After the download the zip file, you can unblock the zip file by check **Unblock** from the file's property or using [Unblock-File](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/unblock-file) cmdlet.
    
    ```powershell
    Unblock-File alter-x.y.z.zip
    ```
    
3. Next, extract files from the zip file. You can extract files by the **Extract All...** context menu in the File Explorer or using [Expand-Archive](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/expand-archive) cmdlet.

    ```powershell
    Expand-Archive alter-x.y.z.zip
    ```

4. Put the extracted files anywhere you like.

    If you don't need this app anymore, you can uninstall it by deleting the app's folder.

## 📋 Supported operating systems

- Alter is tested on the latest version of Windows 11 with latest updates.
- Alter may also work on the following OSs:
    - Non-latest version of Windows 11 with latest updates
    - Windows 10 x64/x86 with latest updates
    - Windows Server 2022 with latest updates
    - Windows Server 2019 with latest updates
    - Windows Server 2016 with latest updates
    - Windows Server 2012 R2 with latest updates

## 🔨 Build from the source

You can build the project using [Visual Studio 2022](https://visualstudio.microsoft.com/).

## ⚖️ License

Copyright (c) 2023-present Takeshi Katano. All rights reserved. This software is released under the [MIT License](https://github.com/tksh164/alter-rdp-client/blob/main/LICENSE).

Disclaimer: The codes stored herein are my own personal codes and do not related my employer's any way.
