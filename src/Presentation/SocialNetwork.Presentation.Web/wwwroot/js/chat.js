const userId = window.currentUserId;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", function (message) {
    const container = document.getElementById("chatBox");

    const isOwnMessage = message.sender && message.sender.id === userId;

    const wrapper = document.createElement("div");
    wrapper.className = isOwnMessage
        ? "chat-message-wrapper own-message"
        : "chat-message-wrapper";

    const messageDiv = document.createElement("div");
    messageDiv.className = "chat-message";

    const senderNameWrapper = document.createElement("div");
    senderNameWrapper.className = "sender-name";

    let senderName;
    if (!message.sender) {
        senderName = document.createElement("span");
        senderName.textContent = "Deleted user";
    } else if (isOwnMessage) {
        senderName = document.createElement("span");
        senderName.textContent = "You";
    } else {
        senderName = document.createElement("a");
        senderName.href = `/${message.sender.username}`;
        senderName.className = "text-decoration-none";
        senderName.textContent = message.sender.name;
    }

    senderNameWrapper.appendChild(senderName);

    const contentDiv = document.createElement("div");
    contentDiv.className = "message-content";
    contentDiv.textContent = message.content;

    messageDiv.appendChild(senderNameWrapper);
    messageDiv.appendChild(contentDiv);

    wrapper.appendChild(messageDiv);
    container.appendChild(wrapper);

    scrollToBottom();
});

connection.start().catch(err => console.error(err.toString()));

function sendMessage() {
    const input = document.getElementById("messageInput");
    const content = input.value.trim();

    if (content) {
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