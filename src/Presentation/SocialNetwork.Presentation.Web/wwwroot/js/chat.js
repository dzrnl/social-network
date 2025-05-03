const userId = window.currentUserId;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", function (message) {
    const container = document.getElementById("chatBox");

    const wrapper = document.createElement("div");
    wrapper.className = "chat-message-wrapper";

    const senderNameWrapper = document.createElement("div");
    senderNameWrapper.className = "sender-name-wrapper";

    const senderName = document.createElement("a");
    senderName.href = `/${message.sender.username}`;
    senderName.className = "sender-name";
    senderName.textContent = message.sender.name;

    senderNameWrapper.appendChild(senderName);

    const msg = document.createElement("div");
    msg.className = "message-content";
    msg.textContent = message.content;

    const chatMessage = document.createElement("div");
    chatMessage.className = "chat-message";
    chatMessage.appendChild(senderNameWrapper);
    chatMessage.appendChild(msg);

    if (message.sender.id === userId) { 
        wrapper.classList.add("own-message");
    }

    wrapper.appendChild(chatMessage);
    container.appendChild(wrapper);

    container.scrollTop = container.scrollHeight;
});

connection.start().catch(err => console.error(err.toString()));

document.getElementById("sendButton").addEventListener("click", function (event) {
    const input = document.getElementById("messageInput");
    const content = input.value.trim();

    if (content) {
        connection.invoke("SendMessage", userId, content)
            .catch(err => console.error(err.toString()));

        input.value = "";
    }

    event.preventDefault();
});
