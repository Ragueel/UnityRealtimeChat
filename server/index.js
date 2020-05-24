const express = require('express');
const http = require('http');
const socketIo = require('socket.io');
const formatMessage = require("./utils/messaging");
const home = require('./routes/home');
const {userJoin, getCurrentUser, userLeave, getRoomUsers} = require("./utils/users");

const app = express();
const PORT = 5010;

app.use(express.json());

app.use('/', home);

const server = http.createServer(app);
const io = socketIo(server);

io.on('connection', connection => {
  console.log('Client connected');

  connection.on('join', data => {
    const parsedData = JSON.parse(data);
    const user = userJoin(connection.id, parsedData.username, parsedData.room);
    connection.join(user.room);

    // Notify all that someone connected
    io.to(user.room).emit('messages', formatMessage('Bot', `New User connected ${user.username}`));
    // Update users in the room
    io.to(user.room).emit('usersCount', getRoomUsers(user.room));
  });

  connection.on('chatMessage', message => {
    const user = getCurrentUser(connection.id);
    const parsedMessage = JSON.parse(message);

    io.to(user.room).emit('messages', formatMessage(parsedMessage.author, parsedMessage.message));
  });

  connection.on('disconnect', () => {
    console.log('User disconnected');
    const user = userLeave(connection.id);

    if (user) {
      // Notify others that user left
      io.to(user.room).emit('messages', formatMessage('Bot', `${user.username} left us :(`));
      // Update total number of users in the room
      io.to(user.room).emit('usersCount', getRoomUsers(user.room));
    }
  })
});

server.listen(PORT, () => {
  console.log(`Launched server on port ${PORT}`);
});