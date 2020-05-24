# Realtime chat with Unity and socket.io
<br>
Implementation of the realtime chat with Unity and socket.io.<br>
Initially I was planning on a implementation of a multiplayer server, but decided to start with something more simple than that. This project could be a good foundation for simple multiplayer turn based games or something similar, also could be part of the existing multiplayer code which needs TCP chat :P
<br><br>
Project consist of two parts:
<br>
<br>
First server side. 
<br>
It was build with `express.js` framework and `socket.io` framework. Socket.io is responsible for handling communication between clients in realtime. Express.js is used in the case if the server side would grow into something more than a simple project.<br>
All socket code could be found in the `index.js`.
<br><br>
Second is unity part. 
<br>It has implementation of socket.io C# client and integration with Unity which includes some dlls. 
<br>
Original implementation of the C# socket.io client could be found at:<br> 
[https://github.com/doghappy/socket.io-client-csharp](https://github.com/doghappy/socket.io-client-csharp)
<br>
I had to modify some source code in order to make it work with Unity.
<br>
<br><br>

# Notice
<br>
Build was tested with Mono. IL2CPP might have some problems. There are some known bugs :)