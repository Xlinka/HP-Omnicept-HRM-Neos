# Heart Rate Omnicept

This is a C# program that retrieves the current heart rate from an HP Reverb Omnicept device and outputs it to the console and to a WebSocket connection. Additionally, a log is made for analytical data use and logging purposes.

## Requirements
- HP Reverb Omnicept device
- .NET framework 4.6.1 or later

## Usage
1. Connect the HP Reverb Omnicept device to the computer.
2. Clone or download this repository.
3. Open the solution file in Visual Studio.
4. Build and run the solution.

## Output
The program outputs the current heart rate to the console in the following format:
Heart Rate: [value] BPM
If the heart rate is higher than 120 BPM, 
the output will be displayed in red. If the heart rate is higher than 100 BPM, the output will be displayed in yellow. 

Additionally, the program sends the heart rate data to a WebSocket client. The WebSocket server is hosted on `ws://localhost:8080`. 

Finally, a log file is created in the following format:
[YYYY-MM-DD HH:MM:SS] Heart Rate: [value] BPM

## Contributing
Contributions are welcome. Feel free to submit a pull request or open an issue.
