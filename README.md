# MSFS_SimRateCounter
Key Counter for MSFS (simrate)



# SimRate Controller

A lightweight Windows application for quick rate adjustment with a transparent overlay. Perfect for simulation speed control or any scenario where quick numerical adjustments are needed.

## Features

- Minimalistic transparent overlay
- Configurable single-key shortcuts
- Always-on-top display
- Reset functionality
- Click-through overlay (won't interfere with other applications)

## Installation

### Method 1: Direct Download
1. Download the latest release ZIP file from the [Releases](https://github.com/Lauzl/MSFS_SRC/releases/tag/v1.0.0) page
2. Extract the ZIP to your preferred location
3. Run `SimRateApp.exe`

### Method 2: Build from Source
1. Clone this repository
2. Open `SimRateApp.sln` in Visual Studio 2019 or later
3. Set build configuration to "Release"
4. Build the solution (F6 or Build → Build Solution)
5. Find the executable in `bin/Release/SimRateApp.exe`

## Configuration

1. Launch SimRateApp.exe
2. Click on "Settings" in the main window
3. Choose which action to configure:
   - Configure UP Key
   - Configure DOWN Key
   - Configure RESET Key
4. Press your desired key and click OK to save

Note: Settings need to be reconfigured each time the application is started.

## Features in Detail

### Overlay Display
- Shows current rate value
- Semi-transparent black background
- Always visible on top of other windows
- Automatically positions itself in the top-right corner
- Click-through functionality (won't interfere with clicking on windows behind it)

### Key Configuration
- Single key shortcuts
- Visual confirmation of current key bindings
- Easy-to-use configuration interface

### Counter Functions
- Increment value (+1)
- Decrement value (-1)
- Reset to zero
- Persistent display of current value

## System Requirements

- Windows 7 or later
- .NET Framework 4.7.2 or higher
- Screen resolution: 1024x768 or higher recommended

## Known Issues

- Settings are not saved between application restarts
- Only single key shortcuts are currently supported
- The overlay might briefly flicker when switching between full-screen applications
- Some applications running with administrator privileges might require SimRate to also run as administrator

## Troubleshooting

### Hotkeys not working?
1. Ensure no other application is using the same hotkeys
2. Try running the application as administrator
3. Check if the main window is responsive

### Overlay not visible?
1. Check if the application is running (check task manager)
2. Ensure your screen resolution is sufficient
3. Try restarting the application

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Version History

- v1.0.0 (Initial Release)
  - Basic functionality
  - Configurable single-key shortcuts
  - Transparent overlay
  - Settings reset on application restart

## Contact

If you have any questions or suggestions, please open an issue on GitHub.
