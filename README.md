# Alter

Alter is a remote desktop client application.

## ✨ Features

- High DPI support - Reflect to the local DPI to the remote desktop session.
- Update the resolution on the window resize.

## 📥 Install and uninstall

1. Download [the app's latest release zip file](https://github.com/tksh164/alter-rdp-client/releases/latest).

2. Unblock the downloaded zip file by check **Unblock** from the zip file's property or using the [Unblock-File](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/unblock-file) cmdlet.
    
    ```powershell
    Unblock-File alter-x.y.z.zip
    ```
    
3. Extract files from the downloaded zip file. You can extract files by the **Extract All...** context menu in the File Explorer or using the [Expand-Archive](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/expand-archive) cmdlet.

    ```powershell
    Expand-Archive alter-x.y.z.zip
    ```

4. Put the extracted folder `alter-x.y.z` anywhere you like.

5. Run the `alter.exe` in the extracted folder.

6. If you don't need this app anymore, you can uninstall this app by deleting the `alter-x.y.z` folder.

## 📋 Supported operating systems

- Alter is tested on the latest version of Windows 11 with latest updates.
- Alter may also work on the following OSs:
    - Windows 11 with latest updates
    - Windows 10 x64/x86 with latest updates
    - Windows Server 2022 with latest updates
    - Windows Server 2019 with latest updates
    - Windows Server 2016 with latest updates
    - Windows Server 2012 R2 with latest updates

## Tips

- You can get the detailed connection status information by clicking the message that at center bottom of the Alter's window. Click again to back the original message.
    - Example: Clicking the `Remote disconnect by user` message then showing detailed connection information that `Reason: 0x2 (RemoteByUser), ExtendedReason: 0xB (RpcInitiatedDisconnectByUser)`.
- The Alter's setting file is located at `%LocalAppData%\AlterRDClient\<Version>\setting.db`. The `setting.db` file is a SQLite database file.

## 🔨 Build from the source

You can build the project using [Visual Studio 2022](https://visualstudio.microsoft.com/).

## ⚖️ License

Copyright (c) 2023-present Takeshi Katano. All rights reserved. This software is released under the [MIT License](https://github.com/tksh164/alter-rdp-client/blob/main/LICENSE).

Disclaimer: The codes stored herein are my own personal codes and do not related my employer's any way.
