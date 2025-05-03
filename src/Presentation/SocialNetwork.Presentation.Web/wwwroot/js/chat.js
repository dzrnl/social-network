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

    let senderName;

    if (message.sender == null) {
        senderName = document.createElement("span");
        senderName.className = "sender-name";
        senderName.textContent = "Deleted user";
    } else if (message.sender.id === userId) {
        senderName = document.createElement("span");
        senderName.className = "sender-name";
        senderName.textContent = "You";
        wrapper.classList.add("own-message");
    } else {
        senderName = document.createElement("a");
        senderName.href = `/${message.sender.username}`;
        senderName.className = "sender-name";
        senderName.textContent = message.sender.name;
    }

    senderNameWrapper.appendChild(senderName);

    const msg = document.createElement("div");
    msg.className = "message-content";
    msg.textContent = message.content;

    const chatMessage = document.createElement("div");
    chatMessage.className = "chat-message";
    chatMessage.appendChild(senderNameWrapper);
    chatMessage.appendChild(msg);

    wrapper.appendChild(chatMessage);
    container.appendChild(wrapper);

    scrollToBottom();
});

connection.start().catch(err => console.error(err.toString()));

function sendMessage() {
    const input = document.getElementById("messageInput");
    const content = input.value.trim();

    const message = input.value.trim();
    if (message) {
        connection.invoke("SendMessage", userId, content)
            .catch(err => console.error(err.toString()));

        input.value = "";
    }
}

document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    sendMessage();
});

document.getElementById("messageInput").addEventListener("keydown", function (event) {
    if (event.key === "Enter" && !event.shiftKey) {
        event.preventDefault();
        sendMessage();
    }
});

function scrollToBottom() {
    const container = document.getElementById("chatBox");
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
}

window.addEventListener("load", scrollToBottom);